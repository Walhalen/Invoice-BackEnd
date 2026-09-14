namespace Domain.Features.Products.DtoS;

public record ProductListItemDto(
    int Id,
    string Code,
    string Name,
    string? IndexCode,
    string? Brand,
    string? GroupName,
    decimal LastPurchasePrice,
    decimal LastVatRate,
    decimal QuantityOnHand);
