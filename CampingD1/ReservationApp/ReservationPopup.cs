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

            // Toon campingplek details met verbeterde opmaak
            var nameLabel = new Label
            {
                Text = $"Plek: {_campingSpot.SpotName}",
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.Black,
                HorizontalOptions = LayoutOptions.Center
            };

            var descriptionLabel = new Label
            {
                Text = $"Beschrijving: {_campingSpot.Description}",
                FontSize = 16,
                TextColor = Colors.Gray,
                HorizontalOptions = LayoutOptions.Start
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
                Text = $"WiFi: {ConvertBoolToYesNo(_campingSpot.Wifi)}",
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

            // Reserveer knop met betere opmaak
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

            // Sluit knop met opmaak
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

            // Controleer of de campingplek beschikbaar is
            if (!_campingSpot.Available)
            {
                // Zet de reserveer knop uit als de campingplek niet beschikbaar is (rode cirkel)
                _reserveButton.IsEnabled = false;
                _reserveButton.BackgroundColor = Colors.Gray;
            }

            // Voeg klik handler toe om de reservatiepagina te openen
            _reserveButton.Clicked += async (sender, e) =>
            {
                if (_campingSpot.Available) // Zorg ervoor dat de knop alleen klikbaar is als de plek beschikbaar is
                {
                    var reservationPage = new ReservationPage(_campingSpot);
                    await Application.Current.MainPage.Navigation.PushAsync(reservationPage);

                    this.Close();
                }
            };

            // Voeg klik handler toe om de popup te sluiten
            _closeButton.Clicked += (sender, e) =>
            {
                this.Close();
            };

            // Container voor informatie over de campingplek
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
                        nameLabel,
                        descriptionLabel,
                        powerLabel,
                        wifiLabel,
                        waterLabel,
                        maxPersonsLabel
                    }
                }
            };

            // Container voor de knoppen
            var buttonContainer = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 10,
                Children = {
                    _reserveButton,
                    _closeButton
                }
            };

            // Rangschik de UI-elementen
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

            // Voeg border en content toe aan de popup
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
