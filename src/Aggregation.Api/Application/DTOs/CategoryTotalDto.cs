using Aggregation.Api.Domain.Enums;

namespace Aggregation.Api.Application.DTOs;

public class CategoryTotalDto
{
    public TransactionCategory Category { get; set; }
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
}