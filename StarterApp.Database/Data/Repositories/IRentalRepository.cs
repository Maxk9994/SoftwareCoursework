using StarterApp.Database.Models;

namespace StarterApp.Database.Data.Repositories;

public interface IRentalRepository
{
    Task<List<Rental>> GetRentalsAsync(string token);

    Task<Rental?> CreateRentalRequestAsync(CreateRentalRequest request, string token);

    Task<Rental?> UpdateRentalStatusAsync(int rentalId, string status, string token);
}