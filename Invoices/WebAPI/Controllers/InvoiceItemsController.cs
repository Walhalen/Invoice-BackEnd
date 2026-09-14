using Domain.Features.IncomingInvoices.Dtos;
using Domain.Features.InvoiceItems.Dtos;
using Domain.Features.InvoiceItems.Handlers.GetHandlers.GetInvoiceItemsByInvoiceId;
using Domain.Features.InvoiceItems.Handlers.GetHandlers.GetInvoiceItemsByProductId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class InvoiceItemsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvoiceItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("byInvoice/{invoiceId:int}")]
    public async Task<ActionResult<ICollection<InvoiceItemDto>>> GetByInvoiceId(int invoiceId)
    {
        var items = await _mediator.Send(new GetInvoiceItemsByInvoiceIdRequest(invoiceId));
        return Ok(items);
    }

    [HttpGet("byProduct/{productId:int}")]
    public async Task<ActionResult<List<ReturnableInvoiceItemDto>>> GetByProductId(int productId)
    {
        var items = await _mediator.Send(new GetInvoiceItemsByProductIdRequest(productId));
        return Ok(items);
    }
}
