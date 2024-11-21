using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;

namespace ReserveringsApp;

public partial class NewPage1 : ContentPage
{
    private Dictionary<Frame, int> circleMap = new(); // Maps circles to IDs
    private List<(int Id, double X, double Y)> circleData = new(); // Temporary storage
    private int nextCircleId = 1; // Auto-incrementing ID
    private double offsetX, offsetY;
    private Frame selectedCircle = null; // Currently selected circle

    public NewPage1()
    {
        InitializeComponent();
    }

    private void OnAddCircleButtonClicked(object sender, EventArgs e)
    {
        AddCircle(nextCircleId++, 25, 25); // Adds a new circle at a default position
    }

    private void OnSaveButtonClicked(object sender, EventArgs e)
    {
        // Print all circles' data
        circleData.Clear();
        foreach (var circle in circleMap.Keys)
        {
            var bounds = AbsoluteLayout.GetLayoutBounds(circle);
            int id = circleMap[circle];
            circleData.Add((id, bounds.X, bounds.Y));
        }

        // Output the list to the console or any other method
        Console.WriteLine("Circle Data:");
        foreach (var data in circleData)
        {
            Console.WriteLine($"ID: {data.Id}, X: {data.X}, Y: {data.Y}");
        }
    }

    private void AddCircle(int id, double x, double y)
    {
        var circle = new Frame
        {
            WidthRequest = 23,
            HeightRequest = 23,
            CornerRadius = 25,
            BackgroundColor = Colors.Red,
            HasShadow = false
        };

        // Add tap gesture for selection
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (s, e) => OnCircleTapped(circle);
        circle.GestureRecognizers.Add(tapGesture);

        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnCircleDragged;
        circle.GestureRecognizers.Add(panGesture);

        // Add circle to Canvas and map its ID
        circleMap[circle] = id;
        AbsoluteLayout.SetLayoutBounds(circle, new Rect(x, y, circle.WidthRequest, circle.HeightRequest));
        AbsoluteLayout.SetLayoutFlags(circle, AbsoluteLayoutFlags.None);
        Canvas.Children.Add(circle);
    }

    private void OnCircleTapped(Frame circle)
    {
        selectedCircle = circle;
        CircleIdEntry.Text = circleMap[circle].ToString(); // Show the current ID in the entry
        SideMenu.IsVisible = true; // Show the side menu
    }

    private void OnSetCircleIdButtonClicked(object sender, EventArgs e)
    {
        if (selectedCircle != null && int.TryParse(CircleIdEntry.Text, out int newId))
        {
            // Update the ID for the selected circle
            circleMap[selectedCircle] = newId;
            Console.WriteLine($"Circle ID updated to {newId}");
        }
    }

    private void OnCircleDragged(object sender, PanUpdatedEventArgs e)
    {
        if (sender is Frame circle && circleMap.ContainsKey(circle))
        {
            int id = circleMap[circle];

            // Get initial position
            var bounds = AbsoluteLayout.GetLayoutBounds(circle);
            double newX = bounds.X;
            double newY = bounds.Y;

            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    // Store initial offset
                    offsetX = bounds.X;
                    offsetY = bounds.Y;
                    break;

                case GestureStatus.Running:
                    // Update circle position during drag
                    newX = offsetX + e.TotalX;
                    newY = offsetY + e.TotalY;

                    // Ensure circle stays within bounds of rectangle
                    newX = Math.Clamp(newX, 0, RectangleContainer.Width - circle.WidthRequest);
                    newY = Math.Clamp(newY, 0, RectangleContainer.Height - circle.HeightRequest);

                    AbsoluteLayout.SetLayoutBounds(circle, new Rect(newX, newY, circle.WidthRequest, circle.HeightRequest));
                    break;

                case GestureStatus.Completed:
                    // Optionally, save updated position to the list
                    Console.WriteLine($"Circle {id} moved to ({newX}, {newY})");
                    break;
            }
        }
    }
}