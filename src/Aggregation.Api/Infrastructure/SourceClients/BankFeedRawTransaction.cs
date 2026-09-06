namespace Aggregation.Api.Infrastructure.SourceClients;

public class BankFeedRawTransaction
{
    public string Reference { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string Narrative { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PostedAt { get; set; }
    public string Direction { get; set; } = string.Empty;
}
