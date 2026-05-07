using Moq;
using StarterApp.Database.Models;
using StarterApp.Services;
using StarterApp.ViewModels;

namespace StarterApp.Tests.ViewModels;

public class ReviewsViewModelTests
{
    [Fact]
    public void Constructor_WhenCreated_HasDefaultState()
    {
        // Arrange
        var mockReviewService = new Mock<IReviewService>();

        // Act
        var viewModel = new ReviewsViewModel(mockReviewService.Object);

        // Assert
        Assert.Empty(viewModel.Reviews);
        Assert.Null(viewModel.Item);
        Assert.Equal("", viewModel.ErrorMessage);
        Assert.False(viewModel.IsBusy);
    }

    [Fact]
    public async Task LoadReviewsAsync_WhenItemIsNull_DoesNotCallService()
    {
        // Arrange
        var mockReviewService = new Mock<IReviewService>();
        var viewModel = new ReviewsViewModel(mockReviewService.Object);

        // Act
        await viewModel.LoadReviewsAsync();

        // Assert
        mockReviewService.Verify(
            service => service.GetReviewsForItemAsync(It.IsAny<int>()),
            Times.Never);

        Assert.Empty(viewModel.Reviews);
    }

    [Fact]
    public async Task LoadReviewsAsync_WhenIsBusy_DoesNotCallService()
    {
        // Arrange
        var mockReviewService = new Mock<IReviewService>();

        var viewModel = new ReviewsViewModel(mockReviewService.Object)
        {
            Item = new Item
            {
                Id = 1,
                Title = "Power Drill"
            },
            IsBusy = true
        };

        // Act
        await viewModel.LoadReviewsAsync();

        // Assert
        mockReviewService.Verify(
            service => service.GetReviewsForItemAsync(It.IsAny<int>()),
            Times.Never);

        Assert.True(viewModel.IsBusy);
        Assert.Empty(viewModel.Reviews);
    }

    [Fact]
    public async Task LoadReviewsAsync_WhenServiceReturnsReviews_PopulatesReviewsCollection()
    {
        // Arrange
        var mockReviewService = new Mock<IReviewService>();

        var reviews = new List<Review>
        {
            new Review
            {
                Id = 1,
                Rating = 5,
                Comment = "Great item"
            },
            new Review
            {
                Id = 2,
                Rating = 4,
                Comment = "Worked well"
            }
        };

        mockReviewService
            .Setup(service => service.GetReviewsForItemAsync(1))
            .ReturnsAsync(reviews);

        var viewModel = new ReviewsViewModel(mockReviewService.Object)
        {
            Item = new Item
            {
                Id = 1,
                Title = "Power Drill"
            }
        };

        // Act
        await viewModel.LoadReviewsAsync();

        // Assert
        Assert.False(viewModel.IsBusy);
        Assert.Equal("", viewModel.ErrorMessage);
        Assert.Equal(2, viewModel.Reviews.Count);
        Assert.Equal("Great item", viewModel.Reviews[0].Comment);
        Assert.Equal(5, viewModel.Reviews[0].Rating);

        mockReviewService.Verify(
            service => service.GetReviewsForItemAsync(1),
            Times.Once);
    }

    [Fact]
    public async Task LoadReviewsAsync_WhenServiceThrowsException_SetsErrorMessage()
    {
        // Arrange
        var mockReviewService = new Mock<IReviewService>();

        mockReviewService
            .Setup(service => service.GetReviewsForItemAsync(1))
            .ThrowsAsync(new Exception("Failed to load reviews"));

        var viewModel = new ReviewsViewModel(mockReviewService.Object)
        {
            Item = new Item
            {
                Id = 1,
                Title = "Power Drill"
            }
        };

        // Act
        await viewModel.LoadReviewsAsync();

        // Assert
        Assert.False(viewModel.IsBusy);
        Assert.Equal("Failed to load reviews", viewModel.ErrorMessage);
        Assert.Empty(viewModel.Reviews);
    }

    [Fact]
    public async Task LoadReviewsAsync_WhenCalled_ClearsExistingReviewsBeforeLoading()
    {
        // Arrange
        var mockReviewService = new Mock<IReviewService>();

        var newReviews = new List<Review>
        {
            new Review
            {
                Id = 2,
                Rating = 4,
                Comment = "New review"
            }
        };

        mockReviewService
            .Setup(service => service.GetReviewsForItemAsync(1))
            .ReturnsAsync(newReviews);

        var viewModel = new ReviewsViewModel(mockReviewService.Object)
        {
            Item = new Item
            {
                Id = 1,
                Title = "Power Drill"
            }
        };

        viewModel.Reviews.Add(new Review
        {
            Id = 1,
            Rating = 2,
            Comment = "Old review"
        });

        // Act
        await viewModel.LoadReviewsAsync();

        // Assert
        Assert.Single(viewModel.Reviews);
        Assert.Equal("New review", viewModel.Reviews[0].Comment);
    }

    [Fact]
    public async Task ApplyQueryAttributes_WhenItemProvided_SetsItemAndLoadsReviews()
    {
        // Arrange
        var mockReviewService = new Mock<IReviewService>();

        var reviews = new List<Review>
        {
            new Review
            {
                Id = 1,
                Rating = 5,
                Comment = "Great item"
            }
        };

        mockReviewService
            .Setup(service => service.GetReviewsForItemAsync(1))
            .ReturnsAsync(reviews);

        var viewModel = new ReviewsViewModel(mockReviewService.Object);

        var item = new Item
        {
            Id = 1,
            Title = "Power Drill"
        };

        var query = new Dictionary<string, object>
        {
            ["Item"] = item
        };

        // Act
        viewModel.ApplyQueryAttributes(query);

        await Task.Delay(100);

        // Assert
        Assert.NotNull(viewModel.Item);
        Assert.Equal("Power Drill", viewModel.Item.Title);
        Assert.Single(viewModel.Reviews);
        Assert.Equal("Great item", viewModel.Reviews[0].Comment);
    }

    [Fact]
    public void ApplyQueryAttributes_WhenNoItemProvided_DoesNotLoadReviews()
    {
        // Arrange
        var mockReviewService = new Mock<IReviewService>();
        var viewModel = new ReviewsViewModel(mockReviewService.Object);

        var query = new Dictionary<string, object>();

        // Act
        viewModel.ApplyQueryAttributes(query);

        // Assert
        Assert.Null(viewModel.Item);
        Assert.Empty(viewModel.Reviews);

        mockReviewService.Verify(
            service => service.GetReviewsForItemAsync(It.IsAny<int>()),
            Times.Never);
    }
}