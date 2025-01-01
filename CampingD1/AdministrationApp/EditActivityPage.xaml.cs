using System;
using Microsoft.Maui.Controls;
using Database.Types;

namespace AdministrationApp;

public partial class EditActivityPage : ContentPage
{
    private Activity _activity;

    public EditActivityPage(Activity activity)
    {
        InitializeComponent();
        _activity = activity;

        // Populate fields with existing activity data
        TitleEntry.Text = _activity.Title;
        DescriptionEditor.Text = _activity.Description;
        DatePicker.Date = _activity.Date;
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        // Update activity properties
        _activity.Title = TitleEntry.Text;
        _activity.Description = DescriptionEditor.Text;
        _activity.Date = DatePicker.Date;

        try
        {
            App.Database.UpdateActivity(_activity); // Zorg dat deze methode bestaat
            await DisplayAlert("Succes", "Activiteit succesvol bijgewerkt.", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Opslaan mislukt: {ex.Message}", "OK");
        }
    }

    private async void OnCancelButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
