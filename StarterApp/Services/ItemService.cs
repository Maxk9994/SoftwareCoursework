using System.Net.Http.Json;
using System.Text.Json;
using StarterApp.Database.Models;
using StarterApp.Services;

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

    if (!response.IsSuccessStatusCode)
        return null;

    return await response.Content.ReadFromJsonAsync<Item>();
}
}
