namespace Domain.Features.IncomingInvoices.Services;

public interface IInvoiceParserFactory
{
    IInvoiceParser GetParser(string invoiceXml);
}
