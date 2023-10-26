using LazyCache;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SantanderTest.API.Models;
using SantanderTest.API.Settings;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;

namespace SantanderTest.API.Services
{
    public class BestStoriesCacheService : IBestStoriesCacheService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<BestStoriesCacheService> logger;
        private readonly HackerNewsApiConfigurationSettings hackerNewsApiConfiguration;
        private readonly HttpClient client;
        public List<StoryInformation> BestStories { get; set; }
        public BestStoriesCacheService(IOptions<HackerNewsApiConfigurationSettings> hackerNewsApiConfiguration, IHttpClientFactory httpClientFactory,
           ILogger<BestStoriesCacheService> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
            this.hackerNewsApiConfiguration = hackerNewsApiConfiguration.Value;
            BestStories = new List<StoryInformation>();
            client = httpClientFactory.CreateClient();
        }

        private async Task<List<StoryInformation>> LoadStories()
        {
            logger.LogInformation("Loading stories from HackerNews");
            var url = new Uri(hackerNewsApiConfiguration.BestStoriesIdUrl);
            try
            {
                logger.LogInformation($"Calling HackerNews API for List for story numbers");
                var httpResponse = await client.GetAsync(url);
                var response = await httpResponse.Content.ReadAsStringAsync();

                if (httpResponse.IsSuccessStatusCode)
                {
                    var stories = JsonConvert.DeserializeObject<List<string>>(response);
                    List<StoryInformation> bestStories = new List<StoryInformation>();
                    var bag = new ConcurrentBag<StoryInformation>();
                    var tasks = stories.Select(async item =>
                    {
                        var urls = new Uri(hackerNewsApiConfiguration.StoryDetailsUrl + item + ".json");
                        var httpResponse1 = await client.GetAsync(urls);
                        var storyDetails = await httpResponse1.Content.ReadAsStringAsync();
                        var details = JsonConvert.DeserializeObject<StoryInformation>(storyDetails);
                        bestStories.Add(details);
                    });
                    await Task.WhenAll(tasks);
                    BestStories.AddRange(bestStories);

                }
                else
                {
                    if (httpResponse.StatusCode.Equals(HttpStatusCode.NotFound))
                    {
                        logger.LogInformation($"Stories was not found");
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, $@"Error calling  HackerNews API => {hackerNewsApiConfiguration.BestStoriesIdUrl}");
            }
            finally
            {
                logger.LogInformation($"End Calling  HackerNews API => {hackerNewsApiConfiguration.BestStoriesIdUrl}");
            }
            return BestStories;
        }

        public async Task<IEnumerable<object>> GetBestStories(int count)
        {
            var get = await LoadStories();
            if (count > get.Count)
            {
                return get.OrderByDescending(st => st.Score).Select(s => new { s.Title, Uri = s.Url, PostedBy = s.By, s.Time, s.Score, CommentCount = s.Descendants });
            }
            return get.OrderByDescending(st => st.Score).Take(count).Select(s => new { s.Title, Uri = s.Url, PostedBy = s.By, s.Time, s.Score, CommentCount = s.Descendants });
        }
    }
}
