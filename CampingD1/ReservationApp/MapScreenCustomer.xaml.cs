using System;
using Microsoft.Maui.Controls;

namespace ReservationApp
{
    public partial class MapScreenCustomer : ContentPage
    {
        public MapScreenCustomer()
        {
            InitializeComponent();

            // Stel de MinimumDate van beide DatePickers in op de huidige datum
            arrivalDatePicker.MinimumDate = DateTime.Now;
            departureDatePicker.MinimumDate = DateTime.Now;
        }

        // Wanneer de zoekbeschikbaarheid knop wordt ingedrukt
        private async void OnSearchAvailabilityClicked(object sender, EventArgs e)
        {
            // Toon een popup met de tekst "De plek is beschikbaar"
            bool isReserveNowClicked = await DisplayAlert(
                "Beschikbaarheid",       // Titel van de popup
                "De plek is beschikbaar", // De tekst in de popup
                "Reserveer nu",          // Eerste knop tekst
                "Ga terug"               // Tweede knop tekst
            );

            // Actie na het klikken op een van de knoppen
            if (isReserveNowClicked)
            {
                // Logica voor het reserveren van de plek (bijv. navigeren naar een reserveringspagina)
                await DisplayAlert("Reservering", "Je hebt de plek gereserveerd!", "Ok");
            }
            // Als de gebruiker op "Ga terug" klikt, gebeurt er verder niets, de popup wordt automatisch gesloten
        }
    }
}
