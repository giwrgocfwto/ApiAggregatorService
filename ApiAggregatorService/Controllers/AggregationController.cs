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
        public async Task<IActionResult> GetWeatherData(string location, string sortBy = null, string filterBy = null)
        {
            var weatherData = await _weatherService.GetWeatherAsync(location, sortBy, filterBy);

            if (weatherData == null || !weatherData.Any())
            {
                return StatusCode(500, "Failed to retrieve weather data.");
            }

            return Ok(weatherData);
        }


        // Separate endpoint for news data
        [HttpGet("news")]
        public async Task<IActionResult> GetNewsData(string keyword, string sortBy = null, string filterBy = null)
        {
            var newsData = await _newsService.GetNewsAsync(keyword, sortBy, filterBy);

            if (newsData == null || !newsData.Any())
            {
                return StatusCode(500, "Failed to retrieve news data.");
            }

            return Ok(newsData);
        }

    }
}
