using Database.Types;

namespace AdministrationApp;

public partial class CreateReservation : ContentPage
{
    private Dictionary<string, int> campingSpotMap;
    public CreateReservation()
    {
        InitializeComponent();

        // Populate the Picker with translated statuses
        //StatusPicker.ItemsSource = Reservation.StatusTranslations.Values.ToList();

        LoadAvailableCampingSpots();
    }

    private void LoadAvailableCampingSpots()
    {
        try
        {
            var campingSpots = App.Database.GetAllCampingSpots();
            campingSpotMap = campingSpots.ToDictionary(spot => spot.Name, spot => spot.Id);
            PlacePicker.ItemsSource = campingSpotMap.Keys.ToList(); // Set names as Picker items
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Failed to load camping spots: {ex.Message}", "OK");
        }
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(FirstNameEntry.Text) || string.IsNullOrWhiteSpace(LastNameEntry.Text))
            {
                await DisplayAlert("Error", "Voornaam en achternaam zijn verplicht.", "OK");
                return;
            }

            if (PlacePicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Selecteer een campingplek.", "OK");
                return;
            }

            // Get the selected camping spot name
            string selectedCampingSpotName = PlacePicker.SelectedItem.ToString();

            // Retrieve the corresponding ID
            if (!campingSpotMap.TryGetValue(selectedCampingSpotName, out int campingSpotId))
            {
                await DisplayAlert("Error", "Kon de geselecteerde campingplek niet vinden.", "OK");
                return;
            }

            // Call the AddReservation method
            App.Database.AddReservation(
                firstName: FirstNameEntry.Text,
                lastName: LastNameEntry.Text,
                campingSpot: campingSpotId, // Pass the spot ID
                fromDate: ArrivalDatePicker.Date,
                toDate: DepartDatePicker.Date,
                phone: PhoneNumberEntry.Text,
                email: EmailEntry.Text
            );

            await DisplayAlert("Succes", "Reservering succesvol toegevoegd!", "OK");
            await Navigation.PopAsync(); // Go back to the previous page
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Er is een fout opgetreden: {ex.Message}", "OK");
        }
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
