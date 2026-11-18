using ExpenseParser.Core.Constants;
using ExpenseParser.Core.Models;
using ExpenseParser.Core.Pipeline;

namespace ExpenseParser.Core.Services;

public sealed class ExpenseParseService : IExpenseParseService
{
    public ExpenseParseResult Parse(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ParseException(ParseErrorCode.EmptyText, "Input text is required.");
        }

        if (text.Length > ParseLimits.MaxTextLength)
        {
            throw new ParseException(
                ParseErrorCode.TextTooLong,
                $"Input text exceeds the maximum length of {ParseLimits.MaxTextLength} characters.");
        }

        // Pipeline: extract → validate → enrich (tax)
        var extracted = MarkupExtractor.Extract(text);
        var validated = FieldValidator.Validate(extracted);
        var tax = TaxCalculator.CalculateFromInclusiveTotal(validated.TotalIncludingTax);

        return new ExpenseParseResult
        {
            Vendor = validated.Vendor,
            Description = validated.Description,
            Date = validated.Date,
            CostCentre = validated.CostCentre,
            PaymentMethod = validated.PaymentMethod,
            TotalIncludingTax = tax.TotalIncludingTax,
            SalesTax = tax.SalesTax,
            TotalExcludingTax = tax.TotalExcludingTax
        };
    }
}
