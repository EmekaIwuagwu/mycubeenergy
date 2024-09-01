using CubeEnergy.DTOs;
using CubeEnergy.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CubeEnergy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPut("update-cash-wallet")]
        public async Task<IActionResult> UpdateCashWallet([FromQuery] string email, [FromQuery] decimal amount, [FromQuery] string accountId, [FromQuery] string transactionType)
        {
            try
            {
                await _userService.UpdateBalanceAndLogTransactionAsync(email, amount, accountId, transactionType);
                return Ok(new { message = "Cash wallet updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error updating cash wallet: {ex.Message}" });
            }
        }

        [HttpPut("debit-cash-wallet-credit-user-wallet")]
        public async Task<IActionResult> DebitCashWalletAndCreditUser([FromQuery] string email, [FromQuery] decimal amount, [FromQuery] string accountId)
        {
            try
            {
                var result = await _userService.DebitCashWalletAndCreditUserAsync(email, amount, accountId);
                return Ok(new { message = "Transaction successful.", cashWalletBalance = result.cashWalletBalance, userWalletBalance = result.userWalletBalance });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error processing transaction: {ex.Message}" });
            }
        }
    }
}
