namespace CubeEnergy.DTOs
{
    public class UpdateSuperAdminDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string AdminType { get; set; } // E.g., "Admin" or "SuperAdmin"
    }

}
