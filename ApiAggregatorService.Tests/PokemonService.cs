using Moq;
using System.Net;

public class PokemonServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly PokemonService _pokemonService;

    public PokemonServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _pokemonService = new PokemonService(_httpClient);
    }

    [Fact]
    public async Task GetPokemonByTypesAsync_SuccessfulResponse_ReturnsPokemonData()
    {
        // Arrange
        var typeResponseContent = "{\"pokemon\":[{\"pokemon\":{\"name\":\"Pikachu\",\"url\":\"https://pokeapi.co/api/v2/pokemon/25/\"}}]}";
        var pokemonResponseContent = "{\"id\":25,\"name\":\"Pikachu\",\"types\":[{\"type\":{\"name\":\"electric\"}}],\"height\":4,\"weight\":60}";
        _httpMessageHandlerMock.SetupRequest(HttpStatusCode.OK, typeResponseContent, "https://pokeapi.co/api/v2/type/electric");
        _httpMessageHandlerMock.SetupRequest(HttpStatusCode.OK, pokemonResponseContent, "https://pokeapi.co/api/v2/pokemon/25/");

        // Act
        var result = await _pokemonService.GetPokemonByTypesAsync("electric");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Pikachu", result[0].Name);
        Assert.Equal(4, result[0].Height);
        Assert.Equal(60, result[0].Weight);
    }

    [Fact]
    public async Task GetPokemonByTypesAsync_ApiFailure_ReturnsEmptyList()
    {
        // Arrange
        _httpMessageHandlerMock.SetupRequest(HttpStatusCode.BadRequest, "Error");

        // Act
        var result = await _pokemonService.GetPokemonByTypesAsync("electric");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
