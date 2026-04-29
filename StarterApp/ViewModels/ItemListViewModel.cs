using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;

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

    public ItemListViewModel(ItemService itemService)
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

            var itemList = await _itemService.GetItemsAsync();
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
}