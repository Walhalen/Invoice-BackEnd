using Domain.Features.IncomingInvoices.Dtos;
using MediatR;

namespace WebAPI.Handlers.GetHandlers;

public record GetInvoiceByIdRequest(int Id) : IRequest<InvoiceDto?>;