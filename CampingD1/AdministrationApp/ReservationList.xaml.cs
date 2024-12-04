using Database.Types;
using Database;

namespace AdministrationApp;

public partial class ReservationList : ContentPage
{
    private Reservation _selectedReservation;
    public ReservationList()
    {
        InitializeComponent();
        LoadReservationsAsync();
        ReservationsCollectionView.ItemsSource = new List<Reservation>();
    }

    private async Task LoadReservationsAsync()
    {
        try
        {
            // Simulate database connection or initialization if needed
            await Task.Run(() => App.databaseHandler.EnsureConnection()); // Ensure the database connection is established

            // Fetch reservations
            var reservations = await Task.Run(() => App.Database.SelectReservations());

            // Bind the data
            ReservationsCollectionView.ItemsSource = reservations;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load reservations: {ex.Message}", "OK");
        }
    }

    private void OnReservationSelected(Object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {
            _selectedReservation = (Reservation)e.CurrentSelection.FirstOrDefault();
            EditButton.IsEnabled = true;
        }
        else
        {
            _selectedReservation = null;
            EditButton.IsEnabled = false;
        }
    }

    private async void OnEditButtonClicked(object sender, EventArgs e)
    {
        if (_selectedReservation != null)
        {
            await Navigation.PushAsync(new EditReservation(_selectedReservation));
        }
    }
}
