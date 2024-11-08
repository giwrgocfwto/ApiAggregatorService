﻿using ApiAggregatorService.Models;

public interface IPokemonService
{
    Task<List<PokemonData>> GetPokemonByTypesAsync(string primaryType, string secondaryType = null, string sortBy = null);
}
