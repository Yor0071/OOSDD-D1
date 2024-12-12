using Microsoft.Maui.Controls;
using Database.Types;
using Database;
using CommunityToolkit.Maui.Views;

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
            if (string.IsNullOrWhiteSpace(firstNameEntry.Text) ||
                string.IsNullOrWhiteSpace(lastNameEntry.Text) ||
                string.IsNullOrWhiteSpace(phoneEntry.Text) ||
                string.IsNullOrWhiteSpace(emailEntry.Text) ||
                string.IsNullOrWhiteSpace(totalCampersEntry.Text) ||
                arrivalDatePicker.Date == null ||
                departureDatePicker.Date == null)
            {
                errorLabel.Text = "Alle verplichte velden moeten worden ingevuld.";
                errorLabel.IsVisible = true;
            }
            else if (!emailEntry.Text.Contains("@") || emailEntry.Text.Length < 5)
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
                var specialNotes = specialNotesEditor.Text;

                // Verkrijg de geselecteerde data
                int campingSpot = _spotDetails.Id;
                DateTime fromDate = arrivalDatePicker.Date;
                DateTime toDate = departureDatePicker.Date;

                try
                {
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
                                               $"👨‍👩‍👧‍👦 Aantal kampeerders: {totalCampers}\n" +
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
        }
    }
}
