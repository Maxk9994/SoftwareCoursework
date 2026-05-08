using System.Net.Http.Json;
using System.Text.Json;
using StarterApp.Database.Models;

namespace StarterApp.Database.Data.Repositories;

/// <summary>
/// API-backed repository for review data access.
/// </summary>
public class ReviewRepository : IReviewRepository
{
    private readonly HttpClient _httpClient;

    public ReviewRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Gets all reviews for a specific item from the API.
    /// </summary>
    /// <param name="itemId">The ID of the item whose reviews should be loaded.</param>
    /// <returns>A list of reviews for the item.</returns>
    public async Task<List<Review>> GetReviewsForItemAsync(int itemId)
    {
        var response = await _httpClient.GetAsync($"items/{itemId}/reviews");

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Load reviews failed: {response.StatusCode} - {json}");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result = JsonSerializer.Deserialize<ReviewResponse>(json, options);

        return result?.Reviews ?? new List<Review>();
    }

    /// <summary>
    /// Submits a new item review to the API.
    /// </summary>
    /// <param name="request">The review data to submit.</param>
    /// <param name="token">The authenticated user's JWT token.</param>
    /// <returns>The created review if successful.</returns>
    public async Task<Review?> CreateReviewAsync(CreateReviewRequest request, string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.PostAsJsonAsync("reviews", request);

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Create review failed: {response.StatusCode} - {json}");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<Review>(json, options);
    }
}