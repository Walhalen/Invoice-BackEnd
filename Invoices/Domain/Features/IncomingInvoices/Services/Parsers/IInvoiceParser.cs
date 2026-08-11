namespace Domain.Features.IncomingInvoices.Services;

public interface IInvoiceParser
{
    string SupplierCode { get; }
    string SupplierName { get; }

    bool CanParse(string invoiceXml);
    Invoice Parse(string invoiceXml);
}
