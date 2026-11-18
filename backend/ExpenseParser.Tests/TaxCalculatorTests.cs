using ExpenseParser.Core.Pipeline;

namespace ExpenseParser.Tests;

public class TaxCalculatorTests
{
    [Theory]
    [InlineData(115, 15, 100)]
    [InlineData(35000, 4565.22, 30434.78)]
    public void CalculateFromInclusiveTotal_UsesGst15(
        decimal inclusive,
        decimal expectedTax,
        decimal expectedExclusive)
    {
        var result = TaxCalculator.CalculateFromInclusiveTotal(inclusive);

        Assert.Equal(expectedTax, result.SalesTax);
        Assert.Equal(expectedExclusive, result.TotalExcludingTax);
    }
}
