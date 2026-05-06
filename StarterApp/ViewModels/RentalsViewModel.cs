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
}