using CommunityToolkit.Maui.Views;
using Database.Types;
using Microsoft.Maui.Controls;

namespace ReservationApp
{
    public class ReservationPopup : Popup
    {
        private CampingSpot _campingSpot;
        private Button _reserveButton;
        private Button _closeButton;

        public ReservationPopup(CampingSpot campingSpot)
        {
            _campingSpot = campingSpot;

            // Helper functie om true/false naar "ja"/"nee" te converteren
            string ConvertBoolToYesNo(bool value) => value ? "Ja" : "Nee";

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
                Text = $"Stroom: {ConvertBoolToYesNo(_campingSpot.Power)}",
                FontSize = 16,
                TextColor = Colors.Gray,
                HorizontalOptions = LayoutOptions.Start
            };

            var wifiLabel = new Label
            {
                Text = $"Internet connectie: {ConvertBoolToYesNo(_campingSpot.Wifi)}",
                FontSize = 16,
                TextColor = Colors.Gray,
                HorizontalOptions = LayoutOptions.Start
            };

            var waterLabel = new Label
            {
                Text = $"Water: {ConvertBoolToYesNo(_campingSpot.Water)}",
                FontSize = 16,
                TextColor = Colors.Gray,
                HorizontalOptions = LayoutOptions.Start
            };

            var maxPersonsLabel = new Label
            {
                Text = $"Maximaal aantal personen: {_campingSpot.MaxPersons}",
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
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            // Close button with styling
            _closeButton = new Button
            {
                Text = "Sluiten",
                FontSize = 18,
                BackgroundColor = Colors.Red,
                TextColor = Colors.White,
                CornerRadius = 10,
                HeightRequest = 50,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            // Add click handler to open ReservationPage
            _reserveButton.Clicked += async (sender, e) =>
            {
                var reservationPage = new ReservationPage(_campingSpot);
                await Application.Current.MainPage.Navigation.PushAsync(reservationPage);

                this.Close();
            };

            // Add click handler to close the popup
            _closeButton.Clicked += (sender, e) =>
            {
                this.Close();
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

            // Buttons container
            var buttonContainer = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 10,
                Children = {
                    _reserveButton,
                    _closeButton
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
                    buttonContainer
                }
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
