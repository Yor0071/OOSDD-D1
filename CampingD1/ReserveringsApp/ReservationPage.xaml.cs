using Microsoft.Maui.Controls;  
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;


namespace ReserveringsApp
{
    public partial class ReservationPage : ContentPage
    {
        public ReservationPage()
        {
            InitializeComponent();

         
            var viewReservationButton = this.FindByName<Button>("Bekijk Reservering");
            ViewReservationButton.Clicked += ViewReservationButtonClicked;
        }

        private async void ViewReservationButtonClicked(object sender, EventArgs e)
        {
            string reservationNumber = ReservationNumberEntry.Text;

            
            if (reservationNumber == "11111")
            {
              
                bool retry = await DisplayAlert("Foutmelding",
                    "Dit nummer komt niet bekend voor.",
                    "Nummer opnieuw invoeren",
                    "Maak een reservering");

                if (retry)
                {
                   
                    ReservationNumberEntry.Text = string.Empty;
                }
                else
                {
                    
                    await Navigation.PushAsync(new CreateReservationPage());
                }
            }
            else
            {
                
                await DisplayAlert("Reservering gevonden",
                    $"Reservering nummer {reservationNumber} wordt opgehaald.",
                    "OK");
            }
        }
    }
}