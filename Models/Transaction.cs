namespace CubeEnergy.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string AccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string Description { get; set; } // Optional
    }
}
