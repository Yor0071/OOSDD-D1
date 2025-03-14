using Database.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.SharedTests
{
    public class ReservationTests
    {
        // Begin of tests for the "ReservationStatus" enum
        [Theory]
        [InlineData(ReservationStatus.cancelled, "Geannuleerd")]
        [InlineData(ReservationStatus.repaid, "Terugbetaald")]
        [InlineData(ReservationStatus.ongoing, "Lopend")]
        [InlineData(ReservationStatus.awaiting, "In afwachting")]
        [InlineData(ReservationStatus.finished, "Afgerond")]
        public void Translate_KnownStatus_ReturnsCorrectTranslation(ReservationStatus status, string expectedTranslation)
        {
            // Act
            string result = Reservation.Translate(status);

            // Assert
            Assert.Equal(expectedTranslation, result);
        }

        [Fact]
        public void Translate_UnknownStatus_ReturnsFallback()
        {
            // Arrange
            ReservationStatus unknownStatus = (ReservationStatus)999;

            // Act
            string result = Reservation.Translate(unknownStatus);

            // Assert
            Assert.Equal("Onbekend", result);
        }
        // End of tests for the "ReservationStatus" enum
    }
}
