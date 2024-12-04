using System.Data;
using Database.types;
using Database.Types;

namespace Database;

public class DatabaseQueryHandler {
    private readonly DatabaseHandler _databaseHandler;

    public DatabaseQueryHandler(DatabaseHandler databaseHandler) {
        _databaseHandler = databaseHandler;
    }
    
    public List<CampingMap> SelectCampingMaps() {
        string query = "SELECT * FROM maps ORDER BY id;";
        List<CampingMap> campingMaps = new List<CampingMap>();

        try {
            DataTable mapsResult = _databaseHandler.ExecuteSelectQuery(query);

            foreach (DataRow mapRow in mapsResult.Rows) {
                int mapId = Convert.ToInt32(mapRow["id"]);
                string name = Convert.ToString(mapRow["name"]);
                int campingSpotId = Convert.ToInt32(mapRow["camping_spot"]);
                
                List<MapCircle> mapCircles = SelectMapCircles(mapId);

                CampingMap campingMap = new CampingMap(mapId, mapCircles, name, campingSpotId);
                campingMaps.Add(campingMap);
            }
        }
        catch (Exception ex) {
            throw new Exception($"Error fetching camping maps: {ex.Message}");
        }

        return campingMaps;
    }
    
    public List<MapCircle> SelectMapCircles(int mapId) {
        string query = $"SELECT * FROM map_circles WHERE map = {mapId};";
        List<MapCircle> mapCircles = new List<MapCircle>();

        try {
            DataTable result = _databaseHandler.ExecuteSelectQuery(query);

            foreach (DataRow row in result.Rows) {
                int id = Convert.ToInt32(row["id"]);
                double coordinateX = Convert.ToDouble(row["coordinate_x"]);
                double coordinateY = Convert.ToDouble(row["coordinate_y"]);

                MapCircle mapCircle = new MapCircle(id, coordinateX, coordinateY);
                mapCircles.Add(mapCircle);
            }
        }
        catch (Exception ex) {
            throw new Exception($"Error fetching map circles for mapId {mapId}: {ex.Message}");
        }

        return mapCircles;
    }
    
    public List<Reservation> SelectReservations()
    {
        string query = "SELECT * FROM reservations ORDER BY id;";
        List<Reservation> reservations = new List<Reservation>();

        try
        {
            DataTable result = _databaseHandler.ExecuteSelectQuery(query);

            foreach (DataRow row in result.Rows)
            {
                int id = Convert.ToInt32(row["id"]);
                string firstName = row["firstname"].ToString();
                string lastName = row["lastname"].ToString();
                int campingSpot = Convert.ToInt32(row["camping_spot"]);
                DateTime fromDate = Convert.ToDateTime(row["from"]);
                DateTime toDate = Convert.ToDateTime(row["to"]);
                string phone = row["phone"].ToString();
                string email = row["email"].ToString();

                Reservation reservation = new Reservation(id, firstName, lastName, campingSpot, fromDate, toDate, phone, email);
                reservations.Add(reservation);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error fetching reservations: {ex.Message}");
        }

        return reservations;
    }

    public bool LoginCheck(string username, string password)
    {
        string query = "select * from admin_accounts where username = @username && PASSWORD = @password LIMIT 1;";

        try
        {
            var parameters = new Dictionary<string, object>
            {
                {"@username", username},
                {"@password", password}
            };

            var result = _databaseHandler.ExecuteSelectQuery(query, parameters);

            if (result.Rows.Count > 0)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error trying to login: {ex.Message}");
        }

        return false;
    }

    public List<CampingSpot> SelectCampingSpots()
    {
        string query = "SELECT * FROM camping_spots ORDER BY id;";
        List<CampingSpot> campingSpots = new List<CampingSpot>();

        try
        {
            DataTable result = _databaseHandler.ExecuteSelectQuery(query);

            foreach (DataRow row in result.Rows)
            {
                int id = Convert.ToInt32(row["id"]);
                string description = row["description"].ToString();
                double surface_m2 = Convert.ToDouble(row["surface_m2"]);
                bool power = Convert.ToBoolean(row["power"]);
                bool water = Convert.ToBoolean(row["water"]);
                bool wifi = Convert.ToBoolean(row["wifi"]);
                int max_persons = Convert.ToInt32(row["max_persons"]);
                double price_m2 = Convert.ToDouble(row["price_m2"]);
                bool available = Convert.ToBoolean(row["available"]);

                CampingSpot campingSpot = new CampingSpot(id, description, surface_m2, power, water, wifi, max_persons, price_m2, available);
                campingSpots.Add(campingSpot);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error fetching camping spots: {ex.Message}");
        }

        return campingSpots;
    }
}