using System.Net;
using System.Text;
using StarterApp.Database.Data.Repositories;

namespace StarterApp.Tests.Repositories;

public class ItemRepositoryTests
{
    [Fact]
    public async Task GetItemsAsync_WhenApiReturnsItems_ReturnsItemList()
    {
        // Arrange
        var jsonResponse = """
        {
            "items": [
                {
                    "id": 1,
                    "title": "Power Drill",
                    "description": "A useful drill",
                    "dailyRate": 10,
                    "isAvailable": true
                },
                {
                    "id": 2,
                    "title": "Camping Tent",
                    "description": "A two-person tent",
                    "dailyRate": 15,
                    "isAvailable": true
                }
            ]
        }
        """;

        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);
        var repository = new ItemRepository(httpClient);

        // Act
        var items = await repository.GetItemsAsync();

        // Assert
        Assert.NotNull(items);
        Assert.Equal(2, items.Count);
        Assert.Equal("Power Drill", items[0].Title);
        Assert.Equal("Camping Tent", items[1].Title);
    }

    [Fact]
    public async Task GetItemsAsync_WhenApiFails_ThrowsException()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(
            HttpStatusCode.InternalServerError,
            "Server error"
        );

        var repository = new ItemRepository(httpClient);

        // Act and Assert
        await Assert.ThrowsAsync<Exception>(() => repository.GetItemsAsync());
    }

    [Fact]
    public async Task GetCategoriesAsync_WhenApiReturnsCategories_ReturnsCategoryList()
    {
        // Arrange
        var jsonResponse = """
        {
            "categories": [
                {
                    "id": 1,
                    "name": "Tools"
                },
                {
                    "id": 2,
                    "name": "Camping"
                }
            ]
        }
        """;

        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);
        var repository = new ItemRepository(httpClient);

        // Act
        var categories = await repository.GetCategoriesAsync();

        // Assert
        Assert.NotNull(categories);
        Assert.Equal(2, categories.Count);
        Assert.Equal("Tools", categories[0].Name);
        Assert.Equal("Camping", categories[1].Name);
    }

    [Fact]
    public async Task GetCategoriesAsync_WhenApiFails_ThrowsException()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(
            HttpStatusCode.BadRequest,
            "Bad request"
        );

        var repository = new ItemRepository(httpClient);

        // Act and Assert
        await Assert.ThrowsAsync<Exception>(() => repository.GetCategoriesAsync());
    }

    private static HttpClient CreateMockHttpClient(HttpStatusCode statusCode, string content)
    {
        var handler = new FakeHttpMessageHandler(statusCode, content);

        return new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost/")
        };
    }

    private class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _content;

        public FakeHttpMessageHandler(HttpStatusCode statusCode, string content)
        {
            _statusCode = statusCode;
            _content = content;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = _statusCode,
                Content = new StringContent(_content, Encoding.UTF8, "application/json")
            };

            return Task.FromResult(response);
        }
    }
}