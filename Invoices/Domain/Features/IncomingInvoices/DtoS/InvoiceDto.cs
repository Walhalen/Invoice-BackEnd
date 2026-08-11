namespace Domain.Features.IncomingInvoices.Dtos;

public record InvoiceDto(
    int Id,
    SupplierSummaryDto? Supplier,
    string PayerCode,
    string ReceiverCode,
    string PaymentMethodCode,
    string DocumentTypeCode,
    string CurrencyCode,
    string Number,
    DateTime IssueDate,
    DateTime DueDate,
    decimal NetAmount,
    decimal GrossAmount,
    decimal VatAmount,
    decimal? ForeignCurrencyAmount,
    decimal? OutstandingAmount,
    decimal? PaidAmount,
    string? WarehouseCode,
    List<InvoiceItemDto> Items);
