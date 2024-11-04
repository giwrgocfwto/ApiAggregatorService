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

        public async Task<WeatherData> GetWeatherAsync(string location)
        {
            var apiKey = "9a0b8e1571e5ab2e7049843b93648ac0";
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={location}&appid={apiKey}&units=metric";

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

                var weatherData = new WeatherData
                {
                    City = apiResponse.Name,
                    Temperature = apiResponse.Main.Temp,
                    Condition = apiResponse.Weather.FirstOrDefault()?.Description
                };

                return weatherData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
                return null;
            }
        }

    }
}
