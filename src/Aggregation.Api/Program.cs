using Aggregation.Api.Application.Interfaces;
using Aggregation.Api.Application.Services;
using Aggregation.Api.Infrastructure.Persistence;
using Aggregation.Api.Infrastructure.SourceClients;
using Aggregation.Api.Infrastructure.BackgroundServices;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Persistence
builder.Services.AddDbContext<TransactionDbContext>(options =>
    options.UseInMemoryDatabase("TransactionsDb"));
builder.Services.AddScoped<ITransactionRepository, EfTransactionRepository>();

// Application services
builder.Services.AddSingleton<ICategorizationService, CategorizationService>();

// Source clients (HttpClients)
builder.Services.AddHttpClient<ISourceClient, BankFeedSourceClient>();
builder.Services.AddHttpClient<ISourceClient, CardProviderSourceClient>();
builder.Services.AddHttpClient<ISourceClient, EftSourceClient>();

//Background service for ingestion
builder.Services.AddHostedService<IngestionHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();