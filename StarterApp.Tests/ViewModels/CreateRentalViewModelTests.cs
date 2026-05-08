using Moq;
using StarterApp.Database.Models;
using StarterApp.Services;
using StarterApp.ViewModels;

namespace StarterApp.Tests.ViewModels;

public class CreateRentalViewModelTests
{
    [Fact]
    public void Constructor_WhenCreated_HasDefaultState()
    {
        // Arrange
        var mockRentalService = new Mock<IRentalService>();

        // Act
        var viewModel = new CreateRentalViewModel(mockRentalService.Object);

        // Assert
        Assert.Null(viewModel.Item);
        Assert.Equal(DateTime.Today, viewModel.StartDate);
        Assert.Equal(DateTime.Today.AddDays(1), viewModel.EndDate);
        Assert.Equal("", viewModel.ErrorMessage);
        Assert.False(viewModel.IsBusy);
        Assert.Equal(0, viewModel.EstimatedTotal);
    }

    [Fact]
    public void ApplyQueryAttributes_WhenItemProvided_SetsItem()
    {
        // Arrange
        var mockRentalService = new Mock<IRentalService>();

        mockRentalService
            .Setup(service => service.CalculateTotalPrice(10m, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Returns(10m);

        var viewModel = new CreateRentalViewModel(mockRentalService.Object);

        var item = new Item
        {
            Id = 1,
            Title = "Power Drill",
            DailyRate = 10m
        };

        var query = new Dictionary<string, object>
        {
            ["Item"] = item
        };

        // Act
        viewModel.ApplyQueryAttributes(query);

        // Assert
        Assert.NotNull(viewModel.Item);
        Assert.Equal(1, viewModel.Item.Id);
        Assert.Equal("Power Drill", viewModel.Item.Title);
    }

    [Fact]
    public void ApplyQueryAttributes_WhenItemNotProvided_DoesNotSetItem()
    {
        // Arrange
        var mockRentalService = new Mock<IRentalService>();
        var viewModel = new CreateRentalViewModel(mockRentalService.Object);

        var query = new Dictionary<string, object>();

        // Act
        viewModel.ApplyQueryAttributes(query);

        // Assert
        Assert.Null(viewModel.Item);
    }

    [Fact]
    public void EstimatedTotal_WhenItemIsNull_ReturnsZero()
    {
        // Arrange
        var mockRentalService = new Mock<IRentalService>();
        var viewModel = new CreateRentalViewModel(mockRentalService.Object);

        // Act
        var result = viewModel.EstimatedTotal;

        // Assert
        Assert.Equal(0, result);

        mockRentalService.Verify(
            service => service.CalculateTotalPrice(
                It.IsAny<decimal>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()),
            Times.Never);
    }

    [Fact]
    public void EstimatedTotal_WhenItemExists_CallsRentalService()
    {
        // Arrange
        var mockRentalService = new Mock<IRentalService>();

        mockRentalService
            .Setup(service => service.CalculateTotalPrice(
                15m,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
            .Returns(45m);

        var viewModel = new CreateRentalViewModel(mockRentalService.Object)
        {
            Item = new Item
            {
                Id = 1,
                Title = "Camping Tent",
                DailyRate = 15m
            },
            StartDate = new DateTime(2026, 5, 1),
            EndDate = new DateTime(2026, 5, 4)
        };

        // Act
        var result = viewModel.EstimatedTotal;

        // Assert
        Assert.Equal(45m, result);

        mockRentalService.Verify(
            service => service.CalculateTotalPrice(
                15m,
                new DateTime(2026, 5, 1),
                new DateTime(2026, 5, 4)),
            Times.Once);
    }

    [Fact]
    public async Task SubmitRentalRequestCommand_WhenItemIsNull_DoesNotCallService()
    {
        // Arrange
        var mockRentalService = new Mock<IRentalService>();

        var viewModel = new CreateRentalViewModel(mockRentalService.Object)
        {
            Item = null
        };

        // Act
        await viewModel.SubmitRentalRequestCommand.ExecuteAsync(null);

        // Assert
        mockRentalService.Verify(
            service => service.RequestRentalAsync(
                It.IsAny<CreateRentalRequest>(),
                It.IsAny<string>()),
            Times.Never);

        Assert.False(viewModel.IsBusy);
    }

    [Fact]
    public async Task SubmitRentalRequestCommand_WhenViewModelIsBusy_DoesNotCallService()
    {
        // Arrange
        var mockRentalService = new Mock<IRentalService>();

        var viewModel = new CreateRentalViewModel(mockRentalService.Object)
        {
            Item = new Item
            {
                Id = 1,
                Title = "Power Drill",
                DailyRate = 10m
            },
            IsBusy = true
        };

        // Act
        await viewModel.SubmitRentalRequestCommand.ExecuteAsync(null);

        // Assert
        mockRentalService.Verify(
            service => service.RequestRentalAsync(
                It.IsAny<CreateRentalRequest>(),
                It.IsAny<string>()),
            Times.Never);

        Assert.True(viewModel.IsBusy);
    }

    [Fact]
    public async Task SubmitRentalRequestCommand_WhenDateRangeInvalid_SetsErrorMessageAndDoesNotCallRequest()
    {
        // Arrange
        var mockRentalService = new Mock<IRentalService>();

        mockRentalService
            .Setup(service => service.IsValidDateRange(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
            .Returns(false);

        var viewModel = new CreateRentalViewModel(mockRentalService.Object)
        {
            Item = new Item
            {
                Id = 1,
                Title = "Power Drill",
                DailyRate = 10m
            },
            StartDate = new DateTime(2026, 5, 4),
            EndDate = new DateTime(2026, 5, 1)
        };

        // Act
        await viewModel.SubmitRentalRequestCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("End date must be after start date.", viewModel.ErrorMessage);
        Assert.False(viewModel.IsBusy);

        mockRentalService.Verify(
            service => service.RequestRentalAsync(
                It.IsAny<CreateRentalRequest>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task SubmitRentalRequestCommand_WhenSecureStorageUnavailable_SetsErrorMessageAndDoesNotCallRequest()
    {
        // Arrange
        var mockRentalService = new Mock<IRentalService>();

        mockRentalService
            .Setup(service => service.IsValidDateRange(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
            .Returns(true);

        var viewModel = new CreateRentalViewModel(mockRentalService.Object)
        {
            Item = new Item
            {
                Id = 1,
                Title = "Power Drill",
                DailyRate = 10m
            },
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1)
        };

        // Act
        await viewModel.SubmitRentalRequestCommand.ExecuteAsync(null);

        // Assert
        Assert.False(viewModel.IsBusy);
        Assert.NotEqual("", viewModel.ErrorMessage);

        mockRentalService.Verify(
            service => service.RequestRentalAsync(
                It.IsAny<CreateRentalRequest>(),
                It.IsAny<string>()),
            Times.Never);
    }
}