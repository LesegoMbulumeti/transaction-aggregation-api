namespace Aggregation.Api.Application.DTOs;

public class AggregationSummaryDto
{
    public int TotalTransactions { get; set; }
    public decimal TotalSpend { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal NetAmount { get; set; }
    public decimal AverageTransactionAmount { get; set; }
}