using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.Features.Products.Handlers.PostHandlers.ReturnInvoiceItemsHandler;

public class ReturnInvoiceItemsHandler : IRequestHandler<ReturnInvoiceItemsRequest, ReturnInvoiceItemsResult>
{
    private readonly AppDbContext _db;

    public ReturnInvoiceItemsHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ReturnInvoiceItemsResult> Handle(ReturnInvoiceItemsRequest request, CancellationToken cancellationToken)
    {
        if (request.InvoiceItemIds.Count == 0)
        {
            return new ReturnInvoiceItemsResult(true, false, "Изберете поне един артикул за връщане.", 0);
        }

        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
        if (product is null)
        {
            return new ReturnInvoiceItemsResult(false, false, null, 0);
        }

        var distinctIds = request.InvoiceItemIds.Distinct().ToList();
        var items = await _db.InvoiceItems
            .Include(item => item.Invoice)
            .Where(item => distinctIds.Contains(item.Id))
            .ToListAsync(cancellationToken);

        if (items.Count != distinctIds.Count || items.Any(item => item.ProductId != request.ProductId))
        {
            return new ReturnInvoiceItemsResult(true, false, "Избраните артикули не бяха намерени за този продукт.", product.QuantityOnHand);
        }

        var totalQuantity = items.Sum(item => item.Quantity);
        if (product.QuantityOnHand - totalQuantity < 0)
        {
            return new ReturnInvoiceItemsResult(true, false, "Наличността не може да стане отрицателна.", product.QuantityOnHand);
        }

        var itemIds = items.Select(item => item.Id).ToList();
        var priorMovements = await _db.StockMovements
            .Where(m => m.InvoiceItemId != null && itemIds.Contains(m.InvoiceItemId.Value))
            .ToListAsync(cancellationToken);

        foreach (var movement in priorMovements)
        {
            movement.InvoiceItemId = null;
        }

        foreach (var item in items)
        {
            var invoice = item.Invoice;
            if (invoice is not null)
            {
                var netAmount = item.Quantity * item.UnitPrice;
                var vatAmount = netAmount * item.VatRate / 100m;
                var grossAmount = netAmount + vatAmount;

                invoice.NetAmount -= netAmount;
                invoice.VatAmount -= vatAmount;
                invoice.GrossAmount -= grossAmount;
                if (invoice.OutstandingAmount.HasValue)
                {
                    invoice.OutstandingAmount -= grossAmount;
                }
            }

            _db.InvoiceItems.Remove(item);
        }

        product.QuantityOnHand -= totalQuantity;
        product.UpdatedAt = DateTime.UtcNow;

        _db.StockMovements.Add(new StockMovement
        {
            Product = product,
            QuantityChange = -totalQuantity,
            Reason = StockMovementReason.Return,
            MovementDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            BalanceAfter = product.QuantityOnHand,
            Note = request.Note
        });

        await _db.SaveChangesAsync(cancellationToken);

        return new ReturnInvoiceItemsResult(true, true, null, product.QuantityOnHand);
    }
}
