using Database;

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
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;

        if (App.Database.LoginCheck(username, password))
        {
            await DisplayAlert("Succes", "Welkom", "Ok");

            await Shell.Current.GoToAsync("///ReservationList");
        }
        else
        {
            await DisplayAlert("Error", "Foute username of wachtwoord", "Ok");
        }
    }
}