using CubeEnergy.DTOs;
using CubeEnergy.Models;
using CubeEnergy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CubeEnergy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
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

        /*[HttpPost("enterUnitPrice")]
        public async Task<IActionResult> EnterUnitPrice(UnitPriceDTO priceDto)
        {
            var price = new UnitPrice { Price = priceDto.Price };
            var createdPrice = await _userService.SaveUnitPriceAsync(price);
            return Ok(createdPrice);
        }

        [HttpPut("updateUnitPrice")]
        public async Task<IActionResult> UpdateUnitPrice(UnitPriceDTO priceDto)
        {
            var price = new UnitPrice { Id = priceDto.Id, Price = priceDto.Price };
            var updatedPrice = await _userService.UpdateUnitPriceAsync(price);
            return Ok(new { Message = "Price for units updated Successfully", UnitPrice = updatedPrice });
        }

        [HttpDelete("deleteUnitPrice")]
        public async Task<IActionResult> DeleteUnitPrice(int id)
        {
            await _userService.DeleteUnitPriceAsync(id);
            return Ok("Price deleted successfully.");
        }
        */

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
            if (amount <= 0)
            {
                return BadRequest("Amount must be greater than zero.");
            }

            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            await _userService.UpdateBalanceAndLogTransactionAsync(email, amount, accountId);

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

        /*[HttpPost("calculate-bills")]
        public async Task<IActionResult> CalculateBills([FromQuery] string email, [FromQuery] double hours)
        {
            if (hours <= 0)
            {
                return BadRequest("Hours must be greater than zero.");
            }

            var usageDuration = TimeSpan.FromHours(hours);
            await _userService.CalculateAndUpdateBillsUsageAsync(email, usageDuration);

            return Ok("Bills usage calculated and balance updated.");
        }
        */

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

    }
}
