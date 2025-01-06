using Microsoft.Maui.Controls;
using Database;
using Database.Types;

namespace AdministrationApp;

public partial class AddActivityPage : ContentPage
{
    public AddActivityPage()
    {
        InitializeComponent();
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        var newActivity = new Activity
        {
            Title = TitleEntry.Text,
            Description = DescriptionEditor.Text,
            Date = DatePicker.Date
        };

        try
        {
            App.Database.AddActivity(newActivity);
            await DisplayAlert("Succes", "Activiteit toegevoegd!", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Kon activiteit niet toevoegen: {ex.Message}", "OK");
        }
    }

    private async void OnCancelButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
