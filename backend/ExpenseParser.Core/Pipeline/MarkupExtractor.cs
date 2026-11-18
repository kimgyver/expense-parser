using System.Text.RegularExpressions;
using ExpenseParser.Core.Constants;
using ExpenseParser.Core.Models;

namespace ExpenseParser.Core.Pipeline;

public static partial class MarkupExtractor
{
    [GeneratedRegex(@"<(?!/)([A-Za-z_][\w\-]*)\s*>")]
    private static partial Regex OpeningTagRegex();

    [GeneratedRegex(@"</([A-Za-z_][\w\-]*)\s*>")]
    private static partial Regex ClosingTagRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();

    public static RawExpenseFields Extract(string text)
    {
        EnsureTagsAreBalanced(text);

        return new RawExpenseFields
        {
            Vendor = ExtractTagValue(text, MarkupTags.Vendor),
            Description = ExtractTagValue(text, MarkupTags.Description),
            Date = ExtractTagValue(text, MarkupTags.Date),
            CostCentre = ExtractTagValue(text, MarkupTags.CostCentre),
            PaymentMethod = ExtractTagValue(text, MarkupTags.PaymentMethod),
            TotalRaw = ExtractTagValue(text, MarkupTags.Total)
        };
    }

    public static void EnsureTagsAreBalanced(string text)
    {
        var openings = BuildTagCounts(OpeningTagRegex(), text);
        var closings = BuildTagCounts(ClosingTagRegex(), text);
        var tagNames = openings.Keys.Union(closings.Keys, StringComparer.OrdinalIgnoreCase);

        foreach (var tagName in tagNames)
        {
            openings.TryGetValue(tagName, out var openCount);
            closings.TryGetValue(tagName, out var closeCount);
            if (openCount == closeCount)
            {
                continue;
            }

            throw new ParseException(
                ParseErrorCode.UnbalancedTag,
                openCount > closeCount
                    ? $"Opening tag <{tagName}> has no matching closing tag."
                    : $"Closing tag </{tagName}> has no matching opening tag.");
        }
    }

    private static Dictionary<string, int> BuildTagCounts(Regex regex, string text)
    {
        var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (Match match in regex.Matches(text))
        {
            var tagName = match.Groups[1].Value;
            counts.TryGetValue(tagName, out var count);
            counts[tagName] = count + 1;
        }

        return counts;
    }

    private static string? ExtractTagValue(string text, string tagName)
    {
        // Find first <tagName>...</tagName> (case-insensitive, may span lines).
        var open = $"<{Regex.Escape(tagName)}\\s*>";
        var close = $"</{Regex.Escape(tagName)}\\s*>";
        var match = Regex.Match(
            text,
            open + "(.*?)" + close,
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        if (!match.Success)
        {
            return null; // tag missing → FieldValidator decides if that is an error
        }

        var inner = match.Groups[1].Value; // text between the tags
        return WhitespaceRegex().Replace(inner, " ").Trim();
    }
}

