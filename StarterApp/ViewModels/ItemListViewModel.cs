using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;
using StarterApp.Views;
using StarterApp.Database.Data.Repositories;

namespace StarterApp.ViewModels;

public partial class ItemListViewModel : ObservableObject
{
    private readonly ItemService _itemService;
    
    [ObservableProperty]
    private ObservableCollection<Item> items = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = "";

    public ItemListViewModel(IItemRepository itemRepository)
    {
        _itemService = itemService;
    }

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = "";

            var itemList = await _itemRepository.GetAllAsync();
            Items = new ObservableCollection<Item>(itemList);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load items: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToItemDetailAsync(Item item)
    {
        if (item == null)
            return;

        await Shell.Current.GoToAsync(
            nameof(ItemDetailPage),
            new Dictionary<string, object>
            {
                ["Item"] = item
            });
    }
}