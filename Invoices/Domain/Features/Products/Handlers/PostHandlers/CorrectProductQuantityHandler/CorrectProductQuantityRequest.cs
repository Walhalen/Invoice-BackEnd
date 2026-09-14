using Domain;
using MediatR;

namespace Domain.Features.Products.Handlers.PostHandlers.CorrectProductQuantityHandler;

public record CorrectProductQuantityRequest(int ProductId, StockMovementReason Reason, decimal Quantity, string? Note)
    : IRequest<CorrectProductQuantityResult>;

public record CorrectProductQuantityResult(bool Found, bool Success, string? Error, decimal QuantityOnHand);
