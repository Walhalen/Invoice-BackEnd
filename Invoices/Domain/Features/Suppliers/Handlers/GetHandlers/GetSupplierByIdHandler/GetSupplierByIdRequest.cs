using Domain.Features.Suppliers.Dtos;
using MediatR;

namespace Domain.Features.Suppliers.Handlers.GetHandlers.GetSupplierByIdHandler;

public record GetSupplierByIdRequest(int Id) : IRequest<SupplierDto?>;
