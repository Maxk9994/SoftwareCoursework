namespace StarterApp.Database.Models;

public class CreateRentalRequest
{
    public int ItemId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}