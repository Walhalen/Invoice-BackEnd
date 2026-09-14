namespace Domain.Features.IncomingInvoices.Dtos;

public record InvoiceListItemDto(
    int Id,
    string SupplierName,
    string Number,
    DateTime IssueDate,
    DateTime DueDate,
    decimal NetAmount,
    decimal GrossAmount,
    decimal VatAmount,
    decimal? OutstandingAmount,
    decimal? PaidAmount);
