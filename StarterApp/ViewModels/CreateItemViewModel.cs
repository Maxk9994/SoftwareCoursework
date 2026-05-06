using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;
using System.Collections.ObjectModel;
using StarterApp.Database.Data.Repositories;

namespace StarterApp.ViewModels;

public partial class CreateItemViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;

    [ObservableProperty] private string title = "";
    [ObservableProperty] private string description = "";
    [ObservableProperty] private string dailyRate = "";
    [ObservableProperty] private string latitude = "55.9533";
    [ObservableProperty] private string longitude = "-3.1883";
    [ObservableProperty] private string errorMessage = "";
    [ObservableProperty] private bool isBusy;

    public ObservableCollection<Category> Categories { get; } = new();

    [ObservableProperty]
    private Category? selectedCategory;

    public CreateItemViewModel(IItemRepository itemRepository, IAuthenticationService authService)
    {
    
        _itemRepository = itemRepository;
    }

    public async Task LoadCategoriesAsync()
    {
        try
        {
            Categories.Clear();

            var categories = await _itemRepository.GetCategoriesAsync();

            foreach (var category in categories)
                Categories.Add(category);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load categories: {ex.Message}";
        }
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

            if (SelectedCategory == null)
                throw new Exception("Please select a category.");

            if (!double.TryParse(Latitude, out var parsedLatitude))
                throw new Exception("Latitude must be a number.");

            if (!double.TryParse(Longitude, out var parsedLongitude))
                throw new Exception("Longitude must be a number.");

            var token = await SecureStorage.GetAsync("jwt_token");

            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("You must be logged in to create an item.");

            var item = new CreateItemRequest
            {
                Title = Title,
                Description = Description,
                DailyRate = parsedRate,
                CategoryId = SelectedCategory.Id,
                Latitude = parsedLatitude,
                Longitude = parsedLongitude
            };

            await _itemRepository.CreateItemAsync(item, token);

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