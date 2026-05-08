using StarterApp.Database.Models;
using StarterApp.ViewModels;

namespace StarterApp.Tests.ViewModels;

public class ItemDetailViewModelTests
{
    [Fact]
    public void Constructor_WhenCreated_HasDefaultState()
    {
        // Arrange and Act
        var viewModel = new ItemDetailViewModel();

        // Assert
        Assert.Null(viewModel.Item);
        Assert.False(viewModel.IsOwner);
        Assert.False(viewModel.CanRequestRental);
    }

    [Fact]
    public void CanRequestRental_WhenItemIsNull_ReturnsFalse()
    {
        // Arrange
        var viewModel = new ItemDetailViewModel
        {
            Item = null,
            IsOwner = false
        };

        // Act
        var result = viewModel.CanRequestRental;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanRequestRental_WhenUserIsOwner_ReturnsFalse()
    {
        // Arrange
        var viewModel = new ItemDetailViewModel
        {
            Item = new Item
            {
                Id = 1,
                Title = "Power Drill",
                OwnerId = 1
            },
            IsOwner = true
        };

        // Act
        var result = viewModel.CanRequestRental;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanRequestRental_WhenUserIsNotOwnerAndItemExists_ReturnsTrue()
    {
        // Arrange
        var viewModel = new ItemDetailViewModel
        {
            Item = new Item
            {
                Id = 1,
                Title = "Power Drill",
                OwnerId = 1
            },
            IsOwner = false
        };

        // Act
        var result = viewModel.CanRequestRental;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ApplyQueryAttributes_WhenItemNotProvided_DoesNotSetItem()
    {
        // Arrange
        var viewModel = new ItemDetailViewModel();

        var query = new Dictionary<string, object>();

        // Act
        viewModel.ApplyQueryAttributes(query);

        // Assert
        Assert.Null(viewModel.Item);
        Assert.False(viewModel.IsOwner);
        Assert.False(viewModel.CanRequestRental);
    }

    [Fact]
    public async Task EditItemCommand_WhenItemIsNull_DoesNothing()
    {
        // Arrange
        var viewModel = new ItemDetailViewModel
        {
            Item = null
        };

        // Act
        await viewModel.EditItemCommand.ExecuteAsync(null);

        // Assert
        Assert.Null(viewModel.Item);
    }

    [Fact]
    public async Task RequestRentalCommand_WhenItemIsNull_DoesNothing()
    {
        // Arrange
        var viewModel = new ItemDetailViewModel
        {
            Item = null
        };

        // Act
        await viewModel.RequestRentalCommand.ExecuteAsync(null);

        // Assert
        Assert.Null(viewModel.Item);
    }

    [Fact]
    public async Task RequestRentalCommand_WhenUserIsOwner_DoesNothing()
    {
        // Arrange
        var viewModel = new ItemDetailViewModel
        {
            Item = new Item
            {
                Id = 1,
                Title = "Power Drill",
                OwnerId = 1
            },
            IsOwner = true
        };

        // Act
        await viewModel.RequestRentalCommand.ExecuteAsync(null);

        // Assert
        Assert.False(viewModel.CanRequestRental);
    }

    [Fact]
    public async Task ViewReviewsCommand_WhenItemIsNull_DoesNothing()
    {
        // Arrange
        var viewModel = new ItemDetailViewModel
        {
            Item = null
        };

        // Act
        await viewModel.ViewReviewsCommand.ExecuteAsync(null);

        // Assert
        Assert.Null(viewModel.Item);
    }
}