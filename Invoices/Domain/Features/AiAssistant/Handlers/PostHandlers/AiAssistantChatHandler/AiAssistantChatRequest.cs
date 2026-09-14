using Domain.Features.AiAssistant.DtoS;
using MediatR;

namespace Domain.Features.AiAssistant.Handlers.PostHandlers.AiAssistantChatHandler;

public record AiAssistantChatRequest(string Message, List<AiAssistantChatHistoryItemDto> History)
    : IRequest<AiAssistantChatResponseDto>;
