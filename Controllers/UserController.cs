using CubeEnergy.DTOs;
using CubeEnergy.Models;
using CubeEnergy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CubeEnergy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        //[HttpGet("profile")]
        //public async Task<IActionResult> GetProfile([FromQuery] string email)
        //{
        //    var user = await _userService.GetUserByEmailAsync(email);
        //    if (user != null)
        //    {
        //        return Ok(user);
        //    }

        //    return NotFound("User not found.");
        //}

        //[HttpDelete("delete-account")]
        //public async Task<IActionResult> DeleteAccount([FromQuery] int userId)
        //{
        //    var success = await _userService.DeleteUserAsync(userId);
        //    if (success)
        //    {
        //        return Ok("Account Deleted");
        //    }

        //    return NotFound("User not found.");
        //}

        //[HttpPut("editProfile")]
        //public async Task<IActionResult> EditProfile([FromQuery] string email, [FromQuery] int userId, [FromBody] UpdateProfileDTO updateDto)
        //{
        //    var user = await _userService.GetUserByEmailOrIdAsync(email, userId);
        //    if (user != null)
        //    {
        //        user.Fullname = updateDto.Fullname;
        //        user.Address = updateDto.Address;
        //        user.Telephone = updateDto.Telephone;
        //        user.City = updateDto.City;
        //        user.State = updateDto.State;
        //        user.Zipcode = updateDto.Zipcode;

        //        await _userService.UpdateUserAsync(user);
        //        return Ok(user);
        //    }

        //    return NotFound("User not found.");
        //}

        //[HttpGet("calculateUnits")]
        //public async Task<IActionResult> CalculateUnits([FromQuery] int id, [FromQuery] int days)
        //{
        //    // Retrieve the unit price as a decimal
        //    var price = await _userService.GetUnitPriceAsync(id);

        //    if (price != null)
        //    {
        //        // Assuming price is a decimal value
        //        var totalCost = days * price;  // Directly use price as a decimal
        //        return Ok(new { TotalCost = totalCost });
        //    }

        //    return BadRequest("Unit price not found.");
        //}


        //[HttpPut("update-balance")]
        //public async Task<IActionResult> UpdateBalance([FromQuery] string email, [FromQuery] decimal amount, [FromQuery] string accountId)
        //{
        //    if (amount <= 0)
        //    {
        //        return BadRequest("Amount must be greater than zero.");
        //    }

        //    var user = await _userService.GetUserByEmailAsync(email);
        //    if (user == null)
        //    {
        //        return NotFound("User not found.");
        //    }

        //    await _userService.UpdateBalanceAndLogTransactionAsync(email, amount, accountId, "Credit");

        //    return Ok(new { Message = "Balance updated successfully.", NewBalance = user.UnitBalance });
        //}

        //[HttpGet("transactions")]
        //public async Task<IActionResult> GetTransactions([FromQuery] string email)
        //{
        //    var transactions = await _userService.GetTransactionsByEmailAsync(email);
        //    return Ok(transactions);
        //}

        //[HttpGet("transactions-by-date")]
        //public async Task<IActionResult> GetTransactionsByDate([FromQuery] string email, [FromQuery] DateTime from, [FromQuery] DateTime to)
        //{
        //    var transactions = await _userService.GetTransactionsByDateAsync(email, from, to);
        //    return Ok(transactions);
        //}

        //[HttpPost("share-units")]
        //public async Task<IActionResult> ShareUnits([FromBody] ShareUnitsDTO dto)
        //{
        //    var result = await _userService.ShareUnitsAsync(dto.OriginAccountId, dto.DestinationAccountId, dto.Amount);

        //    if (!result.Success)
        //    {
        //        return BadRequest(new { Message = result.ErrorMessage });
        //    }

        //    return Ok(new { Message = "Units Shared Successfully" });
        //}

        //[HttpGet("calculateTotalCost")]
        //public async Task<IActionResult> CalculateTotalCost([FromQuery] string email)
        //{
        //    try
        //    {
        //        var totalCost = await _userService.CalculateTotalCostAsync(email);
        //        return Ok(new { Email = email, TotalCost = totalCost });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Message = ex.Message });
        //    }
        //}

        //[HttpGet("showUsageLimitByMonth")]
        //public async Task<IActionResult> ShowUsageLimitByMonth([FromQuery] string email, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        //{
        //    var usageLimits = await _userService.GetUsageLimitsByMonthAsync(email, startDate, endDate);
        //    return Ok(usageLimits);
        //}

        //[HttpPut("updateCashWallet")]
        //public async Task<IActionResult> UpdateCashWallet([FromBody] UpdateCashWalletDTO dto)
        //{
        //    if (dto == null)
        //    {
        //        return BadRequest("Invalid data.");
        //    }

        //    try
        //    {
        //        await _userService.UpdateCashWalletAsync(dto.Email, dto.AccountId, dto.Amount);
        //        return Ok("Cash wallet updated successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error updating cash wallet.");
        //        return StatusCode(500, "Internal server error.");
        //    }
        //}


        //[HttpPut("debitCashWallet")]
        //public async Task<IActionResult> DebitCashWallet([FromBody] DebitCashWalletDTO dto)
        //{
        //    if (dto == null)
        //    {
        //        return BadRequest("Invalid data.");
        //    }

        //    try
        //    {
        //        var unitPrice = await _userService.GetUnitPriceAsync(); // Assuming you have a method to get unit price.
        //        if (unitPrice == null)
        //        {
        //            return BadRequest("Unit price not found.");
        //        }

        //        var unitBalance = dto.Amount * unitPrice.Price;

        //        var balances = await _userService.DebitCashWalletAndCreditUserAsync(dto.Email, dto.Amount, dto.AccountId, unitBalance);
        //        if (balances == null)
        //        {
        //            return BadRequest("Error processing transaction.");
        //        }

        //        return Ok(new
        //        {
        //            CashWalletBalance = balances.CashWalletBalance,
        //            UserWalletBalance = balances.UserWalletBalance
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error debiting cash wallet.");
        //        return StatusCode(500, "Internal server error.");
        //    }
        //}

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile([FromQuery] string email)
        {
            var user = await _userService.GetUserDetailsByEmailAsync(email);
            if (user != null)
            {
                return Ok(user);
            }

            return NotFound("User not found.");
        }

        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount([FromQuery] int userId)
        {
            var success = await _userService.DeleteUserAsync(userId);
            if (success)
            {
                return Ok("Account Deleted");
            }

            return NotFound("User not found.");
        }

        [HttpPut("editProfile")]
        public async Task<IActionResult> EditProfile([FromQuery] string email, [FromQuery] int userId, [FromBody] UpdateProfileDTO updateDto)
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

            return NotFound("User not found.");
        }

        [HttpGet("calculateUnits")]
        public async Task<IActionResult> CalculateUnits([FromQuery] int days)
        {
            var unitPrice = await _userService.GetUnitPriceAsync();

            if (unitPrice != null)
            {
                var totalCost = days * unitPrice.Price; // Ensure price is used correctly
                return Ok(new { TotalCost = totalCost });
            }

            return BadRequest("Unit price not found.");
        }

        [HttpPut("update-balance")]
        public async Task<IActionResult> UpdateBalance([FromQuery] string email, [FromQuery] decimal amount, [FromQuery] string accountId, [FromQuery] string payerName, [FromQuery] string packageType, [FromQuery] int days)
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

            await _userService.UpdateBalanceAndLogTransactionAsync(email, amount, accountId, "Credit", payerName ,packageType ,days, "Credit_debit_card");

            // Update the user balance after transaction
            user.UnitBalance += amount;

            return Ok(new { Message = "Balance updated successfully.", NewBalance = user.UnitBalance });
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions([FromQuery] string email)
        {
            var transactions = await _userService.GetTransactionsByEmailAsync(email);
            return Ok(transactions);
        }

        [HttpGet("transactions-by-date")]
        public async Task<IActionResult> GetTransactionsByDate([FromQuery] string email, [FromQuery] DateTime from, [FromQuery] DateTime to)
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
        public async Task<IActionResult> CalculateTotalCost([FromQuery] string email)
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

        /*[HttpGet("showUsageLimitByMonth")]
        public async Task<IActionResult> ShowUsageLimitByMonth([FromQuery] string email, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var usageLimits = await _userService.GetUsageLimitsByMonthAsync(email, startDate, endDate);
            return Ok(usageLimits);
        }

        [HttpPut("updateCashWallet")]
        public async Task<IActionResult> UpdateCashWallet([FromBody] UpdateCashWalletDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                await _userService.UpdateCashWalletAsync(dto.Email, dto.Amount, dto.AccountId, "Credit");
                return Ok("Cash wallet updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cash wallet.");
                return StatusCode(500, "Internal server error.");
            }
        }
        */

        [HttpPut("debitCashWallet")]
        public async Task<IActionResult> DebitCashWallet([FromBody] DebitCashWalletDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var unitPrice = await _userService.GetUnitPriceAsync(); // Get unit price appropriately
                if (unitPrice == null)
                {
                    return BadRequest("Unit price not found.");
                }

                var unitBalance = dto.Amount * unitPrice.Price;

                var balances = await _userService.DebitCashWalletAndCreditUserAsync(dto.Email, dto.Amount, dto.AccountId);
                if (balances.CashWalletBalance == 0)
                {
                    return BadRequest("Error processing transaction.");
                }

                return Ok(new
                {
                    CashWalletBalance = balances.CashWalletBalance,
                    UserWalletBalance = balances.UserWalletBalance
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error debiting cash wallet.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost("update-wallet")]
        public async Task<IActionResult> UpdateCashWallet([FromBody] CashWalletUpdateModel model)
        {
            try
            {
                await _userService.InsertCashWalletAndTransactionAsync(model.Email, model.Amount, model.AccountId, model.TransactionType);
                return Ok(new { message = "Cash wallet and transaction updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("monthlyTotalCost")]
        public async Task<ActionResult<IEnumerable<MonthlyTotalDTO>>> GetMonthlyTotalCost([FromQuery] string email, [FromQuery] int year)
        {
            if (string.IsNullOrEmpty(email) || year <= 0)
            {
                return BadRequest("Invalid parameters.");
            }

            var result = await _userService.GetMonthlyTotalCostAsync(email, year);
            return Ok(result);
        }

    }
}

public class CashWalletUpdateModel
{
    public string Email { get; set; }
    public decimal Amount { get; set; }
    public string AccountId { get; set; }
    public string TransactionType { get; set; }
}
