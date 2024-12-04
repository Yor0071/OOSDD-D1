namespace Database.types;

public struct CampingMap
{
    public int id;
    public List<MapCircle> cirles;
    public string name;
    public int campingSpotId;

    public CampingMap(int id, List<MapCircle> cirles, string name, int campingSpotId)
    {
        this.id = id;
        this.cirles = cirles;
        this.name = name;
        this.campingSpotId = campingSpotId;
    }
}