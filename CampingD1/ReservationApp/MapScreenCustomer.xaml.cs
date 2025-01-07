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
    private Dictionary<Frame, int> circleMap = new(); // Creates a map linking circles to IDs
    private bool isMapUpdated = false;

    public MapScreenCustomer()
    {
        InitializeComponent();
        LoadCampingMap();

        // Set default values for the date pickers
        arrivalDatePicker.Date = DateTime.Today;
        departureDatePicker.Date = DateTime.Today.AddDays(1);
    }

    public async void LoadCampingMap()
    {
        try
        {
            // Ensure the database connection is established
            await Task.Run(() => App.databaseHandler.EnsureConnection());

            // Retrieve the maps
            var maps = await Task.Run(() => App.Database.SelectCampingMaps());

            // Retrieve the reservations
            var reservations = await Task.Run(() => App.Database.SelectReservations());

            // Haal de geselecteerde datums op van de date pickers
            DateTime selectedFromDate = arrivalDatePicker.Date;
            DateTime selectedToDate = departureDatePicker.Date;

            foreach (var map in maps)
            {
                if (map.isPrimary == true)
                {
                    // Geef de kaart, reserveringen en geselecteerde datums door
                    RenderCampingMap(map, reservations, selectedFromDate, selectedToDate);
                    return;
                }
            }

            await DisplayAlert("Oops", "No map could be found", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load reservations: {ex.Message}", "OK");
        }
    }


    private void RenderCampingMap(CampingMap campingMap, List<Reservation> reservations, DateTime selectedFromDate, DateTime selectedToDate)
    {
        // Verwijder bestaande cirkels
        Canvas.Children.Clear();
        circleMap.Clear();

        // Stel de achtergrondafbeelding in
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

        // Voeg cirkels toe met kleuren op basis van beschikbaarheid
        foreach (var circle in campingMap.cirles)
        {
            AddCircle(circle.id, circle.coordinateX, circle.coordinateY, circle.CampingSpotId, reservations, selectedFromDate, selectedToDate);
        }
    }




    // This method checks if a camping spot is available during the selected date range
    private bool IsCampingSpotAvailable(int campingSpotId, DateTime selectedFromDate, DateTime selectedToDate, List<Reservation> reservations)
    {
        foreach (var reservation in reservations)
        {
            // Check if there is an overlap between the reservation and the selected date range
            if (reservation.PlaceNumber == campingSpotId &&
                ((selectedFromDate >= reservation.Arrival && selectedFromDate <= reservation.Depart) ||
                 (selectedToDate >= reservation.Arrival && selectedToDate <= reservation.Depart) ||
                 (selectedFromDate <= reservation.Arrival && selectedToDate >= reservation.Depart)))
            {
                return false; // Not available due to overlap
            }
        }

        return true; // Available
    }




    private void AddCircle(int id, double x, double y, int campingSpotId, List<Reservation> reservations, DateTime selectedFromDate, DateTime selectedToDate)
    {
        // Standaardkleur is wit
        var circleColor = Colors.White;

        // Controleer op overlappende reserveringen
        foreach (var reservation in reservations)
        {
            if (reservation.PlaceNumber == campingSpotId &&
                ((selectedFromDate >= reservation.Arrival && selectedFromDate <= reservation.Depart) ||
                 (selectedToDate >= reservation.Arrival && selectedToDate <= reservation.Depart) ||
                 (selectedFromDate <= reservation.Arrival && selectedToDate >= reservation.Depart)))
            {
                circleColor = Colors.Red; // Overlap gevonden, markeer als rood
                break;
            }
        }

        // Als geen overlap, markeer de cirkel als groen
        if (circleColor == Colors.White)
        {
            circleColor = Colors.Green;
        }

        // Maak een nieuwe cirkel
        var circle = new Frame
        {
            WidthRequest = 50,
            HeightRequest = 50,
            CornerRadius = 25,
            BackgroundColor = circleColor,
            HasShadow = false
        };

        // Voeg de cirkel toe aan de kaart
        circleMap[circle] = id;
        AbsoluteLayout.SetLayoutBounds(circle, new Rect(x, y, circle.WidthRequest, circle.HeightRequest));
        AbsoluteLayout.SetLayoutFlags(circle, AbsoluteLayoutFlags.None);
        Canvas.Children.Add(circle);

        // Voeg de klikgebeurtenis toe aan de cirkel, alleen als de kaart is bijgewerkt
        if (isMapUpdated)
        {
            circle.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OnCircleClicked(id, campingSpotId)) // Pass both IDs
            });
        }
    }





    private void UpdateCircleColor(Frame circle, int campingSpotId, List<Reservation> reservations)
    {
        // Haal de geselecteerde aankomst- en vertrekdatums op uit de date pickers
        DateTime selectedFromDate = arrivalDatePicker.Date;
        DateTime selectedToDate = departureDatePicker.Date;

        // Controleer of er overlappende reserveringen zijn
        var circleColor = Colors.Green; // Standaard groen (geen overlap)
        foreach (var reservation in reservations)
        {
            if (reservation.PlaceNumber == campingSpotId &&
                ((selectedFromDate >= reservation.Arrival && selectedFromDate <= reservation.Depart) ||
                 (selectedToDate >= reservation.Arrival && selectedToDate <= reservation.Depart) ||
                 (selectedFromDate <= reservation.Arrival && selectedToDate >= reservation.Depart)))
            {
                circleColor = Colors.Red; // Markeer als rood als er een overlap is
                break;
            }
        }

        // Werk de cirkelkleur bij
        circle.BackgroundColor = circleColor;
    }

    private void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        // Haal de lijst met reserveringen op
        var reservations = App.Database.SelectReservations();

        // Werk alle cirkels bij op basis van de nieuwe datumbereik
        foreach (var circle in circleMap.Keys)
        {
            var campingSpotId = circleMap[circle];
            UpdateCircleColor(circle, campingSpotId, reservations);
        }
    }



    private async void OnUpdateMapClicked(object sender, EventArgs e)
    {
        try
        {
            // Haal de geselecteerde datums op
            DateTime selectedFromDate = arrivalDatePicker.Date;
            DateTime selectedToDate = departureDatePicker.Date;

            // Controleer of beide datums zijn geselecteerd
            if (selectedFromDate > selectedToDate)
            {
                await DisplayAlert("Fout", "De aankomstdatum mag niet later zijn dan de vertrekdatum.", "OK");
                return;
            }

            // Haal de laatste reserveringen op
            var reservations = App.Database.SelectReservations();

            // Haal de kaarten op en selecteer de primaire kaart
            var maps = await Task.Run(() => App.Database.SelectCampingMaps());
            foreach (var map in maps)
            {
                if (map.isPrimary == true)
                {
                    // Zet de kaart bijwerken-flag op true
                    isMapUpdated = true;

                    // Render de kaart opnieuw met de laatste reserveringen en geselecteerde datums
                    RenderCampingMap(map, reservations, selectedFromDate, selectedToDate);

                    // Display een bericht dat de kaart is bijgewerkt
                    await DisplayAlert("Succes", "De kaart is bijgewerkt met de geselecteerde datums.", "OK");
                    return;
                }
            }

            await DisplayAlert("Oops", "No map could be found", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to update map: {ex.Message}", "OK");
        }
    }






    private async void OnCircleClicked(int circleId, int campingSpotId)
    {
        // Retrieve details of the selected circle
        var mapCircle = App.Database.SelectMapCircleById(circleId);

        // Retrieve the camping spot associated with the circle
        CampingSpot campingSpot = App.Database.SelectCampingSpotById(campingSpotId);

        // Get the selected arrival and departure dates from the date pickers
        DateTime selectedFromDate = arrivalDatePicker.Date;
        DateTime selectedToDate = departureDatePicker.Date;

        // Retrieve the reservations list
        var reservations = App.Database.SelectReservations();

        // Create and display the ReservationPopup with camping spot details and selected dates
        var popup = new ReservationPopup(campingSpot, selectedFromDate, selectedToDate, reservations);
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
                await DisplayAlert("Entered Reservation",
                    $"Reservation Number: {reservation.Id}\n" +
                    $"Name: {reservation.FirstName} {reservation.LastName}\n" +
                    $"Camping Spot: {reservation.PlaceNumber}\n" +
                    $"From: {reservation.Arrival.ToShortDateString()} To: {reservation.Depart.ToShortDateString()}\n" +
                    $"Phone: {reservation.PhoneNumber}\n" +
                    $"Email: {reservation.Email}", "OK");

                // Change the color of the circle corresponding to the camping spot
                var targetCircle = circleMap.FirstOrDefault(c => c.Value == reservation.PlaceNumber).Key;

                if (targetCircle != null)
                {
                    targetCircle.BackgroundColor = Colors.White;
                }
            }
            else
            {
                await DisplayAlert("Error", "Reservation number does not match any existing reservation.", "OK");
            }
        }
    }
}
