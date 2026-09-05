namespace MockSources.Service.Models

{
     // EFT transaction model eg. direct payments, direct debits.
    public class EftTransaction
    {
         public string  Id { get; set;} = string.Empty;
         public string FromAccount { get; set;} = string.Empty;
         public string ToAccount { get; set;} = string.Empty;
         public string Description { get; set;} = string.Empty;
         public decimal Value { get; set;}
        public DateTime ValueDate { get; set;}

        public string Type { get; set;} = string.Empty; // "CREDIT" or "DEBIT"
          
    }
}