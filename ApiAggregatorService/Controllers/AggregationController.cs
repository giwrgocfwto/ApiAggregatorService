using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ApiAggregatorService.Models;
using ApiAggregatorService.Services;

namespace ApiAggregatorService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AggregationController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private readonly INewsService _newsService;

        public AggregationController(IWeatherService weatherService, INewsService newsService)
        {
            _weatherService = weatherService;
            _newsService = newsService;
        }

        // Separate endpoint for weather data
        [HttpGet("weather")]
        public async Task<IActionResult> GetWeatherData(string location)
        {
            var weatherData = await _weatherService.GetWeatherAsync(location);
            if (weatherData == null)
            {
                return StatusCode(500, "Failed to retrieve weather data.");
            }

            return Ok(weatherData);
        }

        // Separate endpoint for news data
        [HttpGet("news")]
        public async Task<IActionResult> GetNewsData(string keyword)
        {
            var newsData = await _newsService.GetNewsAsync(keyword);

            // Check if newsData is null or an empty list
            if (newsData == null || newsData.Count == 0)
            {
                return StatusCode(500, "Failed to retrieve news data.");
            }

            return Ok(newsData);
        }
    }
}
