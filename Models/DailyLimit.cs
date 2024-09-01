namespace CubeEnergy.Models
{
    public class DailyLimit
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string AccountId { get; set; }
        public DateTime Date { get; set; }
        public decimal DailyUsage { get; set; }
        public decimal DailyTotalCost { get; set; }
    }

}
