namespace StarterApp.Database.Models;

/// <summary>
/// Represents feedback left by a borrower after a completed rental.
/// </summary>
public class Review
{
    public int Id { get; set; }

    public int ItemId { get; set; }

    public string ItemTitle { get; set; } = "";

    public int ReviewerId { get; set; }

    public string ReviewerName { get; set; } = "";

    public int Rating { get; set; }

    public string Comment { get; set; } = "";

    public DateTime CreatedAt { get; set; }
}