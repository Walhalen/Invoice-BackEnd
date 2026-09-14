using System.Text.Json;
using Anthropic;
using Anthropic.Models.Messages;
using Domain.Features.AiAssistant.DtoS;
using Domain.Features.AiAssistant.Services;

using Domain.Features.StockMovements.DtoS;
using Domain.Features.StockMovements.Handlers.GetHandlers.GetStockMovements;
using Domain.Features.StockMovements.Handlers.GetHandlers.GetStockMovementTotals;
using MediatR;

namespace Domain.Features.AiAssistant.Handlers.PostHandlers.AiAssistantChatHandler;

public class AiAssistantChatHandler : IRequestHandler<AiAssistantChatRequest, AiAssistantChatResponseDto>
{
    private const string ModelId = "claude-sonnet-5";
    private const int MaxToolIterations = 5;

    private const string SystemPrompt =
        "Ти си AI асистент за складова наличност. Можеш да отговаряш само на въпроси, свързани с " +
        "историята на движенията по склада (покупки, продажби, корекции, връщания и брак). " +
        "Винаги използвай наличните инструменти, за да извлечеш реални данни от базата, преди да отговориш - " +
        "никога не измисляй числа. Ако инструментите не върнат резултат, кажи го directly. " +
        "Отговаряй кратко, по същество и на български език. " +
        "Не използвай Markdown форматиране - без звездички за удебелен текст, без markdown таблици с '|' и без markdown заглавия. " +
        "Пиши на обикновен текст. Табличните данни вече се показват отделно на потребителя, " +
        "затова в отговора си дай само кратко словесно обобщение, не повтаряй всеки ред от таблицата.";

    private static readonly Dictionary<string, string> ReasonLabels = new()
    {
        ["Purchase"] = "Доставка",
        ["Sale"] = "Продажба",
        ["Adjustment"] = "Корекция",
        ["Return"] = "Връщане",
        ["WriteOff"] = "Брак",
    };

    private readonly AnthropicClient _anthropic;
    private readonly IMediator _mediator;

    public AiAssistantChatHandler(AnthropicClient anthropic, IMediator mediator)
    {
        _anthropic = anthropic;
        _mediator = mediator;
    }

    public async Task<AiAssistantChatResponseDto> Handle(AiAssistantChatRequest request, CancellationToken cancellationToken)
    {
        List<MessageParam> messages = [];
        foreach (var item in request.History)
        {
            messages.Add(new MessageParam
            {
                Role = item.Sender == "user" ? Role.User : Role.Assistant,
                Content = item.Text,
            });
        }

        messages.Add(new MessageParam { Role = Role.User, Content = request.Message });

        AiAssistantTableDto? lastTable = null;

        for (var iteration = 0; iteration < MaxToolIterations; iteration++)
        {
            var response = await _anthropic.Messages.Create(new MessageCreateParams
            {
                Model = ModelId,
                MaxTokens = 1024,
                System = SystemPrompt,
                Tools = [StockMovementTools.GetStockMovements, StockMovementTools.GetStockMovementTotals],
                Messages = messages,
            }, cancellationToken);

            var toolUseBlocks = response.Content
                .Select(block => block.TryPickToolUse(out ToolUseBlock? toolUse) ? toolUse : null)
                .Where(toolUse => toolUse is not null)
                .Select(toolUse => toolUse!)
                .ToList();

            if (toolUseBlocks.Count == 0)
            {
                var text = response.Content
                    .Select(block => block.TryPickText(out TextBlock? textBlock) ? textBlock : null)
                    .FirstOrDefault(textBlock => textBlock is not null)
                    ?.Text ?? string.Empty;

                return new AiAssistantChatResponseDto(text, lastTable);
            }

            List<ContentBlockParam> assistantContent = [];
            List<ContentBlockParam> toolResults = [];

            foreach (var block in response.Content)
            {
                if (block.TryPickText(out TextBlock? textBlock))
                {
                    assistantContent.Add(new TextBlockParam { Text = textBlock.Text });
                }
                else if (block.TryPickToolUse(out ToolUseBlock? toolUseBlock))
                {
                    assistantContent.Add(new ToolUseBlockParam
                    {
                        ID = toolUseBlock.ID,
                        Name = toolUseBlock.Name,
                        Input = toolUseBlock.Input,
                    });

                    var (resultJson, table) = await ExecuteToolAsync(toolUseBlock, cancellationToken);
                    lastTable = table ?? lastTable;

                    toolResults.Add(new ToolResultBlockParam
                    {
                        ToolUseID = toolUseBlock.ID,
                        Content = resultJson,
                    });
                }
            }

            messages.Add(new MessageParam { Role = Role.Assistant, Content = assistantContent });
            messages.Add(new MessageParam { Role = Role.User, Content = toolResults });
        }

        return new AiAssistantChatResponseDto("Не успях да завърша заявката навреме. Опитай да зададеш по-конкретен въпрос.", lastTable);
    }

    private async Task<(string ResultJson, AiAssistantTableDto? Table)> ExecuteToolAsync(
        ToolUseBlock toolUse,
        CancellationToken cancellationToken)
    {
        switch (toolUse.Name)
        {
            case "get_stock_movements":
            {
                var rows = await _mediator.Send(
                    new GetStockMovementsRequest(
                        GetString(toolUse.Input, "productName"),
                        ParseReason(GetString(toolUse.Input, "reason")),
                        GetDate(toolUse.Input, "dateFrom"),
                        GetDate(toolUse.Input, "dateTo"),
                        GetInt(toolUse.Input, "limit") ?? 50),
                    cancellationToken);
                return (JsonSerializer.Serialize(rows), BuildMovementsTable(rows));
            }
            case "get_stock_movement_totals":
            {
                var groupBy = GetString(toolUse.Input, "groupBy") ?? "reason";
                var totals = await _mediator.Send(
                    new GetStockMovementTotalsRequest(
                        GetString(toolUse.Input, "productName"),
                        ParseReason(GetString(toolUse.Input, "reason")),
                        GetDate(toolUse.Input, "dateFrom"),
                        GetDate(toolUse.Input, "dateTo"),
                        groupBy),
                    cancellationToken);
                return (JsonSerializer.Serialize(totals), BuildTotalsTable(totals, groupBy));
            }
            default:
                return ("{\"error\": \"unknown tool\"}", null);
        }
    }

    private static AiAssistantTableDto? BuildMovementsTable(List<StockMovementRowDto> rows)
    {
        if (rows.Count == 0)
        {
            return null;
        }

        return new AiAssistantTableDto(
            ["Дата", "Продукт", "Причина", "Количество", "Наличност след", "Бележка"],
            rows.Select(row => new List<object?>
            {
                row.MovementDate.ToString("dd.MM.yyyy"),
                row.ProductName,
                ReasonLabels.GetValueOrDefault(row.Reason, row.Reason),
                row.QuantityChange,
                row.BalanceAfter,
                row.Note,
            }).ToList());
    }

    private static AiAssistantTableDto? BuildTotalsTable(List<StockMovementTotalDto> totals, string groupBy)
    {
        if (totals.Count == 0)
        {
            return null;
        }

        var groupColumn = string.Equals(groupBy, "product", StringComparison.OrdinalIgnoreCase) ? "Продукт" : "Причина";

        return new AiAssistantTableDto(
            [groupColumn, "Общо количество", "Брой движения"],
            totals.Select(total => new List<object?>
            {
                groupColumn == "Причина" ? ReasonLabels.GetValueOrDefault(total.GroupKey, total.GroupKey) : total.GroupKey,
                total.TotalQuantityChange,
                total.MovementCount,
            }).ToList());
    }

    private static string? GetString(IReadOnlyDictionary<string, JsonElement> input, string key) =>
        input.TryGetValue(key, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;

    private static int? GetInt(IReadOnlyDictionary<string, JsonElement> input, string key) =>
        input.TryGetValue(key, out var value) && value.ValueKind == JsonValueKind.Number ? value.GetInt32() : null;

    private static DateTime? GetDate(IReadOnlyDictionary<string, JsonElement> input, string key)
    {
        var value = GetString(input, key);
        return value is not null && DateTime.TryParse(value, out var date) ? date : null;
    }

    private static StockMovementReason? ParseReason(string? value) =>
        value is not null && Enum.TryParse<StockMovementReason>(value, true, out var reason) ? reason : null;
}
