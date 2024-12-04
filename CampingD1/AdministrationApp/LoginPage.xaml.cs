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

        if (App.Database == null)
        {
            await DisplayAlert("Error", "Database is not connected. Please try again later.", "OK");
            return;
        }


        if (App.Database.LoginCheck(username, password))
        {
            Application.Current.MainPage = new AppShell();

            //await Shell.Current.GoToAsync("//ReservationList");
        }
        else
        {
            await DisplayAlert("Error", "De gebruikersnaam of het wachtwoord is onjuist. Voer de gebruikersnaam en het wachtwoord opnieuw in.", "Ok");
        }
    }

    private void PasswordEntry_Completed(object sender, EventArgs e)
    {
        OnLoginClicked(sender, e);
    }
}