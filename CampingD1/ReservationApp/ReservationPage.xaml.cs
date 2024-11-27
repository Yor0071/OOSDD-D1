using Database.Types;
using Microsoft.Maui.Controls;
namespace ReservationApp
{
    public partial class ReservationPage : ContentPage
    {
       
        public ReservationPage()
        {
            InitializeComponent();

            // Stel de minimumdatum voor aankomstdatum in (vandaag)
            arrivalDatePicker.MinimumDate = DateTime.Today;

            // Stel de minimumdatum voor vertrekdatum in (morgen)
            departureDatePicker.MinimumDate = DateTime.Today.AddDays(1);

            // Haal CircleId op uit de BindingContext (indien aanwezig)
            if (BindingContext is IDictionary<string, object> context && context.TryGetValue("CircleId", out var circleId))
            {
                Title = $"Reservering voor plek {circleId}";
            }
        }

        // Event handler voor de bevestigingsknop
        private async void OnConfirmButtonClicked(object sender, EventArgs e)
        {
            // Reset error label visibility before checking
            errorLabel.IsVisible = false;

            // Check if all required fields are filled
            if (string.IsNullOrWhiteSpace(firstNameEntry.Text) ||
             string.IsNullOrWhiteSpace(lastNameEntry.Text) ||
             string.IsNullOrWhiteSpace(phoneEntry.Text) ||
             string.IsNullOrWhiteSpace(emailEntry.Text) ||
             string.IsNullOrWhiteSpace(totalCampersEntry.Text) ||
             string.IsNullOrWhiteSpace(under12Entry.Text) ||
             arrivalDatePicker.Date == null ||
             departureDatePicker.Date == null)
            {
                // Show error message
                errorLabel.Text = "Alle verplichte velden moeten worden ingevuld.";
                errorLabel.IsVisible = true;
            }
            else if (!emailEntry.Text.Contains("@"))
            {
                // Show email-specific error message
                errorLabel.Text = "Controleer uw e-mailadres.";
                errorLabel.IsVisible = true;
            }
            else
            {
                // Haal de ingevulde gegevens op
                var firstName = firstNameEntry.Text;
                var middleName = middleNameEntry.Text;
                var lastName = lastNameEntry.Text;
                var phone = phoneEntry.Text;
                var email = emailEntry.Text;
                var totalCampers = int.TryParse(totalCampersEntry.Text, out int parsedTotalCampers) ? parsedTotalCampers : 0;
                var under12 = int.TryParse(under12Entry.Text, out int parsedUnder12) ? parsedUnder12 : 0;
                var specialNotes = specialNotesEditor.Text;

                // You may also want to capture the camping spot and date range
                int campingSpot = 1; // Example default value, replace with your actual value
                DateTime fromDate = arrivalDatePicker.Date; // Use selected arrival date
                DateTime toDate = departureDatePicker.Date; // Use selected departure date

                // Create the Reservation object
                Reservation reservation = new Reservation(
                    0, // ID: Assuming 0 because it's a new reservation
                    firstName,
                    lastName,
                    campingSpot,
                    fromDate,
                    toDate
                );

                // Save the reservation to the database
                App.Database.SaveReservation(reservation);

                // Show confirmation message
                await DisplayAlert("Bevestiging",
                                   "Reservering opgeslagen!\n" +
                                   $"Naam: {firstName} {middleName} {lastName}\n" +
                                   $"Telefoon: {phone}\n" +
                                   $"Email: {email}\n" +
                                   $"Aantal kampeerders: {totalCampers}\n" +
                                   $"Onder 12: {under12}\n" +
                                   $"Bijzonderheden: {specialNotes}",
                                   "OK");

                // Hide error label after successful operation
                errorLabel.IsVisible = false;
            }
        }
    }
}
