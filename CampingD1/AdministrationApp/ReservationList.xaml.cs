using Database.Types;
using Database;

namespace AdministrationApp;

public partial class ReservationList : ContentPage
{
    public ReservationList()
    {
        InitializeComponent();
        LoadReservationsAsync();

        // List<Reservation> testje = App.Database.SelectReservations();

        // Dummy data
        // var reservations = new List<Reservation>
        // {
        //     new Reservation(1, "John", "Doe", 101, DateTime.Now, DateTime.Now.AddDays(7)),
        //     new Reservation(2, "Jane", "Smith", 102, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(6)),
        //     new Reservation(3, "Alice", "Johnson", 103, DateTime.Now, DateTime.Now.AddDays(5)),
        // };

        // Bind the data
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
}
