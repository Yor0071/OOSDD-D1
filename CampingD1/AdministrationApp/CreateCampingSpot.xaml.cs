using Database.Types;

namespace AdministrationApp;

public partial class CreateCampingSpot : ContentPage
{

    public CreateCampingSpot()
	{
		InitializeComponent();
	}

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        var _campingSpot = new CampingSpot(
            CampingSpotDescriptionEntry.Text,
            double.TryParse(SurfaceAreaEntry.Text, out double surface) ? surface : 0,
            PowerSwitch.IsToggled,
            WaterSwitch.IsToggled,
            WifiSwitch.IsToggled,
            int.TryParse(MaxPersonsEntry.Text, out int maxPersons) ? maxPersons : 0,
            double.TryParse(PricePerSquareMeterEntry.Text, out double price) ? price : 0,
            AvailableSwitch.IsToggled,
            SpotNameEntry.Text
        );

        try
        {
            App.Database.AddCampingSpot(_campingSpot);
            await DisplayAlert("Succes", "Campingplek succesvol gemaakt!", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to create camping spot: {ex.Message}", "OK");
        }
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}