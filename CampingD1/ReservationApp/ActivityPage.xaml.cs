namespace ReservationApp;

public partial class ActivityPage : ContentPage
{
	public ActivityPage()
	{
		InitializeComponent();
		LoadActivities();
	}

	private async void LoadActivities()
	{
		try
		{
			var acrivities = await Task.Run(() => App.Database.SelectActivities());

			var sortedActivities = acrivities.OrderBy(a => a.Date).ToList();

            ActivitiesCollectionView.ItemsSource = sortedActivities;
		}
		catch (Exception ex)
		{
			await DisplayAlert("Fout", $"Kon activiteiten niet laden: {ex.Message}", "OK");
		}
	}

	public class Activity
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime Date { get; set; }
	}
}