using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using System;

namespace ReservationApp
{
    public partial class ReservationNumberPopup : Popup
    {
        public ReservationNumberPopup()
        {
            InitializeComponent();
        }

        // Sluit de popup als de gebruiker op de "X" knop klikt
        private void OnClosePopupClicked(object sender, EventArgs e)
        {
            Close();
        }

        // Bevestigingsknop: controleer het invoerveld en sluit de popup met de invoerwaarde
        private void OnConfirmButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ReservationEntry.Text))
            {
                Close(ReservationEntry.Text); // Sluit de popup en retourneer het ingevoerde nummer
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Fout", "Vul aub een geldig reserveringsnummer in.", "OK");
            }
        }
    }
}
