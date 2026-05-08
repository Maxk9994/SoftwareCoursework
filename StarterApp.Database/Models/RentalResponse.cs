namespace StarterApp.Database.Models;

public class RentalResponse
{
    public List<Rental> Rentals { get; set; } = new();

    public int TotalRentals { get; set; }
}