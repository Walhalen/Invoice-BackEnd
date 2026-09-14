namespace Domain.Features.InvoiceItems.Dtos;

public record ReturnableInvoiceItemDto(
    int Id,
    int InvoiceId,
    string InvoiceNumber,
    DateTime InvoiceIssueDate,
    decimal Quantity,
    decimal UnitPrice);
