using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ApiAggregatorService.Models;
using ApiAggregatorService.Services.Interfaces;

namespace ApiAggregatorService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AggregationController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private readonly INewsService _newsService;
        private readonly IPokemonService _pokemonService;

        public AggregationController(IWeatherService weatherService, INewsService newsService, IPokemonService pokemonService)
        {
            _weatherService = weatherService;
            _newsService = newsService;
            _pokemonService = pokemonService;
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

        [HttpGet("pokemon")]
        public async Task<IActionResult> GetPokemonByTypes(string primaryType, string secondaryType = null, string sortBy = null)
        {
            var pokemons = await _pokemonService.GetPokemonByTypesAsync(primaryType, secondaryType, sortBy);

            if (pokemons == null || !pokemons.Any())
            {
                return NotFound("No Pokémon found with the specified types.");
            }

            return Ok(pokemons);
        }
    }
}
