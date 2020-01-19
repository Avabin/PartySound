using System.Threading.Tasks;
using RestEase;

namespace GooglePlayMusic.Internal
{
    public interface IGpmConnectorClient
    {
        [Get("/health")]
        Task<string> CheckHealth();

        [Post("/auth")]
        Task<string> PerformOAuth();
    }
}