namespace CubeEnergy.Repositories
{
    public interface IInverterService
    {
        Task<decimal> GetBalanceAsync(string accountId);
        Task SaveDailyLimitAsync(string email, string accountId, decimal hoursSpent);
    }
}
