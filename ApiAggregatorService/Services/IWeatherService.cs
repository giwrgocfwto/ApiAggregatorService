using System.Threading.Tasks;
using ApiAggregatorService.Models;

namespace ApiAggregatorService.Services
{
    public interface IWeatherService
    {
        Task<WeatherData> GetWeatherAsync(string location);
    }
}
