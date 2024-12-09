using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using Database.types;

namespace AdministrationApp;

public partial class MapEditorPage : ContentPage
{
    private Dictionary<Frame, MapCircle> circleMap = new();
    private List<MapCircle> circleData = new();
    private Frame selectedCircle = null;
    private double offsetX, offsetY;
    private List<CampingMap> originalMapsFromDatabase = new();
    private List<CampingMap> currentMaps = new();
    private CampingMap? selectedMap = null;
    private bool isNewMap = true;

    public MapEditorPage()
    {
        InitializeComponent();
        LoadMapsAsync();
    }

    private async Task LoadMapsAsync()
    {
        try
        {
            await Task.Run(() => App.databaseHandler.EnsureConnection());
            var maps = await Task.Run(() => App.Database.SelectCampingMaps());
            Console.WriteLine(maps.First().cirles.First().spotName);
            
            currentMaps = DeepCopyMaps(maps);
            originalMapsFromDatabase = DeepCopyMaps(maps);
            
            PopulateMapPicker();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Kan mappen niet laden: {ex.Message}", "OK");
        }
    }

    private List<CampingMap> DeepCopyMaps(List<CampingMap> maps)
    {
        var copy = new List<CampingMap>();
        foreach (var map in maps)
        {
            var circleCopy = new List<MapCircle>();
            foreach (var circle in map.cirles) {
                circleCopy.Add(circle);
            }
            copy.Add(new CampingMap(map.id, circleCopy, map.name));
        }
        return copy;
    }

    private void PopulateMapPicker()
    {
        MapPicker.Items.Clear();
        
        MapPicker.Items.Add("Nieuwe map");
        
        foreach (var map in currentMaps)
        {
            MapPicker.Items.Add(map.name);
        }
    }

    private void OnMapSelected(object sender, EventArgs e)
    {
        int selectedIndex = MapPicker.SelectedIndex;

        if (selectedIndex == -1)
            return;

        if (selectedIndex == 0)
        {
            selectedMap = new CampingMap(
                id: 0,
                cirles: new List<MapCircle>(),
                name: $"zonder naam"
            );

            isNewMap = true;
            ClearCanvas();
        }
        else
        {
            selectedMap = currentMaps[selectedIndex - 1];
            RenderMap(selectedMap.Value);

            MapPicker.SelectedIndex = selectedIndex;
            MapPicker.SelectedItem = currentMaps[selectedIndex - 1].name;
            
            isNewMap = false;
        }

        MapNameEntry.Text = selectedMap.Value.name;
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

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (s, e) => OnCircleTapped(circle);
        circle.GestureRecognizers.Add(tapGesture);

        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnCircleDragged;
        circle.GestureRecognizers.Add(panGesture);

        circleMap[circle] = mapCircle;

        AbsoluteLayout.SetLayoutBounds(circle, new Rect(mapCircle.coordinateX, mapCircle.coordinateY, circle.WidthRequest, circle.HeightRequest));
        AbsoluteLayout.SetLayoutFlags(circle, AbsoluteLayoutFlags.None);
        Canvas.Children.Add(circle);
    }

    private void OnCircleTapped(Frame circle)
    {
        selectedCircle = circle;
        var mapCircle = circleMap[circle];

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

                    newX = Math.Clamp(newX, 0, RectangleContainer.Width - circle.WidthRequest);
                    newY = Math.Clamp(newY, 0, RectangleContainer.Height - circle.HeightRequest);

                    AbsoluteLayout.SetLayoutBounds(circle, new Rect(newX, newY, circle.WidthRequest, circle.HeightRequest));
                    break;

                case GestureStatus.Completed:
                    mapCircle.coordinateX = bounds.X;
                    mapCircle.coordinateY = bounds.Y;

                    circleMap[circle] = mapCircle;

                    Console.WriteLine($"Cirkel {mapCircle.id} verplaatst naar ({mapCircle.coordinateX}, {mapCircle.coordinateY})");
                    break;
            }
        }
    }

    private void OnAddCircleButtonClicked(object sender, EventArgs e)
    {
        if (!selectedMap.HasValue)
        {
            DisplayAlert("Fout", "Selecteer een kaart voordat je een cirkel toevoegt.", "OK");
            return;
        }

        var map = selectedMap.Value;

        var newCircle = new MapCircle(id: map.cirles.Count + 1, coordinateX: 0, coordinateY: 0);

        var updatedCircles = new List<MapCircle>(map.cirles) { newCircle };
        selectedMap = new CampingMap(
            map.id,
            updatedCircles,
            map.name
        );

        AddCircle(newCircle);
    }

    private void OnResetButtonClicked(object sender, EventArgs e)
    {
        if (!selectedMap.HasValue)
        {
            DisplayAlert("Fout", "Geen kaart geselecteerd om te resetten.", "OK");
            return;
        }

        CampingMap? originalMap = originalMapsFromDatabase.Find(map => map.id == selectedMap.Value.id);

        if (originalMap != null)
        {
            selectedMap = new CampingMap(
                originalMap?.id ?? 1,
                DeepCopyCircles(originalMap?.cirles ?? []),
                originalMap?.name ?? ""
            );

            RenderMap(selectedMap.Value);

            DisplayAlert("Succes", "De kaart is gereset naar de originele staat.", "OK");
        }
        else
        {
            DisplayAlert("Fout", "Originele kaartgegevens niet gevonden.", "OK");
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
        if (!selectedMap.HasValue)
        {
            DisplayAlert("Fout", "Geen kaart geselecteerd om op te slaan.", "OK");
            return;
        }

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
        
        if (!isNewMap)
        {
            App.Database.EditCampingMap(selectedMap.Value.id, MapNameEntry.Text, circleData);
            DisplayAlert("Succes", $"Kaart '{MapNameEntry.Text}' succesvol bijgewerkt.", "OK");
        }
        else
        {
            App.Database.AddCampingMap(MapNameEntry.Text, circleData);
            DisplayAlert("Succes", $"Nieuwe kaart '{MapNameEntry.Text}' succesvol aangemaakt.", "OK");
        }

        LoadMapsAsync();
        setNewEmptyMap();
        MapNameEntry.Text = "";
    }
    
    private void OnDeleteCircleButtonClicked(object sender, EventArgs e)
    {
        if (selectedCircle != null)
        {
            Canvas.Children.Remove(selectedCircle);

            if (circleMap.ContainsKey(selectedCircle))
            {
                circleMap.Remove(selectedCircle);
            }

            selectedCircle = null;

            SideMenu.IsVisible = false;

            Console.WriteLine("Cirkel succesvol verwijderd.");
        }
        else
        {
            DisplayAlert("Fout", "Geen cirkel geselecteerd om te verwijderen.", "OK");
        }
    }
    
    private void OnSetCircleIdButtonClicked(object sender, EventArgs e)
    {
        if (selectedCircle != null)
        {
            if (circleMap.ContainsKey(selectedCircle) && !string.IsNullOrWhiteSpace(CircleNameEntry.Text))
            {
                var updatedCircle = circleMap[selectedCircle];
                updatedCircle.spotName = CircleNameEntry.Text;

                circleMap[selectedCircle] = updatedCircle;

                Console.WriteLine($"Cirkel {updatedCircle.id} bijgewerkt met nieuwe naam: {updatedCircle.spotName}");

                SideMenu.IsVisible = false;
            }
            else
            {
                DisplayAlert("Fout", "Ongeldige invoer of geen cirkel geselecteerd.", "OK");
            }
        }
        else
        {
            DisplayAlert("Fout", "Geen cirkel geselecteerd om bij te werken.", "OK");
        }
    }
    
    private async void OnDeleteMapButtonClicked(object sender, EventArgs e)
    {
        if (!selectedMap.HasValue || selectedMap.Value.id <= 0)
        {
            await DisplayAlert("Fout", "Geen kaart geselecteerd om te verwijderen.", "OK");
            return;
        }
        
        bool confirmDelete = await DisplayAlert(
            "Bevestig verwijdering",
            $"Weet je zeker dat je de kaart '{selectedMap.Value.name}' wilt verwijderen?",
            "Ja",
            "Annuleren"
        );

        if (confirmDelete)
        {
            try
            {
                App.Database.DeleteMap(selectedMap.Value.id);
                await DisplayAlert("Succes", $"Kaart '{selectedMap.Value.name}' is verwijderd.", "OK");
                await LoadMapsAsync();
                setNewEmptyMap();
                MapNameEntry.Text = "";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", $"Er is een fout opgetreden bij het verwijderen: {ex.Message}", "OK");
            }
        }
    }

    private void setNewEmptyMap() {
        selectedMap = new CampingMap(
            id: 0,
            cirles: new List<MapCircle>(),
            name: $"zonder naam"
        );
        
        isNewMap = true;
        ClearCanvas();
    }
}