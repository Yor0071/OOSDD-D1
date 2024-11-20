namespace Database.types;

public class CampingMap {
    public int id;
    public double cordinateX;
    public double cordinateY;
    public int campingSpotId;

    public CampingMap(int id, double cordinateX, double cordinateY, int campingSpotId) {
        this.id = id;
        this.cordinateX = cordinateX;
        this.cordinateY = cordinateY;
        this.campingSpotId = campingSpotId;
    }
}