using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Database.Types
{
    public class CampingSpot
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public double Surface_m2 { get; set; }
        public bool Power { get; set; }
        public bool Water { get; set; }
        public bool Wifi { get; set; }
        public int MaxPersons { get; set; }
        public double Price_m2 { get; set; }
        public bool Available { get; set; }
        public string SpotName { get; set; }

        // Formatted properties
        public string AvailableText => Available ? "Ja" : "Nee";
        public string PowerText => Power ? "Ja" : "Nee";
        public string WaterText => Water ? "Ja" : "Nee";
        public string WifiText => Wifi ? "Ja" : "Nee";

        public CampingSpot(int id, string description, double surface, bool power, bool water, bool wifi, int maxPersons, double price, bool available, string spotName) 
        {
            Id = id;
            Description = description;
            Surface_m2 = surface;
            Power = power;
            Water = water;
            Wifi = wifi;
            MaxPersons = maxPersons;
            Price_m2 = price;
            Available = available;
            SpotName = spotName;
        }

        public CampingSpot(string description, double surface, bool power, bool water, bool wifi, int maxPersons, double price, bool available, string spotName)
        {
            Description = description;
            Surface_m2 = surface;
            Power = power;
            Water = water;
            Wifi = wifi;
            MaxPersons = maxPersons;
            Price_m2 = price;
            Available = available;
            SpotName = spotName;
        }

        public static async Task<bool> ValidateEmptyFields(string description, string spotName, string surface, string maxPersons, string price, Func<string, string, Task> displayAlert)
        {
            List<string> emptyFields = new List<string>();

            if (string.IsNullOrWhiteSpace(description)) emptyFields.Add("Beschrijving");
            if (string.IsNullOrWhiteSpace(spotName)) emptyFields.Add("Naam");
            if (string.IsNullOrWhiteSpace(surface)) emptyFields.Add("Oppervlakte");
            if (string.IsNullOrWhiteSpace(maxPersons)) emptyFields.Add("Max personen");
            if (string.IsNullOrWhiteSpace(price)) emptyFields.Add("Prijs per m²");

            if (emptyFields.Count == 5)
            {
                await displayAlert("Fout", "Alle velden zijn leeg. Vul de vereiste gegevens in.");
                return true;
            }
            else if (emptyFields.Count > 0)
            {
                string missingFieldsMessage = "De volgende velden zijn niet ingevuld: " + string.Join(", ", emptyFields);
                await displayAlert("Let op", missingFieldsMessage);
                return true;
            }

            return false;
        }
    }
}
