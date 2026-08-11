namespace Domain.Features.IncomingInvoices.Dtos;

public record InvoiceItemDto(
    int Id,
    int LineNumber,
    string ProductCode,
    int WarehouseNumber,
    int SubWarehouseNumber,
    string? Brand,
    string? IndexCode,
    string? PersonalCode,
    string Name,
    string? Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal VatRate,
    string? Sww,
    string? GroupName);
