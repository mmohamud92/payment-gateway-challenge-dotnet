using System.Net;
using System.Text;

namespace PaymentGateway.Infrastructure.UnitTests.Helpers;

// A simple fake HttpMessageHandler for testing purposes.
public class FakeHttpMessageHandler(string response, HttpStatusCode statusCode) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponseMessage = new(statusCode)
        {
            Content = new StringContent(response, Encoding.UTF8, "application/json")
        };
        return Task.FromResult(httpResponseMessage);
    }
}