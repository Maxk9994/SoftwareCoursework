using Moq;
using StarterApp.Services;
using StarterApp.ViewModels;

namespace StarterApp.Tests.ViewModels;

public class RegisterViewModelTests
{
    [Fact]
    public void Constructor_WhenCreated_SetsDefaultState()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        // Act
        var viewModel = new RegisterViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        );

        // Assert
        Assert.Equal("Register", viewModel.Title);
        Assert.Equal("", viewModel.FirstName);
        Assert.Equal("", viewModel.LastName);
        Assert.Equal("", viewModel.Email);
        Assert.Equal("", viewModel.Password);
        Assert.Equal("", viewModel.ConfirmPassword);
        Assert.False(viewModel.AcceptTerms);
        Assert.False(viewModel.IsBusy);
    }

    [Fact]
    public async Task RegisterCommand_WhenBusy_DoesNotCallAuthService()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        var viewModel = new RegisterViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        )
        {
            IsBusy = true
        };

        // Act
        await viewModel.RegisterCommand.ExecuteAsync(null);

        // Assert
        mockAuthService.Verify(
            service => service.RegisterAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterCommand_WhenFirstNameMissing_SetsError()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        var viewModel = new RegisterViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        )
        {
            FirstName = "",
            LastName = "User",
            Email = "test@example.com",
            Password = "Password123",
            ConfirmPassword = "Password123",
            AcceptTerms = true
        };

        // Act
        await viewModel.RegisterCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("First name is required", viewModel.ErrorMessage);

        mockAuthService.Verify(
            service => service.RegisterAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterCommand_WhenLastNameMissing_SetsError()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        var viewModel = new RegisterViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        )
        {
            FirstName = "Test",
            LastName = "",
            Email = "test@example.com",
            Password = "Password123",
            ConfirmPassword = "Password123",
            AcceptTerms = true
        };

        // Act
        await viewModel.RegisterCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Last name is required", viewModel.ErrorMessage);

        mockAuthService.Verify(
            service => service.RegisterAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterCommand_WhenEmailMissing_SetsError()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        var viewModel = new RegisterViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        )
        {
            FirstName = "Test",
            LastName = "User",
            Email = "",
            Password = "Password123",
            ConfirmPassword = "Password123",
            AcceptTerms = true
        };

        // Act
        await viewModel.RegisterCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Email is required", viewModel.ErrorMessage);

        mockAuthService.Verify(
            service => service.RegisterAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterCommand_WhenEmailInvalid_SetsError()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        var viewModel = new RegisterViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        )
        {
            FirstName = "Test",
            LastName = "User",
            Email = "invalid-email",
            Password = "Password123",
            ConfirmPassword = "Password123",
            AcceptTerms = true
        };

        // Act
        await viewModel.RegisterCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Please enter a valid email address", viewModel.ErrorMessage);

        mockAuthService.Verify(
            service => service.RegisterAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterCommand_WhenPasswordMissing_SetsError()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        var viewModel = new RegisterViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        )
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Password = "",
            ConfirmPassword = "",
            AcceptTerms = true
        };

        // Act
        await viewModel.RegisterCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Password is required", viewModel.ErrorMessage);

        mockAuthService.Verify(
            service => service.RegisterAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterCommand_WhenPasswordTooShort_SetsError()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        var viewModel = new RegisterViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        )
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Password = "123",
            ConfirmPassword = "123",
            AcceptTerms = true
        };

        // Act
        await viewModel.RegisterCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Password must be at least 6 characters long", viewModel.ErrorMessage);

        mockAuthService.Verify(
            service => service.RegisterAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterCommand_WhenPasswordsDoNotMatch_SetsError()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        var viewModel = new RegisterViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        )
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Password = "Password123",
            ConfirmPassword = "Different123",
            AcceptTerms = true
        };

        // Act
        await viewModel.RegisterCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Passwords do not match", viewModel.ErrorMessage);

        mockAuthService.Verify(
            service => service.RegisterAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterCommand_WhenTermsNotAccepted_SetsError()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        var viewModel = new RegisterViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        )
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Password = "Password123",
            ConfirmPassword = "Password123",
            AcceptTerms = false
        };

        // Act
        await viewModel.RegisterCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Please accept the terms and conditions", viewModel.ErrorMessage);

        mockAuthService.Verify(
            service => service.RegisterAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterCommand_WhenAuthServiceReturnsFailure_SetsError()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        mockAuthService
            .Setup(service => service.RegisterAsync(
                "Test",
                "User",
                "test@example.com",
                "Password123"))
            .ReturnsAsync(new AuthenticationResult(false, "Email already exists"));

        var viewModel = new RegisterViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        )
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Password = "Password123",
            ConfirmPassword = "Password123",
            AcceptTerms = true
        };

        // Act
        await viewModel.RegisterCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Email already exists", viewModel.ErrorMessage);
        Assert.False(viewModel.IsBusy);

        mockNavigationService.Verify(
            service => service.NavigateBackAsync(),
            Times.Never);
    }

    [Fact]
    public async Task RegisterCommand_WhenAuthServiceThrowsException_SetsError()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        mockAuthService
            .Setup(service => service.RegisterAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ThrowsAsync(new Exception("API unavailable"));

        var viewModel = new RegisterViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        )
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Password = "Password123",
            ConfirmPassword = "Password123",
            AcceptTerms = true
        };

        // Act
        await viewModel.RegisterCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Registration failed: API unavailable", viewModel.ErrorMessage);
        Assert.False(viewModel.IsBusy);
    }

    [Fact]
    public async Task NavigateBackToLoginCommand_WhenCalled_CallsNavigationService()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        mockNavigationService
            .Setup(service => service.NavigateBackAsync())
            .Returns(Task.CompletedTask);

        var viewModel = new RegisterViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        );

        // Act
        await viewModel.NavigateBackToLoginCommand.ExecuteAsync(null);

        // Assert
        mockNavigationService.Verify(
            service => service.NavigateBackAsync(),
            Times.Once);
    }
}