namespace CubeEnergy.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }
        public string AdminType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
