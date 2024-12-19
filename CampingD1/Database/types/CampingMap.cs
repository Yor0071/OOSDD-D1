namespace Database.types;

public struct CampingMap
{
    public int id;
    public List<MapCircle> cirles;
    public string name;
    public bool isPrimary;
    public string? backgroundImage; // Nullable field for the background image

    public CampingMap(int id, List<MapCircle> cirles, string name, bool isPrimary = false, string? backgroundImage = null)
    {
        this.id = id;
        this.cirles = cirles;
        this.name = name;
        this.isPrimary = isPrimary;
        this.backgroundImage = backgroundImage;
    }
}