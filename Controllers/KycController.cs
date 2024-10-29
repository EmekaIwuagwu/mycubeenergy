using CubeEnergy.Models;
using CubeEnergy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CubeEnergy.Controllers
{
    [Route("api/User/[controller]")]
    [ApiController]
    [Authorize]
    public class KycController : ControllerBase
    {
        private readonly KycService _kycService;
        private readonly UserService _userService;
        

        public KycController(KycService kycService, UserService userService)
        {
            _kycService = kycService;
            _userService = userService;
        }

        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateKyc([FromForm] KycDto kycDto)
        {
            var result = await _kycService.CreateKycAsync(kycDto);
            return Ok(result);
        }

        [HttpPut("update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateKyc([FromQuery] int id, [FromForm] KycDto kycDto)
        {
            var result = await _kycService.UpdateKycAsync(id, kycDto);
            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteKyc([FromQuery] string email, [FromQuery] int id)
        {
            var result = await _kycService.DeleteKycAsync(email, id);
            return Ok(result);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfileWithKyc([FromQuery] string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            var kyc = await _kycService.GetKycByEmailAsync(email);
            var cashWallet = await _userService.GetCashWalletByEmailAsync(email);

            if (user != null && kyc != null && cashWallet != null)
            {
                var profileWithKyc = new
                {
                    User = user,
                    Kyc = kyc,
                    CashWalletBalance = cashWallet.Balance // Assuming Balance is a property in CashWallet
                };
                return Ok(profileWithKyc);
            }

            return NotFound("User, KYC data, or CashWallet not found.");
        }

    }
}
