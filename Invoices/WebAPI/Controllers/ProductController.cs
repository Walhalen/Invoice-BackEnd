using Domain.Features.Products.DtoS;
using Domain.Features.Products.Handlers.PostHandlers.CorrectProductQuantityHandler;
using Domain.Features.Products.Handlers.PostHandlers.ReturnInvoiceItemsHandler;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Handlers.GetHandlers;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;


    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductListItemDto>>> GetProducts()
    {
        return await _mediator.Send(new GetProductsRequest());
    }

    [HttpPost("{id:int}/correction")]
    public async Task<IActionResult> CorrectQuantity(int id, [FromBody] CorrectProductQuantityDto dto)
    {
        var result = await _mediator.Send(new CorrectProductQuantityRequest(id, dto.Reason, dto.Quantity, dto.Note));

        if (!result.Found)
        {
            return NotFound();
        }

        if (!result.Success)
        {
            return BadRequest(result.Error);
        }

        return Ok(new { quantityOnHand = result.QuantityOnHand });
    }

    [HttpPost("{id:int}/return")]
    public async Task<IActionResult> ReturnInvoiceItems(int id, [FromBody] ReturnInvoiceItemsDto dto)
    {
        var result = await _mediator.Send(new ReturnInvoiceItemsRequest(id, dto.InvoiceItemIds, dto.Note));

        if (!result.Found)
        {
            return NotFound();
        }

        if (!result.Success)
        {
            return BadRequest(result.Error);
        }

        return Ok(new { quantityOnHand = result.QuantityOnHand });
    }
}
