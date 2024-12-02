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
                int campingSpotId = Convert.ToInt32(mapRow["camping_spot"]);
                
                List<MapCircle> mapCircles = SelectMapCircles(mapId);

                CampingMap campingMap = new CampingMap(mapId, mapCircles, campingSpotId);
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
}