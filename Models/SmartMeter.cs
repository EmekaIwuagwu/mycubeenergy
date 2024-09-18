namespace CubeEnergy.Models
{
    public class SmartMeter
    {
        public int Id { get; set; }
        public string MacAddress { get; set; }
        public string AccountId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
