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

     public async Task CreateItemAsync(CreateItemRequest item, string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "items");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        request.Content = JsonContent.Create(item);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Create item failed: {error}");
        }
    }
}
