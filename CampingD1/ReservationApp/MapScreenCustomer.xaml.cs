using System.Collections.Generic;
using Microsoft.Maui.Layouts;
using Database.types;
using CommunityToolkit.Maui.Views;
using ReservationApp;

namespace ReservationApp;

public partial class MapScreenCustomer : ContentPage
{
    private Dictionary<Frame, int> circleMap = new(); // Maps circles to IDs

    public MapScreenCustomer()
    {
        InitializeComponent();
        LoadMapsAsync();
    }
     private async void OnSearchAvailabilityClicked(object sender, EventArgs e)
        {
            // Simulate a check for availability
            bool isReserveNowClicked = await DisplayAlert(
                "Beschikbaarheid",
                "De plek is beschikbaar",
                "Reserveer nu",
                "Ga terug"
            );

            // Action after user selects option
            if (isReserveNowClicked)
            {
                // Handle reservation logic, e.g., navigate to a reservation page
                await DisplayAlert("Reservering", "Je hebt de plek gereserveerd!", "Ok");
            }
        }
    private async void OnReservationNumberClicked(object sender, EventArgs e)
    {
        var popup = new ReservationNumberPopup();
        var result = await this.ShowPopupAsync(popup);

        if (result is string reservationNumber)
        {
            await DisplayAlert("Ingevoerde Reservering", $"Reserveringsnummer: {reservationNumber}", "OK");
            // Voeg hier logica toe om reserveringsinformatie te verwerken
        }
    }
    private async Task LoadMapsAsync()
    {
        try
        {
            // Simulate database connection or initialization if needed
            await Task.Run(() => App.databaseHandler.EnsureConnection()); // Ensure the database connection is established

            // Fetch reservations
            var maps = await Task.Run(() => App.Database.SelectCampingMaps());

            // Bind the data
            // ReservationsCollectionView.ItemsSource = reservations;

            Console.WriteLine(maps.First().cirles.Count);

            RenderCampingMap(maps.First());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load reservations: {ex.Message}", "OK");
        }
    }

    public void RenderCampingMap(CampingMap campingMap)
    {
        Canvas.Children.Clear();
        circleMap.Clear();

        foreach (var circle in campingMap.cirles)
        {
            AddCircle(circle.id, circle.coordinateX, circle.coordinateY);
        }
    }
    private async void OnCircleTapped(int circleId)
    {
        // Toon een popup of alert met het ID van de aangeklikte cirkel
        await DisplayAlert("Cirkel Geklikt", $"Je hebt op de cirkel met ID {circleId} geklikt.", "OK");

        // Hier kun je verdere logica toevoegen om bijvoorbeeld details van de reservering te tonen
    }

    private void AddCircle(int id, double x, double y)
    {
        var circle = new Frame
        {
            WidthRequest = 50,
            HeightRequest = 50,
            CornerRadius = 25,
            BackgroundColor = Colors.Red,
            HasShadow = false
        };

        // Maak een tap gesture recognizer voor het klikken op de cirkel
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (s, e) => OnCircleTapped(id);

        // Voeg de gesture toe aan de circle
        circle.GestureRecognizers.Add(tapGesture);

        // Voeg de cirkel toe aan de Canvas en koppel het ID aan de cirkel
        circleMap[circle] = id;
        AbsoluteLayout.SetLayoutBounds(circle, new Rect(x, y, circle.WidthRequest, circle.HeightRequest));
        AbsoluteLayout.SetLayoutFlags(circle, AbsoluteLayoutFlags.None);
        Canvas.Children.Add(circle);
    }


}
