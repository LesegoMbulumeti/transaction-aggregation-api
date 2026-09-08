using Aggregation.Api.Application.Services;
using Aggregation.Api.Domain.Entities;
using Aggregation.Api.Domain.Enums;
using Aggregation.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Aggregation.Api.Tests;

public class AggregationServiceTests
{
    private static TransactionDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TransactionDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test = full isolation
            .Options;

        return new TransactionDbContext(options);
    }

    private static Transaction MakeTransaction(
        string description,
        decimal amount,
        bool isDebit,
        TransactionCategory category,
        DateTime transactionDate)
    {
        return new Transaction
        {
            Id = Guid.NewGuid(),
            SourceSystem = "Test",
            ExternalReference = Guid.NewGuid().ToString(),
            AccountId = "test-account",
            Description = description,
            Amount = amount,
            Currency = "ZAR",
            IsDebit = isDebit,
            Category = category,
            TransactionDate = transactionDate
        };
    }

    [Fact]
    public async Task GetSummaryAsync_CalculatesTotalsCorrectly()
    {
        await using var context = CreateContext();
        context.Transactions.AddRange(
            MakeTransaction("Groceries Shop", 100m, isDebit: true, TransactionCategory.Groceries, DateTime.UtcNow),
            MakeTransaction("Salary", 1000m, isDebit: false, TransactionCategory.Income, DateTime.UtcNow),
            MakeTransaction("Fuel", 50m, isDebit: true, TransactionCategory.Transport, DateTime.UtcNow)
        );
        await context.SaveChangesAsync();

        var sut = new AggregationService(context);
        var summary = await sut.GetSummaryAsync();

        Assert.Equal(3, summary.TotalTransactions);
        Assert.Equal(150m, summary.TotalSpend);
        Assert.Equal(1000m, summary.TotalIncome);
        Assert.Equal(850m, summary.NetAmount);
    }

    [Fact]
    public async Task GetSummaryAsync_NoTransactions_ReturnsZeroedSummary()
    {
        await using var context = CreateContext();
        var sut = new AggregationService(context);

        var summary = await sut.GetSummaryAsync();

        Assert.Equal(0, summary.TotalTransactions);
        Assert.Equal(0m, summary.TotalSpend);
        Assert.Equal(0m, summary.TotalIncome);
        Assert.Equal(0m, summary.AverageTransactionAmount);
    }

    [Fact]
    public async Task GetTotalsByCategoryAsync_GroupsAndSumsCorrectly()
    {
        await using var context = CreateContext();
        context.Transactions.AddRange(
            MakeTransaction("Checkers", 100m, true, TransactionCategory.Groceries, DateTime.UtcNow),
            MakeTransaction("Woolworths", 200m, true, TransactionCategory.Groceries, DateTime.UtcNow),
            MakeTransaction("Uber", 50m, true, TransactionCategory.Transport, DateTime.UtcNow)
        );
        await context.SaveChangesAsync();

        var sut = new AggregationService(context);
        var results = await sut.GetTotalsByCategoryAsync();

        var groceries = Assert.Single(results, r => r.Category == TransactionCategory.Groceries);
        Assert.Equal(2, groceries.TransactionCount);
        Assert.Equal(300m, groceries.TotalAmount);

        var transport = Assert.Single(results, r => r.Category == TransactionCategory.Transport);
        Assert.Equal(1, transport.TransactionCount);
        Assert.Equal(50m, transport.TotalAmount);
    }

    [Fact]
    public async Task GetTopMerchantsAsync_IsCaseInsensitiveAndExcludesCredits()
    {
        await using var context = CreateContext();
        context.Transactions.AddRange(
            MakeTransaction("Checkers Sandton", 100m, true, TransactionCategory.Groceries, DateTime.UtcNow),
            MakeTransaction("CHECKERS SANDTON", 150m, true, TransactionCategory.Groceries, DateTime.UtcNow),
            MakeTransaction("Salary", 5000m, false, TransactionCategory.Income, DateTime.UtcNow) // credit, should be excluded
        );
        await context.SaveChangesAsync();

        var sut = new AggregationService(context);
        var results = await sut.GetTopMerchantsAsync(top: 10);

        var checkers = Assert.Single(results);
        Assert.Equal(2, checkers.TransactionCount);
        Assert.Equal(250m, checkers.TotalAmount);
    }

    [Fact]
    public async Task GetTopMerchantsAsync_RespectsTopLimit()
    {
        await using var context = CreateContext();
        for (var i = 0; i < 5; i++)
        {
            context.Transactions.Add(
                MakeTransaction($"Merchant {i}", 100m + i, true, TransactionCategory.Shopping, DateTime.UtcNow));
        }
        await context.SaveChangesAsync();

        var sut = new AggregationService(context);
        var results = await sut.GetTopMerchantsAsync(top: 3);

        Assert.Equal(3, results.Count);
    }
}