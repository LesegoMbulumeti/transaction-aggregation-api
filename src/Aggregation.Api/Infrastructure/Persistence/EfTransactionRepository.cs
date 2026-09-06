using Aggregation.Api.Application.DTOs;
using Aggregation.Api.Application.Interfaces;
using Aggregation.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aggregation.Api.Infrastructure.Persistence;
public class EfTransactionRepository : ITransactionRepository
{
    private readonly TransactionDbContext _context;

    public EfTransactionRepository(TransactionDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(IEnumerable<Transaction> transactions)
    {
        await _context.Transactions.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();
    }

     public async Task<Transaction?> GetByIdAsync(Guid id)
    {
        return await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.DeletedAt == null);
    }

    public async Task<IReadOnlyList<Transaction>> QueryAsync(TransactionQueryOptions options)
    {
        var query = _context.Transactions.Where(t => t.DeletedAt == null);

        if (options.Category is not null)
        {
            query = query.Where(t => t.Category == options.Category);
        }

        if (options.FromDate is not null)
        {
            query = query.Where(t => t.TransactionDate >= options.FromDate);
        }

        if (options.ToDate is not null)
        {
            query = query.Where(t => t.TransactionDate <= options.ToDate);
        }

        if (!string.IsNullOrWhiteSpace(options.AccountId))
        {
            query = query.Where(t => t.AccountId == options.AccountId);
        }

        if (!string.IsNullOrWhiteSpace(options.SourceSystem))
        {
            query = query.Where(t => t.SourceSystem == options.SourceSystem);
        }

        return await query
            .OrderByDescending(t => t.TransactionDate)
            .Skip((options.Page - 1) * options.PageSize)
            .Take(options.PageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Transactions.CountAsync(t => t.DeletedAt == null);
    }
    
}
