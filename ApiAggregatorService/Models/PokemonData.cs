using ApiAggregatorService.Services.ApiAggregatorService.Models;

namespace ApiAggregatorService.Models
{
    public class PokemonData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Types { get; set; } 
        public int Height { get; set; }
        public int Weight { get; set; }
    }

    public class SpeciesData
    {
        public string Name { get; set; }
        public List<FlavorTextEntry> FlavorTextEntries { get; set; }
    }

    public class FlavorTextEntry
    {
        public string FlavorText { get; set; }
        public NamedAPIResource Language { get; set; }
    }
}
