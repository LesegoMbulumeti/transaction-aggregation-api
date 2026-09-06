using Aggregation.Api.Application.DTOs;
using Aggregation.Api.Application.Interfaces;
using Aggregation.Api.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Aggregation.Api.Controllers;
[ApiController]
[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionRepository _repository;

    public TransactionsController(ITransactionRepository repository)
    {
        _repository = repository;
    }

    // Lists transactions with optional filtering and paging.

    [HttpGet]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] TransactionCategory? category,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? accountId,
        [FromQuery] string? sourceSystem,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)

    {
        if (page < 1 || pageSize < 1 || pageSize > 200)
        {
            return BadRequest("page must be >= 1 and pageSize must be between 1 and 200.");
        }
        
        var options = new TransactionQueryOptions
        {
            Category = category,
            FromDate = fromDate,
            ToDate = toDate,
            AccountId = accountId,
            SourceSystem = sourceSystem,
            Page = page,
            PageSize = pageSize
        };

        var transactions = await _repository.QueryAsync(options);
        var totalCount = await _repository.CountAsync();

        return Ok(new
        {
            page,
            pageSize,
            totalCount,
            items = transactions
        });
    }

    // Retrieves a single transaction by its ID.
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var transaction = await _repository.GetByIdAsync(id);

        if (transaction is null)
        {
            return NotFound();
        }

        return Ok(transaction);

    }
    
}