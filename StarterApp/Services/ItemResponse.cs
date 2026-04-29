using StarterApp.Database.Models;

namespace StarterApp.Services;

public class ItemResponse
{
    public List<Item> Items { get; set; } = new();
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}