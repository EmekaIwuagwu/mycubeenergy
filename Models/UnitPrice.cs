namespace CubeEnergy.Models
{
    public class UnitPrice
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
