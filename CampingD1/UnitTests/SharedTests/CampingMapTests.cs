using Database.types;

namespace UnitTests.SharedTests;

public class CampingMapTests
{
    [Fact]
    public void CampingMap_ShouldStoreValuesCorrectly()
    {
        // Arrange
        var circles = new List<MapCircle>
        {
            new MapCircle(1, 10.5, 20.5, 100, "Spot A"),
            new MapCircle(2, 15.0, 25.0, 101, "Spot B"),
            new MapCircle(3, 18.2, 28.7, 102, "Spot C")
        };

        var campingMap = new CampingMap(1, circles, "Main Map", true, "background.jpg");

        // Act & Assert
        Assert.Equal(1, campingMap.id);
        Assert.Equal("Main Map", campingMap.name);
        Assert.True(campingMap.isPrimary);
        Assert.Equal("background.jpg", campingMap.backgroundImage);
        Assert.Equal(3, campingMap.cirles.Count);
        Assert.Equal(1, campingMap.cirles[0].id);
        Assert.Equal("Spot A", campingMap.cirles[0].spotName);
        Assert.Equal(10.5, campingMap.cirles[0].coordinateX);
        Assert.Equal(20.5, campingMap.cirles[0].coordinateY);
        Assert.Equal(100, campingMap.cirles[0].CampingSpotId);
    }
}