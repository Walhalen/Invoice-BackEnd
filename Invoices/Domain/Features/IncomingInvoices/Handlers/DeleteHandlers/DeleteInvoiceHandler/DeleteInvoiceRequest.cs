using MediatR;

namespace WebAPI.Handlers.DeleteHandlers;

public record DeleteInvoiceRequest(int Id) : IRequest<bool>;
