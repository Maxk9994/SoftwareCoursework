using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;

using StarterApp.Views;

namespace StarterApp.ViewModels;

public partial class ItemDetailViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private Item? item;


    [ObservableProperty]
    private bool isOwner;

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Item", out var selectedItem))
        {
            Item = selectedItem as Item;

             var userIdText = await SecureStorage.GetAsync("user_id");

            if (int.TryParse(userIdText, out var loggedInUserId) && Item != null)
            {
                IsOwner = Item.OwnerId == loggedInUserId;
            }
            else
            {
                IsOwner = false;
            }
        }
    }

     [RelayCommand]
    private async Task EditItemAsync()
    {
        if (Item == null)
            return;

        await Shell.Current.GoToAsync(
            nameof(EditItemPage),
            new Dictionary<string, object>
            {
                ["Item"] = Item
            });
    }
}