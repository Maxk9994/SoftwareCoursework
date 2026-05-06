namespace StarterApp.Database.Models;

/// <summary>
/// DTO used when submitting a new review to the API.
/// </summary>
public class CreateReviewRequest
{
    public int ItemId { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; } = "";
}