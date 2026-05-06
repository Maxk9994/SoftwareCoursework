using StarterApp.Database.Models;

namespace StarterApp.Database.Data.Repositories;

public interface IItemRepository : IRepository<Item>
{
    Task<Item?> CreateItemAsync(CreateItemRequest request, string token);
    Task<bool> UpdateItemAsync(int itemId, UpdateItemRequest request, string token);
    Task<List<Category>> GetCategoriesAsync();
}