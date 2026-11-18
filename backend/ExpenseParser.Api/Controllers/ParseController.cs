using ExpenseParser.Api.Contracts;
using ExpenseParser.Api.Extensions;
using ExpenseParser.Core.Models;
using ExpenseParser.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseParser.Api.Controllers;

[ApiController]
[Route("api/v1")]
public sealed class ParseController(
    IExpenseParseService parseService,
    ILogger<ParseController> logger) : ControllerBase
{
    [HttpPost("parse")]
    [ProducesResponseType(typeof(ExpenseParseResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ParseErrorResponse), StatusCodes.Status400BadRequest)]
    public ActionResult<ExpenseParseResult> Parse([FromBody] ParseRequest request)
    {
        var correlationId = HttpContext.GetCorrelationId();

        try
        {
            var result = parseService.Parse(request.Text ?? string.Empty);
            logger.LogInformation("Parse succeeded. CorrelationId={CorrelationId}", correlationId);
            return Ok(result);
        }
        catch (ParseException ex)
        {
            logger.LogWarning(
                "Parse rejected. Code={Code} Message={Message} CorrelationId={CorrelationId}",
                ex.Code,
                ex.Message,
                correlationId);

            return BadRequest(new ParseErrorResponse
            {
                Code = ex.Code,
                Message = ex.Message,
                CorrelationId = correlationId
            });
        }
    }
}
