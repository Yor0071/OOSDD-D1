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
        // Check welke velden leeg zijn
        List<string> emptyFields = new List<string>();

        if (string.IsNullOrWhiteSpace(CampingSpotDescriptionEntry.Text)) emptyFields.Add("Beschrijving");
        if (string.IsNullOrWhiteSpace(SpotNameEntry.Text)) emptyFields.Add("Naam");
        if (string.IsNullOrWhiteSpace(SurfaceAreaEntry.Text)) emptyFields.Add("Oppervlakte");
        if (string.IsNullOrWhiteSpace(MaxPersonsEntry.Text)) emptyFields.Add("Max personen");
        if (string.IsNullOrWhiteSpace(PricePerSquareMeterEntry.Text)) emptyFields.Add("Prijs per m²");

        if (emptyFields.Count == 5)
        {
            await DisplayAlert("Fout", "Alle velden zijn leeg. Vul de vereiste gegevens in.", "OK");
            return;
        }
        else if (emptyFields.Count > 0)
        {
            string missingFieldsMessage = "De volgende velden zijn niet ingevuld: " + string.Join(", ", emptyFields);
            await DisplayAlert("Let op", missingFieldsMessage, "OK");
            return;
        }

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
            await DisplayAlert("Error", "Er is een fout opgetreden bij het opslaan van de campingplek.", "OK");
            Console.WriteLine($"SQL Error: {ex.Message}"); // Log de fout voor debugging
        }
    }


    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}