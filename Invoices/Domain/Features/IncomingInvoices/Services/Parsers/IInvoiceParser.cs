namespace Domain.Features.IncomingInvoices.Services;

public interface IInvoiceParser
{
    bool CanParse(string invoiceXml);
    Invoice Parse(string invoiceXml);
}
