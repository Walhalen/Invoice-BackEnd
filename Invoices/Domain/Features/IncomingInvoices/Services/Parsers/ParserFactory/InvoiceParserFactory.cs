namespace Domain.Features.IncomingInvoices.Services;

public class InvoiceParserFactory : IInvoiceParserFactory
{
    private readonly IEnumerable<IInvoiceParser> _parsers;

    public InvoiceParserFactory(IEnumerable<IInvoiceParser> parsers)
    {
        _parsers = parsers;
    }

    public IInvoiceParser GetParser(string invoiceXml)
    {
        foreach (var parser in _parsers)
        {
            if (parser.CanParse(invoiceXml))
            {
                return parser;
            }
        }

        throw new NotSupportedException("No registered parser recognizes this invoice format.");
    }
}
