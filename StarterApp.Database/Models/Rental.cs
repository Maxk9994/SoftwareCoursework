namespace StarterApp.Database.Models;


public class Rental
{
    public int Id { get; set; }

    public int ItemId { get; set; }

    public string ItemTitle { get; set; } = "";

    public string ItemDescription { get; set; } = "";

    public int BorrowerId { get; set; }

    public string BorrowerName { get; set; } = "";

    public decimal? BorrowerRating { get; set; }

    public int OwnerId { get; set; }

    public string OwnerName { get; set; } = "";

    public decimal? OwnerRating { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Status { get; set; } = "";

    public decimal TotalPrice { get; set; }

    public DateTime RequestedAt { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public bool CanApproveOrReject => Status == "Requested";

    public bool CanMarkOutForRent => Status == "Approved";

    public bool CanMarkReturned =>
    string.Equals(Status, "Out for Rent", StringComparison.OrdinalIgnoreCase)
    || string.Equals(Status, "OutForRent", StringComparison.OrdinalIgnoreCase)
    || string.Equals(Status, "Out_For_Rent", StringComparison.OrdinalIgnoreCase)
    || string.Equals(Status, "out_for_rent", StringComparison.OrdinalIgnoreCase);


    public bool CanComplete => Status == "Returned";

    /// <summary>
/// Determines whether the current user can submit a review for this rental.
/// Reviews are only allowed after the rental has been completed.
/// </summary>
public bool CanReview =>
    string.Equals(Status, "Completed", StringComparison.OrdinalIgnoreCase);
}