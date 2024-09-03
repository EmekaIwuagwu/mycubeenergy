namespace CubeEnergy.Repositories
{
    public interface IInverterService
    {
        Task<decimal> GetBalanceAsync(string accountId);
        Task SaveDailyLimitAsync(string accountId, decimal hoursSpent);
    }
}
