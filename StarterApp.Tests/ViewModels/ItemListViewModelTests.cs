using Moq;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.ViewModels;

namespace StarterApp.Tests.ViewModels;

public class ItemListViewModelTests
{
    [Fact]
    public void Constructor_WhenCreated_HasDefaultState()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();

        // Act
        var viewModel = new ItemListViewModel(mockItemRepository.Object);

        // Assert
        Assert.Empty(viewModel.Items);
        Assert.False(viewModel.IsBusy);
        Assert.Equal("", viewModel.ErrorMessage);
    }

    [Fact]
    public async Task LoadItemsCommand_WhenRepositoryReturnsItems_PopulatesItemsCollection()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();

        var items = new List<Item>
        {
            new Item
            {
                Id = 1,
                Title = "Power Drill",
                Description = "A useful drill",
                DailyRate = 10m
            },
            new Item
            {
                Id = 2,
                Title = "Camping Tent",
                Description = "A two-person tent",
                DailyRate = 15m
            }
        };

        mockItemRepository
            .Setup(repo => repo.GetItemsAsync())
            .ReturnsAsync(items);

        var viewModel = new ItemListViewModel(mockItemRepository.Object);

        // Act
        await viewModel.LoadItemsCommand.ExecuteAsync(null);

        // Assert
        Assert.False(viewModel.IsBusy);
        Assert.Equal("", viewModel.ErrorMessage);
        Assert.Equal(2, viewModel.Items.Count);
        Assert.Equal("Power Drill", viewModel.Items[0].Title);
        Assert.Equal("Camping Tent", viewModel.Items[1].Title);

        mockItemRepository.Verify(
            repo => repo.GetItemsAsync(),
            Times.Once);
    }

    [Fact]
    public async Task LoadItemsCommand_WhenRepositoryThrowsException_SetsErrorMessage()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();

        mockItemRepository
            .Setup(repo => repo.GetItemsAsync())
            .ThrowsAsync(new Exception("API unavailable"));

        var viewModel = new ItemListViewModel(mockItemRepository.Object);

        // Act
        await viewModel.LoadItemsCommand.ExecuteAsync(null);

        // Assert
        Assert.False(viewModel.IsBusy);
        Assert.Equal("Failed to load items: API unavailable", viewModel.ErrorMessage);
        Assert.Empty(viewModel.Items);
    }

    [Fact]
    public async Task LoadItemsCommand_WhenIsBusy_DoesNotCallRepository()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();

        var viewModel = new ItemListViewModel(mockItemRepository.Object)
        {
            IsBusy = true
        };

        // Act
        await viewModel.LoadItemsCommand.ExecuteAsync(null);

        // Assert
        Assert.True(viewModel.IsBusy);

        mockItemRepository.Verify(
            repo => repo.GetItemsAsync(),
            Times.Never);
    }

    [Fact]
    public async Task LoadItemsCommand_WhenCalled_ReplacesExistingItems()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();

        var newItems = new List<Item>
        {
            new Item
            {
                Id = 2,
                Title = "New Tent",
                Description = "New item",
                DailyRate = 20m
            }
        };

        mockItemRepository
            .Setup(repo => repo.GetItemsAsync())
            .ReturnsAsync(newItems);

        var viewModel = new ItemListViewModel(mockItemRepository.Object);

        viewModel.Items.Add(new Item
        {
            Id = 1,
            Title = "Old Drill",
            Description = "Old item",
            DailyRate = 10m
        });

        // Act
        await viewModel.LoadItemsCommand.ExecuteAsync(null);

        // Assert
        Assert.Single(viewModel.Items);
        Assert.Equal("New Tent", viewModel.Items[0].Title);
    }

    [Fact]
    public async Task GoToItemDetailCommand_WhenItemIsNull_DoesNothing()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();
        var viewModel = new ItemListViewModel(mockItemRepository.Object);

        // Act
        await viewModel.GoToItemDetailCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("", viewModel.ErrorMessage);

        mockItemRepository.Verify(
            repo => repo.GetItemsAsync(),
            Times.Never);
    }
}