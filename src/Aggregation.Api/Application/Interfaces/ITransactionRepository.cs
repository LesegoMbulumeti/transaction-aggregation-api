using Aggregation.Api.Application.DTOs;
using Aggregation.Api.Domain.Entities;

namespace Aggregation.Api.Application.Interfaces;

public interface ITransactionRepository
{
    Task AddRangeAsync(IEnumerable<Transaction> transactions);
    Task<Transaction?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Transaction>> QueryAsync(TransactionQueryOptions options);
    Task<int> CountAsync();
}