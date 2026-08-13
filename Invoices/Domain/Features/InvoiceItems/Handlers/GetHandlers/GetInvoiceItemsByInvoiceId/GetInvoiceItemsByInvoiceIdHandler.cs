using Domain.Features.IncomingInvoices.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.Features.InvoiceItems.Handlers.GetHandlers.GetInvoiceItemsByInvoiceId;

public class GetInvoiceItemsByInvoiceIdHandler : IRequestHandler<GetInvoiceItemsByInvoiceIdRequest, ICollection<InvoiceItemDto>>
{
    private readonly AppDbContext _context;

    public GetInvoiceItemsByInvoiceIdHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ICollection<InvoiceItemDto>> Handle(GetInvoiceItemsByInvoiceIdRequest request, CancellationToken cancellationToken)
    {
        var requestResult = await _context.InvoiceItems
            .Where(it => it.InvoiceId == request.InvoiceId)
            .Select(item => new InvoiceItemDto(
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
                item.GroupName
                ))
            .ToListAsync(cancellationToken);
        return requestResult;
    }
}