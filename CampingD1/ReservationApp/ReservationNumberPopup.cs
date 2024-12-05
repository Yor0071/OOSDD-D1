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
                Padding = 15,
                BackgroundColor = Color.FromArgb("#FAFAFA"),
                CornerRadius = 16,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = 200,
                WidthRequest = 320,
                HasShadow = true
            };

            // Grid voor inhoud
            var contentGrid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(40) }, // Header
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
                Text = "Vul uw reserveringsnummer in",
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Start,
                TextColor = Colors.Black
            };

            var closeButton = new Button
            {
                Text = "✕",
                BackgroundColor = Colors.Transparent,
                TextColor = Colors.Red,
                FontSize = 15,
                Padding = new Thickness(1),
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };
            closeButton.Clicked += (s, e) => Close();

            headerStack.Children.Add(titleLabel); // Titel
            headerStack.Children.Add(closeButton); // Sluitknop
            Grid.SetColumn(titleLabel, 0);
            Grid.SetColumn(closeButton, 1);

            // Content: Invoer en Bevestigingsknop
            var contentStack = new VerticalStackLayout
            {
                Padding = 10,
                Spacing = 10,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            // Frame voor Entry met afgeronde hoeken
            var reservationEntryFrame = new Frame
            {
                Padding = 0,
                BackgroundColor = Color.FromArgb("#EFEFEF"),
                CornerRadius = 8,
                HasShadow = false, // Geen schaduw voor een minimalistische look
                BorderColor = Colors.LightGray
            };

            var reservationEntry = new Entry
            {
                Placeholder = "Uw reserveringsnummer",
                Keyboard = Keyboard.Numeric,
                HeightRequest = 40,
                TextColor = Colors.Black,
                PlaceholderColor = Colors.DarkGray,
                BackgroundColor = Colors.Transparent // Zorg dat de Entry zelf geen achtergrond heeft
            };

            reservationEntryFrame.Content = reservationEntry; // Plaats de Entry binnen het Frame

            var confirmButton = new Button
            {
                Text = "Bevestigen",
                BackgroundColor = Color.FromArgb("#4CAF50"),
                TextColor = Colors.White,
                HeightRequest = 45,
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
                    await Application.Current.MainPage.DisplayAlert("Fout", "Vul een geldig reserveringsnummer in.", "OK");
                }
            };

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
