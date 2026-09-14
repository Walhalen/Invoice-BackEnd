using Domain.Features.Products.DtoS;
using MediatR;

namespace WebAPI.Handlers.GetHandlers;

public record GetProductsRequest : IRequest<List<ProductListItemDto>>;
