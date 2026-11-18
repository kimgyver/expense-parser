namespace ExpenseParser.Core.Models;

public sealed class TaxAmounts
{
    public decimal TotalIncludingTax { get; init; }
    public decimal SalesTax { get; init; }
    public decimal TotalExcludingTax { get; init; }
}
