using CubeEnergy.Models;

namespace CubeEnergy.Repositories
{
    public interface IKycRepository
    {
        Task CreateKycAsync(Kyc kyc);
        Task UpdateKycAsync(Kyc kyc);
        Task<bool> DeleteKycAsync(string email, int id);
        Task<Kyc> GetKycByEmailAsync(string email);
    }
}
