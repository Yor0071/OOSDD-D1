using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Views;

namespace ReservationApp
{
    public partial class ReservationNumberPopup : Popup
    {
        public ReservationNumberPopup()
        {
            // Achtergrond instellen
            var backgroundGrid = new Grid
            {
                BackgroundColor = Color.FromArgb("#80000000"), // Semi-transparante zwarte achtergrond
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            // Frame voor popup-inhoud
            var popupFrame = new Frame
            {
                Padding = 20,
                BackgroundColor = Colors.White,
                CornerRadius = 12,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = 300,
                WidthRequest = 350, // Breder voor een nettere weergave
                HasShadow = true
            };

            // Grid voor inhoud
            var contentGrid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(50) }, // Header
                    new RowDefinition { Height = GridLength.Star }     // Inhoud
                }
            };

            // Header: Titel en Sluitknop
            var headerStack = new Grid
            {
                Padding = new Thickness(5, 0),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            var titleLabel = new Label
            {
                Text = "Vul uw reserveringsnummer in:", // Dubbelpunt toegevoegd
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Start,
                TextColor = Colors.Black  // Gebruik Colors.Black voor zwart
            };

            var closeButton = new Button
            {
                Text = "X",
                BackgroundColor = Colors.Transparent,
                TextColor = Colors.Red, // Gebruik Colors.Red voor rood
                FontSize = 18,
                Padding = new Thickness(0),
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start
            };
            closeButton.Clicked += (s, e) => Close();

            // Voeg titel en sluitknop toe aan headerStack
            headerStack.Children.Add(titleLabel); // Titel
            headerStack.Children.Add(closeButton); // Sluitknop
            Grid.SetColumn(titleLabel, 0);
            Grid.SetColumn(closeButton, 1);

            // Content: Invoer en Bevestigingsknop
            var contentStack = new VerticalStackLayout
            {
                Padding = 15,
                Spacing = 15,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            // Entry in een Frame voor afgeronde hoeken
            var reservationEntryFrame = new Frame
            {
                Padding = new Thickness(0),
                BackgroundColor = Colors.White,
                CornerRadius = 8,
                HasShadow = false, // Optioneel, voor een schoner uiterlijk zonder schaduw
                BorderColor = Colors.Gray
            };

            var reservationEntry = new Entry
            {
                Placeholder = "Vul hier het reserveringsnummer in",
                Keyboard = Keyboard.Numeric,
                HeightRequest = 45,
                TextColor = Colors.Black,
                PlaceholderColor = Colors.Gray,
                BackgroundColor = Colors.Transparent // Zorg ervoor dat de achtergrond transparant is, anders overschrijft het de Frame-kleur
            };

            reservationEntryFrame.Content = reservationEntry; // Plaats de Entry binnen de Frame

            var confirmButton = new Button
            {
                Text = "Reservering bekijken",
                BackgroundColor = Color.FromArgb("#4CAF50"),
                TextColor = Colors.White,
                HeightRequest = 50,
                CornerRadius = 8,
                FontAttributes = FontAttributes.Bold
            };
            confirmButton.Clicked += async (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(reservationEntry.Text))
                {
                    Close(reservationEntry.Text); // Retourneer invoerwaarde
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Fout", "Vul aub een geldig reserveringsnummer in.", "OK");
                }
            };

            // Voeg invoerveld en bevestigingsknop toe aan contentStack
            contentStack.Children.Add(reservationEntryFrame);
            contentStack.Children.Add(confirmButton);

            // Voeg header en inhoud toe aan contentGrid
            contentGrid.Add(headerStack, 0, 0);
            contentGrid.Add(contentStack, 0, 1);

            // Voeg contentGrid toe aan popupFrame
            popupFrame.Content = contentGrid;

            // Voeg popupFrame toe aan achtergrondGrid
            backgroundGrid.Children.Add(popupFrame);

            // Stel de content van de popup in
            Content = backgroundGrid;
        }
    }
}
