using Domain;
using Domain.Features.StockMovements.DtoS;
using MediatR;

namespace Domain.Features.StockMovements.Handlers.GetHandlers.GetStockMovementTotals;

public record GetStockMovementTotalsRequest(
    string? ProductName,
    StockMovementReason? Reason,
    DateTime? DateFrom,
    DateTime? DateTo,
    string GroupBy) : IRequest<List<StockMovementTotalDto>>;
