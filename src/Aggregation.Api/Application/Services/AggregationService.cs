using Aggregation.Api.Application.DTOs;
using Aggregation.Api.Application.Interfaces;
using Aggregation.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aggregation.Api.Application.Services;

public class AggregationService : IAggregationService
{
    private readonly TransactionDbContext _context;

    public AggregationService(TransactionDbContext context)
    {
        _context = context;
    }

    public async Task<AggregationSummaryDto> GetSummaryAsync()
    {
        var query = _context.Transactions.Where(t => t.DeletedAt == null);

        var totalTransactions = await query.CountAsync();

        var totalSpend = await query
            .Where(t => t.IsDebit)
            .SumAsync(t => (decimal?)t.Amount) ?? 0m;

        var totalIncome = await query
            .Where(t => !t.IsDebit)
            .SumAsync(t => (decimal?)t.Amount) ?? 0m;

        var averageAmount = totalTransactions > 0
            ? await query.AverageAsync(t => t.Amount)
            : 0m;

        return new AggregationSummaryDto
        {
            TotalTransactions = totalTransactions,
            TotalSpend = totalSpend,
            TotalIncome = totalIncome,
            NetAmount = totalIncome - totalSpend,
            AverageTransactionAmount = Math.Round(averageAmount, 2)
        };
    }

    public async Task<IReadOnlyList<CategoryTotalDto>> GetTotalsByCategoryAsync()
    {
        var results = await _context.Transactions
            .Where(t => t.DeletedAt == null)
            .GroupBy(t => t.Category)
            .Select(g => new CategoryTotalDto
            {
                Category = g.Key,
                TransactionCount = g.Count(),
                TotalAmount = g.Sum(t => t.Amount)
            })
            .OrderByDescending(c => c.TotalAmount)
            .ToListAsync();

        return results;
    }

    public async Task<IReadOnlyList<MonthlyTotalDto>> GetTotalsByMonthAsync()
    {
        var transactions = await _context.Transactions
            .Where(t => t.DeletedAt == null)
            .ToListAsync();

        var results = transactions
            .GroupBy(t => new { t.TransactionDate.Year, t.TransactionDate.Month })
            .Select(g => new MonthlyTotalDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalSpend = g.Where(t => t.IsDebit).Sum(t => t.Amount),
                TotalIncome = g.Where(t => !t.IsDebit).Sum(t => t.Amount)
            })
            .OrderBy(m => m.Year).ThenBy(m => m.Month)
            .ToList();

        return results;
    }

    public async Task<IReadOnlyList<MerchantTotalDto>> GetTopMerchantsAsync(int top)
    {
        var transactions = await _context.Transactions
        .Where(t => t.DeletedAt == null && t.IsDebit)
        .ToListAsync();

        var results = transactions
            .GroupBy(t => t.Description.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g => new MerchantTotalDto
            {
                Description = g.First().Description, // keep one representative casing for display
                TransactionCount = g.Count(),
                TotalAmount = g.Sum(t => t.Amount)
            })
            .OrderByDescending(m => m.TotalAmount)
            .Take(top)
            .ToList();

        return results;
    }
}