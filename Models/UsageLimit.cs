namespace CubeEnergy.Models
{
    public class UsageLimit
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public decimal DailyTotalCost { get; set; }
        public DateTime Date { get; set; }
        public decimal Limit { get; set; }
    }
}
