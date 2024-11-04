using System.Net.Http;
using System.Threading.Tasks;
using ApiAggregatorService.Models;
using ApiAggregatorService.Models.Responses;
using ApiAggregatorService.Services.Interfaces;
using Newtonsoft.Json;

namespace ApiAggregatorService.Services
{
    public class NewsService : INewsService
    {
        private readonly HttpClient _httpClient;

        public NewsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<NewsData>> GetNewsAsync(string keyword, string sortBy = null, string filterBy = null)
        {
            var apiKey = "140cd018eaf64dc9b16210dab3ca1fda";
            var encodedKeyword = Uri.EscapeDataString(keyword);
            var url = $"https://newsapi.org/v2/everything?q={encodedKeyword}&language=en&sortBy=publishedAt&pageSize=10&apiKey={apiKey}";

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("User-Agent", "ApiAggregatorService/1.0");

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to fetch news data. Status: {response.StatusCode}, Reason: {response.ReasonPhrase}, Details: {errorContent}");
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var newsApiResponse = JsonConvert.DeserializeObject<NewsApiResponse>(content);
                var newsData = newsApiResponse?.Articles;

                // Apply filtering if needed
                if (!string.IsNullOrEmpty(filterBy))
                {
                    newsData = ApplyNewsFiltering(newsData, filterBy);
                }

                // Apply sorting if needed
                if (!string.IsNullOrEmpty(sortBy))
                {
                    newsData = ApplyNewsSorting(newsData, sortBy);
                }

                return newsData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
                return null;
            }
        }

        private List<NewsData> ApplyNewsSorting(List<NewsData> newsData, string sortBy)
        {
            return sortBy switch
            {
                "date" => newsData.OrderBy(item => DateTime.Parse(item.PublishedAt)).ToList(), // Ascending because default seems to be descending
                "title" => newsData.OrderBy(item => item.Title).ToList(),
                "description" => newsData.OrderBy(item => item.Description).ToList(),
                _ => newsData // Default to unsorted if sortBy is not recognized
            };
        }

        private List<NewsData> ApplyNewsFiltering(List<NewsData> newsData, string filterBy)
        {
            return newsData.Where(item =>
                item.Title.Contains(filterBy, StringComparison.OrdinalIgnoreCase) ||
                item.Description.Contains(filterBy, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }
    }
}
