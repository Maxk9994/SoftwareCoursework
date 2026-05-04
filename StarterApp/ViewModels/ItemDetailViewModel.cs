using CommunityToolkit.Mvvm.ComponentModel;
using StarterApp.Database.Models;

namespace StarterApp.ViewModels;

public partial class ItemDetailViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private Item? item;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Item", out var selectedItem))
        {
            Item = selectedItem as Item;
        }
    }
}