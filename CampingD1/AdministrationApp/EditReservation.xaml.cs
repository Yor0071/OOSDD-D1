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
        FirstNameEntry.Text = _reservation.FirstName;
        LastNameEntry.Text = _reservation.LastName;
        PlaceNumberEntry.Text = _reservation.PlaceNumber.ToString();
        ArrivalDatePicker.Date = _reservation.Arrival;
        DepartDatePicker.Date = _reservation.Depart;
        PhoneNumberEntry.Text = _reservation.PhoneNumber;
        EmailEntry.Text = _reservation.Email;
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
