namespace MockSources.Service.Models
{
    
    public class CardProviderTransaction
    {
        public string TransactionId { get; set;} = string.Empty;
        public string CardAccountId { get; set;} = string.Empty;
        public string MerchantName { get; set;} = string.Empty;
        public long AmountCents { get; set;} // Amount in cents to avoid floating point issues
        public string CurrencyCode { get; set;} = string.Empty;

        public long TransactionTimestampMs { get; set;} 
        public string Status { get; set;} = string.Empty; // "PENDING", "SETTLED", "DECLINED"

    }
}