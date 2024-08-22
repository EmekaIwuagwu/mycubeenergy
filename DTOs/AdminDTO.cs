// DTOs/AdminDTO.cs
namespace CubeEnergy.DTOs
{
    public class AdminDTO
    {
        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }
        public string AdminType { get; set; }
    }

    public class AdminLoginDTO
    {
        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }
    }
}
