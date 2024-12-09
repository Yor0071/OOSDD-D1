using System.Data;
using MySql.Data.MySqlClient;

namespace Database;

public class DatabaseHandler {
    private MySqlConnection _connection;

    public MySqlConnection Connection => _connection;

    public void Connect(string server, string database, string user, string password) {
        string connectionString = $"Server={server};Database={database};User={user};Password={password};";

        try {
            _connection = new MySqlConnection(connectionString);
            _connection.Open();
            Console.WriteLine("connected");
        }
        catch (MySqlException ex) {
            throw new Exception("Could not connect to the database", ex);
        }
    }
    
    public void EnsureConnection()
    {
        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }
    }
    
    public DataTable ExecuteSelectQuery(string query, Dictionary<string, object> parameters = null) {
        DataTable dataTable = new DataTable();
        
        try {
            using (MySqlCommand command = new MySqlCommand(query, this.Connection)) {
                if (parameters != null) {
                    foreach (var param in parameters) {
                        command.Parameters.AddWithValue(param.Key, param.Value);
                    }
                }
        
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command)) {
                    adapter.Fill(dataTable);
                }
            }
        }
        catch (MySqlException ex) {
            throw new Exception("Error executing SELECT query", ex);
        }
        
        return dataTable;
    }
        
    public int ExecuteNonQuery(string query, Dictionary<string, object> parameters = null) {
        try {
            using (MySqlCommand command = new MySqlCommand(query, this.Connection)) {
                if (parameters != null) {
                    foreach (var param in parameters) {
                        command.Parameters.AddWithValue(param.Key, param.Value);
                    }
                }
        
                return command.ExecuteNonQuery();
            }
        }
        catch (MySqlException ex) {
            throw new Exception("Error executing non-SELECT query", ex);
        }
    }
}