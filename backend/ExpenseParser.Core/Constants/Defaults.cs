namespace ExpenseParser.Core.Constants;

public static class TaxDefaults
{
    /// <summary>NZ GST rate. Assumption: tax-inclusive total.</summary>
    public const decimal SalesTaxRate = 0.15m;

    public const int MoneyDecimalPlaces = 2;
}

public static class ExpenseDefaults
{
    public const string CostCentre = "UNKNOWN";
}

public static class ParseLimits
{
    public const int MaxTextLength = 100_000;
}
