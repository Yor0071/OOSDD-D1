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
            
            currentMaps = maps;
            originalMapsFromDatabase = maps;
            
            PopulateMapPicker();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Kan mappen niet laden: {ex.Message}", "OK");
        }
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

    private void OnSetPrimaryMapButtonClicked(object sender, EventArgs e)
    {
        if (isNewMap || !selectedMap.HasValue)
        {
            DisplayAlert("Fout", "Je moet een bestaande kaart selecteren om deze als primaire kaart in te stellen.", "OK");
            return;
        }
    
        App.Database.SetPrimaryMap(selectedMap.Value.id);
        DisplayAlert("Succes", $"Kaart '{selectedMap.Value.name}' is ingesteld als primaire kaart.", "OK");
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
                name: $"zonder naam",
                backgroundImage: ""
            );
                BackgroundImage.Source = "";

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
        try
        {
            if (!string.IsNullOrEmpty(map.backgroundImage))
            {
                try {
                    var imageBytes = Convert.FromBase64String(map.backgroundImage);
                    var imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));

                    BackgroundImage.Source = imageSource;
                }
                catch (Exception e) {
                    BackgroundImage.Source = null;
                }
            }
            else
            {
                BackgroundImage.Source = null;
            }

            // Clear the canvas for new circles
            ClearCanvas();

            // Render circles on the map
            foreach (var circle in map.cirles)
            {
                AddCircle(circle);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error rendering map: {ex.Message}");
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

        var newCircle = new MapCircle(id: map.cirles.Count + 1, coordinateX: 0, coordinateY: 0, campingSpotId: 0, spotName: string.Empty);

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
                originalMap?.cirles ?? [],
                originalMap?.name ?? "",
                originalMap?.isPrimary ?? false,
                originalMap?.backgroundImage
            );

            RenderMap(selectedMap.Value);

            DisplayAlert("Succes", "De kaart is gereset naar de originele staat.", "OK");
        }
        else
        {
            DisplayAlert("Fout", "Originele kaartgegevens niet gevonden.", "OK");
        }
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        if (!selectedMap.HasValue)
        {
            await DisplayAlert("Fout", "Geen kaart geselecteerd om op te slaan.", "OK");
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
            App.Database.EditCampingMap(selectedMap.Value.id, MapNameEntry.Text, circleData, selectedMap.Value.backgroundImage);
             await DisplayAlert("Succes", $"Kaart '{MapNameEntry.Text}' succesvol bijgewerkt.", "OK");
        }
        else
        {
            App.Database.AddCampingMap(MapNameEntry.Text, circleData, selectedMap.Value.backgroundImage);
            await DisplayAlert("Succes", $"Nieuwe kaart '{MapNameEntry.Text}' succesvol aangemaakt.", "OK");
        }

        await LoadMapsAsync();

        //Find the just edited map and reselect it
        CampingMap? updatedMap = currentMaps.FirstOrDefault(map => map.name == MapNameEntry.Text);
        if (updatedMap.HasValue)
        {
            int updatedIndex = currentMaps.IndexOf(updatedMap.Value) + 1;
            MapPicker.SelectedIndex = updatedIndex;
        }
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
    
    private async void OnUploadImageClicked(object sender, EventArgs e)
    {
        Console.WriteLine("Picking image...");
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images
            });

            if (result == null)
                return;

            // Convert the selected image into a byte array
            byte[] imageBytes;
            using (var stream = await result.OpenReadAsync())
            {
                imageBytes = ReadStreamToByteArray(stream);
            }

            BackgroundImage.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            
            string base64Image = Convert.ToBase64String(imageBytes);

            if (selectedMap != null)
            {
                selectedMap = selectedMap.Value with { backgroundImage = base64Image };
            }
            
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Er is iets mis gegaan: {ex.Message}", "OK");
        }
    }
    
    private byte[] ReadStreamToByteArray(Stream inputStream)
    {
        using (var memoryStream = new MemoryStream())
        {
            inputStream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}