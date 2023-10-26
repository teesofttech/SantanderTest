using SantanderTest.API.Models;

namespace SantanderTest.API.Services
{
    public interface IBestStoriesCacheService
    {
        Task<IEnumerable<object>> GetBestStories(int count);
    }
}
