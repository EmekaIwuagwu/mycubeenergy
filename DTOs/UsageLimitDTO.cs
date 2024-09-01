namespace CubeEnergy.DTOs
{
    public class UsageLimitDTO
    {
        public string Month { get; set; }
        public decimal TotalUsage { get; set; }
        public DateTime Date { get; set; } // Ensure this property exists
        public decimal Limit { get; set; }
    }

}
