namespace AdministrationApp;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
        ShowPasswordCheckbox.CheckedChanged += (s, e) =>
        {
            PasswordEntry.IsPassword = !e.Value;
        };
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Succes","welkom", "ok");

        // No login functionality for now

        await Shell.Current.GoToAsync("///ReservationList");
    }


}