using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.ViewModels;

/// <summary>
/// ViewModel for submitting a review after a rental has been completed.
/// </summary>
public partial class CreateReviewViewModel : ObservableObject, IQueryAttributable
{
    private readonly IReviewService _reviewService;

    [ObservableProperty]
    private Rental? rental;

    [ObservableProperty]
    private int rating = 5;

    [ObservableProperty]
    private string comment = "";

    [ObservableProperty]
    private string errorMessage = "";

    [ObservableProperty]
    private bool isBusy;

    public CreateReviewViewModel(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    /// <summary>
    /// Receives the selected completed rental from Shell navigation.
    /// </summary>
    /// <param name="query">Navigation parameters containing the selected rental.</param>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Rental", out var selectedRental))
        {
            Rental = selectedRental as Rental;
        }
    }

    /// <summary>
    /// Submits a review for the completed rental.
    /// </summary>
    [RelayCommand]
    private async Task SubmitReviewAsync()
    {
        if (Rental == null || IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = "";

            var token = await SecureStorage.GetAsync("jwt_token");

            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("You must be logged in to submit a review.");

            var request = new CreateReviewRequest
            {
                RentalId = Rental.Id,
                Rating = Rating,
                Comment = Comment
            };

            // The service validates rating/comment before sending to the repository.
            await _reviewService.CreateReviewAsync(request, token);

            await Shell.Current.DisplayAlert("Review Submitted", "Your review has been saved.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}