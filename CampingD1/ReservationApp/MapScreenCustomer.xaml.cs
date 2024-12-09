using System.Collections.Generic;
using Microsoft.Maui.Layouts;
using Database.types;
using CommunityToolkit.Maui.Views;
using ReservationApp;
using Database;
using Database.Types;

namespace ReservationApp;

public partial class MapScreenCustomer : ContentPage
{
    private Dictionary<Frame, int> circleMap = new(); // Maakt een map van cirkels naar IDs

    public MapScreenCustomer()
    {
        InitializeComponent();
        LoadMapsAsync();
    }

    private async void OnSearchAvailabilityClicked(object sender, EventArgs e)
    {
        // Simuleer beschikbaarheidscontrole
        bool isReserveNowClicked = await DisplayAlert(
            "Beschikbaarheid",
            "De plek is beschikbaar",
            "Reserveer nu",
            "Ga terug"
        );

        // Actie na keuze van de gebruiker
        if (isReserveNowClicked)
        {
            await DisplayAlert("Reservering", "Je hebt de plek gereserveerd!", "Ok");
        }
    }

    private async void OnReservationNumberClicked(object sender, EventArgs e)
    {
        // Show popup to input the reservation number
        var popup = new ReservationNumberPopup();
        var result = await this.ShowPopupAsync(popup);

        if (result is string reservationNumber)
        {
            // Query the database to get all reservations
            var reservations = App.Database.SelectReservations();

            // Find the reservation that matches the input reservation number
            var reservation = reservations.FirstOrDefault(r => r.Id.ToString() == reservationNumber);

            if (reservation != null)
            {
                // Reservation found, show details
                await DisplayAlert("Ingevoerde Reservering",
                    $"Reserveringsnummer: {reservation.Id}\n" +
                    $"Naam: {reservation.FirstName} {reservation.LastName}\n" +
                    $"Camping Spot: {reservation.PlaceNumber}\n" +
                    $"Van: {reservation.Arrival.ToShortDateString()} Tot: {reservation.Depart.ToShortDateString()}\n" +
                    $"Telefoon: {reservation.PhoneNumber}\n" +
                    $"Email: {reservation.Email}", "OK");
            }
            else
            {
                // Reservation not found, show error
                await DisplayAlert("Fout", "Reserveringsnummer komt niet overeen met een bestaande reservering.", "OK");
            }
        }
    }


    private async Task LoadMapsAsync()
    {
        try
        {
            // Simuleer het ophalen van kaarten en reserveringen
            await Task.Run(() => App.databaseHandler.EnsureConnection());

            var maps = await Task.Run(() => App.Database.SelectCampingMaps());
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

        // Voeg de cirkel toe aan het Canvas en koppel het ID
        circleMap[circle] = id;
        AbsoluteLayout.SetLayoutBounds(circle, new Rect(x, y, circle.WidthRequest, circle.HeightRequest));
        AbsoluteLayout.SetLayoutFlags(circle, AbsoluteLayoutFlags.None);
        Canvas.Children.Add(circle);

        // Voeg een click event toe aan de cirkel
        circle.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => OnCircleClicked(id))
        });
    }

    private async void OnCircleClicked(int circleId)
    {
        // Fetch map circle details
        var mapCircle = App.Database.SelectMapCircleById(circleId);

        // Fetch camping spot associated with the map circle
        CampingSpot campingSpot = App.Database.SelectCampingSpotById(mapCircle.Value.CampingSpotId);  // Toegang via .Value

        // Create and show the ReservationPopup with the camping spot details
        var popup = new ReservationPopup(campingSpot);
        await this.ShowPopupAsync(popup);
    }




}
