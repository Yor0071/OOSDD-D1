namespace Database.types;

public class CampingMap {
    public int id;
    public double coordinateX;
    public double coordinateY;
    public int campingSpotId;

    public CampingMap(int id, double coordinateX, double coordinateY, int campingSpotId) {
        this.id = id;
        this.coordinateX = coordinateX;
        this.coordinateY = coordinateY;
        this.campingSpotId = campingSpotId;
    }
}