namespace StarterApp.Models;

public class CreateItemRequest
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal DailyRate { get; set; }
    public int CategoryId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}