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
            DataTable result = _databaseHandler.ExecuteSelectQuery(query);

            foreach (DataRow row in result.Rows) {
                int id = Convert.ToInt32(row["id"]);
                double coordinateX = Convert.ToDouble(row["coordinate_x"]);
                double coordinateY = Convert.ToDouble(row["coordinate_y"]);
                int campingSpotId = Convert.ToInt32(row["camping_spot"]);
                
                CampingMap campingApp = new CampingMap(id, coordinateX, coordinateY, campingSpotId);
                campingMaps.Add(campingApp);
            }
        }
        catch (Exception ex) {
            throw new Exception($"Error fetching camping maps: {ex.Message}");
        }

        return campingMaps;
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

                Reservation reservation = new Reservation(id, firstName, lastName, campingSpot, fromDate, toDate);
                reservations.Add(reservation);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error fetching reservations: {ex.Message}");
        }

        return reservations;
    }
}