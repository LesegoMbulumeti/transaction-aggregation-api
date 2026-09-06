using Aggregation.Api.Application.Interfaces;
using Aggregation.Api.Domain.Entities;

namespace Aggregation.Api.Infrastructure.SourceClients;

public class CardProviderSourceClient : ISourceClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public string SourceName => "CardProvider";

    public CardProviderSourceClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = configuration["MockSources:BaseUrl"]
            ?? throw new InvalidOperationException("MockSources:BaseUrl is not configured");
    }

    public async Task<IReadOnlyList<Transaction>> FetchTransactionsAsync()
    {
        var raw = await _httpClient.GetFromJsonAsync<List<CardProviderRawTransaction>>(
            $"{_baseUrl}/mock/card-provider");

        if (raw is null)
        {
            return [];
        }

        return raw
            .Where(r => r.Status.Equals("SETTLED", StringComparison.OrdinalIgnoreCase))
            .Select(MapToTransaction)
            .ToList();
    }

    private static Transaction MapToTransaction(CardProviderRawTransaction raw)
    {
        return new Transaction
        {
            Id = Guid.NewGuid(),
            SourceSystem = "CardProvider",
            ExternalReference = raw.TransactionId,
            AccountId = raw.CardAccountId,
            Description = raw.MerchantName,
            Amount = raw.AmountCents / 100m,
            Currency = raw.CurrencyCode,
            IsDebit = true, // card transactions in this mock are always spend, never credit
            TransactionDate = DateTimeOffset.FromUnixTimeMilliseconds(raw.TransactionTimestampMs).UtcDateTime,
        };
    }
}

