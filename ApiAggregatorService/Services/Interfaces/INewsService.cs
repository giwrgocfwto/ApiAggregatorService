﻿using ApiAggregatorService.Models;

namespace ApiAggregatorService.Services.Interfaces
{
    public interface INewsService
    {
        Task<List<NewsData>> GetNewsAsync(string keyword, string sortBy = null, string filterBy = null);
    }
}
