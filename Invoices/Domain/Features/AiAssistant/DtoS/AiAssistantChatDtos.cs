namespace Domain.Features.AiAssistant.DtoS;

public record AiAssistantChatHistoryItemDto(string Sender, string Text);

public record AiAssistantTableDto(List<string> Columns, List<List<object?>> Rows);

public record AiAssistantChatResponseDto(string Text, AiAssistantTableDto? Table);
