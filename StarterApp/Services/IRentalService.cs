using StarterApp.Database.Models;

namespace StarterApp.Services;

public interface IRentalService
{
    Task<List<Rental>> GetIncomingRentalsAsync(string token);

    Task<List<Rental>> GetOutgoingRentalsAsync(string token);

    Task<Rental?> RequestRentalAsync(CreateRentalRequest request, string token);

    Task<Rental?> ApproveRentalAsync(int rentalId, string token);

    Task<Rental?> RejectRentalAsync(int rentalId, string token);

    decimal CalculateTotalPrice(decimal dailyRate, DateTime startDate, DateTime endDate);

    bool IsValidDateRange(DateTime startDate, DateTime endDate);

    Task<Rental?> MarkOutForRentAsync(int rentalId, string token);

    Task<Rental?> MarkReturnedAsync(int rentalId, string token);

    Task<Rental?> CompleteRentalAsync(int rentalId, string token);
}