using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.Features.Products.Handlers.PostHandlers.CorrectProductQuantityHandler;

public class 
    
    CorrectProductQuantityHandler : IRequestHandler<CorrectProductQuantityRequest, CorrectProductQuantityResult>
{
    // Purchase/Sale are only ever written by the invoice flows, never by a manual correction.
    private static readonly StockMovementReason[] AllowedReasons =
    {
        StockMovementReason.Adjustment,
        StockMovementReason.Return,
        StockMovementReason.WriteOff
    };

    private readonly AppDbContext _db;

    public CorrectProductQuantityHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<CorrectProductQuantityResult> Handle(CorrectProductQuantityRequest request, CancellationToken cancellationToken)
    {
        if (!AllowedReasons.Contains(request.Reason))
        {
            return new CorrectProductQuantityResult(true, false, "Невалидна причина за корекция.", 0);
        }

        if (request.Quantity <= 0)
        {
            return new CorrectProductQuantityResult(true, false, "Количеството трябва да е по-голямо от 0.", 0);
        }

        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
        if (product is null)
        {
            return new CorrectProductQuantityResult(false, false, null, 0);
        }

        if (product.QuantityOnHand - request.Quantity < 0)
        {
            return new CorrectProductQuantityResult(true, false, "Наличността не може да стане отрицателна.", product.QuantityOnHand);
        }

        product.QuantityOnHand -= request.Quantity;
        product.UpdatedAt = DateTime.UtcNow;

        _db.StockMovements.Add(new StockMovement
        {
            Product = product,
            QuantityChange = -request.Quantity,
            Reason = request.Reason,
            MovementDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            BalanceAfter = product.QuantityOnHand,
            Note = request.Note
        });

        await _db.SaveChangesAsync(cancellationToken);

        return new CorrectProductQuantityResult(true, true, null, product.QuantityOnHand);
    }
}
