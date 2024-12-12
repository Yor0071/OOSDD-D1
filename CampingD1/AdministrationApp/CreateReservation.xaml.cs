using Database.Types;

namespace AdministrationApp;

public partial class CreateReservation : ContentPage
{
    private Reservation _reservation;
    public CreateReservation()
	{
		InitializeComponent();
        StatusPicker.ItemsSource = Reservation.StatusTranslations.Values.ToList();
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        _reservation.FirstName = FirstNameEntry.Text;
        _reservation.LastName = LastNameEntry.Text;
        _reservation.PlaceNumber = int.Parse(PlaceNumberEntry.Text);
        _reservation.Arrival = ArrivalDatePicker.Date;
        _reservation.Depart = DepartDatePicker.Date;
        _reservation.PhoneNumber = PhoneNumberEntry.Text;
        _reservation.Email = EmailEntry.Text;

        // Find the ReservationStatus enum value corresponding to the translated status string selected by the user
        var selectedTranslatedStatus = StatusPicker.SelectedItem.ToString();
        _reservation.Status = Reservation.StatusTranslations
                                         .FirstOrDefault(kvp => kvp.Value == selectedTranslatedStatus)
                                         .Key;

        try
        {
            App.Database.UpdateReservation(_reservation);
            await DisplayAlert("Succes", "Reservering succesvol geüpdate!", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to save reservation: {ex.Message}", "OK");
        }
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}