namespace CubeEnergy.DTOs
{
    public class ResetPasswordDTO
    {
        public string Email { get; set; }
        public string OTP { get; set; }
        public string NewPassword { get; set; }
    }
}
