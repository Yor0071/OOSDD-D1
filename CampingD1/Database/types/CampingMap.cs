namespace Database.types;

public struct CampingMap
{
    public int id;
    public List<MapCircle> cirles;
    public string name;

    public CampingMap(int id, List<MapCircle> cirles, string name)
    {
        this.id = id;
        this.cirles = cirles;
        this.name = name;
    }
}