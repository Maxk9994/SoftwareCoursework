using Moq;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.Tests.Services;

public class RentalServiceTests
{
    [Fact]
    public void CalculateTotalPrice_WhenValidDateRange_ReturnsCorrectTotal()
    {
        // Arrange
        var mockRepository = new Mock<IRentalRepository>();
        var service = new RentalService(mockRepository.Object);

        var dailyRate = 10m;
        var startDate = new DateTime(2026, 5, 1);
        var endDate = new DateTime(2026, 5, 4);

        // Act
        var result = service.CalculateTotalPrice(dailyRate, startDate, endDate);

        // Assert
        Assert.Equal(30m, result);
    }

    [Fact]
    public void CalculateTotalPrice_WhenInvalidDateRange_ReturnsZero()
    {
        // Arrange
        var mockRepository = new Mock<IRentalRepository>();
        var service = new RentalService(mockRepository.Object);

        var dailyRate = 10m;
        var startDate = new DateTime(2026, 5, 4);
        var endDate = new DateTime(2026, 5, 1);

        // Act
        var result = service.CalculateTotalPrice(dailyRate, startDate, endDate);

        // Assert
        Assert.Equal(0m, result);
    }

    [Fact]
    public void IsValidDateRange_WhenEndDateAfterStartDate_ReturnsTrue()
    {
        // Arrange
        var mockRepository = new Mock<IRentalRepository>();
        var service = new RentalService(mockRepository.Object);

        var startDate = new DateTime(2026, 5, 1);
        var endDate = new DateTime(2026, 5, 2);

        // Act
        var result = service.IsValidDateRange(startDate, endDate);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidDateRange_WhenEndDateBeforeStartDate_ReturnsFalse()
    {
        // Arrange
        var mockRepository = new Mock<IRentalRepository>();
        var service = new RentalService(mockRepository.Object);

        var startDate = new DateTime(2026, 5, 2);
        var endDate = new DateTime(2026, 5, 1);

        // Act
        var result = service.IsValidDateRange(startDate, endDate);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasDateOverlapAsync_WhenExistingApprovedRentalOverlaps_ReturnsTrue()
    {
        // Arrange
        var mockRepository = new Mock<IRentalRepository>();
        var service = new RentalService(mockRepository.Object);

        var token = "test-token";

        var rentalToCheck = new Rental
        {
            Id = 2,
            ItemId = 1,
            StartDate = new DateTime(2026, 5, 2),
            EndDate = new DateTime(2026, 5, 5),
            Status = "Pending"
        };

        var existingRentals = new List<Rental>
        {
            new Rental
            {
                Id = 1,
                ItemId = 1,
                StartDate = new DateTime(2026, 5, 1),
                EndDate = new DateTime(2026, 5, 3),
                Status = "Approved"
            }
        };

        mockRepository
            .Setup(repo => repo.GetIncomingRentalsAsync(token))
            .ReturnsAsync(existingRentals);

        // Act
        var result = await service.HasDateOverlapAsync(rentalToCheck, token);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasDateOverlapAsync_WhenNoRentalOverlaps_ReturnsFalse()
    {
        // Arrange
        var mockRepository = new Mock<IRentalRepository>();
        var service = new RentalService(mockRepository.Object);

        var token = "test-token";

        var rentalToCheck = new Rental
        {
            Id = 2,
            ItemId = 1,
            StartDate = new DateTime(2026, 5, 5),
            EndDate = new DateTime(2026, 5, 7),
            Status = "Pending"
        };

        var existingRentals = new List<Rental>
        {
            new Rental
            {
                Id = 1,
                ItemId = 1,
                StartDate = new DateTime(2026, 5, 1),
                EndDate = new DateTime(2026, 5, 3),
                Status = "Approved"
            }
        };

        mockRepository
            .Setup(repo => repo.GetIncomingRentalsAsync(token))
            .ReturnsAsync(existingRentals);

        // Act
        var result = await service.HasDateOverlapAsync(rentalToCheck, token);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ApproveRentalAsync_WhenOverlapExists_ThrowsInvalidOperationException()
    {
        // Arrange
        var mockRepository = new Mock<IRentalRepository>();
        var service = new RentalService(mockRepository.Object);

        var token = "test-token";

        var rentalToApprove = new Rental
        {
            Id = 2,
            ItemId = 1,
            StartDate = new DateTime(2026, 5, 2),
            EndDate = new DateTime(2026, 5, 5),
            Status = "Pending"
        };

        var existingRentals = new List<Rental>
        {
            new Rental
            {
                Id = 1,
                ItemId = 1,
                StartDate = new DateTime(2026, 5, 1),
                EndDate = new DateTime(2026, 5, 3),
                Status = "Approved"
            }
        };

        mockRepository
            .Setup(repo => repo.GetIncomingRentalsAsync(token))
            .ReturnsAsync(existingRentals);

        // Act and Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ApproveRentalAsync(rentalToApprove, token));

        mockRepository.Verify(
            repo => repo.UpdateRentalStatusAsync(
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task ApproveRentalAsync_WhenNoOverlap_UpdatesStatusToApproved()
    {
        // Arrange
        var mockRepository = new Mock<IRentalRepository>();
        var service = new RentalService(mockRepository.Object);

        var token = "test-token";

        var rentalToApprove = new Rental
        {
            Id = 2,
            ItemId = 1,
            StartDate = new DateTime(2026, 5, 5),
            EndDate = new DateTime(2026, 5, 7),
            Status = "Pending"
        };

        var existingRentals = new List<Rental>
        {
            new Rental
            {
                Id = 1,
                ItemId = 1,
                StartDate = new DateTime(2026, 5, 1),
                EndDate = new DateTime(2026, 5, 3),
                Status = "Approved"
            }
        };

        var approvedRental = new Rental
        {
            Id = 2,
            ItemId = 1,
            StartDate = new DateTime(2026, 5, 5),
            EndDate = new DateTime(2026, 5, 7),
            Status = "Approved"
        };

        mockRepository
            .Setup(repo => repo.GetIncomingRentalsAsync(token))
            .ReturnsAsync(existingRentals);

        mockRepository
            .Setup(repo => repo.UpdateRentalStatusAsync(2, "Approved", token))
            .ReturnsAsync(approvedRental);

        // Act
        var result = await service.ApproveRentalAsync(rentalToApprove, token);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Approved", result.Status);

        mockRepository.Verify(
            repo => repo.UpdateRentalStatusAsync(2, "Approved", token),
            Times.Once);
    }
}