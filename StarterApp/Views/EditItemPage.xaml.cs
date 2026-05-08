using StarterApp.ViewModels;

namespace StarterApp.Views;

public partial class EditItemPage : ContentPage
{
    private readonly EditItemViewModel _viewModel;

    public EditItemPage(EditItemViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
}