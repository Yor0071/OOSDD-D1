using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using Database.types;

namespace AdministrationApp;

public partial class MapEditorPage : ContentPage
{
    private Dictionary<Frame, MapCircle> circleMap = new(); // Maps Frame objects to MapCircle instances
    private List<MapCircle> circleData = new(); // Temporary storage for all circles
    private Frame selectedCircle = null; // Currently selected circle for editing
    private double offsetX, offsetY;
    private List<CampingMap> originalMapsFromDatabase = new(); // Original maps from the database
    private List<CampingMap> currentMaps = new(); // List of current maps
    private CampingMap? selectedMap = null; // Currently selected map

    public MapEditorPage()
    {
        InitializeComponent();
        LoadMapsAsync();
    }

    private async Task LoadMapsAsync()
    {
        try
        {
            // Ensure database connection and fetch maps
            await Task.Run(() => App.databaseHandler.EnsureConnection());
            var maps = await Task.Run(() => App.Database.SelectCampingMaps());

            // Deep copy maps to avoid reference issues
            currentMaps = DeepCopyMaps(maps);
            originalMapsFromDatabase = DeepCopyMaps(maps);

            // Populate the picker with map options on the main thread
            MainThread.BeginInvokeOnMainThread(() => PopulateMapPicker());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load maps: {ex.Message}", "OK");
        }
    }

    private List<CampingMap> DeepCopyMaps(List<CampingMap> maps)
    {
        var copy = new List<CampingMap>();
        foreach (var map in maps)
        {
            var circleCopy = new List<MapCircle>();
            foreach (var circle in map.cirles)
            {
                circleCopy.Add(circle); // Copy each circle
            }
            copy.Add(new CampingMap(map.id, circleCopy, map.name, map.campingSpotId));
        }
        return copy;
    }

    private void PopulateMapPicker()
    {
        MapPicker.Items.Clear(); // Clear existing items

        // Add "Nieuwe map" as the first option
        MapPicker.Items.Add("Nieuwe map");

        // Populate picker with existing maps
        foreach (var map in currentMaps)
        {
            MapPicker.Items.Add(map.name);
        }
    }

    private void OnMapSelected(object sender, EventArgs e)
    {
        int selectedIndex = MapPicker.SelectedIndex;

        if (selectedIndex == -1)
            return; // No selection made

        if (selectedIndex == 0)
        {
            // "Nieuwe map" selected - Create a new empty map
            selectedMap = new CampingMap(
                id: currentMaps.Count + 1,
                cirles: new List<MapCircle>(),
                name: $"New Map {currentMaps.Count + 1}",
                campingSpotId: 0
            );
            ClearCanvas();
        }
        else
        {
            // Existing map selected - Render circles
            selectedMap = currentMaps[selectedIndex - 1]; // Adjust index for "Nieuwe map"
            RenderMap(selectedMap.Value);

            // Update the selected index to match the user's selection
            MapPicker.SelectedIndex = selectedIndex;
            MapPicker.SelectedItem = currentMaps[selectedIndex - 1].name;
        }
    }

    private void RenderMap(CampingMap map)
    {
        ClearCanvas();

        foreach (var circle in map.cirles)
        {
            Console.WriteLine("Adding");
            AddCircle(circle);
        }
    }

    private void ClearCanvas()
    {
        Canvas.Children.Clear();
        circleMap.Clear();
    }

    private void AddCircle(MapCircle mapCircle)
    {
        var circle = new Frame
        {
            WidthRequest = 50,
            HeightRequest = 50,
            CornerRadius = 25,
            BackgroundColor = Colors.Red,
            HasShadow = false
        };

        // Add tap gesture for selection
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (s, e) => OnCircleTapped(circle);
        circle.GestureRecognizers.Add(tapGesture);

        // Add drag functionality
        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnCircleDragged;
        circle.GestureRecognizers.Add(panGesture);

        // Map the circle's data to the frame
        circleMap[circle] = mapCircle;

        // Add the frame to the canvas
        AbsoluteLayout.SetLayoutBounds(circle, new Rect(mapCircle.coordinateX, mapCircle.coordinateY, circle.WidthRequest, circle.HeightRequest));
        AbsoluteLayout.SetLayoutFlags(circle, AbsoluteLayoutFlags.None);
        Canvas.Children.Add(circle);
    }

    private void OnCircleTapped(Frame circle)
    {
        selectedCircle = circle;
        var mapCircle = circleMap[circle];

        // Update UI to reflect the selected circle's name
        CircleNameEntry.Text = mapCircle.spotName;
        SideMenu.IsVisible = true;
    }

    private void OnCircleDragged(object sender, PanUpdatedEventArgs e)
    {
        if (sender is Frame circle && circleMap.ContainsKey(circle))
        {
            var mapCircle = circleMap[circle];
            var bounds = AbsoluteLayout.GetLayoutBounds(circle);

            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    offsetX = bounds.X;
                    offsetY = bounds.Y;
                    break;

                case GestureStatus.Running:
                    var newX = offsetX + e.TotalX;
                    var newY = offsetY + e.TotalY;

                    // Ensure the circle stays within bounds
                    newX = Math.Clamp(newX, 0, RectangleContainer.Width - circle.WidthRequest);
                    newY = Math.Clamp(newY, 0, RectangleContainer.Height - circle.HeightRequest);

                    AbsoluteLayout.SetLayoutBounds(circle, new Rect(newX, newY, circle.WidthRequest, circle.HeightRequest));
                    break;

                case GestureStatus.Completed:
                    // Update MapCircle coordinates
                    mapCircle.coordinateX = bounds.X;
                    mapCircle.coordinateY = bounds.Y;

                    // Update the dictionary entry
                    circleMap[circle] = mapCircle;

                    Console.WriteLine($"Circle {mapCircle.id} moved to ({mapCircle.coordinateX}, {mapCircle.coordinateY})");
                    break;
            }
        }
    }

    private void OnAddCircleButtonClicked(object sender, EventArgs e)
    {
        if (!selectedMap.HasValue)
        {
            DisplayAlert("Error", "Select a map before adding a circle.", "OK");
            return;
        }

        // Get a copy of the selected map
        var map = selectedMap.Value;

        // Create a new circle at (0, 0)
        var newCircle = new MapCircle(id: map.cirles.Count + 1, coordinateX: 0, coordinateY: 0);

        // Update the circle list and reassign the map
        var updatedCircles = new List<MapCircle>(map.cirles) { newCircle };
        selectedMap = new CampingMap(
            map.id,
            updatedCircles,
            map.name,
            map.campingSpotId
        );

        // Add the circle to the UI
        AddCircle(newCircle);
    }

    private void OnResetButtonClicked(object sender, EventArgs e)
    {
        if (!selectedMap.HasValue)
        {
            DisplayAlert("Error", "No map selected to reset.", "OK");
            return;
        }

        // Find the original map from the database
        CampingMap? originalMap = originalMapsFromDatabase.Find(map => map.id == selectedMap.Value.id);

        if (originalMap != null)
        {
            // Replace the current map with the original map
            selectedMap = new CampingMap(
                originalMap?.id ?? 1,
                DeepCopyCircles(originalMap?.cirles ?? []),
                originalMap?.name ?? "",
                originalMap?.campingSpotId ?? 0
            );

            // Re-render the map with the restored data
            RenderMap(selectedMap.Value);

            DisplayAlert("Success", "The map has been reset to its original state.", "OK");
        }
        else
        {
            DisplayAlert("Error", "Original map data not found.", "OK");
        }
    }

    private List<MapCircle> DeepCopyCircles(List<MapCircle> circles)
    {
        var copy = new List<MapCircle>();
        foreach (var circle in circles)
        {
            copy.Add(circle);
        }
        return copy;
    }

    private void OnSaveButtonClicked(object sender, EventArgs e)
    {
        // Save the latest positions to circleData
        circleData.Clear();
        foreach (var kvp in circleMap)
        {
            var frame = kvp.Key;
            var mapCircle = kvp.Value;

            var bounds = AbsoluteLayout.GetLayoutBounds(frame);
            mapCircle.coordinateX = bounds.X;
            mapCircle.coordinateY = bounds.Y;

            circleData.Add(mapCircle);
        }

        Console.WriteLine("Saved Circle Data:");
        foreach (var circle in circleData)
        {
            Console.WriteLine($"ID: {circle.id}, X: {circle.coordinateX}, Y: {circle.coordinateY}, Name: {circle.spotName}");
        }
    }
    
    private void OnDeleteCircleButtonClicked(object sender, EventArgs e)
    {
        if (selectedCircle != null)
        {
            // Remove the circle from the canvas
            Canvas.Children.Remove(selectedCircle);

            // Remove the circle from the map
            if (circleMap.ContainsKey(selectedCircle))
            {
                circleMap.Remove(selectedCircle);
            }

            selectedCircle = null;

            // Hide the side menu
            SideMenu.IsVisible = false;

            Console.WriteLine("Circle deleted successfully.");
        }
        else
        {
            DisplayAlert("Error", "No circle selected to delete.", "OK");
        }
    }
    
    private void OnSetCircleIdButtonClicked(object sender, EventArgs e)
    {
        if (selectedCircle != null)
        {
            if (circleMap.ContainsKey(selectedCircle) && !string.IsNullOrWhiteSpace(CircleNameEntry.Text))
            {
                // Update the spotName of the selected circle
                var updatedCircle = circleMap[selectedCircle];
                updatedCircle.spotName = CircleNameEntry.Text;

                // Reassign the updated circle back into the dictionary
                circleMap[selectedCircle] = updatedCircle;

                Console.WriteLine($"Circle {updatedCircle.id} updated with new name: {updatedCircle.spotName}");

                // Optionally, hide the side menu after setting the ID
                SideMenu.IsVisible = false;
            }
            else
            {
                DisplayAlert("Error", "Invalid input or no circle selected.", "OK");
            }
        }
        else
        {
            DisplayAlert("Error", "No circle selected to update.", "OK");
        }
    }
}