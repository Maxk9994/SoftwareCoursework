using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

public partial class EditItemViewModel : ObservableObject, IQueryAttributable
{
    private readonly ItemService _itemService;

    private int itemId;

    [ObservableProperty] private string title = "";
    [ObservableProperty] private string description = "";
    [ObservableProperty] private string dailyRate = "";
    [ObservableProperty] private bool isAvailable;
    [ObservableProperty] private string errorMessage = "";
    [ObservableProperty] private bool isBusy;

    public ObservableCollection<Category> Categories { get; } = new();

    [ObservableProperty]
    private Category? selectedCategory;

    public EditItemViewModel(ItemService itemService)
    {
        _itemService = itemService;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Item", out var selectedItem) && selectedItem is Item item)
        {
            itemId = item.Id;
            Title = item.Title;
            Description = item.Description;
            DailyRate = item.DailyRate.ToString("F2");
            IsAvailable = item.IsAvailable;

            await LoadCategoriesAsync(item.CategoryId);
        }
    }

    private async Task LoadCategoriesAsync(int categoryId)
    {
        try
        {
            Categories.Clear();

            var categories = await _itemService.GetCategoriesAsync();

            foreach (var category in categories)
                Categories.Add(category);

            SelectedCategory = Categories.FirstOrDefault(c => c.Id == categoryId);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load categories: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task UpdateItemAsync()
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

            var token = await SecureStorage.GetAsync("jwt_token");

            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("You must be logged in to update an item.");

            var request = new UpdateItemRequest
            {
                Title = Title,
                Description = Description,
                DailyRate = parsedRate,
                CategoryId = SelectedCategory.Id,
                IsAvailable = IsAvailable
            };

            await _itemService.UpdateItemAsync(itemId, request, token);

            await Shell.Current.GoToAsync("../..");
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