namespace CubeEnergy.Models
{
    public class DailyLimit
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string AccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public decimal DailyUsage { get; set; }
        public decimal DailyTotalCost { get; set; }
    }

}
