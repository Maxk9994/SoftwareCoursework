using System.Net;
using System.Text;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;

namespace StarterApp.Tests.Repositories;

public class ReviewRepositoryTests
{
    [Fact]
    public async Task GetReviewsForItemAsync_WhenApiReturnsReviews_ReturnsReviewList()
    {
        // Arrange
        var jsonResponse = """
        {
            "reviews": [
                {
                    "id": 1,
                    "rating": 5,
                    "comment": "Great item"
                },
                {
                    "id": 2,
                    "rating": 4,
                    "comment": "Worked well"
                }
            ]
        }
        """;

        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);
        var repository = new ReviewRepository(httpClient);

        // Act
        var reviews = await repository.GetReviewsForItemAsync(10);

        // Assert
        Assert.NotNull(reviews);
        Assert.Equal(2, reviews.Count);
        Assert.Equal(5, reviews[0].Rating);
        Assert.Equal("Great item", reviews[0].Comment);
        Assert.Equal(4, reviews[1].Rating);
        Assert.Equal("Worked well", reviews[1].Comment);
    }

    [Fact]
    public async Task GetReviewsForItemAsync_WhenApiFails_ThrowsException()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(
            HttpStatusCode.InternalServerError,
            "Server error"
        );

        var repository = new ReviewRepository(httpClient);

        // Act and Assert
        await Assert.ThrowsAsync<Exception>(() =>
            repository.GetReviewsForItemAsync(10));
    }

    [Fact]
    public async Task CreateReviewAsync_WhenApiReturnsReview_ReturnsCreatedReview()
    {
        // Arrange
        var jsonResponse = """
        {
            "id": 3,
            "rating": 5,
            "comment": "Excellent rental"
        }
        """;

        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);
        var repository = new ReviewRepository(httpClient);

        var request = new CreateReviewRequest
        {
            Rating = 5,
            Comment = "Excellent rental"
        };

        // Act
        var review = await repository.CreateReviewAsync(request, "test-token");

        // Assert
        Assert.NotNull(review);
        Assert.Equal(3, review.Id);
        Assert.Equal(5, review.Rating);
        Assert.Equal("Excellent rental", review.Comment);
    }

    [Fact]
    public async Task CreateReviewAsync_WhenApiFails_ThrowsException()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(
            HttpStatusCode.BadRequest,
            "Invalid review"
        );

        var repository = new ReviewRepository(httpClient);

        var request = new CreateReviewRequest
        {
            Rating = 5,
            Comment = "Excellent rental"
        };

        // Act and Assert
        await Assert.ThrowsAsync<Exception>(() =>
            repository.CreateReviewAsync(request, "test-token"));
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