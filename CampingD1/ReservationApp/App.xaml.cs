using Database;
namespace ReservationApp
{
    public partial class App : Application
    {
        public static DatabaseQueryHandler Database { get; private set; }
        public static DatabaseHandler databaseHandler;

        // Constructor van de App
        public App()
        {
            InitializeDatabase();  // Initializeert de database
            InitializeComponent();

            // Zet de MainPage als AppShell, je zou hier een NavigationPage kunnen gebruiken
            MainPage = new AppShell();
        }

        // Methode om de databaseverbinding te initialiseren
        private void InitializeDatabase()
        {
            databaseHandler = new DatabaseHandler();

            try
            {
                // Verbindt met de database via MySQL
                databaseHandler.Connect(
                    server: "mysql2.derrin.nl",
                    database: "mountain_campingapp",
                    user: "mountain_campingapp",
                    password: "iWN4RVoC8jPLfKVNuGfANgj8yX_.e8x6KMzLh7UE!XR7FAeg"
                );

                // Maak de databasequeryhandler aan
                Database = new DatabaseQueryHandler(databaseHandler);
            }
            catch (Exception ex)
            {
                // Hier zou je een alert kunnen tonen in geval van een fout (optioneel)
                // MainPage.DisplayAlert("Error", "Failed to connect to the database: " + ex.Message, "OK");
            }
        }

        // Methode om een specifieke campingplek te reserveren
        public static void ReserveSpot(int spotId)
        {
            // Haal de campingplek details op uit de database
            var spotDetails = Database.SelectCampingSpots().FirstOrDefault(s => s.Id == spotId);

            // Check of de plek bestaat
            if (spotDetails != null)
            {
                // Maak de reserveringspagina aan en navigeer er naartoe
                Application.Current.MainPage.Navigation.PushAsync(new ReservationPage(spotDetails));
            }
            else
            {
                // Geef een foutmelding als de plek niet gevonden werd (optioneel)
                Application.Current.MainPage.DisplayAlert("Error", "Camping spot not found", "OK");
            }
        }
    }
}
