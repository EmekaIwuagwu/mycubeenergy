using CubeEnergy.Models;
using System.Threading.Tasks;

namespace CubeEnergy.Repositories
{
    public interface IWalletRepository
    {
        Task<CashWallet> GetCashWalletByEmailAsync(string email);
        Task UpdateCashWalletAsync(CashWallet cashWallet);
        Task UpdateBalanceAndLogTransactionAsync(string email, decimal amount, string accountId, string transactionType);
    }
}
