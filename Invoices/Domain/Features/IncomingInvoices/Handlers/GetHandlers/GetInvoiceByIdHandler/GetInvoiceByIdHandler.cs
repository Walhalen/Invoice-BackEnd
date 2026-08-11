using Domain;
using Domain.Features.IncomingInvoices.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.Handlers.GetHandlers;


public class GetInvoiceByIdHandler : IRequestHandler<GetInvoiceByIdRequest, InvoiceDto?>
{
    private readonly AppDbContext _db;

    public GetInvoiceByIdHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<InvoiceDto?> Handle(GetInvoiceByIdRequest request, CancellationToken cancellationToken)
    {
        var invoice = await _db.Invoices
            .Include(invoice => invoice.Items)
            .Include(invoice => invoice.Supplier)
            .FirstOrDefaultAsync(invoice => invoice.Id == request.Id, cancellationToken);

        if (invoice is null)
        {
            return null;
        }

        return new InvoiceDto(
            invoice.Id,
            invoice.Supplier is null ? null : new SupplierSummaryDto(invoice.Supplier.Id, invoice.Supplier.Code, invoice.Supplier.Name),
            invoice.PayerCode,
            invoice.ReceiverCode,
            invoice.PaymentMethodCode,
            invoice.DocumentTypeCode,
            invoice.CurrencyCode,
            invoice.Number,
            invoice.IssueDate,
            invoice.DueDate,
            invoice.NetAmount,
            invoice.GrossAmount,
            invoice.VatAmount,
            invoice.ForeignCurrencyAmount,
            invoice.OutstandingAmount,
            invoice.PaidAmount,
            invoice.WarehouseCode,
            invoice.Items.Select(item => new InvoiceItemDto(
                item.Id,
                item.LineNumber,
                item.ProductCode,
                item.WarehouseNumber,
                item.SubWarehouseNumber,
                item.Brand,
                item.IndexCode,
                item.PersonalCode,
                item.Name,
                item.Description,
                item.Quantity,
                item.UnitPrice,
                item.VatRate,
                item.Sww,
                item.GroupName)).ToList());
    }
}