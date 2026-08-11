using Domain;
using Domain.Features.IncomingInvoices.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Handlers.PostHandlers;


public class PostInvoiceHandler : IRequestHandler<PostInvoiceRequest, Invoice>
{
    private readonly AppDbContext _db;
    private readonly IInvoiceParserFactory _parserFactory;

    public PostInvoiceHandler(AppDbContext db, IInvoiceParserFactory parserFactory)
    {
        _db = db;
        _parserFactory = parserFactory;
    }

    public async Task<Invoice> Handle(PostInvoiceRequest request, CancellationToken cancellationToken)
    {
        var parser = _parserFactory.GetParser(request.InvoiceXml);
        var invoice = parser.Parse(request.InvoiceXml);

        var supplier = await _db.Suppliers.FirstOrDefaultAsync(s => s.Code == parser.SupplierCode, cancellationToken);
        if (supplier is null)
        {
            supplier = new Supplier { Code = parser.SupplierCode, Name = parser.SupplierName };
            _db.Suppliers.Add(supplier);
        }

        invoice.Supplier = supplier;

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync(cancellationToken);
        return invoice;
    }
}
