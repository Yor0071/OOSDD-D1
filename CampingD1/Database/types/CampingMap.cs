namespace Database.types;

public struct CampingMap
{
    public int id;
    public List<MapCircle> cirles;
    public string name;
    public bool isPrimary;

    public CampingMap(int id, List<MapCircle> cirles, string name, bool isPrimary = false)
    {
        this.id = id;
        this.cirles = cirles;
        this.name = name;
        this.isPrimary = isPrimary;
    }
}