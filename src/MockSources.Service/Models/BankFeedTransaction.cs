namespace MockSources.Service.Models
{
    // Bank feed transaction model
    public class BankFeedTransaction
    {
        public string Reference { get; set;} = string.Empty;
        public string AccountNumber { get; set;} = string.Empty;
        public string Narrative { get; set;} = string.Empty;
        public decimal Amount { get; set;}
        public DateTime PostedAt { get; set;}
        public string Direction { get; set;} = string.Empty; // "CR" or "DR"

    }
}