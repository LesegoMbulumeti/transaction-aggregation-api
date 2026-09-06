using Aggregation.Api.Application.Interfaces;
using Aggregation.Api.Domain.Entities;

namespace Aggregation.Api.Infrastructure.SourceClients;

public class EftSourceClient : ISourceClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public string SourceName => "Eft";

    public EftSourceClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = configuration["MockSources:BaseUrl"]
            ?? throw new InvalidOperationException("MockSources:BaseUrl is not configured");
    }

    public async Task<IReadOnlyList<Transaction>> FetchTransactionsAsync()
    {
        var raw = await _httpClient.GetFromJsonAsync<List<EftRawTransaction>>(
            $"{_baseUrl}/mock/eft-transfers");

        if (raw is null)
        {
            return [];
        }

        return raw.Select(MapToTransaction).ToList();
    }

    private static Transaction MapToTransaction(EftRawTransaction raw)
    {
        var isDebit = raw.Type.Equals("DEBIT", StringComparison.OrdinalIgnoreCase);

      
        // account for this transaction: FromAccount if money left (debit),
        // ToAccount if money arrived (credit).
        var ourAccount = isDebit ? raw.FromAccount : raw.ToAccount;

        return new Transaction
        {
            Id = Guid.NewGuid(),
            SourceSystem = "Eft",
            ExternalReference = raw.Id,
            AccountId = ourAccount,
            Description = raw.Description,
            Amount = raw.Value,
            Currency = "ZAR",
            IsDebit = isDebit,
            TransactionDate = raw.ValueDate,
        };
    }
}