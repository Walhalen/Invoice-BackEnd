using MediatR;

namespace Domain.Features.Products.Handlers.PostHandlers.ReturnInvoiceItemsHandler;

public record ReturnInvoiceItemsRequest(int ProductId, List<int> InvoiceItemIds, string? Note)
    : IRequest<ReturnInvoiceItemsResult>;

public record ReturnInvoiceItemsResult(bool Found, bool Success, string? Error, decimal QuantityOnHand);
