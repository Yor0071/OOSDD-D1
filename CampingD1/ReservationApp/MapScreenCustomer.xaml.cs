using System.Collections.Generic;
using Microsoft.Maui.Layouts;
using Database.types;
using CommunityToolkit.Maui.Views;
using ReservationApp;
using ReservationApp.Views;

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
        var popup = new ReservationNumberPopup();
        var result = await this.ShowPopupAsync(popup);

        if (result is string reservationNumber)
        {
            await DisplayAlert("Ingevoerde Reservering", $"Reserveringsnummer: {reservationNumber}", "OK");
        }
    }

    private async Task LoadMapsAsync()
    {
        try
        {
            // Simuleer het ophalen van kaarten en reserveringen
            await Task.Run(() => App.databaseHandler.EnsureConnection());

            var maps = await Task.Run(() => App.Database.SelectCampingMaps());

            // RenderCampingMap(maps.First());
            // CampingMap? primaryMap = maps.FirstOrDefault(map => map.isPrimary);

            foreach (var map in maps) {
                if (map.isPrimary == true) {
                    RenderCampingMap(map);
                    return;
                }
            }

            // if (primaryMap.HasValue == false)
            // {
            //     // Render the primary map
            //     RenderCampingMap(primaryMap.Value);
            // }
            // else
            // {
            //     await DisplayAlert("No Primary Map", "No primary camping map found.", "OK");
            // }
            await DisplayAlert("Oeps", "Er kan geen kaart gevonden worden", "OK");

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
    // Toon de popup voor de specifieke plek
    var popup = new ReservationPopup(circleId);
    await this.ShowPopupAsync(popup);
}

}
