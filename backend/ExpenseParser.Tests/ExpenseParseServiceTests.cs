using ExpenseParser.Core.Constants;
using ExpenseParser.Core.Models;
using ExpenseParser.Core.Services;

namespace ExpenseParser.Tests;

public class ExpenseParseServiceTests
{
    private readonly ExpenseParseService _sut = new();

    [Fact]
    public void Parse_EmailSample_ExtractsFieldsAndTax()
    {
        var emailSample = """
            Hi Patricia,
            Please create an expense claim for the below. Relevant details are marked
            up as requested…

            <expense><cost_centre>DEV632</cost_centre><total>35,000</total><payment_method>personal
            card</payment_method></expense>
            From: William Steele
            Sent: Friday, 16 June 2022 10:32 AM
            To: Maria Washington
            Subject: test
            Hi Maria,
            Please create a reservation for 10 at the <vendor>Seaside Steakhouse</vendor> for our
            <description>development team’s project end celebration</description> on <date>27 April
            2022</date> at 7.30pm.
            Regards,
            William
            """;

        var result = _sut.Parse(emailSample);

        Assert.Equal("Seaside Steakhouse", result.Vendor);
        Assert.Equal("development team’s project end celebration", result.Description);
        Assert.Equal("27 April 2022", result.Date);
        Assert.Equal("DEV632", result.CostCentre);
        Assert.Equal("personal card", result.PaymentMethod);
        Assert.Equal(35000m, result.TotalIncludingTax);
        Assert.Equal(4565.22m, result.SalesTax);
        Assert.Equal(30434.78m, result.TotalExcludingTax);
    }

    [Fact]
    public void Parse_MissingCostCentre_DefaultsToUnknown()
    {
        var text = """
            <expense>
              <total>89.99</total>
              <payment_method>personal card</payment_method>
            </expense>
            """;

        var result = _sut.Parse(text);

        Assert.Equal(ExpenseDefaults.CostCentre, result.CostCentre);
        Assert.Equal(89.99m, result.TotalIncludingTax);
    }

    [Fact]
    public void Parse_MissingTotal_Throws()
    {
        var text = """
            <expense>
              <cost_centre>OPS200</cost_centre>
              <payment_method>personal card</payment_method>
            </expense>
            """;

        var ex = Assert.Throws<ParseException>(() => _sut.Parse(text));
        Assert.Equal(ParseErrorCode.MissingTotal, ex.Code);
    }

    [Fact]
    public void Parse_InvalidTotal_Throws()
    {
        var text = """
            <expense>
              <cost_centre>DEV632</cost_centre>
              <total>35,000a</total>
            </expense>
            """;

        var ex = Assert.Throws<ParseException>(() => _sut.Parse(text));
        Assert.Equal(ParseErrorCode.InvalidTotal, ex.Code);
    }

    [Fact]
    public void Parse_UnbalancedTag_Throws()
    {
        var text = """
            Reservation at <vendor>Seaside Steakhouse for the team on <date>27 April 2022</date>.
            <expense><cost_centre>DEV632</cost_centre><total>350</total></expense>
            """;

        var ex = Assert.Throws<ParseException>(() => _sut.Parse(text));
        Assert.Equal(ParseErrorCode.UnbalancedTag, ex.Code);
    }

    [Fact]
    public void Parse_EmptyText_Throws()
    {
        var ex = Assert.Throws<ParseException>(() => _sut.Parse("   "));
        Assert.Equal(ParseErrorCode.EmptyText, ex.Code);
    }
}
