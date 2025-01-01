using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Database.Types;

namespace AdministrationApp;

public partial class ActivityList : ContentPage
{
    private Activity _selectedActivity;

    public ActivityList()
    {
        InitializeComponent();
        ActivitiesCollectionView.ItemsSource = new List<Activity>();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadActivitiesAsync();
    }

    private async Task LoadActivitiesAsync()
    {
        try
        {
            await Task.Run(() => App.databaseHandler.EnsureConnection());

            var activities = await Task.Run(() => App.Database.SelectActivities());
            ActivitiesCollectionView.ItemsSource = activities.OrderBy(a => a.Date).ToList();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Kon activiteiten niet laden: {ex.Message}", "OK");
        }
    }

    private void OnActivitySelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {
            _selectedActivity = (Activity)e.CurrentSelection.FirstOrDefault();
            EditButton.IsEnabled = true;
        }
        else
        {
            _selectedActivity = null;
            EditButton.IsEnabled = false;
        }
    }

    private async void OnEditButtonClicked(object sender, EventArgs e)
    {
        if (_selectedActivity != null)
        {
            await Navigation.PushAsync(new EditActivityPage(_selectedActivity));
        }
    }
}
