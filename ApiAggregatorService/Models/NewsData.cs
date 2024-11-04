namespace ApiAggregatorService.Models
{
    public class NewsData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string PublishedAt { get; set; }
        public string Category { get; set; } // Optional; only include if relevant to your data
    }
}
