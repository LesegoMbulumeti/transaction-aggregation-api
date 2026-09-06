using Aggregation.Api.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aggregation.Api.Infrastructure.BackgroundServices;

// Runs in the background and ingests transactions from various sources into the system
public class IngestionHostedService : IHostedService
{
    private readonly IEnumerable<ISourceClient> _sourceClients;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<IngestionHostedService> _logger;

    public IngestionHostedService(
        IEnumerable<ISourceClient> sourceClients,
        IServiceScopeFactory scopeFactory,
        ILogger<IngestionHostedService> logger)
    {
        _sourceClients = sourceClients;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("IngestionHostedService is starting.");

        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
        var categorizer = scope.ServiceProvider.GetRequiredService<ICategorizationService>();

        var totalIngested = 0;

        foreach (var client in _sourceClients)
        {
            try
            {
                var transactions = await client.FetchTransactionsAsync();

                foreach (var transaction in transactions)
                {
                    transaction.Category = categorizer.Categorize(transaction.Description);
                }

                await repository.AddRangeAsync(transactions);
                totalIngested += transactions.Count;

                _logger.LogInformation(
                    "Ingested {Count} transactions from {Source}",
                    transactions.Count, client.SourceName);
            }
            catch (Exception ex)
            {
                // One source failing shouldn't take down the whole app -
                // log it and continue with the other sources.
                _logger.LogError(ex,
                    "Failed to ingest transactions from {Source}", client.SourceName);
            }
        }

        _logger.LogInformation("Ingestion complete. Total transactions ingested: {Total}", totalIngested);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    
}
