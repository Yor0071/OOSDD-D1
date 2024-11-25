using Microsoft.Maui.Controls;

namespace ReserveringsApp
{
    public partial class CreateReservationPage : ContentPage
    {
        public CreateReservationPage()
        {
            InitializeComponent();
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            // Ga terug naar de vorige pagina
            await Navigation.PopAsync();
        }
    }
}
