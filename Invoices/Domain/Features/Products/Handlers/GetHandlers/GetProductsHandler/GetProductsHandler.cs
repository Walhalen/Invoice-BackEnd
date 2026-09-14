using Domain;
using Domain.Features.Products.DtoS;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.Handlers.GetHandlers;

public class GetProductsHandler : IRequestHandler<GetProductsRequest, List<ProductListItemDto>>
{
    private readonly AppDbContext _db;

    public GetProductsHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProductListItemDto>> Handle(GetProductsRequest request, CancellationToken cancellationToken)
    {
        return await _db.Products
            .Where(product => product.QuantityOnHand > 0)
            .Select(product => new ProductListItemDto(
                product.Id,
                product.Code,
                product.Name,
                product.IndexCode,
                product.Brand,
                product.GroupName,
                product.LastPurchasePrice,
                product.LastVatRate,
                product.QuantityOnHand))
            .ToListAsync(cancellationToken);
    }
}
