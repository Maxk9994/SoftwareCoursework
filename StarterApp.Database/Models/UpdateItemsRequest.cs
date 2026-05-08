namespace StarterApp.Database.Models;

public class UpdateItemRequest
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal DailyRate { get; set; }
    public int CategoryId { get; set; }
    public bool IsAvailable { get; set; }
}