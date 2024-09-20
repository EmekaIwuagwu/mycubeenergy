using System.Threading.Tasks;

namespace CubeEnergy.Services
{
    public interface INewsletterService
    {
        Task SubscribeAsync(string email);
    }
}
