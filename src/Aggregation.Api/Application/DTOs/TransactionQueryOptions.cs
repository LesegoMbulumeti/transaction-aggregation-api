using Aggregation.Api.Domain.Enums;

namespace Aggregation.Api.Application.DTOs;

public class TransactionQueryOptions
{
    public TransactionCategory? Category { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? AccountId { get; set; }
    public string? SourceSystem { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;

}