using System.Net.Http.Json;
using System.Text.Json;
using StarterApp.Database.Models;

namespace StarterApp.Database.Data.Repositories;

public class RentalRepository : IRentalRepository
{
    private readonly HttpClient _httpClient;

    public RentalRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Rental>> GetRentalsAsync(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync("rentals");

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Load rentals failed: {response.StatusCode} - {json}");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var rentals = JsonSerializer.Deserialize<List<Rental>>(json, options);

        return rentals ?? new List<Rental>();
    }

    public async Task<Rental?> CreateRentalRequestAsync(CreateRentalRequest request, string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.PostAsJsonAsync("rentals", request);

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Create rental request failed: {response.StatusCode} - {json}");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<Rental>(json, options);
    }

    public async Task<Rental?> UpdateRentalStatusAsync(int rentalId, string status, string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            Status = status
        };

        var response = await _httpClient.PutAsJsonAsync($"rentals/{rentalId}/status", request);

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Update rental status failed: {response.StatusCode} - {json}");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<Rental>(json, options);
    }
}