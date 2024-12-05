using Database.Types;

namespace AdministrationApp;

public partial class CampingSpotList : ContentPage
{
    private CampingSpot _selectedCampingSpot;

	public CampingSpotList()
	{
		InitializeComponent();

        CampingSpotsCollectionView.ItemsSource = new List<CampingSpot>();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadCampingSpotAsync();
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

    private void OnCampingSpotSelected(object sender, SelectionChangedEventArgs e)
    {
        if(e.CurrentSelection.Count > 0)
        {
            _selectedCampingSpot = (CampingSpot)e.CurrentSelection.FirstOrDefault();
            EditButton.IsEnabled = true;
        }    
        else
        {
            _selectedCampingSpot = null;
            EditButton.IsEnabled = false;
        }
    }

    private async void OnEditButtonClicked(object sender, EventArgs e)
    {
        if (_selectedCampingSpot != null)
        {
            await Navigation.PushAsync(new EditCampingSpotPage(_selectedCampingSpot));
        }
    }
}