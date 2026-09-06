namespace Aggregation.Api.Infrastructure.SourceClients;


public class EftRawTransaction
{
    public string Id { get; set; } = string.Empty;
    public string FromAccount { get; set; } = string.Empty;
    public string ToAccount { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public DateTime ValueDate { get; set; }
    public string Type { get; set; } = string.Empty;

}
