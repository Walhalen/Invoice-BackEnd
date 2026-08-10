using Domain;
using MediatR;

namespace WebAPI.Handlers.PostHandlers;


public class PostInvoiceHandler : IRequestHandler<PostInvoiceRequest, Invoice>
{
    private readonly AppDbContext _db;

    public PostInvoiceHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Invoice> Handle(PostInvoiceRequest request, CancellationToken cancellationToken)
    {
        _db.Invoices.Add(request.Invoice);
        await _db.SaveChangesAsync(cancellationToken);
        return request.Invoice;
    }
}
