using System.Threading.Tasks;
using ApiAggregatorService.Models;

namespace ApiAggregatorService.Services
{
    public interface INewsService
    {
        Task<List<NewsData>> GetNewsAsync(string keyword);
    }
}
