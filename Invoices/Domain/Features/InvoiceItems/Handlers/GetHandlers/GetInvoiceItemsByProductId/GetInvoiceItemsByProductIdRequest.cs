using Domain.Features.InvoiceItems.Dtos;
using MediatR;

namespace Domain.Features.InvoiceItems.Handlers.GetHandlers.GetInvoiceItemsByProductId;

public record GetInvoiceItemsByProductIdRequest(int ProductId) : IRequest<List<ReturnableInvoiceItemDto>>;
