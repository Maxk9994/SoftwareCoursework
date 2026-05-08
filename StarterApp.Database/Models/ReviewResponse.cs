namespace StarterApp.Database.Models;

/// <summary>
/// Represents the API response returned when loading reviews.
/// </summary>
public class ReviewResponse
{
    public List<Review> Reviews { get; set; } = new();

    public int TotalReviews { get; set; }
}