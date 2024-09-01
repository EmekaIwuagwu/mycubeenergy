namespace CubeEnergy.Models
{
    public class CashWallet
    {
        public int Id { get; set; } // Primary key
        public string Email { get; set; }
        public string AccountId { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
