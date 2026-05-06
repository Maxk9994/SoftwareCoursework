namespace StarterApp.Database.Models;

public class Rental
{
    public int Id { get; set; }

    public int ItemId { get; set; }
    public Item? Item { get; set; }

    public int RenterId { get; set; }
    public User? Renter { get; set; }

    public int OwnerId { get; set; }
    public User? Owner { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = "Requested";
}