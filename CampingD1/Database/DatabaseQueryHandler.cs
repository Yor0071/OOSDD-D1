using System.Data;
using System.Data.SqlTypes;
using System.Resources;
using Database.types;
using Database.Types;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using Org.BouncyCastle.Asn1;

namespace Database;

public class DatabaseQueryHandler {
    private readonly DatabaseHandler _databaseHandler;

    public DatabaseQueryHandler(DatabaseHandler databaseHandler) {
        _databaseHandler = databaseHandler;
    }

    public List<CampingMap> SelectCampingMaps() {
        string query = "SELECT * FROM maps ORDER BY id;";
        //Console.WriteLine(query);
        List<CampingMap> campingMaps = new List<CampingMap>();

        try
        {
            DataTable mapsResult = _databaseHandler.ExecuteSelectQuery(query);

            foreach (DataRow mapRow in mapsResult.Rows)
            {
                int mapId = Convert.ToInt32(mapRow["id"]);
                string name = Convert.ToString(mapRow["name"]);
                bool isPrimary = Convert.ToBoolean(mapRow["is_primary"]);


                List<MapCircle> mapCircles = SelectMapCircles(mapId);

                CampingMap campingMap = new CampingMap(mapId, mapCircles, name, isPrimary);
                campingMaps.Add(campingMap);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error fetching camping maps: {ex.Message}");
        }

        return campingMaps;
    }

    public List<MapCircle> SelectMapCircles(int mapId)
    {
        string query = @"
        SELECT 
            map_circles.id,
            map_circles.coordinate_x,
            map_circles.coordinate_y,
            camping_spots.spot_name 
        FROM 
            map_circles
        LEFT JOIN 
            camping_spots 
        ON 
            map_circles.camping_spot = camping_spots.id
        WHERE 
            map_circles.map = @mapId;";

        List<MapCircle> mapCircles = new List<MapCircle>();

        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@mapId", mapId }
            };

            DataTable result = _databaseHandler.ExecuteSelectQuery(query, parameters);

            foreach (DataRow row in result.Rows)
            {
                int id = Convert.ToInt32(row["id"]);
                double coordinateX = Convert.ToDouble(row["coordinate_x"]);
                double coordinateY = Convert.ToDouble(row["coordinate_y"]);
                string spotName = row["spot_name"] != DBNull.Value ? row["spot_name"].ToString() : null;
                Console.WriteLine(spotName);

                MapCircle mapCircle = new MapCircle(id, coordinateX, coordinateY, spotName);
                mapCircles.Add(mapCircle);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error fetching map circles for mapId {mapId}: {ex.Message}");
        }

        return mapCircles;
    }
    public void AddReservation(Reservation reservation)
    {
        string query = @"
    INSERT INTO reservations (firstname, lastname, camping_spot, `from`, `to`, phone, email) 
    VALUES (@firstname, @lastname, @campingSpot, @fromDate, @toDate, @phone, @email);
    ";

        var parameters = new Dictionary<string, object>
    {
        { "@firstname", reservation.FirstName },
        { "@lastname", reservation.LastName },
        { "@campingSpot", reservation.PlaceNumber },
        { "@fromDate", reservation.Arrival },
        { "@toDate", reservation.Depart },
        { "@phone", reservation.PhoneNumber },
        { "@email", reservation.Email }
    };

        try
        {
            _databaseHandler.ExecuteNonQuery(query, parameters);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error adding reservation: {ex.Message}");
        }
    }

    public bool SetPrimaryMap(int mapId)
    {
        try
        {
            // Step 1: Set `is_primary` to 0 for all maps
            string resetPrimaryQuery = "UPDATE maps SET is_primary = 0;";
            _databaseHandler.ExecuteNonQuery(resetPrimaryQuery);

            // Step 2: Set `is_primary` to 1 for the given mapId
            string setPrimaryQuery = "UPDATE maps SET is_primary = 1 WHERE id = @mapId;";
            var parameters = new Dictionary<string, object>
            {
                { "@mapId", mapId }
            };
            int rowsAffected = _databaseHandler.ExecuteNonQuery(setPrimaryQuery, parameters);

            // Return true if the specific map was successfully updated
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error setting primary map with ID {mapId}: {ex.Message}", ex);
        }
    }
    public List<Reservation> SelectReservations() {
        string query = "SELECT * FROM reservations ORDER BY id;";
        List<Reservation> reservations = new List<Reservation>();

        try {
            DataTable result = _databaseHandler.ExecuteSelectQuery(query);

            foreach (DataRow row in result.Rows) {
                int id = Convert.ToInt32(row["id"]);
                string firstName = row["firstname"].ToString();
                string lastName = row["lastname"].ToString();
                int campingSpot = Convert.ToInt32(row["camping_spot"]);
                DateTime fromDate = Convert.ToDateTime(row["from"]);
                DateTime toDate = Convert.ToDateTime(row["to"]);
                string phone = row["phone"].ToString();
                string email = row["email"].ToString();
                ReservationStatus status = System.Enum.Parse<ReservationStatus>((string)row["status"]);

                Reservation reservation =
                    new Reservation(id, firstName, lastName, campingSpot, fromDate, toDate, phone, email, status);
                reservations.Add(reservation);
            }
        }
        catch (Exception ex) {
            throw new Exception($"Error fetching reservations: {ex.Message}");
        }

        return reservations;
    }

    public List<Reservation> SelectFilteredReservations(string nameFilter, int? spotFilter, string emailFilter, string fromDateFilter, string toDateFilter)
    {
        string query = @"SELECT * FROM reservations WHERE 1 = 1";
        if (!string.IsNullOrEmpty(nameFilter))
        {
            query += " AND (firstname LIKE @nameFilter OR lastname LIKE @nameFilter)";
        }

        if (spotFilter.HasValue)
        {
            query += " AND camping_spot = @campingSpot";
        }

        if (!string.IsNullOrEmpty(emailFilter))
        {
            query += " AND email LIKE @emailFilter";
        }

        if (!string.IsNullOrEmpty(fromDateFilter))
        {
            query += " AND `from` >= @fromDateFilter";
        }

        if (!string.IsNullOrEmpty(toDateFilter))
        {
            query += " AND `to` <= @toDateFilter";
        }

       

        query += " ORDER BY id;";

        List<Reservation> reservations = new List<Reservation>();

        var parameters = new Dictionary<string, object>
        {
            { "@nameFilter", $"%{nameFilter}%" },
            { "@campingSpot", spotFilter },
            {"@emailFilter", $"%{emailFilter}%" },
            { "@fromDateFilter", fromDateFilter },
            { "@toDateFilter", toDateFilter }
        };

        try
        {
            DataTable result = _databaseHandler.ExecuteSelectQuery(query, parameters);

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
        string query = "select * from admin_accounts where binary username = @username && binary PASSWORD = @password LIMIT 1;";

        try {
            var parameters = new Dictionary<string, object> {
                { "@username", username },
                { "@password", password }
            };

            var result = _databaseHandler.ExecuteSelectQuery(query, parameters);

            if (result.Rows.Count > 0) {
                return true;
            }
        }
        catch (Exception ex) {
            throw new Exception($"Error trying to login: {ex.Message}");
        }

        return false;
    }

    public List<CampingSpot> SelectCampingSpots() {
        string query = "SELECT * FROM camping_spots ORDER BY id;";
        List<CampingSpot> campingSpots = new List<CampingSpot>();

        try {
            DataTable result = _databaseHandler.ExecuteSelectQuery(query);

            foreach (DataRow row in result.Rows) {
                int id = Convert.ToInt32(row["id"]);
                string description = row["description"].ToString();
                double surface_m2 = Convert.ToDouble(row["surface_m2"]);
                bool power = Convert.ToBoolean(row["power"]);
                bool water = Convert.ToBoolean(row["water"]);
                bool wifi = Convert.ToBoolean(row["wifi"]);
                int max_persons = Convert.ToInt32(row["max_persons"]);
                double price_m2 = Convert.ToDouble(row["price_m2"]);
                bool available = Convert.ToBoolean(row["available"]);

                CampingSpot campingSpot = new CampingSpot(id, description, surface_m2, power, water, wifi, max_persons,
                    price_m2, available);
                campingSpots.Add(campingSpot);
            }
        }
        catch (Exception ex) {
            throw new Exception($"Error fetching camping spots: {ex.Message}");
        }

        return campingSpots;
    }

    public void UpdateReservation(Reservation reservation)
    {
        string query = @"UPDATE reservations SET firstname = @firstname, lastname = @lastname, camping_spot = @placeNumber, `from` = @fromDate, `to` = @toDate, phone = @phone, email = @email, status = @status WHERE id = @id;";

        var parameters = new Dictionary<string, object>
        {
            { "@firstname", reservation.FirstName },
            { "@lastname", reservation.LastName },
            { "@placeNumber", reservation.PlaceNumber },
            { "@fromDate", reservation.Arrival },
            { "@toDate", reservation.Depart },
            { "@phone", reservation.PhoneNumber },
            { "@email", reservation.Email },
            { "@status", reservation.Status.ToString() },
            { "@id", reservation.Id }
        };

        try
        {
            _databaseHandler.ExecuteNonQuery(query, parameters);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating reservation: {ex.Message}");
        }
    }
    
    public void AddCampingMap(string mapName, List<MapCircle> mapCircles) {
        try {
            _databaseHandler.EnsureConnection();

            // Step 1: Fetch a default camping_spot ID
            string getDefaultCampingSpotQuery = "SELECT id FROM camping_spots LIMIT 1;";
            DataTable campingSpotTable = _databaseHandler.ExecuteSelectQuery(getDefaultCampingSpotQuery);
            if (campingSpotTable.Rows.Count == 0) {
                throw new Exception("No camping spots available to assign to the map.");
            }

            // Step 2: Insert new map into the "maps" table
            string insertMapQuery = "INSERT INTO maps (name) VALUES (@name);";
            var mapParameters = new Dictionary<string, object> {
                { "@name", mapName },
            };
            _databaseHandler.ExecuteNonQuery(insertMapQuery, mapParameters);

            // Step 3: Retrieve the last inserted map ID
            string getLastInsertIdQuery = "SELECT LAST_INSERT_ID();";
            DataTable mapIdTable = _databaseHandler.ExecuteSelectQuery(getLastInsertIdQuery);
            if (mapIdTable.Rows.Count == 0) {
                throw new Exception("Failed to retrieve the last inserted map ID.");
            }

            int mapId = Convert.ToInt32(mapIdTable.Rows[0][0]);

            // Step 4: Insert MapCircles into the "map_circles" table
            foreach (var circle in mapCircles) {
                Console.WriteLine("d");
                Console.WriteLine(circle.spotName);
                string getCampingSpotIdQuery = "SELECT id FROM camping_spots WHERE spot_name = @spotName;";
                var campingSpotParams = new Dictionary<string, object> {
                    { "@spotName", circle.spotName }
                };
                DataTable campingSpotResult =
                    _databaseHandler.ExecuteSelectQuery(getCampingSpotIdQuery, campingSpotParams);
                Console.WriteLine("testje");
                Console.WriteLine(campingSpotResult);
                if (campingSpotResult.Rows.Count == 0) {
                    throw new Exception($"Camping spot '{circle.spotName}' not found.");
                }

                int campingSpotId = Convert.ToInt32(campingSpotResult.Rows[0][0]);

                string insertCircleQuery = @"
                INSERT INTO map_circles (coordinate_x, coordinate_y, map, camping_spot) 
                VALUES (@coordinateX, @coordinateY, @mapId, @campingSpotId);";
                var circleParams = new Dictionary<string, object> {
                    { "@coordinateX", circle.coordinateX },
                    { "@coordinateY", circle.coordinateY },
                    { "@mapId", mapId },
                    { "@campingSpotId", campingSpotId }
                };
                _databaseHandler.ExecuteNonQuery(insertCircleQuery, circleParams);
            }
        }
        catch (Exception ex) {
            throw new Exception("Error adding camping map and circles.", ex);
        }
    }

    public void EditCampingMap(int mapId, string mapName, List<MapCircle> mapCircles) {
        try {
            _databaseHandler.EnsureConnection();

            // Step 1: Update the map name in the "maps" table
            string updateMapQuery = "UPDATE maps SET name = @name WHERE id = @mapId;";
            var mapParameters = new Dictionary<string, object> {
                { "@name", mapName },
                { "@mapId", mapId }
            };
            int rowsAffected = _databaseHandler.ExecuteNonQuery(updateMapQuery, mapParameters);
            if (rowsAffected == 0) {
                throw new Exception($"Map with ID {mapId} not found.");
            }

            // Step 2: Delete existing map circles for the given map
            string deleteCirclesQuery = "DELETE FROM map_circles WHERE map = @mapId;";
            var deleteParams = new Dictionary<string, object> {
                { "@mapId", mapId }
            };
            _databaseHandler.ExecuteNonQuery(deleteCirclesQuery, deleteParams);

            // Step 3: Insert the updated map circles into the "map_circles" table
            foreach (var circle in mapCircles) {
                string getCampingSpotIdQuery = "SELECT id FROM camping_spots WHERE spot_name = @spotName;";
                var campingSpotParams = new Dictionary<string, object> {
                    { "@spotName", circle.spotName }
                };
                DataTable campingSpotResult =
                    _databaseHandler.ExecuteSelectQuery(getCampingSpotIdQuery, campingSpotParams);
                if (campingSpotResult.Rows.Count == 0) {
                    throw new Exception($"Camping spot '{circle.spotName}' not found.");
                }

                int campingSpotId = Convert.ToInt32(campingSpotResult.Rows[0][0]);

                string insertCircleQuery = @"
                INSERT INTO map_circles (coordinate_x, coordinate_y, map, camping_spot) 
                VALUES (@coordinateX, @coordinateY, @mapId, @campingSpotId);";
                var circleParams = new Dictionary<string, object> {
                    { "@coordinateX", circle.coordinateX },
                    { "@coordinateY", circle.coordinateY },
                    { "@mapId", mapId },
                    { "@campingSpotId", campingSpotId }
                };
                _databaseHandler.ExecuteNonQuery(insertCircleQuery, circleParams);
            }
        }
        catch (Exception ex) {
            throw new Exception($"Error editing camping map with ID {mapId}: {ex.Message}", ex);
        }
    }
    
    public void DeleteMap(int mapId)
    {
        try
        {
            _databaseHandler.EnsureConnection();

            // Step 1: Delete associated circles from map_circles
            string deleteCirclesQuery = "DELETE FROM map_circles WHERE map = @mapId;";
            var deleteCirclesParams = new Dictionary<string, object>
            {
                { "@mapId", mapId }
            };
            _databaseHandler.ExecuteNonQuery(deleteCirclesQuery, deleteCirclesParams);

            // Step 2: Delete the map from maps table
            string deleteMapQuery = "DELETE FROM maps WHERE id = @mapId;";
            var deleteMapParams = new Dictionary<string, object>
            {
                { "@mapId", mapId }
            };
            _databaseHandler.ExecuteNonQuery(deleteMapQuery, deleteMapParams);

            Console.WriteLine($"Map with ID {mapId} and its associated circles have been deleted.");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting map with ID {mapId}: {ex.Message}", ex);
        }
    }

    public void UpdateCampingSpot(CampingSpot campingSpot)
    {
        string query = @"UPDATE camping_spots 
                         SET description = @description,
                         surface_m2 = @surfaceM2,
                         power = @power,
                         water = @water,
                         wifi = @wifi,
                         max_persons = @maxPersons,
                         price_m2 = @priceM2,
                         available = @available
                         WHERE id = @id;";

        var parameters = new Dictionary<string, object>
        {
            {"@description", campingSpot.Description},
            {"@surfaceM2", campingSpot.Surface_m2},
            {"@power", campingSpot.Power},
            {"@water", campingSpot.Water},
            {"@wifi", campingSpot.Wifi},
            {"@maxPersons", campingSpot.MaxPersons},
            {"@priceM2", campingSpot.Price_m2},
            {"@available", campingSpot.Available},
            {"@id", campingSpot.Id}
        };

        try
        {
            _databaseHandler.ExecuteNonQuery(query, parameters);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating camping spot: {ex.Message}");
        }
    }

    public void DeleteReservation(int reservationId)
    {
        string query = "DELETE FROM reservations WHERE id = @id;";

        var parameters = new Dictionary<string, object>
    {
        { "@id", reservationId }
    };

        try
        {
            _databaseHandler.ExecuteNonQuery(query, parameters);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting reservation with ID {reservationId}: {ex.Message}");
        }
    }

}