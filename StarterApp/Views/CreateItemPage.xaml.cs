using StarterApp.ViewModels;

namespace StarterApp.Views;

public partial class CreateItemPage : ContentPage
{
    public CreateItemPage(CreateItemViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is CreateItemViewModel vm)
        {
            await vm.LoadCategoriesAsync();
        }
    }
}
