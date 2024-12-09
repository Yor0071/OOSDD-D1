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

            // Show camping spot details with improved formatting
            var descriptionLabel = new Label
            {
                Text = $"Spot: {_campingSpot.Description}",
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.Black,
                HorizontalOptions = LayoutOptions.Center
            };

            var powerLabel = new Label
            {
                Text = $"Power: {_campingSpot.Power}",
                FontSize = 16,
                TextColor = Colors.Gray,
                HorizontalOptions = LayoutOptions.Start
            };

            var wifiLabel = new Label
            {
                Text = $"WiFi: {_campingSpot.Wifi}",
                FontSize = 16,
                TextColor = Colors.Gray,
                HorizontalOptions = LayoutOptions.Start
            };

            var waterLabel = new Label
            {
                Text = $"Water: {_campingSpot.Water}",
                FontSize = 16,
                TextColor = Colors.Gray,
                HorizontalOptions = LayoutOptions.Start
            };

            var maxPersonsLabel = new Label
            {
                Text = $"Max Persons: {_campingSpot.MaxPersons}",
                FontSize = 16,
                TextColor = Colors.Gray,
                HorizontalOptions = LayoutOptions.Start
            };

            // Reserve button with better styling
            _reserveButton = new Button
            {
                Text = "Reserveer nu!",
                FontSize = 18,
                BackgroundColor = Color.FromArgb("#4CAF50"),
                TextColor = Colors.White,
                CornerRadius = 10,
                HeightRequest = 50,
                HorizontalOptions = LayoutOptions.Fill
            };

            // Card-style container for labels
            var infoContainer = new Frame
            {
                BackgroundColor = Colors.White,
                BorderColor = Colors.LightGray,
                CornerRadius = 15,
                Padding = 15,
                Content = new StackLayout
                {
                    Spacing = 10,
                    Children = {
                        descriptionLabel,
                        powerLabel,
                        wifiLabel,
                        waterLabel,
                        maxPersonsLabel
                    }
                }
            };

            // Arrange UI elements
            var layout = new StackLayout
            {
                Padding = 20,
                Spacing = 20,
                BackgroundColor = Colors.LightGray.WithLuminosity(1),
                Children = {
                    infoContainer,
                    _reserveButton
                }
            };

            // Add click handler to open ReservationPage
            _reserveButton.Clicked += async (sender, e) =>
            {
                var reservationPage = new ReservationPage(_campingSpot);
                await Application.Current.MainPage.Navigation.PushAsync(reservationPage);

                this.Close();
            };

            // Add border and content to the popup
            Content = new Frame
            {
                CornerRadius = 20,
                BackgroundColor = Colors.White,
                BorderColor = Colors.Gray,
                Padding = 0,
                Content = layout
            };
        }
    }
}
