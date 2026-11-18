namespace ExpenseParser.Api.Contracts;

public sealed class ParseErrorResponse
{
    public required string Code { get; init; }
    public required string Message { get; init; }
    public string? CorrelationId { get; init; }
}
