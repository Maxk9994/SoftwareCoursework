using StarterApp.Database.Models;

namespace StarterApp.Database.Data.Repositories;

/// <summary>
/// Defines data access operations for item reviews.
/// </summary>
public interface IReviewRepository
{
    /// <summary>
    /// Gets all reviews for a specific item.
    /// </summary>
    /// <param name="itemId">The ID of the item whose reviews should be loaded.</param>
    /// <returns>A list of reviews for the item.</returns>
    Task<List<Review>> GetReviewsForItemAsync(int itemId);

    /// <summary>
    /// Creates a new review using the shared API.
    /// </summary>
    /// <param name="request">The review data to submit.</param>
    /// <param name="token">The authenticated user's JWT token.</param>
    /// <returns>The created review if successful.</returns>
    Task<Review?> CreateReviewAsync(CreateReviewRequest request, string token);
}