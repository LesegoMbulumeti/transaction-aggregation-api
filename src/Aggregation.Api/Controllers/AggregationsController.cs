using Aggregation.Api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aggregation.Api.Controllers;

[ApiController]
[Route("api/aggregations")]
public class AggregationsController : ControllerBase
{
    private readonly IAggregationService _aggregationService;

    public AggregationsController(IAggregationService aggregationService)
    {
        _aggregationService = aggregationService;
    }

    //Overall totals: transaction count, total spend, total income, net amount, average transaction amount.
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _aggregationService.GetSummaryAsync();
        return Ok(summary);
    }

    //Totals grouped by category
    [HttpGet("by-category")]
    public async Task<IActionResult> GetByCategory()
    {
        var results = await _aggregationService.GetTotalsByCategoryAsync();
        return Ok(results);
    }

    //Totals grouped by month/year
    [HttpGet("by-month")]
    public async Task<IActionResult> GetByMonth()
    {
        var results = await _aggregationService.GetTotalsByMonthAsync();
        return Ok(results);
    }

    // Top N merchants by total spend, default top 10 
    [HttpGet("top-merchants")]
    public async Task<IActionResult> GetTopMerchants([FromQuery] int top = 10)
    {
        if (top < 1 || top > 100)
        {
            return BadRequest("top must be between 1 and 100.");
        }
        
        var results = await _aggregationService.GetTopMerchantsAsync(top);
        return Ok(results);

    }
}
