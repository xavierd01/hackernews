using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using hackernews.Models;

namespace hackernews.Controllers
{
    public class BestStoryController : Controller
    {
        // TODO Use in-memory cache for now. 
        // Will investigate distributed cache before uploading to Azure.
        private IMemoryCache cache;

        private static HttpClient client = new HttpClient();

        public BestStoryController(IMemoryCache memoryCache)
        {
            cache = memoryCache;
        }

        // TODO Could store in configuration file/data store.
        const string BestStoriesApi = "https://hacker-news.firebaseio.com/v0/beststories.json";
        const string StoryApiTemplate = "https://hacker-news.firebaseio.com/v0/item/{0}.json";
        
        public async Task<ActionResult> Index(string searchString)
        {
            List<HackerStorySummary> stories = new List<HackerStorySummary>();

            var response = await client.GetAsync(BestStoriesApi);
            if (response.IsSuccessStatusCode)
            {
                var storiesResponse = response.Content.ReadAsStringAsync().Result;
                var bestIds = JsonConvert.DeserializeObject<List<int>>(storiesResponse);

                var tasks = bestIds.Select(GetStoryAsync);
                stories = (await Task.WhenAll(tasks)).ToList();

                if (!String.IsNullOrEmpty(searchString))
                {
                    var search = searchString.ToLower();
                    // Provide feedback to View
                    ViewData["Filter"] = searchString;
                    stories = stories.Where(s => 
                                        s.Title.ToLower().IndexOf(search) > -1 || s.By.ToLower().IndexOf(search) > -1)
                                        .ToList();
                }
            }
            else
            {
                // TODO Just indicate failed attempt. Could add more specific errors. 
                ViewData["FailedConnection"] = "true";
            }
            return View(stories);
        }

        private async Task<HackerStorySummary> GetStoryAsync(int storyId)
        {
            return await cache.GetOrCreateAsync<HackerStorySummary>(storyId,
                async cacheEntry => {
                    HackerStorySummary story = new HackerStorySummary();

                    var response = await client.GetAsync(string.Format(StoryApiTemplate, storyId));
                    if (response.IsSuccessStatusCode)
                    {
                        var storyResponse = response.Content.ReadAsStringAsync().Result;
                        story = JsonConvert.DeserializeObject<HackerStorySummary>(storyResponse);
                    }
                    else
                    {
                        ViewData["FailedConnection"] = "true";
                        story.Title = string.Format("ERROR RETRIEVING STORY (ID {0})", storyId);
                    }
                    
                    return story;                    
                });
        }
    }
}
