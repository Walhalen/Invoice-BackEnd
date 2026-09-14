namespace Domain.Features.Suppliers.Dtos;

public record SupplierDto(
    int Id,
    string Name,
    string Code,
    string? VatNumber,
    string? DefaultCurrencyCode,
    string? Email,
    string? PhoneNumber,
    string? IBAN,
    bool IsActive);
