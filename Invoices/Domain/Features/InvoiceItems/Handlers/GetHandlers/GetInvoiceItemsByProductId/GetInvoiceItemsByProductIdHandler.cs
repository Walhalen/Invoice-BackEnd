using Domain.Features.InvoiceItems.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.Features.InvoiceItems.Handlers.GetHandlers.GetInvoiceItemsByProductId;

public class GetInvoiceItemsByProductIdHandler : IRequestHandler<GetInvoiceItemsByProductIdRequest, List<ReturnableInvoiceItemDto>>
{
    private readonly AppDbContext _db;

    public GetInvoiceItemsByProductIdHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ReturnableInvoiceItemDto>> Handle(GetInvoiceItemsByProductIdRequest request, CancellationToken cancellationToken)
    {
        return await _db.InvoiceItems
            .Where(item => item.ProductId == request.ProductId)
            .OrderByDescending(item => item.Invoice!.IssueDate)
            .Select(item => new ReturnableInvoiceItemDto(
                item.Id,
                item.InvoiceId,
                item.Invoice!.Number,
                item.Invoice!.IssueDate,
                item.Quantity,
                item.UnitPrice))
            .ToListAsync(cancellationToken);
    }
}
