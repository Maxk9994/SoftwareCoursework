using StarterApp.ViewModels;
using StarterApp.Views;

namespace StarterApp;

public partial class AppShell : Shell
{
	public AppShell(AppShellViewModel viewModel)
	{	
		BindingContext = viewModel;
		InitializeComponent();

		Routing.RegisterRoute(nameof(ItemListPage), typeof(ItemListPage));
		Routing.RegisterRoute(nameof(CreateItemPage), typeof(CreateItemPage));
		Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
		Routing.RegisterRoute(nameof(EditItemPage), typeof(EditItemPage));
	}
}
