using Database.Types;

namespace AdministrationApp;

public partial class CreateReservation : ContentPage
{
    public CreateReservation()
    {
        InitializeComponent();

        // Populate the Picker with translated statuses
        StatusPicker.ItemsSource = Reservation.StatusTranslations.Values.ToList();
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

            if (!int.TryParse(PlaceNumberEntry.Text, out int CampingSpot))
            {
                await DisplayAlert("Error", "Pleknummer moet een geldig nummer zijn.", "OK");
                return;
            }

            // Call the working AddReservation method
            App.Database.AddReservation(
                firstName: FirstNameEntry.Text,
                lastName: LastNameEntry.Text,
                campingSpot: CampingSpot,
                fromDate: ArrivalDatePicker.Date,
                toDate: DepartDatePicker.Date,
                phone: PhoneNumberEntry.Text,
                email: EmailEntry.Text
            );

            await DisplayAlert("Succes", "Reservering succesvol toegevoegd!", "OK");
            await Navigation.PopAsync(); // Go back to previous page
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
