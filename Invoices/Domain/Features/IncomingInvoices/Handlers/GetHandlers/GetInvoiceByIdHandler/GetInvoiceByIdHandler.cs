using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.Handlers.GetHandlers;


public class GetInvoiceByIdHandler : IRequestHandler<GetInvoiceByIdRequest, Invoice?>
{
    private readonly AppDbContext _db;

    public GetInvoiceByIdHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Invoice?> Handle(GetInvoiceByIdRequest request, CancellationToken cancellationToken)
    {
        return await _db.Invoices.FindAsync(new object?[] { request.Id }, cancellationToken);
    }
}