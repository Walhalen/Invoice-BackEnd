using System.Globalization;
using System.Xml.Linq;

namespace Domain.Features.IncomingInvoices.Services;

public class InterCarsInvoiceParser : IInvoiceParser
{
    public bool CanParse(string invoiceXml)
    {
        try
        {
            var root = XDocument.Parse(invoiceXml).Root;
            if (root is null || root.Name.LocalName != "VFPDataSet")
            {
                return false;
            }

            var nag = root.Elements().FirstOrDefault(e => e.Name.LocalName == "nag");
            var poz = root.Elements().FirstOrDefault(e => e.Name.LocalName == "poz");

            return nag is not null
                   && poz is not null
                   && nag.Elements().Any(e => e.Name.LocalName == "kh_kod_pla")
                   && poz.Elements().Any(e => e.Name.LocalName == "tow_kod");
        }
        catch (System.Xml.XmlException)
        {
            return false;
        }
    }

    public Invoice Parse(string invoiceXml)
    {
        var root = XDocument.Parse(invoiceXml).Root
                   ?? throw new InvalidOperationException("Invoice XML has no root element.");

        var nag = root.Elements().First(e => e.Name.LocalName == "nag");

        var invoice = new Invoice
        {
            PayerCode = GetString(nag, "kh_kod_pla") ?? string.Empty,
            ReceiverCode = GetString(nag, "kh_kod_odb") ?? string.Empty,
            PaymentMethodCode = GetString(nag, "spo_pl_kod") ?? string.Empty,
            DocumentTypeCode = GetString(nag, "rod_d_kod") ?? string.Empty,
            CurrencyCode = GetString(nag, "wal_kod") ?? string.Empty,
            Number = GetString(nag, "numer") ?? string.Empty,
            IssueDate = GetDate(nag, "dat_w"),
            DueDate = GetDate(nag, "dat_pl"),
            NetAmount = GetDecimal(nag, "war_n") ?? 0m,
            GrossAmount = GetDecimal(nag, "war_b") ?? 0m,
            VatAmount = GetDecimal(nag, "war_v") ?? 0m,
            ForeignCurrencyAmount = GetDecimal(nag, "war_wal"),
            OutstandingAmount = GetDecimal(nag, "war_wn"),
            PaidAmount = GetDecimal(nag, "war_zapl"),
            WarehouseCode = GetString(nag, "mag_fir_ko"),
        };

        foreach (var poz in root.Elements().Where(e => e.Name.LocalName == "poz"))
        {
            invoice.Items.Add(new InvoiceItem
            {
                LineNumber = (int)(GetDecimal(poz, "lp") ?? 0m),
                ProductCode = GetString(poz, "tow_kod") ?? string.Empty,
                WarehouseNumber = (int)(GetDecimal(poz, "magazyn") ?? 0m),
                SubWarehouseNumber = (int)(GetDecimal(poz, "podmaga") ?? 0m),
                Brand = GetString(poz, "marka"),
                IndexCode = GetString(poz, "indeks"),
                PersonalCode = GetString(poz, "pesel"),
                Name = GetString(poz, "nazwa") ?? string.Empty,
                Description = GetString(poz, "opis"),
                Quantity = GetDecimal(poz, "ilosc") ?? 0m,
                UnitPrice = GetDecimal(poz, "cena") ?? 0m,
                VatRate = GetDecimal(poz, "vat") ?? 0m,
                Sww = GetString(poz, "sww"),
                GroupName = GetString(poz, "grunaz"),
            });
        }

        return invoice;
    }

    private static string? GetString(XElement parent, string name)
    {
        var element = parent.Elements().FirstOrDefault(e => e.Name.LocalName == name);
        if (element is null || IsNil(element))
        {
            return null;
        }

        return element.Value;
    }

    private static decimal? GetDecimal(XElement parent, string name)
    {
        var value = GetString(parent, name);
        return value is null ? null : decimal.Parse(value, CultureInfo.InvariantCulture);
    }

    private static DateTime GetDate(XElement parent, string name)
    {
        var value = GetString(parent, name);
        return value is null ? default : DateTime.Parse(value, CultureInfo.InvariantCulture);
    }

    private static bool IsNil(XElement element)
    {
        var nilAttribute = element.Attribute(XName.Get("nil", "http://www.w3.org/2001/XMLSchema-instance"));
        return nilAttribute is not null && string.Equals(nilAttribute.Value, "true", StringComparison.OrdinalIgnoreCase);
    }
}