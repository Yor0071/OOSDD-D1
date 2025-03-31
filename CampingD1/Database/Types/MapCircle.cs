namespace Database.types
{
    public struct MapCircle
    {
        public int id;
        public double coordinateX;
        public double coordinateY;
        public string? spotName;
        public int CampingSpotId;
        
        public MapCircle(int id, double coordinateX, double coordinateY, int campingSpotId, string spotName = null)
        {
            this.id = id;
            this.coordinateX = coordinateX;
            this.coordinateY = coordinateY;
            this.spotName = spotName;
            this.CampingSpotId = campingSpotId;
        }
    }
}
