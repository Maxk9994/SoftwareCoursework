namespace StarterApp.Database.Models;

/// <summary>
/// DTO used when submitting a review for a completed rental.
/// </summary>
public class CreateReviewRequest
{
    /// <summary>
    /// The completed rental being reviewed.
    /// </summary>
    public int RentalId { get; set; }

    /// <summary>
    /// Rating value from 1 to 5.
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Written feedback for the rental experience.
    /// </summary>
    public string Comment { get; set; } = "";
}