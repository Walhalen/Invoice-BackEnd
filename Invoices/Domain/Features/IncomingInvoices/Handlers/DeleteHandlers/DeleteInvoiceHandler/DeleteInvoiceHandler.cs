using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Handlers.DeleteHandlers;

public class DeleteInvoiceHandler : IRequestHandler<DeleteInvoiceRequest, bool>
{
    private readonly AppDbContext _db;

    public DeleteInvoiceHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> Handle(DeleteInvoiceRequest request, CancellationToken cancellationToken)
    {
        var invoice = await _db.Invoices
            .Include(i => i.Items)
            .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
        
        if (invoice is null)
        {
            return false;
        }

        foreach (var item in invoice.Items)
        {
            if (item.Product is not null)
            {
                item.Product.QuantityOnHand -= item.Quantity;
                item.Product.UpdatedAt = DateTime.UtcNow;

                _db.StockMovements.Add(new StockMovement
                {
                    Product = item.Product,
                    InvoiceItem = item,
                    QuantityChange = 0 - item.Quantity,
                    Reason = StockMovementReason.WriteOff,
                    MovementDate = item.Product.UpdatedAt,
                    CreatedAt = DateTime.UtcNow,
                    BalanceAfter = item.Product.QuantityOnHand,
                    Note = "Премахната фактура"
                });
            }
        }

        _db.Invoices.Remove(invoice);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
