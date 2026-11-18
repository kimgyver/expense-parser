using ExpenseParser.Api.Middleware;

namespace ExpenseParser.Api.Extensions;

public static class CorrelationIdExtensions
{
    public static string? GetCorrelationId(this HttpContext context) =>
        context.Items[CorrelationIdMiddleware.HeaderName] as string;
}
