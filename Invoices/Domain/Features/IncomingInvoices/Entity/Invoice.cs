using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

// Mapped from the <nag> node of the supplier's VFP XML export.
public class Invoice
{
    public int Id { get; set; }

    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public string PayerCode { get; set; } = string.Empty;      
    public string ReceiverCode { get; set; } = string.Empty;   
    public string PaymentMethodCode { get; set; } = string.Empty; 
    public string DocumentTypeCode { get; set; } = string.Empty;  
    public string CurrencyCode { get; set; } = string.Empty;   
    public string Number { get; set; } = string.Empty;         

    public DateTime IssueDate { get; set; }   
    public DateTime DueDate { get; set; }     

    [Column(TypeName = "decimal(11,2)")]
    public decimal NetAmount { get; set; }    

    [Column(TypeName = "decimal(11,2)")]
    public decimal GrossAmount { get; set; }  

    [Column(TypeName = "decimal(11,2)")]
    public decimal VatAmount { get; set; }    

    [Column(TypeName = "decimal(11,2)")]
    public decimal? ForeignCurrencyAmount { get; set; } 

    [Column(TypeName = "decimal(11,2)")]
    public decimal? OutstandingAmount { get; set; }     

    [Column(TypeName = "decimal(11,2)")]
    public decimal? PaidAmount { get; set; }  

    public string? WarehouseCode { get; set; } 

    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}
