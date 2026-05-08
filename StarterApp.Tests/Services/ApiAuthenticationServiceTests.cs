using System.Net;
using System.Text;
using StarterApp.Services;

namespace StarterApp.Tests.Services;

public class ApiAuthenticationServiceTests
{
    [Fact]
    public async Task RegisterAsync_WhenApiReturnsSuccess_ReturnsSuccessfulResult()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, "{}");
        var service = new ApiAuthenticationService(httpClient);

        // Act
        var result = await service.RegisterAsync(
            "Test",
            "User",
            "test@example.com",
            "Password123!"
        );

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Registration successful. Please log in.", result.Message);
    }

    [Fact]
    public async Task RegisterAsync_WhenApiReturnsError_ReturnsFailureResult()
    {
        // Arrange
        var jsonResponse = """
        {
            "error": "ValidationError",
            "message": "Email already exists"
        }
        """;

        var httpClient = CreateMockHttpClient(HttpStatusCode.BadRequest, jsonResponse);
        var service = new ApiAuthenticationService(httpClient);

        // Act
        var result = await service.RegisterAsync(
            "Test",
            "User",
            "test@example.com",
            "Password123!"
        );

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Email already exists", result.Message);
    }

    [Fact]
    public async Task LoginAsync_WhenApiReturnsError_ReturnsFailureResult()
    {
        // Arrange
        var jsonResponse = """
        {
            "error": "InvalidCredentials",
            "message": "Invalid email or password"
        }
        """;

        var httpClient = CreateMockHttpClient(HttpStatusCode.Unauthorized, jsonResponse);
        var service = new ApiAuthenticationService(httpClient);

        // Act
        var result = await service.LoginAsync("test@example.com", "WrongPassword");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid email or password", result.Message);
        Assert.False(service.IsAuthenticated);
        Assert.Null(service.CurrentUser);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenCalled_ReturnsFalse()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, "{}");
        var service = new ApiAuthenticationService(httpClient);

        // Act
        var result = await service.ChangePasswordAsync("old-password", "new-password");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasRole_WhenNoRolesExist_ReturnsFalse()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, "{}");
        var service = new ApiAuthenticationService(httpClient);

        // Act
        var result = service.HasRole("Admin");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasAnyRole_WhenNoRolesExist_ReturnsFalse()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, "{}");
        var service = new ApiAuthenticationService(httpClient);

        // Act
        var result = service.HasAnyRole("Admin", "User");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasAllRoles_WhenNoRolesExist_ReturnsFalse()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, "{}");
        var service = new ApiAuthenticationService(httpClient);

        // Act
        var result = service.HasAllRoles("Admin", "User");

        // Assert
        Assert.False(result);
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