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

    public async Task<List<Rental>> GetIncomingRentalsAsync(string token)
    {
    return await _rentalRepository.GetIncomingRentalsAsync(token);
    }

    public async Task<List<Rental>> GetOutgoingRentalsAsync(string token)
    {
    return await _rentalRepository.GetOutgoingRentalsAsync(token);
    }   

    public async Task<Rental?> RequestRentalAsync(CreateRentalRequest request, string token)
    {
        if (!IsValidDateRange(request.StartDate, request.EndDate))
            throw new ArgumentException("End date must be after start date.");

        return await _rentalRepository.CreateRentalRequestAsync(request, token);
    }

   /// <summary>
/// Approves a rental request after checking that the item is not already booked
/// for the requested date range.
/// </summary>
/// <param name="rental">The rental request to approve.</param>
/// <param name="token">The authenticated user's JWT token.</param>
/// <returns>The updated rental if approval succeeds.</returns>
/// <exception cref="InvalidOperationException">
/// Thrown when another approved or active rental overlaps with the requested dates.
/// </exception>
public async Task<Rental?> ApproveRentalAsync(Rental rental, string token)
{
    // Business rule: do not allow two approved/active rentals for the same item to overlap in date range.
    var hasOverlap = await HasDateOverlapAsync(rental, token);

    if (hasOverlap)
        throw new InvalidOperationException("This item is already booked for the selected dates.");

    // Repository handles the API call after the business rule passes.
    return await _rentalRepository.UpdateRentalStatusAsync(rental.Id, "Approved", token);
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

    public async Task<Rental?> MarkOutForRentAsync(int rentalId, string token)
    {
        return await _rentalRepository.UpdateRentalStatusAsync(rentalId, "Out for Rent", token);
    }

    public async Task<Rental?> MarkReturnedAsync(int rentalId, string token)
    {
        return await _rentalRepository.UpdateRentalStatusAsync(rentalId, "Returned", token);
    }

    public async Task<Rental?> CompleteRentalAsync(int rentalId, string token)
    {
        return await _rentalRepository.UpdateRentalStatusAsync(rentalId, "Completed", token);
    }

    /// <summary>
/// Checks whether a rental request overlaps with another approved or active rental
/// for the same item.
/// </summary>
/// <param name="rental">The rental request being checked.</param>
/// <param name="token">The authenticated user's JWT token.</param>
/// <returns>True if the item is already booked for any of the requested dates.</returns>
public async Task<bool> HasDateOverlapAsync(Rental rental, string token)
{
    var incomingRentals = await _rentalRepository.GetIncomingRentalsAsync(token);

    // Only statuses that represent confirmed or active bookings should block approval.
    var blockingStatuses = new[]
    {
        "Approved",
        "Out for Rent"
    };

    var existingBookings = incomingRentals.Where(existingRental =>
        existingRental.Id != rental.Id &&
        existingRental.ItemId == rental.ItemId &&
        blockingStatuses.Contains(existingRental.Status, StringComparer.OrdinalIgnoreCase));

    foreach (var existingRental in existingBookings)
    {
        // Date ranges overlap when each rental starts before the other one ends.
        var datesOverlap =
            rental.StartDate.Date < existingRental.EndDate.Date &&
            existingRental.StartDate.Date < rental.EndDate.Date;

        if (datesOverlap)
            return true;
    }

    return false;
}
}