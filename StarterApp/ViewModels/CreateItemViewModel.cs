using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.ViewModels;

public partial class CreateItemViewModel : ObservableObject
{
    private readonly ItemService _itemService;
    private readonly IAuthenticationService _authService;

    [ObservableProperty] private string title = "";
    [ObservableProperty] private string description = "";
    [ObservableProperty] private string dailyRate = "";
    [ObservableProperty] private string categoryId = "";
    [ObservableProperty] private string latitude = "55.9533";
    [ObservableProperty] private string longitude = "-3.1883";
    [ObservableProperty] private string errorMessage = "";
    [ObservableProperty] private bool isBusy;

    public CreateItemViewModel(ItemService itemService, IAuthenticationService authService)
    {
        _itemService = itemService;
        _authService = authService;
    }

    [RelayCommand]
    private async Task CreateItemAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = "";

            if (!decimal.TryParse(DailyRate, out var parsedRate))
                throw new Exception("Daily rate must be a number.");

            if (!int.TryParse(CategoryId, out var parsedCategoryId))
                throw new Exception("Category ID must be a number.");

            if (!double.TryParse(Latitude, out var parsedLatitude))
                throw new Exception("Latitude must be a number.");

            if (!double.TryParse(Longitude, out var parsedLongitude))
                throw new Exception("Longitude must be a number.");

            var token = await SecureStorage.GetAsync("auth_token");

            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("You must be logged in to create an item.");

            var item = new CreateItemRequest
            {
                Title = Title,
                Description = Description,
                DailyRate = parsedRate,
                CategoryId = parsedCategoryId,
                Latitude = parsedLatitude,
                Longitude = parsedLongitude
            };

            await _itemService.CreateItemAsync(item, token);

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