using StarterApp.ViewModels;

namespace StarterApp.Views;

public partial class ItemListPage : ContentPage
{
    private readonly ItemListViewModel _viewModel;

    public ItemListPage(ItemListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadItemsCommand.ExecuteAsync(null);
    }

    private async void OnCreateItemClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CreateItemPage));
    }
}