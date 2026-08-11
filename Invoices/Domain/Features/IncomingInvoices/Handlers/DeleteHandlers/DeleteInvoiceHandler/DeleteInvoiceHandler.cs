using Domain;
using MediatR;

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
        var invoice = await _db.Invoices.FindAsync(new object?[] { request.Id }, cancellationToken);
        if (invoice is null)
        {
            return false;
        }

        _db.Invoices.Remove(invoice);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
