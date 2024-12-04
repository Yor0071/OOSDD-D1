using Database.Types;

namespace AdministrationApp;

public partial class CampingSpotList : ContentPage
{
	public CampingSpotList()
	{
		InitializeComponent();
        LoadCampingSpotAsync();

        CampingSpotsCollectionView.ItemsSource = new List<CampingSpot>();
    }

    private async Task LoadCampingSpotAsync()
    {
        try
        {
            // Simulate database connection or initialization if needed
            await Task.Run(() => App.databaseHandler.EnsureConnection()); // Ensure the database connection is established

            // Fetch reservations
            var campingSpots = await Task.Run(() => App.Database.SelectCampingSpots());

            // Bind the data
            CampingSpotsCollectionView.ItemsSource = campingSpots;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load camping spots: {ex.Message}", "OK");
        }
    }
}