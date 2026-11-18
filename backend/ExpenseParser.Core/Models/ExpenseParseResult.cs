namespace ExpenseParser.Core.Models;

public sealed class ExpenseParseResult
{
    public string? Vendor { get; init; }
    public string? Description { get; init; }
    public string? Date { get; init; }
    public required string CostCentre { get; init; }
    public string? PaymentMethod { get; init; }
    public decimal TotalIncludingTax { get; init; }
    public decimal SalesTax { get; init; }
    public decimal TotalExcludingTax { get; init; }
}
