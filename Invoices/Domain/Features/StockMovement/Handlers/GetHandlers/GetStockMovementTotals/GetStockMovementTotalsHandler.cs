using Domain.Features.StockMovements.DtoS;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.Features.StockMovements.Handlers.GetHandlers.GetStockMovementTotals;

public class GetStockMovementTotalsHandler : IRequestHandler<GetStockMovementTotalsRequest, List<StockMovementTotalDto>>
{
    private readonly AppDbContext _db;

    public GetStockMovementTotalsHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<StockMovementTotalDto>> Handle(GetStockMovementTotalsRequest request, CancellationToken cancellationToken)
    {
        var query = _db.StockMovements.Include(m => m.Product).AsQueryable();

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

        if (string.Equals(request.GroupBy, "product", StringComparison.OrdinalIgnoreCase))
        {
            return await query
                .GroupBy(m => m.Product != null ? m.Product.Name : "—")
                .Select(g => new StockMovementTotalDto(g.Key, g.Sum(m => m.QuantityChange), g.Count()))
                .ToListAsync(cancellationToken);
        }

        return await query
            .GroupBy(m => m.Reason)
            .Select(g => new StockMovementTotalDto(g.Key.ToString(), g.Sum(m => m.QuantityChange), g.Count()))
            .ToListAsync(cancellationToken);
    }
}
