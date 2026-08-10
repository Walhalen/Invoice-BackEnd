using System.ComponentModel.DataAnnotations.Schema;
using Domain.Features.Products.Entity;

namespace Domain;

// Why the value happened, not just that it happened — lets you report
// "colko съм купил vs. продал vs. коригирал" separately later.
public enum StockMovementReason
{
    Purchase = 1,     // incoming — created from an imported InvoiceItem
    Sale = 2,         // outgoing — used/sold on a repair job
    Adjustment = 3,   // manual correction (e.g. physical inventory count fix)
    Return = 4,       // returned to supplier, or by a customer
    WriteOff = 5,     // damaged / lost / expired
}

// The full history of everything that has ever entered or left the warehouse
// for a given Product. This table is append-only — rows are never edited or
// deleted, so it always answers "какво е влязло/излязло и кога" for reports.
public class StockMovement
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    // SIGNED quantity: positive = added to stock, negative = removed.
    // Chosen over an unsigned Quantity + separate In/Out Type because current
    // balance is then just SUM(QuantityChange) — no CASE WHEN needed in
    // reports. Reason (below) still tells you *why*, which Type alone wouldn't.
    [Column(TypeName = "decimal(10,2)")]
    public decimal QuantityChange { get; set; }

    public StockMovementReason Reason { get; set; }

    // When it actually happened (e.g. the invoice's IssueDate for a Purchase).
    public DateTime MovementDate { get; set; }

    // When this row was written to the database — an audit timestamp, may
    // differ from MovementDate if an invoice is imported days after arrival.
    public DateTime CreatedAt { get; set; }

    // Populated only when Reason == Purchase: which invoice line brought this
    // stock in. Null for Sale/Adjustment/Return/WriteOff — those have no
    // invoice to point to (Sale will eventually point to a future
    // RepairJob/SaleId FK instead, once that module exists).
    public int? InvoiceItemId { get; set; }
    public InvoiceItem? InvoiceItem { get; set; }

    // Running stock balance immediately after this movement. Denormalized on
    // purpose: without it, "what was the stock level on 2026-05-01" requires
    // summing every prior movement for that product every time you ask.
    // Written once at insert time, never recalculated afterwards.
    [Column(TypeName = "decimal(10,2)")]
    public decimal? BalanceAfter { get; set; }

    public string? Note { get; set; }
}