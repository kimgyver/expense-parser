using System.Globalization;
using ExpenseParser.Core.Constants;
using ExpenseParser.Core.Models;

namespace ExpenseParser.Core.Pipeline;

public static class FieldValidator
{
    public static ValidatedExpenseFields Validate(RawExpenseFields fields)
    {
        if (string.IsNullOrWhiteSpace(fields.TotalRaw))
        {
            throw new ParseException(
                ParseErrorCode.MissingTotal,
                $"Missing required <{MarkupTags.Total}> tag.");
        }

        if (!TryParseAmount(fields.TotalRaw, out var totalIncludingTax))
        {
            throw new ParseException(
                ParseErrorCode.InvalidTotal,
                $"Invalid <{MarkupTags.Total}> value '{fields.TotalRaw}'.");
        }

        var costCentre = string.IsNullOrWhiteSpace(fields.CostCentre)
            ? ExpenseDefaults.CostCentre
            : fields.CostCentre.Trim();

        return new ValidatedExpenseFields
        {
            Vendor = NullIfWhiteSpace(fields.Vendor),
            Description = NullIfWhiteSpace(fields.Description),
            Date = NullIfWhiteSpace(fields.Date),
            CostCentre = costCentre,
            PaymentMethod = NullIfWhiteSpace(fields.PaymentMethod),
            TotalIncludingTax = totalIncludingTax
        };
    }

    public static bool TryParseAmount(string raw, out decimal amount)
    {
        var normalized = raw.Replace(",", string.Empty, StringComparison.Ordinal).Trim();
        return decimal.TryParse(
            normalized,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out amount);
    }

    private static string? NullIfWhiteSpace(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
