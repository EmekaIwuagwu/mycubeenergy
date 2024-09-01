using CubeEnergy.DTOs;
using CubeEnergy.Models;
using CubeEnergy.Repositories;
using CubeEnergy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CubeEnergy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IUserRepository _userRepository;

        public UserController(UserService userService, IUserRepository userRepository)
        {
            _userService = userService;
            _userRepository = userRepository;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile([FromQuery] string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user != null)
            {
                return Ok(user);
            }

            return NotFound("User not found.");
        }

        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount(int userId)
        {
            await _userService.DeleteUserAsync(userId);
            return Ok("Account Deleted");
        }

        [HttpPut("editProfile")]
        public async Task<IActionResult> EditProfile([FromQuery] string email, [FromQuery] int userId, UpdateProfileDTO updateDto)
        {
            var user = await _userService.GetUserByEmailOrIdAsync(email, userId);
            if (user != null)
            {
                user.Fullname = updateDto.Fullname;
                user.Address = updateDto.Address;
                user.Telephone = updateDto.Telephone;
                user.City = updateDto.City;
                user.State = updateDto.State;
                user.Zipcode = updateDto.Zipcode;

                await _userService.UpdateUserAsync(user);
                return Ok(user);
            }

            return NotFound();
        }

        [HttpGet("calculateUnits")]
        public async Task<IActionResult> CalculateUnits(int id, int days)
        {
            var price = await _userService.GetUnitPriceAsync(id);
            if (price != null)
            {
                var totalCost = days * price.Price;
                return Ok(new { TotalCost = totalCost });
            }

            return BadRequest("Unit price not found.");
        }

        [HttpPut("update-balance")]
        public async Task<IActionResult> UpdateBalance([FromQuery] string email, [FromQuery] decimal amount, [FromQuery] string accountId)
        {
            string transactionType = "Credit";

            if (amount <= 0)
            {
                return BadRequest("Amount must be greater than zero.");
            }

            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            await _userService.UpdateBalanceAndLogTransactionAsync(email, amount, accountId, transactionType);

            return Ok(new { Message = "Balance updated successfully.", NewBalance = user.UnitBalance });
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions(string email)
        {
            var transactions = await _userService.GetTransactionsByEmailAsync(email);
            return Ok(transactions);
        }

        [HttpGet("transactions-by-date")]
        public async Task<IActionResult> GetTransactionsByDate(string email, DateTime from, DateTime to)
        {
            var transactions = await _userService.GetTransactionsByDateAsync(email, from, to);
            return Ok(transactions);
        }

        [HttpPost("share-units")]
        public async Task<IActionResult> ShareUnits([FromBody] ShareUnitsDTO dto)
        {
            var result = await _userService.ShareUnitsAsync(dto.OriginAccountId, dto.DestinationAccountId, dto.Amount);

            if (!result.Success)
            {
                return BadRequest(new { Message = result.ErrorMessage });
            }

            return Ok(new { Message = "Units Shared Successfully" });
        }

        [HttpGet("calculateTotalCost")]
        public async Task<IActionResult> CalculateTotalCost(string email)
        {
            try
            {
                var totalCost = await _userService.CalculateTotalCostAsync(email);
                return Ok(new { Email = email, TotalCost = totalCost });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("showUsageLimitByMonth")]
        public async Task<IActionResult> ShowUsageLimitByMonth(string email, DateTime startDate, DateTime endDate)
        {
            var usageLimits = await _userService.GetUsageLimitsByMonthAsync(email, startDate, endDate);
            return Ok(usageLimits);
        }

        [HttpPut("updateCashWallet")]
        public async Task<IActionResult> UpdateCashWallet([FromBody] UpdateCashWalletDTO dto)
        {
            // Ensure you have all required parameters
            await _userRepository.UpdateCashWalletAsync(dto.Email, dto.Amount, dto.AccountId, "CREDIT");
            return Ok(new { Message = "Cash wallet updated successfully" });
        }

        [HttpPut("debitCashWallet")]
        public async Task<IActionResult> DebitCashWallet([FromBody] DebitCashWalletDTO dto)
        {
            var balances = await _userRepository.DebitCashWalletAndCreditUserAsync(dto.Email, dto.Amount, dto.AccountId);
            return Ok(new { CashWalletBalance = balances.cashWalletBalance, UserWalletBalance = balances.userWalletBalance });
        }
    }
}
