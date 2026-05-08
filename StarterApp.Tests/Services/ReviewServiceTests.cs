using Moq;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.Tests.Services;

public class ReviewServiceTests
{
    [Fact]
    public async Task GetReviewsForItemAsync_WhenCalled_ReturnsReviewsFromRepository()
    {
        // Arrange
        var mockRepository = new Mock<IReviewRepository>();

        var expectedReviews = new List<Review>
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

        mockRepository
            .Setup(repo => repo.GetReviewsForItemAsync(10))
            .ReturnsAsync(expectedReviews);

        var service = new ReviewService(mockRepository.Object);

        // Act
        var reviews = await service.GetReviewsForItemAsync(10);

        // Assert
        Assert.NotNull(reviews);
        Assert.Equal(2, reviews.Count);
        Assert.Equal("Great item", reviews[0].Comment);

        mockRepository.Verify(
            repo => repo.GetReviewsForItemAsync(10),
            Times.Once);
    }

    [Fact]
    public async Task CreateReviewAsync_WhenValidRequest_ReturnsCreatedReview()
    {
        // Arrange
        var mockRepository = new Mock<IReviewRepository>();

        var request = new CreateReviewRequest
        {
            Rating = 5,
            Comment = "Excellent rental"
        };

        var createdReview = new Review
        {
            Id = 3,
            Rating = 5,
            Comment = "Excellent rental"
        };

        mockRepository
            .Setup(repo => repo.CreateReviewAsync(request, "test-token"))
            .ReturnsAsync(createdReview);

        var service = new ReviewService(mockRepository.Object);

        // Act
        var result = await service.CreateReviewAsync(request, "test-token");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Id);
        Assert.Equal(5, result.Rating);
        Assert.Equal("Excellent rental", result.Comment);

        mockRepository.Verify(
            repo => repo.CreateReviewAsync(request, "test-token"),
            Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public async Task CreateReviewAsync_WhenRatingIsOutsideValidRange_ThrowsArgumentException(int rating)
    {
        // Arrange
        var mockRepository = new Mock<IReviewRepository>();

        var request = new CreateReviewRequest
        {
            Rating = rating,
            Comment = "Valid comment"
        };

        var service = new ReviewService(mockRepository.Object);

        // Act and Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateReviewAsync(request, "test-token"));

        Assert.Equal("Rating must be between 1 and 5.", exception.Message);

        mockRepository.Verify(
            repo => repo.CreateReviewAsync(
                It.IsAny<CreateReviewRequest>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CreateReviewAsync_WhenCommentIsEmpty_ThrowsArgumentException(string? comment)
    {
        // Arrange
        var mockRepository = new Mock<IReviewRepository>();

        var request = new CreateReviewRequest
        {
            Rating = 5,
            Comment = comment
        };

        var service = new ReviewService(mockRepository.Object);

        // Act and Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateReviewAsync(request, "test-token"));

        Assert.Equal("Comment is required.", exception.Message);

        mockRepository.Verify(
            repo => repo.CreateReviewAsync(
                It.IsAny<CreateReviewRequest>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public async Task CreateReviewAsync_WhenRatingIsValid_CallsRepository(int rating)
    {
        // Arrange
        var mockRepository = new Mock<IReviewRepository>();

        var request = new CreateReviewRequest
        {
            Rating = rating,
            Comment = "Valid comment"
        };

        var createdReview = new Review
        {
            Id = 1,
            Rating = rating,
            Comment = "Valid comment"
        };

        mockRepository
            .Setup(repo => repo.CreateReviewAsync(request, "test-token"))
            .ReturnsAsync(createdReview);

        var service = new ReviewService(mockRepository.Object);

        // Act
        var result = await service.CreateReviewAsync(request, "test-token");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(rating, result.Rating);

        mockRepository.Verify(
            repo => repo.CreateReviewAsync(request, "test-token"),
            Times.Once);
    }
}