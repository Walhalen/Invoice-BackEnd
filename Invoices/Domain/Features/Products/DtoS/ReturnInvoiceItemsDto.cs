namespace Domain.Features.Products.DtoS;

public record ReturnInvoiceItemsDto(List<int> InvoiceItemIds, string? Note);
