using Moq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ApiAggregatorService.Services;

public class NewsServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly NewsService _newsService;

    public NewsServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _newsService = new NewsService(_httpClient);
    }

    [Fact]
    public async Task GetNewsAsync_SuccessfulResponse_ReturnsNewsData()
    {
        // Arrange
        var responseContent = "{\"articles\":[{\"title\":\"Sample News\",\"description\":\"Sample Description\"}]}";
        _httpMessageHandlerMock.SetupRequest(HttpStatusCode.OK, responseContent);

        // Act
        var result = await _newsService.GetNewsAsync("sample");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Sample News", result[0].Title);
    }

    [Fact]
    public async Task GetNewsAsync_ApiFailure_ReturnsEmptyList()
    {
        // Arrange
        _httpMessageHandlerMock.SetupRequest(HttpStatusCode.BadRequest, "Error");

        // Act
        var result = await _newsService.GetNewsAsync("sample");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
