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

        // Stel de achtergrondafbeelding in (optioneel)
        if (!string.IsNullOrEmpty(campingMap.backgroundImage))
        {
            try
            {
                var imageBytes = Convert.FromBase64String(campingMap.backgroundImage);
                var imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                BackgroundImage.Source = imageSource;
            }
            catch (Exception)
            {
                BackgroundImage.Source = null;
            }
        }
        else
        {
            BackgroundImage.Source = null;
        }

        // Voeg de cirkels toe op basis van de campingMap
        foreach (var circle in campingMap.cirles)
        {
            AddCircle(circle.id, circle.coordinateX, circle.coordinateY, circle.CampingSpotId);
        }
    }

    private void AddCircle(int id, double x, double y, int campingSpotId)
    {
        // Haal de campingplek op via het ID
        var campingSpot = App.Database.SelectCampingSpotById(campingSpotId);

        // Stel de kleur van de cirkel in op basis van beschikbaarheid
        var circleColor = campingSpot?.Available == true ? Colors.Green : Colors.Red;

        // Maak een nieuwe cirkel
        var circle = new Frame
        {
            WidthRequest = 50,
            HeightRequest = 50,
            CornerRadius = 25,
            BackgroundColor = circleColor,
            HasShadow = false
        };

        // Voeg de cirkel toe aan de kaart en koppel het ID
        circleMap[circle] = id;
        AbsoluteLayout.SetLayoutBounds(circle, new Rect(x, y, circle.WidthRequest, circle.HeightRequest));
        AbsoluteLayout.SetLayoutFlags(circle, AbsoluteLayoutFlags.None);
        Canvas.Children.Add(circle);

        // Voeg een klikgebeurtenis toe aan de cirkel
        circle.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => OnCircleClicked(id, campingSpotId))  // Verzend beide IDs
        });
    }

    // Dit blijft ongewijzigd
    private async void OnCircleClicked(int circleId, int campingSpotId)
    {
        // Haal details van de geselecteerde cirkel op
        var mapCircle = App.Database.SelectMapCircleById(circleId);

        // Haal de campingplek op die bij de cirkel hoort
        CampingSpot campingSpot = App.Database.SelectCampingSpotById(campingSpotId);  // Toegang via .Value

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
