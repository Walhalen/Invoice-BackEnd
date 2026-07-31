using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly AppDbContext _db;

    public InvoicesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Invoice>>> Get()
    {
        return await _db.Invoices.ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Invoice>> Get(int id)
    {
        var invoice = await _db.Invoices.FindAsync(id);
        if (invoice is null)
        {
            return NotFound();
        }

        return invoice;
    }

    [HttpPost]
    public async Task<ActionResult<Invoice>> Post(Invoice invoice)
    {
        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = invoice.Id }, invoice);
    }
}
