using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;

namespace StarterApp.Services;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;

    public RentalService(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }

    public async Task<List<Rental>> GetRentalsAsync(string token)
    {
        return await _rentalRepository.GetRentalsAsync(token);
    }

    public async Task<Rental?> RequestRentalAsync(CreateRentalRequest request, string token)
    {
        if (!IsValidDateRange(request.StartDate, request.EndDate))
            throw new ArgumentException("End date must be after start date.");

        return await _rentalRepository.CreateRentalRequestAsync(request, token);
    }

    public async Task<Rental?> ApproveRentalAsync(int rentalId, string token)
    {
        return await _rentalRepository.UpdateRentalStatusAsync(rentalId, "Approved", token);
    }

    public async Task<Rental?> RejectRentalAsync(int rentalId, string token)
    {
        return await _rentalRepository.UpdateRentalStatusAsync(rentalId, "Rejected", token);
    }

    public decimal CalculateTotalPrice(decimal dailyRate, DateTime startDate, DateTime endDate)
    {
        var days = (endDate.Date - startDate.Date).Days;

        if (days <= 0)
            return 0;

        return dailyRate * days;
    }

    public bool IsValidDateRange(DateTime startDate, DateTime endDate)
    {
        return endDate.Date > startDate.Date;
    }
}