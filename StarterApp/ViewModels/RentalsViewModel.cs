using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

public partial class RentalsViewModel : ObservableObject
{
    private readonly IRentalService _rentalService;

    public ObservableCollection<Rental> IncomingRentals { get; } = new();

    public ObservableCollection<Rental> OutgoingRentals { get; } = new();

    [ObservableProperty]
    private string errorMessage = "";

    [ObservableProperty]
    private bool isBusy;

    public RentalsViewModel(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    [RelayCommand]
public async Task LoadRentalsAsync()
{
    if (IsBusy)
        return;

    try
    {
        IsBusy = true;
        ErrorMessage = "";

        IncomingRentals.Clear();
        OutgoingRentals.Clear();

        var token = await SecureStorage.GetAsync("jwt_token");

        if (string.IsNullOrWhiteSpace(token))
            throw new Exception("You must be logged in to view rentals.");

        var incoming = await _rentalService.GetIncomingRentalsAsync(token);
        var outgoing = await _rentalService.GetOutgoingRentalsAsync(token);

        foreach (var rental in incoming)
            IncomingRentals.Add(rental);

        foreach (var rental in outgoing)
            OutgoingRentals.Add(rental);
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

[RelayCommand]
private async Task ApproveRentalAsync(Rental rental)
{
    if (rental == null || IsBusy)
        return;

    try
    {
        IsBusy = true;
        ErrorMessage = "";

        var token = await SecureStorage.GetAsync("jwt_token");

        if (string.IsNullOrWhiteSpace(token))
            throw new Exception("You must be logged in to approve rentals.");

        await _rentalService.ApproveRentalAsync(rental.Id, token);

        IsBusy = false;
        await LoadRentalsAsync();
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

[RelayCommand]
private async Task RejectRentalAsync(Rental rental)
{
    if (rental == null || IsBusy)
        return;

    try
    {
        IsBusy = true;
        ErrorMessage = "";

        var token = await SecureStorage.GetAsync("jwt_token");

        if (string.IsNullOrWhiteSpace(token))
            throw new Exception("You must be logged in to reject rentals.");

        await _rentalService.RejectRentalAsync(rental.Id, token);
        IsBusy = false;
        await LoadRentalsAsync();
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

[RelayCommand]
private async Task MarkOutForRentAsync(Rental rental)
{
    if (rental == null || IsBusy)
        return;

    try
    {
        IsBusy = true;
        ErrorMessage = "";

        var token = await SecureStorage.GetAsync("jwt_token");

        if (string.IsNullOrWhiteSpace(token))
            throw new Exception("You must be logged in to update rentals.");

        await _rentalService.MarkOutForRentAsync(rental.Id, token);

        await Shell.Current.DisplayAlert("Updated", "Rental marked as out for rent.", "OK");
    }
    catch (Exception ex)
    {
        ErrorMessage = ex.Message;
    }
    finally
    {
        IsBusy = false;
        await LoadRentalsAsync();
    }
}

[RelayCommand]
private async Task MarkReturnedAsync(Rental rental)
{
    if (rental == null || IsBusy)
        return;

    try
    {
        IsBusy = true;
        ErrorMessage = "";

        var token = await SecureStorage.GetAsync("jwt_token");

        if (string.IsNullOrWhiteSpace(token))
            throw new Exception("You must be logged in to update rentals.");

        await _rentalService.MarkReturnedAsync(rental.Id, token);

        await Shell.Current.DisplayAlert("Updated", "Rental marked as returned.", "OK");
    }
    catch (Exception ex)
    {
        ErrorMessage = ex.Message;
    }
    finally
    {
        IsBusy = false;
        await LoadRentalsAsync();
    }
}

[RelayCommand]
private async Task CompleteRentalAsync(Rental rental)
{
    if (rental == null || IsBusy)
        return;

    try
    {
        IsBusy = true;
        ErrorMessage = "";

        var token = await SecureStorage.GetAsync("jwt_token");

        if (string.IsNullOrWhiteSpace(token))
            throw new Exception("You must be logged in to update rentals.");

        await _rentalService.CompleteRentalAsync(rental.Id, token);

        await Shell.Current.DisplayAlert("Updated", "Rental completed.", "OK");
    }
    catch (Exception ex)
    {
        ErrorMessage = ex.Message;
    }
    finally
    {
        IsBusy = false;
        await LoadRentalsAsync();
    }
}
}