using MockSources.Service.Models;

namespace MockSources.Service.Data;

// Mock data generator for bank feed, card provider, and EFT transactions.

public static class MockDataGenerator
{
        private static readonly Random Rng = new(Seed: 42);

    private static readonly (string Name, decimal MinAmount, decimal MaxAmount)[] Merchants =
    [
        ("Checkers Sandton City", 150, 1200),
        ("Woolworths Rosebank",   200, 1800),
        ("Uber Trip",              45, 250),
        ("Netflix Subscription",  199, 199),
        ("Spotify Premium",        69.99m, 69.99m),
        ("Eskom Prepaid",          300, 1500),
        ("Vodacom Airtime",        50, 500),
        ("Ocean Basket",           180, 650),
        ("Takealot.com",           99, 3500),
        ("Gym Membership - Virgin Active", 599, 599),
        ("Salary - Acme Corp",     18000, 32000),
        ("ATM Withdrawal",         200, 3000),
        ("Bank Fee - Monthly Account Fee", 99, 149),
    ];

    public static List<BankFeedTransaction> GenerateBankFeed(int count = 40)
    {
        var results = new List<BankFeedTransaction>();
        for (var i = 0; i < count; i++)
        {
            var merchant = Merchants[Rng.Next(Merchants.Length)];
            var isCredit = merchant.Name.StartsWith("Salary");

            results.Add(new BankFeedTransaction
            {
                Reference = $"BF-{Guid.NewGuid():N}"[..12],
                AccountNumber = "62812345678",
                Narrative = merchant.Name.ToUpperInvariant(),
                Amount = RandomAmount(merchant.MinAmount, merchant.MaxAmount),
                PostedAt = RandomRecentDate(),
                Direction = isCredit ? "CR" : "DR"
            });
        }
        return results;
    }

    public static List<CardProviderTransaction> GenerateCardProvider(int count = 40)
    {
        var results = new List<CardProviderTransaction>();
        for (var i = 0; i < count; i++)
        {
            var merchant = Merchants[Rng.Next(Merchants.Length)];
            var amount = RandomAmount(merchant.MinAmount, merchant.MaxAmount);

            results.Add(new CardProviderTransaction
            {
                TransactionId = $"cp_{Guid.NewGuid():N}"[..16],
                CardAccountId = "card-acc-9981",
                MerchantName = merchant.Name,
                AmountCents = (long)(amount * 100),
                CurrencyCode = "ZAR",
                TransactionTimestampMs = new DateTimeOffset(RandomRecentDate()).ToUnixTimeMilliseconds(),
                Status = "SETTLED"
            });
        }
        return results;
    }

    public static List<EftTransaction> GenerateEft(int count = 20)
    {
        var results = new List<EftTransaction>();
        for (var i = 0; i < count; i++)
        {
            var merchant = Merchants[Rng.Next(Merchants.Length)];
            var isCredit = merchant.Name.StartsWith("Salary");

            results.Add(new EftTransaction
            {
                Id = $"eft-{Guid.NewGuid():N}"[..14],
                FromAccount = isCredit ? "external-employer-001" : "62812345678",
                ToAccount = isCredit ? "62812345678" : "external-merchant-002",
                Description = merchant.Name,
                Value = RandomAmount(merchant.MinAmount, merchant.MaxAmount),
                ValueDate = RandomRecentDate(),
                Type = isCredit ? "CREDIT" : "DEBIT"
            });
        }
        return results;
    }

    private static decimal RandomAmount(decimal min, decimal max)
    {
        if (min == max) return min;
        var range = (double)(max - min);
        return min + (decimal)(Rng.NextDouble() * range);
    }

    private static DateTime RandomRecentDate()
    {
        var daysAgo = Rng.Next(0, 90); // spread across the last ~3 months
        return DateTime.UtcNow.AddDays(-daysAgo).Date.AddHours(Rng.Next(6, 22));
    }
}