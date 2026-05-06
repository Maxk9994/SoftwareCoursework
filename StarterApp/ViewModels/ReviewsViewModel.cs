using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

/// <summary>
/// ViewModel for displaying reviews for a selected item.
/// </summary>
public partial class ReviewsViewModel : ObservableObject, IQueryAttributable
{
    private readonly IReviewService _reviewService;

    public ObservableCollection<Review> Reviews { get; } = new();

    [ObservableProperty]
    private Item? item;

    [ObservableProperty]
    private string errorMessage = "";

    [ObservableProperty]
    private bool isBusy;

    public ReviewsViewModel(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    /// <summary>
    /// Receives the selected item passed through Shell navigation.
    /// </summary>
    /// <param name="query">Navigation parameters containing the selected item.</param>
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Item", out var selectedItem))
        {
            Item = selectedItem as Item;
            await LoadReviewsAsync();
        }
    }

    /// <summary>
    /// Loads all reviews for the selected item.
    /// </summary>
    [RelayCommand]
    public async Task LoadReviewsAsync()
    {
        if (Item == null || IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = "";
            Reviews.Clear();

            var reviews = await _reviewService.GetReviewsForItemAsync(Item.Id);

            foreach (var review in reviews)
                Reviews.Add(review);
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