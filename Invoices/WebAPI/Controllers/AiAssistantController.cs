using Domain.Features.AiAssistant.DtoS;
using Domain.Features.AiAssistant.Handlers.PostHandlers.AiAssistantChatHandler;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AiAssistantController : ControllerBase
{
    private readonly IMediator _mediator;

    public AiAssistantController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("chat")]
    public async Task<ActionResult<AiAssistantChatResponseDto>> Chat([FromBody] AiAssistantChatRequestBody body)
    {
        var result = await _mediator.Send(new AiAssistantChatRequest(body.Message, body.History));
        return Ok(result);
    }
}

public record AiAssistantChatRequestBody(string Message, List<AiAssistantChatHistoryItemDto> History);
