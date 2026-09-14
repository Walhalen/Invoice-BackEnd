using Domain;

namespace Domain.Features.Products.DtoS;

public record CorrectProductQuantityDto(StockMovementReason Reason, decimal Quantity, string? Note);
