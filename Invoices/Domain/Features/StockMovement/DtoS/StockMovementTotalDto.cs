namespace Domain.Features.StockMovements.DtoS;

public record StockMovementTotalDto(string GroupKey, decimal TotalQuantityChange, int MovementCount);
