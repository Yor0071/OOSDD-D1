using Database.Types;
using Database;
using System.Xml;
using System.Globalization;

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
