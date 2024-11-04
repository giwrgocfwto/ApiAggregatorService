using System.Net.Http;
using System.Threading.Tasks;
using ApiAggregatorService.Models;
using Newtonsoft.Json;

namespace ApiAggregatorService.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<WeatherData>> GetWeatherAsync(string location, string sortBy = null, string filterBy = null)
        {
            var apiKey = "9a0b8e1571e5ab2e7049843b93648ac0";
            var encodedLocation = Uri.EscapeDataString(location);
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={encodedLocation}&appid={apiKey}&units=metric";

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("User-Agent", "ApiAggregatorService/1.0");

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to fetch weather data. Status: {response.StatusCode}, Reason: {response.ReasonPhrase}, Details: {errorContent}");
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<WeatherApiResponse>(content);

                // Convert to List for easier sorting and filtering
                var weatherData = new List<WeatherData>
                {
                    new WeatherData
                    {
                        City = apiResponse.Name,
                        Temperature = apiResponse.Main.Temp,
                        Condition = apiResponse.Weather.FirstOrDefault()?.Description
                    }
                };

                // Apply filtering if needed
                if (!string.IsNullOrEmpty(filterBy))
                {
                    weatherData = ApplyWeatherFiltering(weatherData, filterBy);
                }

                // Apply sorting if needed
                if (!string.IsNullOrEmpty(sortBy))
                {
                    weatherData = ApplyWeatherSorting(weatherData, sortBy);
                }

                return weatherData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
                return null;
            }
        }

        private List<WeatherData> ApplyWeatherSorting(List<WeatherData> weatherData, string sortBy) // Not needed though
        {
            return sortBy switch
            {
                "temperature" => weatherData.OrderByDescending(item => item.Temperature).ToList(),
                "city" => weatherData.OrderBy(item => item.City).ToList(),
                _ => weatherData // Return unsorted if `sortBy` is not recognized
            };
        }

        private List<WeatherData> ApplyWeatherFiltering(List<WeatherData> weatherData, string filterBy) // Same for this
        {
            return weatherData.Where(item =>
                item.Condition.Contains(filterBy, StringComparison.OrdinalIgnoreCase) ||
                item.City.Contains(filterBy, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }
    }
}
