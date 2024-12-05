using Database;
using Database.Types;

namespace AdministrationApp;

public partial class EditCampingSpotPage : ContentPage
{
	private CampingSpot _campingSpot;

	public EditCampingSpotPage(CampingSpot campingSpot)
	{
		InitializeComponent();
		_campingSpot = campingSpot;

        CampingSpotIdLabel.Text = _campingSpot.Id.ToString();
        CampingSpotDescriptionEntry.Text = _campingSpot.Description;
        SurfaceAreaEntry.Text = _campingSpot.Surface_m2.ToString();
        PowerSwitch.IsToggled = _campingSpot.Power;
        WaterSwitch.IsToggled = _campingSpot.Water;
        WifiSwitch.IsToggled = _campingSpot.Wifi;
        MaxPersonsEntry.Text = _campingSpot.MaxPersons.ToString();
        PricePerSquareMeterEntry.Text = _campingSpot.Price_m2.ToString();
        AvailableSwitch.IsToggled = _campingSpot.Available;
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        _campingSpot.Description = CampingSpotDescriptionEntry.Text;
        _campingSpot.Surface_m2 = double.TryParse(SurfaceAreaEntry.Text, out double surface) ? surface : 0;
        _campingSpot.Power = PowerSwitch.IsToggled;
        _campingSpot.Water = WaterSwitch.IsToggled;
        _campingSpot.Wifi = WifiSwitch.IsToggled;
        _campingSpot.MaxPersons = int.TryParse(MaxPersonsEntry.Text, out int maxPersons) ? maxPersons : 0;
        _campingSpot.Price_m2 = double.TryParse(PricePerSquareMeterEntry.Text, out double price) ? price : 0;
        _campingSpot.Available = AvailableSwitch.IsToggled;

        try
        {
            App.Database.UpdateCampingSpot(_campingSpot);
            await DisplayAlert("Succes", "Kampeerplek succesvol geüpdate!", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to save camping spot: {ex.Message}", "OK");
        }
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}