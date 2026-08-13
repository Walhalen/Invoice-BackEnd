using Domain.Features.IncomingInvoices.Dtos;
using MediatR;

namespace Domain.Features.InvoiceItems.Handlers.GetHandlers.GetInvoiceItemsByInvoiceId;

public record GetInvoiceItemsByInvoiceIdRequest(int InvoiceId) : IRequest<ICollection<InvoiceItemDto>>; 