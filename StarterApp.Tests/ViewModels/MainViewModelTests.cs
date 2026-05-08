using Moq;
using StarterApp.Database.Models;
using StarterApp.Services;
using StarterApp.ViewModels;

namespace StarterApp.Tests.ViewModels;

public class MainViewModelTests
{
    [Fact]
    public void Constructor_WhenUserIsAuthenticated_LoadsUserData()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        var user = new User
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com"
        };

        mockAuthService
            .Setup(service => service.CurrentUser)
            .Returns(user);

        mockAuthService
            .Setup(service => service.HasRole("Admin"))
            .Returns(false);

        // Act
        var viewModel = new MainViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        );

        // Assert
        Assert.Equal("Dashboard", viewModel.Title);
        Assert.NotNull(viewModel.CurrentUser);
        Assert.Equal("test@example.com", viewModel.CurrentUser.Email);
        Assert.Equal("Welcome, Test User!", viewModel.WelcomeMessage);
        Assert.False(viewModel.IsAdmin);
    }

    [Fact]
    public void Constructor_WhenUserIsAdmin_SetsIsAdminTrue()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        var user = new User
        {
            Id = 1,
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@example.com"
        };

        mockAuthService
            .Setup(service => service.CurrentUser)
            .Returns(user);

        mockAuthService
            .Setup(service => service.HasRole("Admin"))
            .Returns(true);

        // Act
        var viewModel = new MainViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        );

        // Assert
        Assert.True(viewModel.IsAdmin);
        Assert.Equal("Welcome, Admin User!", viewModel.WelcomeMessage);
    }

    [Fact]
    public void Constructor_WhenNoCurrentUser_DoesNotSetWelcomeMessage()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        mockAuthService
            .Setup(service => service.CurrentUser)
            .Returns((User?)null);

        mockAuthService
            .Setup(service => service.HasRole("Admin"))
            .Returns(false);

        // Act
        var viewModel = new MainViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        );

        // Assert
        Assert.Null(viewModel.CurrentUser);
        Assert.Equal("", viewModel.WelcomeMessage);
        Assert.False(viewModel.IsAdmin);
    }

    [Fact]
    public async Task NavigateToProfileCommand_WhenCalled_NavigatesToTempPage()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        mockNavigationService
            .Setup(service => service.NavigateToAsync("TempPage"))
            .Returns(Task.CompletedTask);

        var viewModel = new MainViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        );

        // Act
        await viewModel.NavigateToProfileCommand.ExecuteAsync(null);

        // Assert
        mockNavigationService.Verify(
            service => service.NavigateToAsync("TempPage"),
            Times.Once);
    }

    [Fact]
    public async Task NavigateToSettingsCommand_WhenCalled_NavigatesToTempPage()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        mockNavigationService
            .Setup(service => service.NavigateToAsync("TempPage"))
            .Returns(Task.CompletedTask);

        var viewModel = new MainViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        );

        // Act
        await viewModel.NavigateToSettingsCommand.ExecuteAsync(null);

        // Assert
        mockNavigationService.Verify(
            service => service.NavigateToAsync("TempPage"),
            Times.Once);
    }

    [Fact]
    public async Task NavigateToItemsCommand_WhenCalled_NavigatesToItemListPage()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        mockNavigationService
            .Setup(service => service.NavigateToAsync("ItemListPage"))
            .Returns(Task.CompletedTask);

        var viewModel = new MainViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        );

        // Act
        await viewModel.NavigateToItemsCommand.ExecuteAsync(null);

        // Assert
        mockNavigationService.Verify(
            service => service.NavigateToAsync("ItemListPage"),
            Times.Once);
    }

    [Fact]
    public async Task NavigateToUserListCommand_WhenUserIsAdmin_NavigatesToUserListPage()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        mockAuthService
            .Setup(service => service.HasRole("Admin"))
            .Returns(true);

        mockNavigationService
            .Setup(service => service.NavigateToAsync("UserListPage"))
            .Returns(Task.CompletedTask);

        var viewModel = new MainViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        );

        // Act
        await viewModel.NavigateToUserListCommand.ExecuteAsync(null);

        // Assert
        mockNavigationService.Verify(
            service => service.NavigateToAsync("UserListPage"),
            Times.Once);
    }

    [Fact]
    public async Task RefreshDataCommand_WhenCalled_ReloadsUserData()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthenticationService>();
        var mockNavigationService = new Mock<INavigationService>();

        var user = new User
        {
            Id = 1,
            FirstName = "Refresh",
            LastName = "User",
            Email = "refresh@example.com"
        };

        mockAuthService
            .Setup(service => service.CurrentUser)
            .Returns(user);

        mockAuthService
            .Setup(service => service.HasRole("Admin"))
            .Returns(false);

        var viewModel = new MainViewModel(
            mockAuthService.Object,
            mockNavigationService.Object
        );

        // Act
        await viewModel.RefreshDataCommand.ExecuteAsync(null);

        // Assert
        Assert.False(viewModel.IsBusy);
        Assert.Equal("Welcome, Refresh User!", viewModel.WelcomeMessage);
        Assert.False(viewModel.IsAdmin);
    }
}