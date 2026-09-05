using Aggregation.Api.Application.Services;
using Aggregation.Api.Domain.Enums;
using Xunit;

namespace Aggregation.Api.Tests;

public class CategorizationServiceTests
{
    private readonly CategorizationService _sut = new();

    [Theory]
    [InlineData("CHECKERS SANDTON CITY", TransactionCategory.Groceries)]
    [InlineData("UBER TRIP", TransactionCategory.Transport)]
    [InlineData("NETFLIX SUBSCRIPTION", TransactionCategory.Entertainment)]
    [InlineData("ESKOM PREPAID", TransactionCategory.Utilities)]
    [InlineData("SALARY - ACME CORP", TransactionCategory.Income)]
    [InlineData("TAKEALOT.COM", TransactionCategory.Shopping)]
    [InlineData("GYM MEMBERSHIP - VIRGIN ACTIVE", TransactionCategory.Fitness)]
    [InlineData("BANK FEE - MONTHLY ACCOUNT FEE", TransactionCategory.Fees)]
    public void Categorize_KnownKeywords_ReturnsExpectedCategory(string description, TransactionCategory expected)
    {
        var result = _sut.Categorize(description);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Categorize_UnknownDescription_ReturnsUncategorized()
    {
        var result = _sut.Categorize("SOME RANDOM MERCHANT XYZ");

        Assert.Equal(TransactionCategory.Uncategorized, result);
    }

    [Fact]
    public void Categorize_EmptyDescription_ReturnsUncategorized()
    {
        var result = _sut.Categorize("");

        Assert.Equal(TransactionCategory.Uncategorized, result);
    }

    [Fact]
    public void Categorize_IsCaseInsensitive()
    {
        var result = _sut.Categorize("checkers sandton city");

        Assert.Equal(TransactionCategory.Groceries, result);
    }
}