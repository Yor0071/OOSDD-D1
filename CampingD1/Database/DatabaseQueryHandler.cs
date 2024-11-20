using System.Data;
using Database.types;

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
                double cordinateX = Convert.ToDouble(row["cordinate_x"]);
                double cordinateY = Convert.ToDouble(row["cordinate_y"]);
                int campingSpotId = Convert.ToInt32(row["camping_spot"]);
                
                CampingMap campingApp = new CampingMap(id, cordinateX, cordinateY, campingSpotId);
                campingMaps.Add(campingApp);
            }
        }
        catch (Exception ex) {
            throw new Exception($"Error fetching high scores: {ex.Message}");
        }

        return campingMaps;
    }
}