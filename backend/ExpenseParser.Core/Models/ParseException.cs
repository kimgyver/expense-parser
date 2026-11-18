namespace ExpenseParser.Core.Models;

public sealed class ParseException(string code, string message) : Exception(message)
{
    public string Code { get; } = code;
}
