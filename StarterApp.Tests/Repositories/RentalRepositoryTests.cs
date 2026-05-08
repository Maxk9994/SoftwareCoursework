using System.Net;
using System.Text;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;

namespace StarterApp.Tests.Repositories;

public class RentalRepositoryTests
{
    [Fact]
    public async Task GetIncomingRentalsAsync_WhenApiReturnsRentals_ReturnsRentalList()
    {
        // Arrange
        var jsonResponse = """
        {
            "rentals": [
                {
                    "id": 1,
                    "itemId": 10,
                    "startDate": "2026-05-01",
                    "endDate": "2026-05-04",
                    "status": "Pending"
                },
                {
                    "id": 2,
                    "itemId": 11,
                    "startDate": "2026-05-10",
                    "endDate": "2026-05-12",
                    "status": "Approved"
                }
            ]
        }
        """;

        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);
        var repository = new RentalRepository(httpClient);

        // Act
        var rentals = await repository.GetIncomingRentalsAsync("test-token");

        // Assert
        Assert.NotNull(rentals);
        Assert.Equal(2, rentals.Count);
        Assert.Equal("Pending", rentals[0].Status);
        Assert.Equal("Approved", rentals[1].Status);
    }

    [Fact]
    public async Task GetIncomingRentalsAsync_WhenApiFails_ThrowsException()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(
            HttpStatusCode.InternalServerError,
            "Server error"
        );

        var repository = new RentalRepository(httpClient);

        // Act and Assert
        await Assert.ThrowsAsync<Exception>(() =>
            repository.GetIncomingRentalsAsync("test-token"));
    }

    [Fact]
    public async Task GetOutgoingRentalsAsync_WhenApiReturnsRentals_ReturnsRentalList()
    {
        // Arrange
        var jsonResponse = """
        {
            "rentals": [
                {
                    "id": 3,
                    "itemId": 20,
                    "startDate": "2026-06-01",
                    "endDate": "2026-06-03",
                    "status": "Out for Rent"
                }
            ]
        }
        """;

        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);
        var repository = new RentalRepository(httpClient);

        // Act
        var rentals = await repository.GetOutgoingRentalsAsync("test-token");

        // Assert
        Assert.NotNull(rentals);
        Assert.Single(rentals);
        Assert.Equal("Out for Rent", rentals[0].Status);
    }

    [Fact]
    public async Task GetOutgoingRentalsAsync_WhenApiFails_ThrowsException()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(
            HttpStatusCode.BadRequest,
            "Bad request"
        );

        var repository = new RentalRepository(httpClient);

        // Act and Assert
        await Assert.ThrowsAsync<Exception>(() =>
            repository.GetOutgoingRentalsAsync("test-token"));
    }

    [Fact]
    public async Task CreateRentalRequestAsync_WhenApiReturnsRental_ReturnsCreatedRental()
    {
        // Arrange
        var jsonResponse = """
        {
            "id": 5,
            "itemId": 10,
            "startDate": "2026-05-01",
            "endDate": "2026-05-04",
            "status": "Pending"
        }
        """;

        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);
        var repository = new RentalRepository(httpClient);

        var request = new CreateRentalRequest
        {
            ItemId = 10,
            StartDate = new DateTime(2026, 5, 1),
            EndDate = new DateTime(2026, 5, 4)
        };

        // Act
        var rental = await repository.CreateRentalRequestAsync(request, "test-token");

        // Assert
        Assert.NotNull(rental);
        Assert.Equal(5, rental.Id);
        Assert.Equal(10, rental.ItemId);
        Assert.Equal("Pending", rental.Status);
    }

    [Fact]
    public async Task CreateRentalRequestAsync_WhenApiFails_ThrowsException()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(
            HttpStatusCode.BadRequest,
            "Invalid rental request"
        );

        var repository = new RentalRepository(httpClient);

        var request = new CreateRentalRequest
        {
            ItemId = 10,
            StartDate = new DateTime(2026, 5, 1),
            EndDate = new DateTime(2026, 5, 4)
        };

        // Act and Assert
        await Assert.ThrowsAsync<Exception>(() =>
            repository.CreateRentalRequestAsync(request, "test-token"));
    }

    [Fact]
    public async Task UpdateRentalStatusAsync_WhenApiReturnsRental_ReturnsUpdatedRental()
    {
        // Arrange
        var jsonResponse = """
        {
            "id": 7,
            "itemId": 10,
            "startDate": "2026-05-01",
            "endDate": "2026-05-04",
            "status": "Approved"
        }
        """;

        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);
        var repository = new RentalRepository(httpClient);

        // Act
        var rental = await repository.UpdateRentalStatusAsync(
            rentalId: 7,
            status: "Approved",
            token: "test-token"
        );

        // Assert
        Assert.NotNull(rental);
        Assert.Equal(7, rental.Id);
        Assert.Equal("Approved", rental.Status);
    }

    [Fact]
    public async Task UpdateRentalStatusAsync_WhenApiFails_ThrowsException()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(
            HttpStatusCode.InternalServerError,
            "Status update failed"
        );

        var repository = new RentalRepository(httpClient);

        // Act and Assert
        await Assert.ThrowsAsync<Exception>(() =>
            repository.UpdateRentalStatusAsync(7, "Approved", "test-token"));
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