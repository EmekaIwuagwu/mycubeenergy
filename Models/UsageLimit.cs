namespace CubeEnergy.Models
{
    public class UsageLimit
    {
        public string Email { get; set; }
        public decimal DailyTotalCost { get; set; }
        public DateTime Date { get; set; }
    }
}
