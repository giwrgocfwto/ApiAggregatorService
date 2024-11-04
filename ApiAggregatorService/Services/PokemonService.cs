using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
        // Step 1: Fetch all Pokémon for the primary type
        var primaryUrl = $"https://pokeapi.co/api/v2/type/{primaryType.ToLower()}";
        var primaryResponse = await _httpClient.GetStringAsync(primaryUrl);
        var primaryTypeData = JsonConvert.DeserializeObject<PokemonTypeResponse>(primaryResponse);

        // Step 2: If a secondary type is specified, fetch all Pokémon for that type and use a HashSet for fast lookup
        HashSet<string> secondaryTypePokemonUrls = null;
        if (!string.IsNullOrEmpty(secondaryType))
        {
            var secondaryUrl = $"https://pokeapi.co/api/v2/type/{secondaryType.ToLower()}";
            var secondaryResponse = await _httpClient.GetStringAsync(secondaryUrl);
            var secondaryTypeData = JsonConvert.DeserializeObject<PokemonTypeResponse>(secondaryResponse);

            secondaryTypePokemonUrls = new HashSet<string>(secondaryTypeData.Pokemon.Select(p => p.Pokemon.Url));
        }

        // Step 3: Filter primary type Pokémon by checking against the secondary type list (if provided), and keep only names and URLs
        var filteredPokemonEntries = primaryTypeData.Pokemon
            .Where(entry => secondaryTypePokemonUrls == null || secondaryTypePokemonUrls.Contains(entry.Pokemon.Url))
            .Select(entry => new { entry.Pokemon.Name, entry.Pokemon.Url })
            .ToList();

        // Step 4: Sort the filtered list based on the selected criteria (name or ID)
        filteredPokemonEntries = sortBy switch
        {
            "name" => filteredPokemonEntries.OrderBy(p => p.Name).ToList(),
            "id" => filteredPokemonEntries.OrderBy(p => p.Url).ToList(), // Assuming URL contains ID as a unique part
            _ => filteredPokemonEntries
        };

        // Step 5: Limit to top 10 after sorting
        var top10PokemonEntries = filteredPokemonEntries.Take(10).ToList();

        // Step 6: Fetch detailed data only for these top 10 Pokémon
        var pokemons = new List<PokemonData>();
        foreach (var entry in top10PokemonEntries)
        {
            var pokemonResponse = await _httpClient.GetStringAsync(entry.Url);
            var dynamicPokemonData = JsonConvert.DeserializeObject<dynamic>(pokemonResponse);

            var typeNames = ((IEnumerable<dynamic>)dynamicPokemonData.types)
                .Select(t => (string)t.type.name)
                .ToList();

            pokemons.Add(new PokemonData
            {
                Id = (int)dynamicPokemonData.id,
                Name = (string)dynamicPokemonData.name,
                Types = typeNames,
                Height = (int)dynamicPokemonData.height,
                Weight = (int)dynamicPokemonData.weight
            });
        }

        return pokemons;
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
