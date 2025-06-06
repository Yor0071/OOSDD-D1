using Database.Types;
using Database;
using AdministrationApp;

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

    private async Task LoadReservationsAsync(ReservationFilter filter = null)
    {
        try
        {
            // Simulate database connection or initialization if needed
            await Task.Run(() => App.databaseHandler.EnsureConnection()); // Ensure the database connection is established

            List<Reservation> reservations;
            if (filter == null)
            {
                filter = new ReservationFilter();
            }

            reservations = await Task.Run(() => App.Database.SelectFilteredReservations(filter.SpotNameFilter, filter.NameFilter, filter.EmailFilter, filter.FromDateFilter, filter.ToDateFilter));

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

    private async void OnSearchButtonPressed(object sender, EventArgs e)
    {
        SearchBar searchBar = (SearchBar)sender;
        string searchText = searchBar.Text?.Trim();
        var filter = new ReservationFilter();

        if (!string.IsNullOrEmpty(searchText))
        {
            var inputs = searchText.Split(' ');

            foreach (var input in inputs)
            {
                // Regex for 1 uppercase letter followed by 1 digit, such as 'B1' or 'I8'
                if (System.Text.RegularExpressions.Regex.IsMatch(input, @"^[A-Z]\d$"))
                {
                    filter.SpotNameFilter = input;
                }
                else if (input.Contains("@"))
                {
                    filter.EmailFilter = input;
                }
                else
                {
                    // Anders interpreteer het als naam
                    filter.NameFilter = input;
                }
            }
        }
        await LoadReservationsAsync(filter);
    }

    private async void OnArrivalButtonClicked(object sender, EventArgs e)
    {
        var filter = new ReservationFilter
        {
            FromDateFilter = ArrivalDatePicker.Date.ToString("yyyy-MM-dd"),
            ToDateFilter = DepartureDatePicker.Date.ToString("yyyy-MM-dd")
        };
        await LoadReservationsAsync(filter);
    }

    private async void OnResetFilterClicked(object sender, EventArgs e)
    {
        SearchBar.Text = string.Empty;
        ArrivalDatePicker.Date = DateTime.Today;
        DepartureDatePicker.Date = DateTime.Today;

        await LoadReservationsAsync();
    }
}
