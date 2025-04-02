using System;
using System.Collections.Generic;
using Xunit;
using Database.Types;
using System.Text.RegularExpressions;

namespace UnitTests.ReservationAppTests
{
    public class UnitTest1
    {
        private List<string> ValidateReservation(string firstName, string lastName, string phone, string email, string totalCampersText, DateTime fromDate, DateTime toDate)
        {
            List<string> errors = new List<string>();

            // Basisvalidatie (lege velden)
            if (string.IsNullOrWhiteSpace(firstName)) errors.Add("Voornaam is verplicht.");
            if (string.IsNullOrWhiteSpace(lastName)) errors.Add("Achternaam is verplicht.");
            if (string.IsNullOrWhiteSpace(phone)) errors.Add("Telefoonnummer is verplicht.");
            if (string.IsNullOrWhiteSpace(email)) errors.Add("E-mailadres is verplicht.");
            if (string.IsNullOrWhiteSpace(totalCampersText)) errors.Add("Aantal kampeerders is verplicht.");

            // E-mailvalidatie
            if (!string.IsNullOrWhiteSpace(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                errors.Add("Voer een geldig e-mailadres in.");

            // Telefoonnummer: alleen cijfers (8-15 lang)
            if (!string.IsNullOrWhiteSpace(phone) && !Regex.IsMatch(phone, @"^\d{8,15}$"))
                errors.Add("Telefoonnummer mag alleen cijfers bevatten (8-15 cijfers).");

            // Aankomst- en vertrekdatum controleren
            if (toDate <= fromDate)
                errors.Add("Vertrekdatum moet later zijn dan aankomstdatum.");

            // Aantal kampeerders moet een positief getal zijn
            if (!int.TryParse(totalCampersText, out int totalCampers) || totalCampers <= 0)
                errors.Add("Aantal kampeerders moet een positief getal zijn.");

            return errors;
        }

        [Fact]
        public void ValidateReservation_ValidInput_ReturnsNoErrors()
        {
            var errors = ValidateReservation(
                firstName: "Jim",
                lastName: "Doe",
                phone: "12345678",
                email: "Jim.doe@example.com",
                totalCampersText: "2",
                fromDate: DateTime.Today,
                toDate: DateTime.Today.AddDays(1)
            );
            Assert.Empty(errors);
        }

        [Fact]
        public void ValidateReservation_MissingFields_ReturnsErrors()
        {
            var errors = ValidateReservation(
                firstName: "",
                lastName: "",
                phone: "",
                email: "",
                totalCampersText: "",
                fromDate: DateTime.Today,
                toDate: DateTime.Today.AddDays(1)
            );

            Assert.Contains("Voornaam is verplicht.", errors);
            Assert.Contains("Achternaam is verplicht.", errors);
            Assert.Contains("Telefoonnummer is verplicht.", errors);
            Assert.Contains("E-mailadres is verplicht.", errors);
            Assert.Contains("Aantal kampeerders is verplicht.", errors);
        }

        [Fact]
        public void ValidateReservation_InvalidEmail_ReturnsError()
        {
            var errors = ValidateReservation(
                firstName: "Jim",
                lastName: "Doe",
                phone: "12345678",
                email: "invalidemail",
                totalCampersText: "2",
                fromDate: DateTime.Today,
                toDate: DateTime.Today.AddDays(1)
            );
            Assert.Contains("Voer een geldig e-mailadres in.", errors);
        }

        [Fact]
        public void ValidateReservation_InvalidPhone_ReturnsError()
        {
            var errors = ValidateReservation(
                firstName: "Jim",
                lastName: "Doe",
                phone: "1234",
                email: "Jim.doe@example.com",
                totalCampersText: "2",
                fromDate: DateTime.Today,
                toDate: DateTime.Today.AddDays(1)
            );
            Assert.Contains("Telefoonnummer mag alleen cijfers bevatten (8-15 cijfers).", errors);
        }

        [Fact]
        public void ValidateReservation_InvalidDates_ReturnsError()
        {
            var errors = ValidateReservation(
                firstName: "Jim",
                lastName: "Doe",
                phone: "12345678",
                email: "Jim.doe@example.com",
                totalCampersText: "2",
                fromDate: DateTime.Today,
                toDate: DateTime.Today
            );
            Assert.Contains("Vertrekdatum moet later zijn dan aankomstdatum.", errors);
        }

        [Fact]
        public void ValidateReservation_InvalidTotalCampers_ReturnsError()
        {
            var errors = ValidateReservation(
                firstName: "Jim",
                lastName: "Doe",
                phone: "12345678",
                email: "Jim.doe@example.com",
                totalCampersText: "0",
                fromDate: DateTime.Today,
                toDate: DateTime.Today.AddDays(1)
            );
            Assert.Contains("Aantal kampeerders moet een positief getal zijn.", errors);

            errors = ValidateReservation(
                firstName: "Jim",
                lastName: "Doe",
                phone: "12345678",
                email: "Jim.doe@example.com",
                totalCampersText: "-1",
                fromDate: DateTime.Today,
                toDate: DateTime.Today.AddDays(1)
            );
            Assert.Contains("Aantal kampeerders moet een positief getal zijn.", errors);

            errors = ValidateReservation(
                firstName: "Jim",
                lastName: "Doe",
                phone: "12345678",
                email: "Jim.doe@example.com",
                totalCampersText: "abc",
                fromDate: DateTime.Today,
                toDate: DateTime.Today.AddDays(1)
            );
            Assert.Contains("Aantal kampeerders moet een positief getal zijn.", errors);
        }

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
}