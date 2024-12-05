using System;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using System.Threading.Tasks;
using Database.types;
using Microsoft.Maui.Layouts;
using System.Collections.Generic;

namespace ReservationApp
{
    public partial class MapScreenCustomer : ContentPage
    {
        private Dictionary<Frame, int> circleMap = new(); // Maps circles to IDs

        public MapScreenCustomer()
        {
            InitializeComponent();
            arrivalDatePicker.MinimumDate = DateTime.Now;
            departureDatePicker.MinimumDate = DateTime.Now;

           LoadCampingMapAsync();
        }

        private async Task LoadCampingMapAsync()
        {
            try
            {
                // Simulate database connection or initialization if needed
                await Task.Run(() => App.databaseHandler.EnsureConnection()); // Ensure the database connection is established

                var maps = await Task.Run(() => App.Database.SelectCampingMaps());

                if (maps != null && maps.Any())
                {
                    RenderCampingMap(maps.First());
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load map: {ex.Message}", "OK");
            }
        }

        // Render camping map with circles on it1
        public void RenderCampingMap(CampingMap campingMap)
        {
            // Clear any existing children on the canvas
            Canvas.Children.Clear();
            circleMap.Clear();

            // Get the dimensions of the image (for scaling purposes)
            var imageWidth = 1000; // Example width of the image
            var imageHeight = 700; // Example height of the image

            // Iterate over each circle in the map and place them on the canvas
            foreach (var circle in campingMap.cirles)
            {
                // Assuming the coordinates are in percentage terms, you can scale them
                double x = circle.coordinateX * imageWidth;  // Scale X coordinate
                double y = circle.coordinateY * imageHeight;  // Scale Y coordinate

                AddCircle(circle.id, x, y);
            }
        }

        private void AddCircle(int id, double x, double y)
        {
            // Create a frame to represent the circle
            var circle = new Frame
            {
                WidthRequest = 50,  // Set a constant size for the circle
                HeightRequest = 50,
                CornerRadius = 25,
                BackgroundColor = Colors.Red,
                HasShadow = false
            };

            // Map the circle to its ID
            circleMap[circle] = id;

            // Set the position of the circle on the absolute layout
            AbsoluteLayout.SetLayoutBounds(circle, new Rect(x, y, circle.WidthRequest, circle.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(circle, AbsoluteLayoutFlags.None);

            // Add the circle to the layout
            Canvas.Children.Add(circle);
        }


        // Search availability button click event
        private async void OnSearchAvailabilityClicked(object sender, EventArgs e)
        {
            // Simulate a check for availability
            bool isReserveNowClicked = await DisplayAlert(
                "Beschikbaarheid",
                "De plek is beschikbaar",
                "Reserveer nu",
                "Ga terug"
            );

            // Action after user selects option
            if (isReserveNowClicked)
            {
                // Handle reservation logic, e.g., navigate to a reservation page
                await DisplayAlert("Reservering", "Je hebt de plek gereserveerd!", "Ok");
            }
        }
    }
}
