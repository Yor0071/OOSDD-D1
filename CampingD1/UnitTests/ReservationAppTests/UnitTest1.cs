using Database.Types;

namespace UnitTests.ReservationAppTests;

public class UnitTest1 {
    [Fact]
    public void TestIsCampingSpotAvailable_WithNoConflicts_ShouldReturnTrue()
    {
        // Arrange: Setup a MapScreenCustomer and a list of reservations
        var mapScreenCustomer = new MapScreenCustomer();
        var reservations = new List<Reservation>
            {
                new Reservation
                {
                    PlaceNumber = 1,
                    Arrival = DateTime.Today.AddDays(3),
                    Depart = DateTime.Today.AddDays(5)
                }
            };

        // Act: Check if the spot is available
        bool isAvailable = mapScreenCustomer.IsCampingSpotAvailable(2, DateTime.Today.AddDays(1), DateTime.Today.AddDays(2), reservations);

        // Assert: The spot should be available because no conflict exists
        Assert.True(isAvailable);
    }
}