using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SantanderTest.API.Services;

namespace SantanderTest.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BestStoriesController : ControllerBase
    {
        private readonly ILogger<BestStoriesController> _logger;
        private readonly IBestStoriesCacheService _bestStoriesCacheService;

        public BestStoriesController(ILogger<BestStoriesController> logger, IBestStoriesCacheService bestStoriesCacheService)
        {
            _logger = logger;

            _bestStoriesCacheService = bestStoriesCacheService;
        }

        [HttpGet]
        [Route("get/{num}")]
        public async Task<IEnumerable<object>> Get(int num)
        {
            return await _bestStoriesCacheService.GetBestStories(num);
        }
    }
}
