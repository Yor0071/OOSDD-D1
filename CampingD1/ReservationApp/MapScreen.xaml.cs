//using System.Collections.Generic;
//using Microsoft.Maui.Layouts;
//using Database.types;
//namespace ReservationApp;
//public partial class MapScreen : ContentPage
//{
//    private Dictionary<Frame, int> circleMap = new(); // Maps circles to IDs
//    public MapScreen()
//    {
//        InitializeComponent();
//        LoadMapsAsync();
//    }

//    private async Task LoadMapsAsync()
//    {
//        try
//        {
//            // Simulate database connection or initialization if needed
//            await Task.Run(() => App.databaseHandler.EnsureConnection()); // Ensure the database connection is established
//            // Fetch reservations
//            var maps = await Task.Run(() => App.Database.SelectCampingMaps());
//            // Bind the data
//            // ReservationsCollectionView.ItemsSource = reservations;

//            Console.WriteLine(maps.First().cirles.Count);
//            RenderCampingMap(maps.First());
//        }
//        catch (Exception ex)
//        {
//            await DisplayAlert("Error", $"Failed to load reservations: {ex.Message}", "OK");
//        }
//    }
//    public void RenderCampingMap(CampingMap campingMap)
//    {
//        Canvas.Children.Clear();
//        circleMap.Clear();
//        foreach (var circle in campingMap.cirles)
//        {
//            AddCircle(circle.id, circle.coordinateX, circle.coordinateY);
//        }
//    }
//    private void AddCircle(int id, double x, double y)
//    {
//        var circle = new Frame
//        {
//            WidthRequest = 50,
//            HeightRequest = 50,
//            CornerRadius = 25,
//            BackgroundColor = Colors.Red,
//            HasShadow = false
//        };
//        // Add circle to Canvas and map its ID
//        circleMap[circle] = id;
//        AbsoluteLayout.SetLayoutBounds(circle, new Rect(x, y, circle.WidthRequest, circle.HeightRequest));
//        AbsoluteLayout.SetLayoutFlags(circle, AbsoluteLayoutFlags.None);
//        Canvas.Children.Add(circle);
//    }
//}