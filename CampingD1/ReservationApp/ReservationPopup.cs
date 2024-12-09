using CommunityToolkit.Maui.Views;
using Database.Types;
using Microsoft.Maui.Controls;

namespace ReservationApp
{
    public class ReservationPopup : Popup
    {
        private CampingSpot _campingSpot;
        private Button _reserveButton;

        public ReservationPopup(CampingSpot campingSpot)
        {
            _campingSpot = campingSpot;

            // Show camping spot details
            var descriptionLabel = new Label { Text = $"Spot: {_campingSpot.Description}", FontSize = 18 };
            var powerLabel = new Label { Text = $"Power: {_campingSpot.Power}", FontSize = 16 };
            var wifiLabel = new Label { Text = $"WiFi: {_campingSpot.Wifi}", FontSize = 16 };
            var waterLabel = new Label { Text = $"Water: {_campingSpot.Water}", FontSize = 16 };
            var maxPersonsLabel = new Label { Text = $"Max persons: {_campingSpot.MaxPersons}", FontSize = 16 };

            // Reserve button
            _reserveButton = new Button { Text = "Reserve Now", FontSize = 18 };

            // Arrange UI elements
            var layout = new StackLayout
            {
                Padding = 20,
                Spacing = 15,
                Children = {
                    descriptionLabel,
                    powerLabel,
                    wifiLabel,
                    waterLabel,
                    maxPersonsLabel,
                    _reserveButton
                }
            };

            // Add click handler to open ReservationPage
            _reserveButton.Clicked += async (sender, e) =>
            {
                // Navigate to the ReservationPage
                var reservationPage = new ReservationPage(_campingSpot);
                await Application.Current.MainPage.Navigation.PushAsync(reservationPage);

                // Close the popup after navigation
                this.Close();
            };

            Content = layout;
        }
    }
}
