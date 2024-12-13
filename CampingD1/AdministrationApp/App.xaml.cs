using Database;
using System.Globalization;

namespace AdministrationApp
{
    public partial class App : Application
    {
        public static DatabaseQueryHandler Database { get; private set; }
        public static DatabaseHandler databaseHandler;
        public App()
        {
            InitializeDatabase();
            InitializeComponent();
            MainPage = new LoginPage();
        }

        private void InitializeDatabase()
        {
            databaseHandler = new DatabaseHandler();

            try
            {
                databaseHandler.Connect(
                    server: "mysql2.derrin.nl",
                    database: "mountain_campingapp",
                    user: "mountain_campingapp",
                    password: "iWN4RVoC8jPLfKVNuGfANgj8yX_.e8x6KMzLh7UE!XR7FAeg"
                );

                Database = new DatabaseQueryHandler(databaseHandler);
            }
            catch (Exception ex)
            {
                // MainPage.DisplayAlert("Error", "Failed to connect to the database: " + ex.Message, "OK");
            }
        }
    }
}