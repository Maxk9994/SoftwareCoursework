namespace StarterApp.Database.Models;

/// <summary>
/// Represents feedback left by a user after a completed rental.
/// </summary>
public class Review
{
    public int Id { get; set; }

    public int RentalId { get; set; }

    public int ReviewerId { get; set; }

    public string ReviewerName { get; set; } = "";

    public int Rating { get; set; }

    public string Comment { get; set; } = "";

    public DateTime CreatedAt { get; set; }
}