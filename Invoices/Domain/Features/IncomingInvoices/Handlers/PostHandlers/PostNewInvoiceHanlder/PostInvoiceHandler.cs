using Domain;
using Domain.Features.IncomingInvoices.Services;
using Domain.Features.Products.Entity;
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

        foreach (var item in invoice.Items)
        {
            var product = _db.Products.Local.FirstOrDefault(p => p.Code == item.ProductCode && p.SupplierId == supplier.Id)
                ?? await _db.Products.FirstOrDefaultAsync(p => p.Code == item.ProductCode && p.SupplierId == supplier.Id, cancellationToken);

            if (product is null)
            {
                product = new Product
                {
                    Supplier = supplier,
                    Code = item.ProductCode,
                    Name = item.Name,
                    IndexCode = item.IndexCode,
                    Brand = item.Brand,
                    GroupName = item.GroupName,
                    LastPurchasePrice = item.UnitPrice,
                    LastVatRate = item.VatRate,
                    QuantityOnHand = item.Quantity,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _db.Products.Add(product);
            }
            else
            {
                product.QuantityOnHand += item.Quantity;
                product.LastPurchasePrice = item.UnitPrice;
                product.LastVatRate = item.VatRate;
                product.UpdatedAt = DateTime.UtcNow;
            }

            item.Product = product;

            _db.StockMovements.Add(new StockMovement
            {
                Product = product,
                InvoiceItem = item,
                QuantityChange = item.Quantity,
                Reason = StockMovementReason.Purchase,
                MovementDate = invoice.IssueDate,
                CreatedAt = DateTime.UtcNow,
                BalanceAfter = product.QuantityOnHand,
                Note = "По Поръчка"
            });
        }

        await _db.SaveChangesAsync(cancellationToken);
        return invoice;
    }
}
