using Microsoft.Maui.Controls;
using Database.Types;
using Database;

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

            // Controleer of alle verplichte velden zijn ingevuld
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
                // Verkrijg de invoer van de gebruiker
                var firstName = firstNameEntry.Text;
                var lastName = lastNameEntry.Text;
                var phone = phoneEntry.Text;
                var email = emailEntry.Text;
                var totalCampers = int.TryParse(totalCampersEntry.Text, out int parsedTotalCampers) ? parsedTotalCampers : 0;
                var under12 = int.TryParse(under12Entry.Text, out int parsedUnder12) ? parsedUnder12 : 0;
                var specialNotes = specialNotesEditor.Text;

                // Verkrijg de geselecteerde data
                int campingSpot = _spotDetails.Id;
                DateTime fromDate = arrivalDatePicker.Date;
                DateTime toDate = departureDatePicker.Date;

                // SQL-query om de reservering direct toe te voegen
                string query = @"
            INSERT INTO reservations (firstname, lastname, camping_spot, `from`, `to`, phone, email) 
            VALUES (@firstname, @lastname, @campingSpot, @fromDate, @toDate, @phone, @email);
        ";

                // Parameters voor de SQL-query
                var parameters = new Dictionary<string, object>
        {
            { "@firstname", firstName ?? string.Empty },
            { "@lastname", lastName ?? string.Empty },
            { "@campingSpot", campingSpot },
            { "@fromDate", fromDate.ToString("yyyy-MM-dd") }, // Verzeker dat de datum in de juiste indeling is
            { "@toDate", toDate.ToString("yyyy-MM-dd") }, // Verzeker dat de datum in de juiste indeling is
            { "@phone", phone ?? string.Empty },
            { "@email", email ?? string.Empty }
        };

                try
                {
                    // Voer de query uit via je database handler
                    //App.Database.(query, parameters);

                    // Toon bevestiging
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
                catch (Exception ex)
                {
                    // Toon een foutmelding als de query faalt
                    errorLabel.Text = $"Fout bij het opslaan van de reservering: {ex.Message}";
                    errorLabel.IsVisible = true;
                }
            }
        }


    }
}
