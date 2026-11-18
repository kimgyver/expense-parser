namespace ExpenseParser.Core.Constants;

public static class ParseErrorCode
{
    public const string EmptyText = "EMPTY_TEXT";
    public const string TextTooLong = "TEXT_TOO_LONG";
    public const string UnbalancedTag = "UNBALANCED_TAG";
    public const string MissingTotal = "MISSING_TOTAL";
    public const string InvalidTotal = "INVALID_TOTAL";
}
