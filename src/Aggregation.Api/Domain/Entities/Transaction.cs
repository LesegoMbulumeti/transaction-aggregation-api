using Aggregation.Api.Domain.Enums;

namespace Aggregation.Api.Domain.Entities;
    public class Transaction
    {
        public Guid Id { get; set; }

        public string SourceSystem { get; set;} = string.Empty; // The source system from which the transaction originated bank, credit card

        public string ExternalReference { get; set; } = string.Empty;

        public string AccountId { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public String Currency { get; set; } = "ZAR";

        public bool IsDebit { get; set; }

        public DateTime TransactionDate { get; set; }

        public TransactionCategory Category { get; set; } = TransactionCategory.Uncategorized;

        //Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        public DateTime? DeletedAt { get; set; }
    }