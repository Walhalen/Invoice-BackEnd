using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

// Mapped from the <nag> node of the supplier's VFP XML export.
public class Invoice
{
    public int Id { get; set; }

    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public string PayerCode { get; set; } = string.Empty;      // kh_kod_pla
    public string ReceiverCode { get; set; } = string.Empty;   // kh_kod_odb
    public string PaymentMethodCode { get; set; } = string.Empty; // spo_pl_kod
    public string DocumentTypeCode { get; set; } = string.Empty;  // rod_d_kod
    public string CurrencyCode { get; set; } = string.Empty;   // wal_kod
    public string Number { get; set; } = string.Empty;         // nr / numer

    public DateTime IssueDate { get; set; }   // dat_w
    public DateTime DueDate { get; set; }     // dat_pl

    [Column(TypeName = "decimal(11,2)")]
    public decimal NetAmount { get; set; }    // war_n

    [Column(TypeName = "decimal(11,2)")]
    public decimal GrossAmount { get; set; }  // war_b

    [Column(TypeName = "decimal(11,2)")]
    public decimal VatAmount { get; set; }    // war_v

    [Column(TypeName = "decimal(11,2)")]
    public decimal? ForeignCurrencyAmount { get; set; } // war_wal

    [Column(TypeName = "decimal(11,2)")]
    public decimal? OutstandingAmount { get; set; }     // war_wn

    [Column(TypeName = "decimal(11,2)")]
    public decimal? PaidAmount { get; set; }  // war_zapl

    public string? WarehouseCode { get; set; } // mag_fir_ko

    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}
