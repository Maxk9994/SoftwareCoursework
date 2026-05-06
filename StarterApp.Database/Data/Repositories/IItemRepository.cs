using StarterApp.Database.Models;

namespace StarterApp.Database.Data.Repositories;

public interface IItemRepository
{
    Task<List<Item>> GetItemsAsync();

    Task<Item?> CreateItemAsync(CreateItemRequest request, string token);
}