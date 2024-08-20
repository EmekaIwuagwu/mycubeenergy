using Microsoft.AspNetCore.Http;

namespace CubeEnergy.Models
{
    public class KycDto
    {
        public string Email { get; set; }
        public IFormFile BankStatement { get; set; }
        public IFormFile PassportDataPage { get; set; }
        public IFormFile NationalID { get; set; }
    }
}
