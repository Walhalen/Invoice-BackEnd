using System.Text;
using Domain;
using Domain.Features.IncomingInvoices.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Handlers.DeleteHandlers;
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
    public async Task<ActionResult<IEnumerable<InvoiceListItemDto>>> Get()
    {
        return await _db.Invoices
            .Select(invoice => new InvoiceListItemDto(
                invoice.Id,
                invoice.Supplier != null ? invoice.Supplier.Name : string.Empty,
                invoice.Number,
                invoice.IssueDate,
                invoice.DueDate,
                invoice.NetAmount,
                invoice.GrossAmount,
                invoice.VatAmount,
                invoice.OutstandingAmount,
                invoice.PaidAmount))
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InvoiceDto>> Get(int id)
    {
        var invoice = await _mediator.Send(new GetInvoiceByIdRequest(id));
        if (invoice is null)
        {
            return NotFound();
        }

        return invoice;
    }

    [HttpPost]
    public async Task<ActionResult<bool>> Post(IFormFile xml)
    {
        // The supplier's VFP export has no <?xml encoding=...?> declaration but is actually Windows-1251.
        using var reader = new StreamReader(xml.OpenReadStream(), Encoding.GetEncoding(1251));
        var invoiceXml = await reader.ReadToEndAsync();

        var created = await _mediator.Send(new PostInvoiceRequest(invoiceXml));
        return true;
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _mediator.Send(new DeleteInvoiceRequest(id));
        if (!deleted)
        {
            return NotFound();
        }

        return Ok(true); 
    }
}   
