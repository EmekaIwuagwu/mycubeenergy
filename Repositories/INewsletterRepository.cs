using System.Threading.Tasks;

namespace CubeEnergy.Repositories
{
    public interface INewsletterRepository
    {
        Task SaveSubscriberEmailAsync(string email);
    }
}
