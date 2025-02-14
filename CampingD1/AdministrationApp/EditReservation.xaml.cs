using Database.Types;

namespace AdministrationApp;

public partial class EditReservation : ContentPage
{
    private Reservation _reservation;

    public EditReservation(Reservation reservation)
    {
        InitializeComponent();
        _reservation = reservation;

        // Populate labels with existing data
        SpotNameLabel.Text = _reservation.SpotName;
        FirstNameEntry.Text = _reservation.FirstName;
        LastNameEntry.Text = _reservation.LastName;
        ArrivalDatePicker.Date = _reservation.Arrival;
        DepartDatePicker.Date = _reservation.Depart;
        PhoneNumberEntry.Text = _reservation.PhoneNumber;
        EmailEntry.Text = _reservation.Email;

        // Populate the Picker with translated statuses and select the current status
        StatusPicker.ItemsSource = Reservation.StatusTranslations.Values.ToList();
        StatusPicker.SelectedItem = _reservation.TranslatedStatus;
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        _reservation.FirstName = FirstNameEntry.Text;
        _reservation.LastName = LastNameEntry.Text;
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
            await DisplayAlert("Succes", "Reservering succesvol bewerkt!", "OK");
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
