using Moq;
using Moq.Protected;
using System.Net;

public static class HttpMessageHandlerExtensions
{
    public static void SetupRequest(this Mock<HttpMessageHandler> mock, HttpStatusCode statusCode, string content, string url = null)
    {
        mock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => url == null || req.RequestUri.ToString() == url),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            });
    }
}
