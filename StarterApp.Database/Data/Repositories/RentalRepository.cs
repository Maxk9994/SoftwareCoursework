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

   public async Task<List<Rental>> GetIncomingRentalsAsync(string token)
{
    _httpClient.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

    var response = await _httpClient.GetAsync("rentals/incoming");

    var json = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
        throw new Exception($"Load incoming rentals failed: {response.StatusCode} - {json}");

    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    var result = JsonSerializer.Deserialize<RentalResponse>(json, options);

    return result?.Rentals ?? new List<Rental>();
}

public async Task<List<Rental>> GetOutgoingRentalsAsync(string token)
{
    _httpClient.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

    var response = await _httpClient.GetAsync("rentals/outgoing");

    var json = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
        throw new Exception($"Load outgoing rentals failed: {response.StatusCode} - {json}");

    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    var result = JsonSerializer.Deserialize<RentalResponse>(json, options);

    return result?.Rentals ?? new List<Rental>();
}

    public async Task<Rental?> CreateRentalRequestAsync(CreateRentalRequest request, string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var apiRequest = new
        {
        itemId = request.ItemId,
        startDate = request.StartDate.ToString("yyyy-MM-dd"),
        endDate = request.EndDate.ToString("yyyy-MM-dd")
        };
       
       
        var response = await _httpClient.PostAsJsonAsync("rentals", apiRequest);

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

    var requestBody = new
    {
        status = status
    };

    var request = new HttpRequestMessage(HttpMethod.Patch, $"rentals/{rentalId}/status")
    {
        Content = JsonContent.Create(requestBody)
    };

    var response = await _httpClient.SendAsync(request);

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