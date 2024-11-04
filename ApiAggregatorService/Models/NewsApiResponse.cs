﻿using System.Collections.Generic;

namespace ApiAggregatorService.Models
{
    public class NewsApiResponse
    {
        public string Status { get; set; }
        public int TotalResults { get; set; }
        public List<NewsData> Articles { get; set; }
    }
}