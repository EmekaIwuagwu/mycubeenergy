using CubeEnergy.Data;
using CubeEnergy.Models;
using Microsoft.EntityFrameworkCore;

namespace CubeEnergy.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly ApplicationDbContext _context;

        public WalletRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CashWallet> GetCashWalletByEmailAsync(string email)
        {
            return await _context.CashWallets.FirstOrDefaultAsync(cw => cw.Email == email);
        }

        public async Task UpdateCashWalletAsync(CashWallet cashWallet)
        {
            _context.CashWallets.Update(cashWallet);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBalanceAndLogTransactionAsync(string email, decimal amount, string accountId, string transactionType)
        {
            var cashWallet = await GetCashWalletByEmailAsync(email);

            if (cashWallet == null)
            {
                throw new Exception("Cash Wallet not found");
            }

            cashWallet.Balance += amount;

            var transaction = new Transaction
            {
                AccountId = accountId,
                Email = email,
                Amount = amount,
                TransactionType = transactionType,
                TransactionDate = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
