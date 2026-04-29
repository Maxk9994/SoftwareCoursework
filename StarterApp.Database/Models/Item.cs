namespace StarterApp.Database.Models;

public class Item
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal DailyRate { get; set; }
    public int CategoryId { get; set; }
    public string Category { get; set; } = "";
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = "";
    public double OwnerRating { get; set; }
    public bool IsAvailable { get; set; }
    public double AverageRating { get; set; }
    public DateTime CreatedAt { get; set; }
}