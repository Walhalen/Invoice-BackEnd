using Domain;
using MediatR;

namespace WebAPI.Handlers.PostHandlers;

public record PostInvoiceRequest(Invoice Invoice) : IRequest<Invoice>;
