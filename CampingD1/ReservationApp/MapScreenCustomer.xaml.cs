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
        LoadCampingMap();

        // Stel de standaardwaarden voor de date pickers in
        arrivalDatePicker.Date = DateTime.Today;
        departureDatePicker.Date = DateTime.Today.AddDays(1);

    }

    private async void LoadCampingMap()
    {
        try
        {
            // Zorg ervoor dat de database verbinding is gemaakt
            await Task.Run(() => App.databaseHandler.EnsureConnection());

            // Haal de kaarten op
            var maps = await Task.Run(() => App.Database.SelectCampingMaps());

            foreach (var map in maps)
            {
                if (map.isPrimary == true)
                {
                    RenderCampingMap(map);
                    return;
                }
            }

            await DisplayAlert("Oeps", "Er kan geen kaart gevonden worden", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load reservations: {ex.Message}", "OK");
        }
    }

    public void RenderCampingMap(CampingMap campingMap)
    {
        // Verwijder bestaande cirkels van het canvas
        Canvas.Children.Clear();
        circleMap.Clear();

        // Voeg nieuwe cirkels toe op basis van de kaart
        foreach (var circle in campingMap.cirles)
        {
            AddCircle(circle.id, circle.coordinateX, circle.coordinateY);
        }
    }

    private void AddCircle(int id, double x, double y)
    {
        // Maak een nieuwe cirkel
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
        // Haal details van de geselecteerde cirkel op
        var mapCircle = App.Database.SelectMapCircleById(circleId);

        // Haal de campingplek op die bij de cirkel hoort
        CampingSpot campingSpot = App.Database.SelectCampingSpotById(mapCircle.Value.CampingSpotId);  // Toegang via .Value

        // Maak en toon de ReservationPopup met de details van de campingplek
        var popup = new ReservationPopup(campingSpot);
        await this.ShowPopupAsync(popup);
    }

    private async void OnReservationNumberClicked(object sender, EventArgs e)
    {
        var popup = new ReservationNumberPopup();
        var result = await this.ShowPopupAsync(popup);

        if (result is string reservationNumber)
        {
            var reservations = App.Database.SelectReservations();
            var reservation = reservations.FirstOrDefault(r => r.Id.ToString() == reservationNumber);

            if (reservation != null)
            {
                await DisplayAlert("Ingevoerde Reservering",
                    $"Reserveringsnummer: {reservation.Id}\n" +
                    $"Naam: {reservation.FirstName} {reservation.LastName}\n" +
                    $"Camping Spot: {reservation.PlaceNumber}\n" +
                    $"Van: {reservation.Arrival.ToShortDateString()} Tot: {reservation.Depart.ToShortDateString()}\n" +
                    $"Telefoon: {reservation.PhoneNumber}\n" +
                    $"Email: {reservation.Email}", "OK");

                // Verander de kleur van de cirkel die overeenkomt met de campingplek
                var targetCircle = circleMap.FirstOrDefault(c => c.Value == reservation.PlaceNumber).Key;

                if (targetCircle != null)
                {
                    targetCircle.BackgroundColor = Colors.White;
                }
            }
            else
            {
                await DisplayAlert("Fout", "Reserveringsnummer komt niet overeen met een bestaande reservering.", "OK");
            }
        }
    }
}
