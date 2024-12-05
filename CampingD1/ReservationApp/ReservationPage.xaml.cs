using Microsoft.Maui.Controls;
using Database.Types;

namespace ReservationApp
{
    public partial class ReservationPage : ContentPage
    {
        private CampingSpot _spotDetails;

        public ReservationPage(CampingSpot spotDetails)
        {
            InitializeComponent();

            _spotDetails = spotDetails;

            arrivalDatePicker.MinimumDate = DateTime.Today;
            departureDatePicker.MinimumDate = DateTime.Today.AddDays(1);

            Title = $"Reservering voor plek {_spotDetails.Description}";
        }

        private async void OnConfirmButtonClicked(object sender, EventArgs e)
        {
            errorLabel.IsVisible = false;

            if (string.IsNullOrWhiteSpace(firstNameEntry.Text) ||
                string.IsNullOrWhiteSpace(lastNameEntry.Text) ||
                string.IsNullOrWhiteSpace(phoneEntry.Text) ||
                string.IsNullOrWhiteSpace(emailEntry.Text) ||
                string.IsNullOrWhiteSpace(totalCampersEntry.Text) ||
                string.IsNullOrWhiteSpace(under12Entry.Text) ||
                arrivalDatePicker.Date == null ||
                departureDatePicker.Date == null)
            {
                errorLabel.Text = "Alle verplichte velden moeten worden ingevuld.";
                errorLabel.IsVisible = true;
            }
            else if (!emailEntry.Text.Contains("@"))
            {
                errorLabel.Text = "Controleer uw e-mailadres.";
                errorLabel.IsVisible = true;
            }
            else
            {
                var firstName = firstNameEntry.Text;
                var lastName = lastNameEntry.Text;
                var phone = phoneEntry.Text;
                var email = emailEntry.Text;
                var totalCampers = int.TryParse(totalCampersEntry.Text, out int parsedTotalCampers) ? parsedTotalCampers : 0;
                var under12 = int.TryParse(under12Entry.Text, out int parsedUnder12) ? parsedUnder12 : 0;
                var specialNotes = specialNotesEditor.Text;

                int campingSpot = _spotDetails.Id;
                DateTime fromDate = arrivalDatePicker.Date;
                DateTime toDate = departureDatePicker.Date;

                Reservation reservation = new Reservation(
                    0,               // ID: Aangenomen als 0 voor nieuwe reservering
                    firstName,       // Voornaam
                    lastName,        // Achternaam
                    campingSpot,     // Campingplek ID
                    fromDate,        // Aankomstdatum
                    toDate,          // Vertrekdatum
                    phone,           // Telefoonnummer
                    email            // E-mailadres
                );

                App.Database.AddReservation(reservation);

                await DisplayAlert("Bevestiging",
                    "Reservering opgeslagen!\n" +
                    $"Naam: {firstName} {lastName}\n" +
                    $"Telefoon: {phone}\n" +
                    $"Email: {email}\n" +
                    $"Aantal kampeerders: {totalCampers}\n" +
                    $"Onder 12: {under12}\n" +
                    $"Bijzonderheden: {specialNotes}",
                    "OK");

                errorLabel.IsVisible = false;
            }
        }
    }
}
