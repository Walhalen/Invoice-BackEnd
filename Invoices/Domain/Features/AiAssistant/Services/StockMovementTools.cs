using System.Text.Json;
using Anthropic.Models.Messages;

namespace Domain.Features.AiAssistant.Services;

public static class StockMovementTools
{
    public static Tool GetStockMovements => new()
    {
        Name = "get_stock_movements",
        Description = "Връща списък с движения по склада (StockMovement) - покупки, продажби, корекции, връщания и брак - филтрирани по продукт, причина и период. Използвай това, когато потребителят иска да види конкретни редове/събития.",
        InputSchema = new()
        {
            Properties = new Dictionary<string, JsonElement>
            {
                ["productName"] = JsonSerializer.SerializeToElement(new { type = "string", description = "Част от името или кода на продукта, по който да се филтрира." }),
                ["reason"] = JsonSerializer.SerializeToElement(new { type = "string", @enum = new[] { "Purchase", "Sale", "Adjustment", "Return", "WriteOff" }, description = "Причина за движението." }),
                ["dateFrom"] = JsonSerializer.SerializeToElement(new { type = "string", description = "Начална дата във формат YYYY-MM-DD (включително)." }),
                ["dateTo"] = JsonSerializer.SerializeToElement(new { type = "string", description = "Крайна дата във формат YYYY-MM-DD (включително)." }),
                ["limit"] = JsonSerializer.SerializeToElement(new { type = "integer", description = "Максимален брой редове за връщане. По подразбиране 50, максимум 200." }),
            },
        },
    };

    public static Tool GetStockMovementTotals => new()
    {
        Name = "get_stock_movement_totals",
        Description = "Връща обобщени суми (общо количество и брой движения) от StockMovement, групирани по причина или по продукт, за даден период. Използвай това за въпроси от типа 'колко съм бракувал през август' или 'общо количество продажби на X'.",
        InputSchema = new()
        {
            Properties = new Dictionary<string, JsonElement>
            {
                ["productName"] = JsonSerializer.SerializeToElement(new { type = "string", description = "Част от името или кода на продукта, по който да се филтрира." }),
                ["reason"] = JsonSerializer.SerializeToElement(new { type = "string", @enum = new[] { "Purchase", "Sale", "Adjustment", "Return", "WriteOff" }, description = "Причина за движението." }),
                ["dateFrom"] = JsonSerializer.SerializeToElement(new { type = "string", description = "Начална дата във формат YYYY-MM-DD (включително)." }),
                ["dateTo"] = JsonSerializer.SerializeToElement(new { type = "string", description = "Крайна дата във формат YYYY-MM-DD (включително)." }),
                ["groupBy"] = JsonSerializer.SerializeToElement(new { type = "string", @enum = new[] { "reason", "product" }, description = "По какво да се групират сумите. По подразбиране 'reason'." }),
            },
        },
    };
}
