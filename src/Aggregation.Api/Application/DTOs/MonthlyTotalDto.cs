namespace Aggregation.Api.Application.DTOs;

public class MonthlyTotalDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalSpend { get; set; }
    public decimal TotalIncome { get; set; }
}