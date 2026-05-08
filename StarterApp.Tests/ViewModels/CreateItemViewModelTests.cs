using Moq;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;
using StarterApp.ViewModels;

namespace StarterApp.Tests.ViewModels;

public class CreateItemViewModelTests
{
    [Fact]
    public void Constructor_WhenCreated_HasDefaultState()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();
        var mockAuthService = new Mock<IAuthenticationService>();

        // Act
        var viewModel = new CreateItemViewModel(
            mockItemRepository.Object,
            mockAuthService.Object
        );

        // Assert
        Assert.Equal("", viewModel.Title);
        Assert.Equal("", viewModel.Description);
        Assert.Equal("", viewModel.DailyRate);
        Assert.Equal("", viewModel.ErrorMessage);
        Assert.False(viewModel.IsBusy);
        Assert.Empty(viewModel.Categories);
        Assert.Null(viewModel.SelectedCategory);
        Assert.Equal("", viewModel.Latitude);
        Assert.Equal("", viewModel.Longitude);
    }

    [Fact]
    public async Task LoadCategoriesAsync_WhenRepositoryReturnsCategories_PopulatesCategories()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();
        var mockAuthService = new Mock<IAuthenticationService>();

        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Tools" },
            new Category { Id = 2, Name = "Camping" }
        };

        mockItemRepository
            .Setup(repo => repo.GetCategoriesAsync())
            .ReturnsAsync(categories);

        var viewModel = new CreateItemViewModel(
            mockItemRepository.Object,
            mockAuthService.Object
        );

        // Act
        await viewModel.LoadCategoriesAsync();

        // Assert
        Assert.Equal(2, viewModel.Categories.Count);
        Assert.Equal("Tools", viewModel.Categories[0].Name);
        Assert.Equal("Camping", viewModel.Categories[1].Name);
        Assert.Equal("", viewModel.ErrorMessage);

        mockItemRepository.Verify(
            repo => repo.GetCategoriesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task LoadCategoriesAsync_WhenRepositoryThrowsException_SetsErrorMessage()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();
        var mockAuthService = new Mock<IAuthenticationService>();

        mockItemRepository
            .Setup(repo => repo.GetCategoriesAsync())
            .ThrowsAsync(new Exception("API unavailable"));

        var viewModel = new CreateItemViewModel(
            mockItemRepository.Object,
            mockAuthService.Object
        );

        // Act
        await viewModel.LoadCategoriesAsync();

        // Assert
        Assert.Empty(viewModel.Categories);
        Assert.Equal("Failed to load categories: API unavailable", viewModel.ErrorMessage);
    }

    [Fact]
    public async Task LoadCategoriesAsync_WhenCalled_ClearsExistingCategoriesBeforeLoading()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();
        var mockAuthService = new Mock<IAuthenticationService>();

        var categories = new List<Category>
        {
            new Category { Id = 2, Name = "Camping" }
        };

        mockItemRepository
            .Setup(repo => repo.GetCategoriesAsync())
            .ReturnsAsync(categories);

        var viewModel = new CreateItemViewModel(
            mockItemRepository.Object,
            mockAuthService.Object
        );

        viewModel.Categories.Add(new Category
        {
            Id = 1,
            Name = "Old Category"
        });

        // Act
        await viewModel.LoadCategoriesAsync();

        // Assert
        Assert.Single(viewModel.Categories);
        Assert.Equal("Camping", viewModel.Categories[0].Name);
    }

    [Fact]
    public async Task CreateItemCommand_WhenViewModelIsBusy_DoesNotCallRepository()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();
        var mockAuthService = new Mock<IAuthenticationService>();

        var viewModel = new CreateItemViewModel(
            mockItemRepository.Object,
            mockAuthService.Object
        )
        {
            IsBusy = true
        };

        // Act
        await viewModel.CreateItemCommand.ExecuteAsync(null);

        // Assert
        mockItemRepository.Verify(
            repo => repo.CreateItemAsync(
                It.IsAny<CreateItemRequest>(),
                It.IsAny<string>()),
            Times.Never);

        Assert.True(viewModel.IsBusy);
    }

    [Fact]
    public async Task CreateItemCommand_WhenDailyRateIsNotNumber_SetsErrorMessage()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();
        var mockAuthService = new Mock<IAuthenticationService>();

        var viewModel = new CreateItemViewModel(
            mockItemRepository.Object,
            mockAuthService.Object
        )
        {
            Title = "Power Drill",
            Description = "A useful drill",
            DailyRate = "not-a-number"
        };

        // Act
        await viewModel.CreateItemCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Daily rate must be a number.", viewModel.ErrorMessage);
        Assert.False(viewModel.IsBusy);

        mockItemRepository.Verify(
            repo => repo.CreateItemAsync(
                It.IsAny<CreateItemRequest>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateItemCommand_WhenCategoryIsNotSelected_SetsErrorMessage()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();
        var mockAuthService = new Mock<IAuthenticationService>();

        var viewModel = new CreateItemViewModel(
            mockItemRepository.Object,
            mockAuthService.Object
        )
        {
            Title = "Power Drill",
            Description = "A useful drill",
            DailyRate = "10"
        };

        // Act
        await viewModel.CreateItemCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Please select a category.", viewModel.ErrorMessage);
        Assert.False(viewModel.IsBusy);

        mockItemRepository.Verify(
            repo => repo.CreateItemAsync(
                It.IsAny<CreateItemRequest>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateItemCommand_WhenLatitudeIsNotNumber_SetsErrorMessage()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();
        var mockAuthService = new Mock<IAuthenticationService>();

        var viewModel = new CreateItemViewModel(
            mockItemRepository.Object,
            mockAuthService.Object
        )
        {
            Title = "Power Drill",
            Description = "A useful drill",
            DailyRate = "10",
            SelectedCategory = new Category { Id = 1, Name = "Tools" },
            Latitude = "invalid",
            Longitude = "-3.1883"
        };

        // Act
        await viewModel.CreateItemCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Latitude must be a number.", viewModel.ErrorMessage);
        Assert.False(viewModel.IsBusy);

        mockItemRepository.Verify(
            repo => repo.CreateItemAsync(
                It.IsAny<CreateItemRequest>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateItemCommand_WhenLongitudeIsNotNumber_SetsErrorMessage()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();
        var mockAuthService = new Mock<IAuthenticationService>();

        var viewModel = new CreateItemViewModel(
            mockItemRepository.Object,
            mockAuthService.Object
        )
        {
            Title = "Power Drill",
            Description = "A useful drill",
            DailyRate = "10",
            SelectedCategory = new Category { Id = 1, Name = "Tools" },
            Latitude = "55.9533",
            Longitude = "invalid"
        };

        // Act
        await viewModel.CreateItemCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Longitude must be a number.", viewModel.ErrorMessage);
        Assert.False(viewModel.IsBusy);

        mockItemRepository.Verify(
            repo => repo.CreateItemAsync(
                It.IsAny<CreateItemRequest>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateItemCommand_WhenSecureStorageUnavailable_SetsErrorMessageAndDoesNotCallRepository()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();
        var mockAuthService = new Mock<IAuthenticationService>();

        var viewModel = new CreateItemViewModel(
            mockItemRepository.Object,
            mockAuthService.Object
        )
        {
            Title = "Power Drill",
            Description = "A useful drill",
            DailyRate = "10",
            SelectedCategory = new Category { Id = 1, Name = "Tools" },
            Latitude = "55.9533",
            Longitude = "-3.1883"
        };

        // Act
        await viewModel.CreateItemCommand.ExecuteAsync(null);

        // Assert
        Assert.False(viewModel.IsBusy);
        Assert.NotEqual("", viewModel.ErrorMessage);

        mockItemRepository.Verify(
            repo => repo.CreateItemAsync(
                It.IsAny<CreateItemRequest>(),
                It.IsAny<string>()),
            Times.Never);
    }
}