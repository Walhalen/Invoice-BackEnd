using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Handlers.GetHandlers;
using WebAPI.Handlers.PostHandlers;


namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMediator _mediator;

    public InvoicesController(AppDbContext db, IMediator mediator)
    {
        _db = db;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Invoice>>> Get()
    {
        return await _db.Invoices.ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Invoice>> Get(int id)
    {
        var invoice = await _mediator.Send(new GetInvoiceByIdRequest(id));
        if (invoice is null)
        {
            return NotFound();
        }

        return invoice;
    }

    [HttpPost]
    public async Task<ActionResult<Invoice>> Post(Invoice invoice)
    {
        var created = await _mediator.Send(new PostInvoiceRequest(invoice));
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }
}
