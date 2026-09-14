using Domain.Features.Suppliers.Dtos;
using Domain.Features.Suppliers.Handlers.GetHandlers.GetSupplierByIdHandler;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SuppliersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SupplierDto>> Get(int id)
    {
        var supplier = await _mediator.Send(new GetSupplierByIdRequest(id));
        if (supplier is null)
        {
            return NotFound();
        }

        return supplier;
    }
}
