namespace Aggregation.Api.Application.DTOs;

public class MerchantTotalDto
{
    public string Description { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
}