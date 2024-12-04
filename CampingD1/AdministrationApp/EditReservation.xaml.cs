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
        FirstNameLabel.Text = _reservation.FirstName;
        LastNameLabel.Text = _reservation.LastName;
        PlaceNumberLabel.Text = _reservation.PlaceNumber.ToString();
        ArrivalLabel.Text = _reservation.Arrival.ToString("MM/dd/yyyy");
        DepartLabel.Text = _reservation.Depart.ToString("MM/dd/yyyy");
        PhoneNumberLabel.Text = _reservation.PhoneNumber;
        EmailLabel.Text = _reservation.Email;
    }
}
