namespace ExpenseParser.Core.Models;

public sealed class RawExpenseFields
{
    public string? Vendor { get; init; }
    public string? Description { get; init; }
    public string? Date { get; init; }
    public string? CostCentre { get; init; }
    public string? PaymentMethod { get; init; }
    public string? TotalRaw { get; init; }
}
