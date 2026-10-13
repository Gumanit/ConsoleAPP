using System.Net;
using System.Net.Http;
using System.Text;
using ConsoleApp.Core.Api;

namespace ConsoleApp.Tests.Api;

/// <summary>Тесты HTTP-клиента <see cref="TodoApiClient"/> с подставным обработчиком.</summary>
public sealed class TodoApiClientTests
{
    [Fact]
    public async Task GetTodosAsync_DeserializesPayload_AndRespectsLimit()
    {
        const string json = """
            [
              { "id": 1, "userId": 1, "title": "alpha", "completed": false },
              { "id": 2, "userId": 1, "title": "beta", "completed": true }
            ]
            """;

        var client = CreateClient(new StubHandler(HttpStatusCode.OK, json));

        var todos = await client.GetTodosAsync(limit: 1);

        Assert.Single(todos);
        Assert.Equal("alpha", todos[0].Title);
    }

    [Fact]
    public async Task GetTodosAsync_Throws_WhenServiceUnavailable()
    {
        var client = CreateClient(new StubHandler(HttpStatusCode.ServiceUnavailable));

        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetTodosAsync(limit: 5));
    }

    private static TodoApiClient CreateClient(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.test/"),
        };

        return new TodoApiClient(httpClient);
    }

    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string? _content;

        public StubHandler(HttpStatusCode statusCode, string? content = null)
        {
            _statusCode = statusCode;
            _content = content;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_statusCode);

            if (_content is not null)
            {
                response.Content = new StringContent(_content, Encoding.UTF8, "application/json");
            }

            return Task.FromResult(response);
        }
    }
}
