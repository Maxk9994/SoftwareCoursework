using System.Net.Http.Json;
using StarterApp.Database.Models;

namespace StarterApp.Services;

public class ItemService
{
    private readonly HttpClient _httpClient;

    public ItemService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Item>> GetItemsAsync()
    {
        var response = await _httpClient.GetAsync("items");

        if (!response.IsSuccessStatusCode)
            return new List<Item>();

        var result = await response.Content.ReadFromJsonAsync<ItemResponse>();

        return result?.Items ?? new List<Item>();
    }
}