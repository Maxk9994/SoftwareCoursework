using Moq;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.ViewModels;

namespace StarterApp.Tests.ViewModels;

public class EditItemViewModelTests
{
    [Fact]
    public void Constructor_WhenCreated_HasDefaultState()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();

        // Act
        var viewModel = new EditItemViewModel(mockItemRepository.Object);

        // Assert
        Assert.Equal("", viewModel.Title);
        Assert.Equal("", viewModel.Description);
        Assert.Equal("", viewModel.DailyRate);
        Assert.False(viewModel.IsAvailable);
        Assert.Equal("", viewModel.ErrorMessage);
        Assert.False(viewModel.IsBusy);
        Assert.Empty(viewModel.Categories);
        Assert.Null(viewModel.SelectedCategory);
    }

    [Fact]
    public async Task ApplyQueryAttributes_WhenItemProvided_SetsItemPropertiesAndLoadsCategories()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();

        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Tools" },
            new Category { Id = 2, Name = "Camping" }
        };

        mockItemRepository
            .Setup(repo => repo.GetCategoriesAsync())
            .ReturnsAsync(categories);

        var viewModel = new EditItemViewModel(mockItemRepository.Object);

        var item = new Item
        {
            Id = 10,
            Title = "Power Drill",
            Description = "A useful drill",
            DailyRate = 12.50m,
            CategoryId = 1,
            IsAvailable = true
        };

        var query = new Dictionary<string, object>
        {
            ["Item"] = item
        };

        // Act
        viewModel.ApplyQueryAttributes(query);

        await Task.Delay(100);

        // Assert
        Assert.Equal("Power Drill", viewModel.Title);
        Assert.Equal("A useful drill", viewModel.Description);
        Assert.Equal("12.50", viewModel.DailyRate);
        Assert.True(viewModel.IsAvailable);
        Assert.Equal(2, viewModel.Categories.Count);
        Assert.NotNull(viewModel.SelectedCategory);
        Assert.Equal("Tools", viewModel.SelectedCategory.Name);
    }

    [Fact]
    public void ApplyQueryAttributes_WhenItemNotProvided_DoesNotSetProperties()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();
        var viewModel = new EditItemViewModel(mockItemRepository.Object);

        var query = new Dictionary<string, object>();

        // Act
        viewModel.ApplyQueryAttributes(query);

        // Assert
        Assert.Equal("", viewModel.Title);
        Assert.Equal("", viewModel.Description);
        Assert.Equal("", viewModel.DailyRate);
        Assert.Empty(viewModel.Categories);

        mockItemRepository.Verify(
            repo => repo.GetCategoriesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task ApplyQueryAttributes_WhenCategoryLoadFails_SetsErrorMessage()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();

        mockItemRepository
            .Setup(repo => repo.GetCategoriesAsync())
            .ThrowsAsync(new Exception("API unavailable"));

        var viewModel = new EditItemViewModel(mockItemRepository.Object);

        var item = new Item
        {
            Id = 10,
            Title = "Power Drill",
            Description = "A useful drill",
            DailyRate = 12.50m,
            CategoryId = 1,
            IsAvailable = true
        };

        var query = new Dictionary<string, object>
        {
            ["Item"] = item
        };

        // Act
        viewModel.ApplyQueryAttributes(query);

        await Task.Delay(100);

        // Assert
        Assert.Equal("Failed to load categories: API unavailable", viewModel.ErrorMessage);
        Assert.Empty(viewModel.Categories);
        Assert.Null(viewModel.SelectedCategory);
    }

    [Fact]
    public async Task UpdateItemCommand_WhenViewModelIsBusy_DoesNotCallRepository()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();

        var viewModel = new EditItemViewModel(mockItemRepository.Object)
        {
            IsBusy = true
        };

        // Act
        await viewModel.UpdateItemCommand.ExecuteAsync(null);

        // Assert
        Assert.True(viewModel.IsBusy);

        mockItemRepository.Verify(
            repo => repo.UpdateItemAsync(
                It.IsAny<int>(),
                It.IsAny<UpdateItemRequest>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateItemCommand_WhenDailyRateIsNotNumber_SetsErrorMessage()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();

        var viewModel = new EditItemViewModel(mockItemRepository.Object)
        {
            Title = "Power Drill",
            Description = "A useful drill",
            DailyRate = "not-a-number",
            SelectedCategory = new Category
            {
                Id = 1,
                Name = "Tools"
            },
            IsAvailable = true
        };

        // Act
        await viewModel.UpdateItemCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Daily rate must be a number.", viewModel.ErrorMessage);
        Assert.False(viewModel.IsBusy);

        mockItemRepository.Verify(
            repo => repo.UpdateItemAsync(
                It.IsAny<int>(),
                It.IsAny<UpdateItemRequest>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateItemCommand_WhenCategoryIsNotSelected_SetsErrorMessage()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();

        var viewModel = new EditItemViewModel(mockItemRepository.Object)
        {
            Title = "Power Drill",
            Description = "A useful drill",
            DailyRate = "12.50",
            SelectedCategory = null,
            IsAvailable = true
        };

        // Act
        await viewModel.UpdateItemCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Please select a category.", viewModel.ErrorMessage);
        Assert.False(viewModel.IsBusy);

        mockItemRepository.Verify(
            repo => repo.UpdateItemAsync(
                It.IsAny<int>(),
                It.IsAny<UpdateItemRequest>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateItemCommand_WhenSecureStorageUnavailable_SetsErrorMessageAndDoesNotCallRepository()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();

        var viewModel = new EditItemViewModel(mockItemRepository.Object)
        {
            Title = "Power Drill",
            Description = "A useful drill",
            DailyRate = "12.50",
            SelectedCategory = new Category
            {
                Id = 1,
                Name = "Tools"
            },
            IsAvailable = true
        };

        // Act
        await viewModel.UpdateItemCommand.ExecuteAsync(null);

        // Assert
        Assert.False(viewModel.IsBusy);
        Assert.NotEqual("", viewModel.ErrorMessage);

        mockItemRepository.Verify(
            repo => repo.UpdateItemAsync(
                It.IsAny<int>(),
                It.IsAny<UpdateItemRequest>(),
                It.IsAny<string>()),
            Times.Never);
    }
}