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
            DeleteButton.IsEnabled = true;
        }    
        else
        {
            _selectedCampingSpot = null;
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
        }
    }

    private async void OnEditButtonClicked(object sender, EventArgs e)
    {
        if (_selectedCampingSpot != null)
        {
            await Navigation.PushAsync(new EditCampingSpotPage(_selectedCampingSpot));
        }
    }

    private async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        if (_selectedCampingSpot != null)
        {
            bool confirmDelete = await DisplayAlert("Bevestigen", "Weet u zeker dat u deze campingplek wilt verwijderen?", "Ja", "Nee");

            if (confirmDelete)
            {
                try
                {
                    await Task.Run(() => App.Database.DeleteCampingSpot(_selectedCampingSpot.Id));

                    await LoadCampingSpotAsync();

                    EditButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Verwijderen van campingplek mislukt: {ex.Message}", "Ok");
                }
            }
        }
    } 
}