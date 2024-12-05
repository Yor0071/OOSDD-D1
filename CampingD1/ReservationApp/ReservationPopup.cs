using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;

namespace ReservationApp.Views
{
    public class ReservationPopup : Popup
    {
        // Property om het reserveringsnummer door te geven
        public string ReservationId { get; set; }

        public ReservationPopup(int circleId)
        {
            // Stel het reserveringsnummer in
            ReservationId = circleId.ToString();

            // Maak de UI voor de popup
            var stackLayout = new VerticalStackLayout
            {
                Spacing = 20,
                Children =
                {
                    // Popup header
                    new Label
                    {
                        Text = "Reserveringsdetails",
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center
                    },

                    // Toon het reserveringsnummer
                    new Label
                    {
                        Text = $"Reserveringsnummer: {ReservationId}",
                        FontSize = 18,
                        HorizontalOptions = LayoutOptions.Center
                    },

                    // Sluitknop voor de popup
                    new Button
                    {
                        Text = "Sluit",
                        BackgroundColor = Color.FromHex("#4CAF50"),
                        TextColor = Colors.White
                    }
                    .Clicked((sender, e) =>
                    {
                        // Sluit de popup wanneer de knop wordt ingedrukt
                        Close();
                    })
                }
            };

            // Zet de inhoud van de popup
            Content = stackLayout;
        }
    }
}
