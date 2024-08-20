using CubeEnergy.Data;
using CubeEnergy.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CubeEnergy.Repositories
{
    public class KycRepository : IKycRepository
    {
        private readonly ApplicationDbContext _context;

        public KycRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateKycAsync(Kyc kyc)
        {
            _context.Kycs.Add(kyc);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateKycAsync(Kyc kyc)
        {
            _context.Kycs.Update(kyc);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteKycAsync(string email, int id)
        {
            var kyc = await _context.Kycs.FirstOrDefaultAsync(k => k.Email == email && k.Id == id);
            if (kyc != null)
            {
                _context.Kycs.Remove(kyc);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<Kyc> GetKycByEmailAsync(string email)
        {
            return await _context.Kycs.FirstOrDefaultAsync(k => k.Email == email);
        }
    }
}
