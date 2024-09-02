using CubeEnergy.Models;

namespace CubeEnergy.DTOs
{
    public class UserDetailDTO
    {
        public User User { get; set; }
        public Kyc Kyc { get; set; }
        public CashWallet CashWallet { get; set; }
    }
}
