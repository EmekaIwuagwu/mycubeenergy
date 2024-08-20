namespace CubeEnergy.Models
{
    public class OTP
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTime ExpiryTime { get; set; } 
    }
}
