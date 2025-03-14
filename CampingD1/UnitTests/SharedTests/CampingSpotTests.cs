using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Types;

namespace UnitTests.SharedTests
{
    public class CampingSpotTests
    {
        [Fact]
        public async Task ValidateEmptyFields_AllFieldsEmpty_ReturnsTrueAndShowsError()
        {
            // Arrange
            string description = "";
            string spotName = "";
            string surface = "";
            string maxPersons = "";
            string price = "";

            bool alertWasCalled = false;
            string alertTitle = "";
            string alertMessage = "";

            Func<string, string, Task> mockDisplayAlert = (title, message) =>
            {
                alertWasCalled = true;
                alertTitle = title;
                alertMessage = message;
                return Task.CompletedTask;
            };

            // Act
            bool result = await CampingSpot.ValidateEmptyFields(description, spotName, surface, maxPersons, price, mockDisplayAlert);

            // Assert
            Assert.True(result);
            Assert.True(alertWasCalled);
            Assert.Equal("Fout", alertTitle);
            Assert.Equal("Alle velden zijn leeg. Vul de vereiste gegevens in.", alertMessage);
        }

        [Theory]
        [InlineData("TestPlek", "B5", "", "", "", "Oppervlakte, Max personen, Prijs per m²")]
        [InlineData("", "", "38", "12", "13", "Beschrijving, Naam")]
        public async Task ValidateEmptyFields_SomeFieldsEmpty_ReturnsTrueAndShowsMissingFields(string description, string spotName, string surface, string maxPersons, string price, string expectedMessage)
        {
            // Arrange
            bool alertWasCalled = false;
            string alertTitle = "";
            string alertMessage = "";

            Func<string, string, Task> mockDisplayAlert = (title, message) =>
            {
                alertWasCalled = true;
                alertTitle = title;
                alertMessage = message;
                return Task.CompletedTask;
            };

            // Act
            bool result = await CampingSpot.ValidateEmptyFields(description, spotName, surface, maxPersons, price, mockDisplayAlert);

            // Assert
            Assert.True(result);
            Assert.True(alertWasCalled);
            Assert.Equal("Let op", alertTitle);
            Assert.Equal("De volgende velden zijn niet ingevuld: " + expectedMessage, alertMessage);
        }

        [Fact]
        public async Task ValidateEmptyFields_AllFieldsFilled_ReturnsFalseAndNoAlert()
        {
            // Arrange
            string description = "TestPlek";
            string spotName = "B5";
            string surface = "38";
            string maxPersons = "12";
            string price = "13";

            bool alertWasCalled = false;

            Func<string, string, Task> mockDisplayAlert = (title, message) =>
            {
                alertWasCalled = true;
                return Task.CompletedTask;
            };

            // Act
            bool result = await CampingSpot.ValidateEmptyFields(description, spotName, surface, maxPersons, price, mockDisplayAlert);

            // Assert
            Assert.False(result);
            Assert.False(alertWasCalled);
        }
    }
}
