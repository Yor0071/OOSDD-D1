﻿using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using Database.Types;

namespace ReservationApp.Views
{
    public class ReservationPopup : Popup
    {
        public ReservationPopup(int spotId)
        {
            var spotDetails = App.Database.SelectCampingSpots().FirstOrDefault(s => s.Id == spotId);

            if (spotDetails == null)
            {
                Content = new VerticalStackLayout
                {
                    Children =
                    {
                        new Label
                        {
                            Text = "Geen details gevonden voor deze plek.",
                            FontSize = 14,
                            TextColor = Color.FromArgb("#FF0000"),
                            HorizontalOptions = LayoutOptions.Center
                        },
                        new Button
                        {
                            Text = "Sluit",
                            FontSize = 14,
                            BackgroundColor = Color.FromArgb("#FF5722"),
                            TextColor = Colors.White,
                            CornerRadius = 12,
                            Padding = new Thickness(5),
                            HeightRequest = 40,
                            Command = new Command(() => Close())
                        }
                    }
                };
                return;
            }

            Content = new ScrollView
            {
                Content = new VerticalStackLayout
                {
                    Spacing = 10,
                    Padding = new Thickness(5),
                    Children =
                    {
                        new Label
                        {
                            Text = "Details van de Plek",
                            FontSize = 14,
                            FontAttributes = FontAttributes.Bold,
                            HorizontalOptions = LayoutOptions.Center,
                            TextColor = Color.FromArgb("#333333")
                        },
                        new Frame
                        {
                            BorderColor = Color.FromArgb("#FF5722"),
                            CornerRadius = 14,
                            Padding = new Thickness(5),
                            BackgroundColor = Color.FromArgb("#F4F4F4"),
                            Content = new Label
                            {
                                Text = $"Beschrijving: {spotDetails.Description}",
                                FontSize = 14,
                                TextColor = Color.FromArgb("#555555")
                            }
                        },
                        new HorizontalStackLayout
                        {
                            Spacing = 20,
                            Children =
                            {
                                new Button
                                {
                                    Text = "Sluit",
                                    FontSize = 14,
                                    BackgroundColor = Color.FromArgb("#FF5722"),
                                    TextColor = Colors.White,
                                    CornerRadius = 12,
                                    Padding = new Thickness(5),
                                    HeightRequest = 40,
                                    Command = new Command(() => Close())
                                },
                                new Button
                                {
                                    Text = "Reserveer nu",
                                    FontSize = 14,
                                    BackgroundColor = Color.FromArgb("#FF9800"),
                                    TextColor = Colors.White,
                                    CornerRadius = 12,
                                    Padding = new Thickness(5),
                                    HeightRequest = 70,
                                    Command = new Command(() => ReserveSpot(spotDetails))
                                }
                            }
                        }
                    }
                }
            };
        }

        private async void ReserveSpot(CampingSpot spotDetails)
        {
            var page = new ReservationPage(spotDetails); // doorgeven van de geselecteerde campingplek
            await Shell.Current.GoToAsync(nameof(ReservationPage)); // Navigeer naar ReservationPage
            Close(); // Sluit de popup
        }
    }
}
