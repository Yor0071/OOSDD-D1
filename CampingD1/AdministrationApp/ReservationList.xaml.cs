using Database.Types;
using Database;

namespace AdministrationApp;

public partial class ReservationList : ContentPage
{
    private Reservation _selectedReservation;
    public ReservationList()
    {
        InitializeComponent();
        ReservationsCollectionView.ItemsSource = new List<Reservation>();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadReservationsAsync();
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
            DeleteButton.IsEnabled = true;
        }
        else
        {
            _selectedReservation = null;
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
        }
    }

    private async void OnEditButtonClicked(object sender, EventArgs e)
    {
        if (_selectedReservation != null)
        {
            await Navigation.PushAsync(new EditReservation(_selectedReservation));
        }
    }

    private async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        if (_selectedReservation != null)
        {
            bool confirmDelete = await DisplayAlert("Bevestigen", "Weet u zeker dat u deze reservering wilt verwijderen?", "Ja", "Nee");

            if (confirmDelete)
            {
                try
                {
                    // Delete the reservation from the database
                    await Task.Run(() => App.Database.DeleteReservation(_selectedReservation.Id));

                    // Refresh the reservations list
                    await LoadReservationsAsync();

                    // Disable buttons after removal
                    EditButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Verwijderen van reservering mislukt: {ex.Message}", "OK");
                }
            }
        }
    }

}
