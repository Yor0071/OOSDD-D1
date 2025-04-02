using Microsoft.Maui.Controls;
using Database.Types;
using Database;
using CommunityToolkit.Maui.Views;
using System.Text.RegularExpressions;

namespace ReservationApp
{
    public partial class ReservationPage : ContentPage
    {
        private CampingSpot _spotDetails;

        public ReservationPage(CampingSpot spotDetails)
        {
            InitializeComponent();

            _spotDetails = spotDetails;

            // Stel het label in met de beschrijving of nummer van de plek
            campingSpotNumberLabel.Text = _spotDetails.Description.ToString();

            arrivalDatePicker.MinimumDate = DateTime.Today;
            departureDatePicker.MinimumDate = DateTime.Today.AddDays(1);

            Title = $"Reservering voor plek {_spotDetails.Description}";
        }

        private async void OnConfirmButtonClicked(object sender, EventArgs e)
        {
            errorLabel.IsVisible = false;

            // Controleer of alle verplichte velden zijn ingevuld
            string firstName = firstNameEntry.Text?.Trim();
            string lastName = lastNameEntry.Text?.Trim();
            string phone = phoneEntry.Text?.Trim();
            string email = emailEntry.Text?.Trim();
            string totalCampersText = totalCampersEntry.Text?.Trim();
            var specialNotes = specialNotesEditor.Text;
            DateTime fromDate = arrivalDatePicker.Date;
            DateTime toDate = departureDatePicker.Date;

            var errors = ValidateReservation(firstName, lastName, phone, email, totalCampersText, fromDate, toDate);

            if (errors.Count > 0)
            {
                errorLabel.Text = string.Join("\n", errors);
                errorLabel.IsVisible = true;
                return;
            }

            try
            {
                int campingSpot = _spotDetails.Id;

                // Voer de query uit via de database handler
                App.Database.AddReservation(
                    firstName, lastName, campingSpot, fromDate, toDate, phone, email
                );

                // Declareer de pop-up eerst
                Popup confirmationPopup = null;

                // Maak een aangepaste bevestigingspop-up
                confirmationPopup = new Popup
                {
                    Content = new Frame
                    {
                        CornerRadius = 25,
                        BackgroundColor = Color.FromArgb("#f9f9f9"),
                        BorderColor = Color.FromArgb("#cccccc"),
                        Padding = 20,
                        Shadow = new Shadow
                        {
                            Offset = new Point(0, 4),
                            Radius = 8,
                            Opacity = 1
                        },
                        Content = new StackLayout
                        {
                            Spacing = 20,
                            Children = {
                                    // Header label
                                    new Label
                                    {
                                        Text = "✅ Bevestiging",
                                        FontSize = 24,
                                        FontAttributes = FontAttributes.Bold,
                                        HorizontalTextAlignment = TextAlignment.Center,
                                        TextColor = Color.FromArgb("#333333")
                                    },
                                    
                                    // Content label
                                    new Label
                                    {
                                        Text = $"Reservering succesvol opgeslagen!\n" +
                                               $"📍 Naam: {firstName} {lastName}\n" +
                                               $"📞 Telefoon: {phone}\n" +
                                               $"📧 Email: {email}\n" +
                                               $"👨‍👩‍👧‍👦 Aantal kampeerders: {totalCampersText}\n" +
                                               $"📝 Bijzonderheden: {specialNotes}",
                                        FontSize = 16,
                                        TextColor = Color.FromArgb("#555555"),
                                        LineHeight = 1.5
                                    },
                                    
                                    // Return to map button
                                    new Button
                                    {
                                        Text = "🔙 Terug naar Kaart",
                                        BackgroundColor = Color.FromArgb("#007BFF"),
                                        TextColor = Colors.White,
                                        CornerRadius = 12,
                                        FontSize = 18,
                                        HeightRequest = 50,
                                        HorizontalOptions = LayoutOptions.FillAndExpand,
                                        Command = new Command(async () =>
                                        {
                                            confirmationPopup.Close(); // Sluit de pop-up
                                            await Application.Current.MainPage.Navigation.PopToRootAsync(); // Ga terug naar MapScreenCustomer
                                        })
                                    }
                                }
                        }
                    },
                    CanBeDismissedByTappingOutsideOfPopup = false // Prevent closing when tapping outside
                };

                // Toon de pop-up
                Application.Current.MainPage.ShowPopup(confirmationPopup);

                errorLabel.IsVisible = false;
            }
            catch (Exception ex)
            {
                // Toon een foutmelding
                errorLabel.Text = $"Er is een fout opgetreden: {ex.Message}";
                errorLabel.IsVisible = true;
            }
        }

        public List<string> ValidateReservation(string firstName, string lastName, string phone, string email, string totalCampersText, DateTime fromDate, DateTime toDate)
        {
            List<string> errors = new List<string>();

            // Basisvalidatie (lege velden)
            if (string.IsNullOrWhiteSpace(firstName)) errors.Add("Voornaam is verplicht.");
            if (string.IsNullOrWhiteSpace(lastName)) errors.Add("Achternaam is verplicht.");
            if (string.IsNullOrWhiteSpace(phone)) errors.Add("Telefoonnummer is verplicht.");
            if (string.IsNullOrWhiteSpace(email)) errors.Add("E-mailadres is verplicht.");
            if (string.IsNullOrWhiteSpace(totalCampersText)) errors.Add("Aantal kampeerders is verplicht.");

            // E-mailvalidatie
            if (!string.IsNullOrWhiteSpace(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                errors.Add("Voer een geldig e-mailadres in.");

            // Telefoonnummer: alleen cijfers (8-15 lang)
            if (!string.IsNullOrWhiteSpace(phone) && !Regex.IsMatch(phone, @"^\d{8,15}$"))
                errors.Add("Telefoonnummer mag alleen cijfers bevatten (8-15 cijfers).");

            // Aankomst- en vertrekdatum controleren
            if (toDate <= fromDate)
                errors.Add("Vertrekdatum moet later zijn dan aankomstdatum.");

            // Aantal kampeerders moet een positief getal zijn
            if (!int.TryParse(totalCampersText, out int totalCampers) || totalCampers <= 0)
                errors.Add("Aantal kampeerders moet een positief getal zijn.");

            return errors;
        }
    }
}
