namespace Domain.Features.StockMovements.DtoS;

public record StockMovementRowDto(
    DateTime MovementDate,
    string ProductName,
    string ProductCode,
    string Reason,
    decimal QuantityChange,
    decimal? BalanceAfter,
    string? Note,
    string? InvoiceNumber);
