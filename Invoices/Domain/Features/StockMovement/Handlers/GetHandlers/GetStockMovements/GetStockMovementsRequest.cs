using Domain;
using Domain.Features.StockMovements.DtoS;
using MediatR;

namespace Domain.Features.StockMovements.Handlers.GetHandlers.GetStockMovements;

public record GetStockMovementsRequest(
    string? ProductName,
    StockMovementReason? Reason,
    DateTime? DateFrom,
    DateTime? DateTo,
    int Limit) : IRequest<List<StockMovementRowDto>>;
