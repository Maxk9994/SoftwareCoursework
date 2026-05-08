using Moq;
using StarterApp.Services;
using StarterApp.ViewModels;

namespace StarterApp.Tests.ViewModels;

public class LoginViewModelTests
{
    [Fact]
    public void Constructor_WhenCreated_SetsDefaultState()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        // Act
        var viewModel = new LoginViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        );

        // Assert
        Assert.Equal("Login", viewModel.Title);
        Assert.Equal("", viewModel.Email);
        Assert.Equal("", viewModel.Password);
        Assert.False(viewModel.RememberMe);
        Assert.False(viewModel.IsBusy);
    }

    [Fact]
    public async Task LoginCommand_WhenBusy_DoesNotCallAuthService()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        var viewModel = new LoginViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        )
        {
            IsBusy = true,
            Email = "test@example.com",
            Password = "Password123"
        };

        // Act
        await viewModel.LoginCommand.ExecuteAsync(null);

        // Assert
        mockAuthService.Verify(
            service => service.LoginAsync(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginCommand_WhenEmailIsMissing_SetsErrorAndDoesNotCallAuthService()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        var viewModel = new LoginViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        )
        {
            Email = "",
            Password = "Password123"
        };

        // Act
        await viewModel.LoginCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Please enter both email and password", viewModel.ErrorMessage);

        mockAuthService.Verify(
            service => service.LoginAsync(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginCommand_WhenPasswordIsMissing_SetsErrorAndDoesNotCallAuthService()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        var viewModel = new LoginViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        )
        {
            Email = "test@example.com",
            Password = ""
        };

        // Act
        await viewModel.LoginCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Please enter both email and password", viewModel.ErrorMessage);

        mockAuthService.Verify(
            service => service.LoginAsync(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginCommand_WhenAuthServiceReturnsFailure_SetsErrorMessage()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        mockAuthService
            .Setup(service => service.LoginAsync("test@example.com", "WrongPassword"))
            .ReturnsAsync(new AuthenticationResult(false, "Invalid email or password"));

        var viewModel = new LoginViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        )
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        // Act
        await viewModel.LoginCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Invalid email or password", viewModel.ErrorMessage);
        Assert.False(viewModel.IsBusy);

        mockNavigationService.Verify(
            service => service.NavigateToAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginCommand_WhenAuthServiceThrowsException_SetsErrorMessage()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        mockAuthService
            .Setup(service => service.LoginAsync(
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ThrowsAsync(new Exception("API unavailable"));

        var viewModel = new LoginViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        )
        {
            Email = "test@example.com",
            Password = "Password123"
        };

        // Act
        await viewModel.LoginCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Login failed: API unavailable", viewModel.ErrorMessage);
        Assert.False(viewModel.IsBusy);
    }

    [Fact]
    public async Task LoginCommand_WhenAuthServiceReturnsSuccess_NavigatesToMainPage()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        mockAuthService
            .Setup(service => service.LoginAsync("test@example.com", "Password123"))
            .ReturnsAsync(new AuthenticationResult(true, "Login successful"));

        mockNavigationService
            .Setup(service => service.NavigateToAsync("MainPage"))
            .Returns(Task.CompletedTask);

        var viewModel = new LoginViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        )
        {
            Email = "test@example.com",
            Password = "Password123"
        };

        // Act
        await viewModel.LoginCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("", viewModel.ErrorMessage);
        Assert.False(viewModel.IsBusy);

        mockNavigationService.Verify(
            service => service.NavigateToAsync("MainPage"),
            Times.Once);
    }

    [Fact]
    public async Task NavigateToRegisterCommand_WhenCalled_NavigatesToRegisterPage()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        mockNavigationService
            .Setup(service => service.NavigateToAsync("RegisterPage"))
            .Returns(Task.CompletedTask);

        var viewModel = new LoginViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        );

        // Act
        await viewModel.NavigateToRegisterCommand.ExecuteAsync(null);

        // Assert
        mockNavigationService.Verify(
            service => service.NavigateToAsync("RegisterPage"),
            Times.Once);
    }
}
