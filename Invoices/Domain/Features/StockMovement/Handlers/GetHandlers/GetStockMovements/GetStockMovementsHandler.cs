using Domain.Features.StockMovements.DtoS;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.Features.StockMovements.Handlers.GetHandlers.GetStockMovements;

public class GetStockMovementsHandler : IRequestHandler<GetStockMovementsRequest, List<StockMovementRowDto>>
{
    private readonly AppDbContext _db;

    public GetStockMovementsHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<StockMovementRowDto>> Handle(GetStockMovementsRequest request, CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(request.Limit <= 0 ? 50 : request.Limit, 1, 200);

        var query = _db.StockMovements
            .Include(m => m.Product)
            .Include(m => m.InvoiceItem)
            .ThenInclude(item => item!.Invoice)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.ProductName))
        {
            var term = request.ProductName.Trim();
            query = query.Where(m => m.Product != null
                && (m.Product.Name.Contains(term) || m.Product.Code.Contains(term)));
        }

        if (request.Reason.HasValue)
        {
            query = query.Where(m => m.Reason == request.Reason.Value);
        }

        if (request.DateFrom.HasValue)
        {
            query = query.Where(m => m.MovementDate >= request.DateFrom.Value);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(m => m.MovementDate <= request.DateTo.Value);
        }

        return await query
            .OrderByDescending(m => m.MovementDate)
            .Take(limit)
            .Select(m => new StockMovementRowDto(
                m.MovementDate,
                m.Product != null ? m.Product.Name : string.Empty,
                m.Product != null ? m.Product.Code : string.Empty,
                m.Reason.ToString(),
                m.QuantityChange,
                m.BalanceAfter,
                m.Note,
                m.InvoiceItem != null && m.InvoiceItem.Invoice != null ? m.InvoiceItem.Invoice.Number : null))
            .ToListAsync(cancellationToken);
    }
}
