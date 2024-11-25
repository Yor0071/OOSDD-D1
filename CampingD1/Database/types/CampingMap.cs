namespace Database.types;

public class CampingMap {
    public int id;
    public List<MapCircle> cirles;
    public int campingSpotId;

    public CampingMap(int id, List<MapCircle> cirles, int campingSpotId) {
        this.id = id;
        this.cirles = cirles;
        this.campingSpotId = campingSpotId;
    }
}