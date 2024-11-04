using ApiAggregatorService.Services;
using Moq;
using System.Net;

public class WeatherServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly WeatherService _weatherService;

    public WeatherServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _weatherService = new WeatherService(_httpClient);
    }

    [Fact]
    public async Task GetWeatherAsync_SuccessfulResponse_ReturnsWeatherData()
    {
        // Arrange
        var responseContent = "{\"name\":\"Sample City\",\"main\":{\"temp\":25},\"weather\":[{\"description\":\"Sunny\"}]}";
        _httpMessageHandlerMock.SetupRequest(HttpStatusCode.OK, responseContent);

        // Act
        var result = await _weatherService.GetWeatherAsync("Sample City");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Sample City", result[0].City);
        Assert.Equal(25, result[0].Temperature);
        Assert.Equal("Sunny", result[0].Condition);
    }

    [Fact]
    public async Task GetWeatherAsync_ApiFailure_ReturnsFallbackWeatherData()
    {
        // Arrange
        _httpMessageHandlerMock.SetupRequest(HttpStatusCode.BadRequest, "Error");

        // Act
        var result = await _weatherService.GetWeatherAsync("Sample City");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Sample City", result[0].City);
        Assert.Equal(0, result[0].Temperature);
        Assert.Equal("Unavailable", result[0].Condition);
    }
}
