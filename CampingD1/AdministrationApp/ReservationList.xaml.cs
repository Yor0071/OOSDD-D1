using Database.Types;
using Database;
using System.Xml;
using System.Globalization;

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

    private async Task LoadReservationsAsync(string nameFilter = null, int? spotFilter = null, string emailFilter = null, DateTime? fromDateFilter = null)
    {
        try
        {
            // Simulate database connection or initialization if needed
            await Task.Run(() => App.databaseHandler.EnsureConnection()); // Ensure the database connection is established

            List<Reservation> reservations;

            reservations = await Task.Run(() => App.Database.SelectFilteredReservations(nameFilter, spotFilter, emailFilter, fromDateFilter));

            ReservationsCollectionView.ItemsSource = reservations;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load reservations: {ex.Message}", "OK");
        }
    }

    private async void OnSearchButtonPressed(object sender, EventArgs e)
    {
        SearchBar searchBar = (SearchBar)sender;
        string searchText = searchBar.Text?.Trim();
        string nameFilter = null;
        int? spotFilter = null;
        string emailFilter = null;
        DateTime? fromDateFilter = null;

        if (!string.IsNullOrEmpty(searchText))
        {
            var inputs = searchText.Split(' ');

            foreach (var input in inputs)
            {
                if (int.TryParse(input, out int campingSpot))
                {
                    // Als het een getal is, interpreteer het als campingplek
                    spotFilter = campingSpot;
                }
                else if (input.Contains("@"))
                {
                    emailFilter = input;
                }
                else if (DateTime.TryParseExact(input, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate)) 
                { 
                    fromDateFilter = parsedDate;
                }
                else
                {
                    // Anders interpreteer het als naam
                    nameFilter = input;
                }
            }
        }
        await LoadReservationsAsync(nameFilter, spotFilter, emailFilter, fromDateFilter);
    }

}
