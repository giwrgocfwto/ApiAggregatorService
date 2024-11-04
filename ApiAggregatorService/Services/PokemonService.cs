using Newtonsoft.Json;
using ApiAggregatorService.Models;
using ApiAggregatorService.Services.ApiAggregatorService.Models;

public class PokemonService : IPokemonService
{
    private readonly HttpClient _httpClient;

    public PokemonService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<PokemonData>> GetPokemonByTypesAsync(string primaryType, string secondaryType = null, string sortBy = null)
    {
        try
        {
            // Step 1: Fetch all Pokémon for the primary type
            var primaryUrl = $"https://pokeapi.co/api/v2/type/{primaryType.ToLower()}";
            var primaryResponse = await _httpClient.GetStringAsync(primaryUrl);
            var primaryTypeData = JsonConvert.DeserializeObject<PokemonTypeResponse>(primaryResponse);

            HashSet<string> secondaryTypePokemonUrls = null;
            if (!string.IsNullOrEmpty(secondaryType))
            {
                var secondaryUrl = $"https://pokeapi.co/api/v2/type/{secondaryType.ToLower()}";
                var secondaryResponse = await _httpClient.GetStringAsync(secondaryUrl);
                var secondaryTypeData = JsonConvert.DeserializeObject<PokemonTypeResponse>(secondaryResponse);

                secondaryTypePokemonUrls = new HashSet<string>(secondaryTypeData.Pokemon.Select(p => p.Pokemon.Url));
            }

            var matchingPokemons = new List<PokemonData>();

            foreach (var entry in primaryTypeData.Pokemon ?? Enumerable.Empty<PokemonEntry>())
            {
                if (secondaryTypePokemonUrls == null || secondaryTypePokemonUrls.Contains(entry.Pokemon.Url))
                {
                    var pokemonResponse = await _httpClient.GetStringAsync(entry.Pokemon.Url);
                    var dynamicPokemonData = JsonConvert.DeserializeObject<dynamic>(pokemonResponse);

                    var typeNames = ((IEnumerable<dynamic>)dynamicPokemonData.types).Select(t => (string)t.type.name).ToList();

                    matchingPokemons.Add(new PokemonData
                    {
                        Id = (int)dynamicPokemonData.id,
                        Name = (string)dynamicPokemonData.name,
                        Types = typeNames,
                        Height = (int)dynamicPokemonData.height,
                        Weight = (int)dynamicPokemonData.weight
                    });
                }
            }

            var sortedPokemons = ApplySorting(matchingPokemons, sortBy);
            return sortedPokemons.Take(10).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching Pokémon data: {ex.Message}");
            // Fallback response: return an empty list or default message
            return new List<PokemonData>(); // or custom fallback data
        }
    }

    private List<PokemonData> ApplySorting(List<PokemonData> pokemons, string sortBy)
    {
        return sortBy switch
        {
            "name" => pokemons.OrderBy(p => p.Name).ToList(),
            "id" => pokemons.OrderBy(p => p.Id).ToList(),
            _ => pokemons
        };
    }
}
