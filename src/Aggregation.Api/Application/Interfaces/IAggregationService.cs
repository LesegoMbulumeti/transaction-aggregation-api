using Aggregation.Api.Application.DTOs;

namespace Aggregation.Api.Application.Interfaces;

public interface IAggregationService
{
    Task<AggregationSummaryDto> GetSummaryAsync();
    Task<IReadOnlyList<CategoryTotalDto>> GetTotalsByCategoryAsync();
    Task<IReadOnlyList<MonthlyTotalDto>> GetTotalsByMonthAsync();
    Task<IReadOnlyList<MerchantTotalDto>> GetTopMerchantsAsync(int top);
}