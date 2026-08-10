using Domain;
using MediatR;

namespace WebAPI.Handlers.GetHandlers;

public record GetInvoiceByIdRequest(int Id) : IRequest<Invoice?>;
    