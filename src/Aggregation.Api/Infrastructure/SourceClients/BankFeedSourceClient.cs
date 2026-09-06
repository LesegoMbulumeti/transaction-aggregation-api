using Aggregation.Api.Application.Interfaces;
using Aggregation.Api.Domain.Entities;

namespace Aggregation.Api.Infrastructure.SourceClients;

public class BankFeedSourceClient : ISourceClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public string SourceName => "BankFeed";

    public BankFeedSourceClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = configuration["MockSources:BaseUrl"]
            ?? throw new InvalidOperationException("MockSources:BaseUrl is not configured");
    }

    public async Task<IReadOnlyList<Transaction>> FetchTransactionsAsync()
    {
        var raw = await _httpClient.GetFromJsonAsync<List<BankFeedRawTransaction>>(
            $"{_baseUrl}/mock/bank-feed");
        
        if (raw == null)
        {
            return [];
        }

        return raw.Select(MapToTransaction).ToList();
    }

    private static Transaction MapToTransaction(BankFeedRawTransaction raw)
    {
        return new Transaction
        {
            Id = Guid.NewGuid(),
            SourceSystem = "BankFeed",
            ExternalReference = raw.Reference,
            AccountId = raw.AccountNumber,
            Description = raw.Narrative,
            Amount = raw.Amount,
            Currency = "ZAR",
            IsDebit = raw.Direction.Equals("DR", StringComparison.OrdinalIgnoreCase),
            TransactionDate = raw.PostedAt,
        };
    }
}
