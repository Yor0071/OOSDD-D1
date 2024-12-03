using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // Formatted properties
        public string AvailableText => Available ? "Ja" : "Nee";
        public string PowerText => Power ? "Ja" : "Nee";
        public string WaterText => Water ? "Ja" : "Nee";
        public string WifiText => Wifi ? "Ja" : "Nee";

        public CampingSpot(int id, string description, double surface, bool power, bool water, bool wifi, int maxPersons, double price, bool available) 
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
        }
    }
}
