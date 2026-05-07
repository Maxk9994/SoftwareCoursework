using Moq;
using StarterApp.Database.Models;
using StarterApp.Services;
using StarterApp.ViewModels;

namespace StarterApp.Tests.ViewModels;

public class RentalsViewModelTests
{
    [Fact]
    public void Constructor_WhenCreated_HasDefaultState()
    {
        // Arrange
        var mockRentalService = new Mock<IRentalService>();

        // Act
        var viewModel = new RentalsViewModel(mockRentalService.Object);

        // Assert
        Assert.Empty(viewModel.IncomingRentals);
        Assert.Empty(viewModel.OutgoingRentals);
        Assert.Equal("", viewModel.ErrorMessage);
        Assert.False(viewModel.IsBusy);
    }

    [Fact]
    public async Task LoadRentalsAsync_WhenIsBusy_DoesNotCallService()
    {
        // Arrange
        var mockRentalService = new Mock<IRentalService>();
        var viewModel = new RentalsViewModel(mockRentalService.Object)
        {
            IsBusy = true
        };

        // Act
        await viewModel.LoadRentalsAsync();

        // Assert
        mockRentalService.Verify(
            service => service.GetIncomingRentalsAsync(It.IsAny<string>()),
            Times.Never);

        mockRentalService.Verify(
            service => service.GetOutgoingRentalsAsync(It.IsAny<string>()),
            Times.Never);

        Assert.True(viewModel.IsBusy);
    }

    [Fact]
    public async Task LoadRentalsAsync_WhenNoTokenIsAvailable_SetsErrorMessageAndDoesNotCallService()
    {
        // Arrange
        var mockRentalService = new Mock<IRentalService>();
        var viewModel = new RentalsViewModel(mockRentalService.Object);

        // Act
        await viewModel.LoadRentalsAsync();

        // Assert
        Assert.False(viewModel.IsBusy);
        Assert.NotEqual("", viewModel.ErrorMessage);

        mockRentalService.Verify(
            service => service.GetIncomingRentalsAsync(It.IsAny<string>()),
            Times.Never);

        mockRentalService.Verify(
            service => service.GetOutgoingRentalsAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task ApproveRentalCommand_WhenRentalIsNull_DoesNotCallService()
    {
        // Arrange
        var mockRentalService = new Mock<IRentalService>();
        var viewModel = new RentalsViewModel(mockRentalService.Object);

        // Act
        await viewModel.ApproveRentalCommand.ExecuteAsync(null);

        // Assert
        mockRentalService.Verify(
            service => service.ApproveRentalAsync(
                It.IsAny<Rental>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task ApproveRentalCommand_WhenViewModelIsBusy_DoesNotCallService()
    {
        // Arrange
        var mockRentalService = new Mock<IRentalService>();
        var viewModel = new RentalsViewModel(mockRentalService.Object)
        {
            IsBusy = true
        };

        var rental = new Rental
        {
            Id = 1,
            ItemId = 10,
            Status = "Pending",
            StartDate = new DateTime(2026, 5, 1),
            EndDate = new DateTime(2026, 5, 3)
        };

        // Act
        await viewModel.ApproveRentalCommand.ExecuteAsync(rental);

        // Assert
        mockRentalService.Verify(
            service => service.ApproveRentalAsync(
                It.IsAny<Rental>(),
                It.IsAny<string>()),
            Times.Never);

        Assert.True(viewModel.IsBusy);
    }

    [Fact]
    public async Task RejectRentalCommand_WhenRentalIsNull_DoesNotCallService()
    {
        // Arrange
        var mockRentalService = new Mock<IRentalService>();
        var viewModel = new RentalsViewModel(mockRentalService.Object);

        // Act
        await viewModel.RejectRentalCommand.ExecuteAsync(null);

        // Assert
        mockRentalService.Verify(
            service => service.RejectRentalAsync(
                It.IsAny<int>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task RejectRentalCommand_WhenViewModelIsBusy_DoesNotCallService()
    {
        // Arrange
        var mockRentalService = new Mock<IRentalService>();
        var viewModel = new RentalsViewModel(mockRentalService.Object)
        {
            IsBusy = true
        };

        var rental = new Rental
        {
            Id = 1,
            ItemId = 10,
            Status = "Pending",
            StartDate = new DateTime(2026, 5, 1),
            EndDate = new DateTime(2026, 5, 3)
        };

        // Act
        await viewModel.RejectRentalCommand.ExecuteAsync(rental);

        // Assert
        mockRentalService.Verify(
            service => service.RejectRentalAsync(
                It.IsAny<int>(),
                It.IsAny<string>()),
            Times.Never);

        Assert.True(viewModel.IsBusy);
    }
}