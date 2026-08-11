using Domain;
using MediatR;

namespace WebAPI.Handlers.PostHandlers;

public record PostInvoiceRequest(string InvoiceXml) : IRequest<Invoice>;

