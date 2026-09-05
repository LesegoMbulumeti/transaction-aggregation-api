using Aggregation.Api.Application.Interfaces;
using Aggregation.Api.Domain.Enums;

namespace Aggregation.Api.Application.Services;

// Assigns a category to a transaction by matching keywords in its description 

public class CategorizationService : ICategorizationService
{
    private static readonly (TransactionCategory Category, String[] Keywords)[] Rules = [
        (TransactionCategory.Income,        ["SALARY"]),
        (TransactionCategory.Groceries,     ["CHECKERS", "WOOLWORTHS", "PICK N PAY", "SPAR"]),
        (TransactionCategory.Transport,     ["UBER", "BOLT", "GAUTRAIN", "PETROL", "FUEL"]),
        (TransactionCategory.Entertainment, ["NETFLIX", "SPOTIFY", "SHOWMAX", "DSTV"]),
        (TransactionCategory.Utilities,     ["ESKOM", "VODACOM", "MTN", "TELKOM", "PREPAID"]),
        (TransactionCategory.Dining,        ["OCEAN BASKET", "RESTAURANT", "KFC", "NANDOS", "MCDONALD"]),
        (TransactionCategory.Shopping,      ["TAKEALOT", "AMAZON"]),
        (TransactionCategory.Fitness,       ["VIRGIN ACTIVE", "GYM", "PLANET FITNESS"]),
        (TransactionCategory.Fees,          ["BANK FEE", "ATM WITHDRAWAL", "ACCOUNT FEE"]),
    ];

    public TransactionCategory Categorize(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return TransactionCategory.Uncategorized;
        }

        var normalized = description.ToUpperInvariant();

        foreach (var (category, keywords) in Rules)
        {
            if (keywords.Any(keyword => normalized.Contains(keyword, StringComparison.Ordinal)))
            {
                return category;
            }
        }

        return TransactionCategory.Uncategorized;
    }
}