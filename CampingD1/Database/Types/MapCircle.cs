namespace Database.types;

public struct MapCircle
{
    public int id;
    public double coordinateX;
    public double coordinateY;
    public string? spotName;

    public MapCircle(int id, double coordinateX, double coordinateY, string spotName = null)
    {
        this.id = id;
        this.coordinateX = coordinateX;
        this.coordinateY = coordinateY;
        this.spotName = spotName;
    }
}