using Aggregation.Api.Domain.Entities;

namespace Aggregation.Api.Application.Interfaces;

public interface ISourceClient
{
 // Retrieves a list of transactions from the source system.
  string SourceName { get; }

   Task<IReadOnlyList<Transaction>> FetchTransactionsAsync();
}
