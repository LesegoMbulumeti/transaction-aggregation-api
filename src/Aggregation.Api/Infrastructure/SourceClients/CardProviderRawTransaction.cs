namespace Aggregation.Api.Infrastructure.SourceClients;


public class CardProviderRawTransaction
{
    public string TransactionId { get; set; } = string.Empty;
    public string CardAccountId { get; set; } = string.Empty;
    public string MerchantName { get; set; } = string.Empty;
    public long AmountCents { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public long TransactionTimestampMs { get; set; }
    public string Status { get; set; } = string.Empty;
}
