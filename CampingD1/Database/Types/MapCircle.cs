namespace Database.types;

public class MapCircle {
    public int id;
    public double coordinateX;
    public double coordinateY;

    public MapCircle(int id, double coordinateX, double coordinateY) {
        this.id = id;
        this.coordinateX = coordinateX;
        this.coordinateY = coordinateY;
    }
}