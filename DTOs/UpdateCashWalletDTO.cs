namespace CubeEnergy.DTOs
{
    public class UpdateCashWalletDTO
    {
        public string Email { get; set; }
        public string AccountId { get; set; }
        public decimal Amount { get; set; }
        public decimal TransactionType { get; set; }
    }
}
