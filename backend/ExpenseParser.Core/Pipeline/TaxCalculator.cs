using ExpenseParser.Core.Constants;
using ExpenseParser.Core.Models;

namespace ExpenseParser.Core.Pipeline;

/// <summary>
/// Derives sales tax from a tax-inclusive total.
/// Assumption: NZ GST 15% (total = exclusive * 1.15).
/// </summary>
public static class TaxCalculator
{
    public static TaxAmounts CalculateFromInclusiveTotal(decimal totalIncludingTax)
    {
        var totalExcludingTax = RoundMoney(totalIncludingTax / (1m + TaxDefaults.SalesTaxRate));
        var salesTax = RoundMoney(totalIncludingTax - totalExcludingTax);

        return new TaxAmounts
        {
            TotalIncludingTax = RoundMoney(totalIncludingTax),
            SalesTax = salesTax,
            TotalExcludingTax = totalExcludingTax
        };
    }

    private static decimal RoundMoney(decimal value) =>
        Math.Round(value, TaxDefaults.MoneyDecimalPlaces);
}
