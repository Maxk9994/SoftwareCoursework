using Moq;
using StarterApp.Database.Models;
using StarterApp.Services;
using StarterApp.ViewModels;

namespace StarterApp.Tests.ViewModels;

public class CreateReviewViewModelTests
{
    [Fact]
    public void Constructor_WhenCreated_HasDefaultState()
    {
        // Arrange
        var mockReviewService = new Mock<IReviewService>();

        // Act
        var viewModel = new CreateReviewViewModel(mockReviewService.Object);

        // Assert
        Assert.Null(viewModel.Rental);
        Assert.Equal(5, viewModel.Rating);
        Assert.Equal("", viewModel.Comment);
        Assert.Equal("", viewModel.ErrorMessage);
        Assert.False(viewModel.IsBusy);
    }

    [Fact]
    public void ApplyQueryAttributes_WhenRentalProvided_SetsRental()
    {
        // Arrange
        var mockReviewService = new Mock<IReviewService>();
        var viewModel = new CreateReviewViewModel(mockReviewService.Object);

        var rental = new Rental
        {
            Id = 1,
            ItemId = 10,
            Status = "Completed",
            StartDate = new DateTime(2026, 5, 1),
            EndDate = new DateTime(2026, 5, 3)
        };

        var query = new Dictionary<string, object>
        {
            ["Rental"] = rental
        };

        // Act
        viewModel.ApplyQueryAttributes(query);

        // Assert
        Assert.NotNull(viewModel.Rental);
        Assert.Equal(1, viewModel.Rental.Id);
        Assert.Equal("Completed", viewModel.Rental.Status);
    }

    [Fact]
    public void ApplyQueryAttributes_WhenRentalNotProvided_DoesNotSetRental()
    {
        // Arrange
        var mockReviewService = new Mock<IReviewService>();
        var viewModel = new CreateReviewViewModel(mockReviewService.Object);

        var query = new Dictionary<string, object>();

        // Act
        viewModel.ApplyQueryAttributes(query);

        // Assert
        Assert.Null(viewModel.Rental);
    }

    [Fact]
    public async Task SubmitReviewCommand_WhenRentalIsNull_DoesNotCallService()
    {
        // Arrange
        var mockReviewService = new Mock<IReviewService>();
        var viewModel = new CreateReviewViewModel(mockReviewService.Object)
        {
            Rental = null,
            Rating = 5,
            Comment = "Great rental"
        };

        // Act
        await viewModel.SubmitReviewCommand.ExecuteAsync(null);

        // Assert
        mockReviewService.Verify(
            service => service.CreateReviewAsync(
                It.IsAny<CreateReviewRequest>(),
                It.IsAny<string>()),
            Times.Never);

        Assert.False(viewModel.IsBusy);
    }

    [Fact]
    public async Task SubmitReviewCommand_WhenViewModelIsBusy_DoesNotCallService()
    {
        // Arrange
        var mockReviewService = new Mock<IReviewService>();
        var viewModel = new CreateReviewViewModel(mockReviewService.Object)
        {
            Rental = new Rental
            {
                Id = 1,
                ItemId = 10,
                Status = "Completed"
            },
            IsBusy = true,
            Rating = 5,
            Comment = "Great rental"
        };

        // Act
        await viewModel.SubmitReviewCommand.ExecuteAsync(null);

        // Assert
        mockReviewService.Verify(
            service => service.CreateReviewAsync(
                It.IsAny<CreateReviewRequest>(),
                It.IsAny<string>()),
            Times.Never);

        Assert.True(viewModel.IsBusy);
    }

    [Fact]
    public async Task SubmitReviewCommand_WhenNoTokenIsAvailable_SetsErrorMessageAndDoesNotCallService()
    {
        // Arrange
        var mockReviewService = new Mock<IReviewService>();
        var viewModel = new CreateReviewViewModel(mockReviewService.Object)
        {
            Rental = new Rental
            {
                Id = 1,
                ItemId = 10,
                Status = "Completed"
            },
            Rating = 5,
            Comment = "Great rental"
        };

        // Act
        await viewModel.SubmitReviewCommand.ExecuteAsync(null);

        // Assert
        Assert.False(viewModel.IsBusy);
        Assert.NotEqual("", viewModel.ErrorMessage);

        mockReviewService.Verify(
            service => service.CreateReviewAsync(
                It.IsAny<CreateReviewRequest>(),
                It.IsAny<string>()),
            Times.Never);
    }
}

