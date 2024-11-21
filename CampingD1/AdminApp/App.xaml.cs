using Database;

namespace AdminApp
{
    public partial class App : Application
    {
        public static DatabaseQueryHandler Database { get; private set; }
        public static DatabaseHandler databaseHandler;
        public App()
        {
            
            
            InitializeDatabase();
            InitializeComponent();
            MainPage = new AppShell();
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
                    password: ""
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
