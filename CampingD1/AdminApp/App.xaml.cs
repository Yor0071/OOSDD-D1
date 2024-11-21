using Database;

namespace AdminApp
{
    public partial class App : Application
    {
        public static DatabaseQueryHandler Database { get; private set; }
        private DatabaseHandler databaseHandler;
        public App()
        {
            
            
            InitializeComponent();
            InitializeDatabase();
            MainPage = new AppShell();
        }
        
        private void InitializeDatabase()
        {
            databaseHandler = new DatabaseHandler();

            try
            {
                databaseHandler.Connect(
                    server: "mysql1.derrin.nl",
                    database: "mountain_campingapp",
                    user: "mountain_campingapp",
                    password: ""
                );

                Database = new DatabaseQueryHandler(databaseHandler);
            }
            catch (Exception ex)
            {
                MainPage.DisplayAlert("Error", "Failed to connect to the database: " + ex.Message, "OK");
            }
        }
    }
}
