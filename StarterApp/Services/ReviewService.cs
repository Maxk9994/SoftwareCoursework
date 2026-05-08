using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;

namespace StarterApp.Services;

/// <summary>
/// Handles review-related business rules before delegating data access to the repository.
/// </summary>
public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewService(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    /// <summary>
    /// Gets all reviews for a specific item.
    /// </summary>
    /// <param name="itemId">The ID of the item whose reviews should be loaded.</param>
    /// <returns>A list of reviews for the item.</returns>
    public async Task<List<Review>> GetReviewsForItemAsync(int itemId)
    {
        return await _reviewRepository.GetReviewsForItemAsync(itemId);
    }

    /// <summary>
    /// Submits a review after validating that the rating and comment are valid.
    /// </summary>
    /// <param name="request">The review request to submit.</param>
    /// <param name="token">The authenticated user's JWT token.</param>
    /// <returns>The created review if successful.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the rating is outside the allowed 1-5 range or the comment is empty.
    /// </exception>
    public async Task<Review?> CreateReviewAsync(CreateReviewRequest request, string token)
    {
        // Review validation belongs in the service layer because it is business logic.
        if (request.Rating < 1 || request.Rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");

        if (string.IsNullOrWhiteSpace(request.Comment))
            throw new ArgumentException("Comment is required.");

        return await _reviewRepository.CreateReviewAsync(request, token);
    }
}