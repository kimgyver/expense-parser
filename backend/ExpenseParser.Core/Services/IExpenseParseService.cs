using ExpenseParser.Core.Models;

namespace ExpenseParser.Core.Services;

public interface IExpenseParseService
{
    ExpenseParseResult Parse(string text);
}
