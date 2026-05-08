using System.Text.Json;
using StarterApp.Database.Models;
using System.Net.Http.Json;

namespace StarterApp.Database.Data.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly HttpClient _httpClient;

    public ItemRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Item>> GetItemsAsync()
    {
        var response = await _httpClient.GetAsync("items");

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"API failed: {response.StatusCode} - {json}");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result = JsonSerializer.Deserialize<ItemResponse>(json, options);

        if (result == null)
            throw new Exception("API returned null result");

        return result.Items;
    }

    public async Task<Item?> CreateItemAsync(CreateItemRequest request, string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.PostAsJsonAsync("items", request);

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Create item failed: {response.StatusCode} - {json}");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<Item>(json, options);
    }

    public async Task<List<Category>> GetCategoriesAsync()
{
    var response = await _httpClient.GetAsync("categories");

    var json = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
        throw new Exception($"API failed: {response.StatusCode} - {json}");

    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    var result = JsonSerializer.Deserialize<CategoryResponse>(json, options);

    if (result == null)
        throw new Exception("API returned null category result");

    return result.Categories;
}

    public async Task<Item?> UpdateItemAsync(int itemId, UpdateItemRequest request, string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.PutAsJsonAsync($"items/{itemId}", request);

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Update item failed: {response.StatusCode} - {json}");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<Item>(json, options);
    }
}