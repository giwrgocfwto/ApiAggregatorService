using System.Net.Http;
using System.Threading.Tasks;
using ApiAggregatorService.Models;
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

        public async Task<List<NewsData>> GetNewsAsync(string keyword)
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

                return newsApiResponse?.Articles; // Return the list of articles
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
                return null;
            }
        }


    }
}
