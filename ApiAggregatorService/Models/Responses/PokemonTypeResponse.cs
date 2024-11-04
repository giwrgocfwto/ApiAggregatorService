namespace ApiAggregatorService.Services
{
    namespace ApiAggregatorService.Models
    {
        public class PokemonTypeResponse
        {
            public List<PokemonEntry> Pokemon { get; set; }
        }

        public class PokemonEntry
        {
            public NamedAPIResource Pokemon { get; set; }
        }

        public class NamedAPIResource
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }

        public class PokemonType
        {
            public NamedAPIResource Type { get; set; }
        }
    }
}
