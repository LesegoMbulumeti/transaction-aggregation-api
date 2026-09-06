using Aggregation.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aggregation.Api.Infrastructure.Persistence;


public class TransactionDbContext : DbContext
{
    public TransactionDbContext(DbContextOptions<TransactionDbContext> options)
        : base(options)
    {
    }

    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaction>(entity =>
        {
             entity.HasKey(t => t.Id);
            entity.HasIndex(t => t.Category);
            entity.HasIndex(t => t.TransactionDate);
            entity.HasIndex(t => t.AccountId);
        });
    }
}
