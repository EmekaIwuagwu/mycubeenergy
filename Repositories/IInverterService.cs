namespace CubeEnergy.Repositories
{
    public interface IInverterService
    {
        Task<decimal> GetBalanceAsync(string accountId);
        Task SaveDailyLimitAsync(string accountId, decimal hoursSpent);
        Task RegisterSmartMeterAsync(string macAddress, string accountId);
        Task<string> GetAccountIdByMacAddressAsync(string macAddress);
    }
}
