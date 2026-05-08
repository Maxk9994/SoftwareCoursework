using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.ViewModels;

public partial class CreateRentalViewModel : ObservableObject, IQueryAttributable
{
    private readonly IRentalService _rentalService;

    [ObservableProperty]
    private Item? item;

    [ObservableProperty]
    private DateTime startDate = DateTime.Today;

    [ObservableProperty]
    private DateTime endDate = DateTime.Today.AddDays(1);

    [ObservableProperty]
    private string errorMessage = "";

    [ObservableProperty]
    private bool isBusy;

    public decimal EstimatedTotal =>
        Item == null ? 0 : _rentalService.CalculateTotalPrice(Item.DailyRate, StartDate, EndDate);

    public CreateRentalViewModel(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Item", out var selectedItem))
        {
            Item = selectedItem as Item;
            OnPropertyChanged(nameof(EstimatedTotal));
        }
    }

    partial void OnStartDateChanged(DateTime value)
    {
        OnPropertyChanged(nameof(EstimatedTotal));
    }

    partial void OnEndDateChanged(DateTime value)
    {
        OnPropertyChanged(nameof(EstimatedTotal));
    }

    [RelayCommand]
    private async Task SubmitRentalRequestAsync()
    {
        if (Item == null || IsBusy)
            return;

        ErrorMessage = "";

        if (!_rentalService.IsValidDateRange(StartDate, EndDate))
        {
            ErrorMessage = "End date must be after start date.";
            return;
        }

        try
        {
            IsBusy = true;

            var token = await SecureStorage.GetAsync("jwt_token");

            if (string.IsNullOrWhiteSpace(token))
            {
                ErrorMessage = "You must be logged in to request a rental.";
                return;
            }

            var request = new CreateRentalRequest
            {
                ItemId = Item.Id,
                StartDate = StartDate,
                EndDate = EndDate
            };

            await _rentalService.RequestRentalAsync(request, token);

            await Shell.Current.DisplayAlert("Rental Requested", "Your rental request has been submitted.", "OK");
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