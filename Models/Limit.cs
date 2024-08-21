namespace CubeEnergy.Models
{
    public class Limit
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public decimal DailyTotalCost { get; set; }
        public DateTime Date { get; set; }
    }
}
