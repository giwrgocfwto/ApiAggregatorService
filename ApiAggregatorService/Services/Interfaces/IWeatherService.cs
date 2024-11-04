using System.Threading.Tasks;
using ApiAggregatorService.Models;

namespace ApiAggregatorService.Services.Interfaces
{
    public interface IWeatherService
    {
        Task<List<WeatherData>> GetWeatherAsync(string location, string sortBy = null, string filterBy = null);
    }
}
