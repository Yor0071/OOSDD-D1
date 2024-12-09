using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Types
{
    public class Reservation
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PlaceNumber { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime Depart { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public Reservation(int id, string firstName, string lastName, int placeNumber, DateTime arrival, DateTime depart, string phoneNumber, string email)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            PlaceNumber = placeNumber;
            Arrival = arrival;
            Depart = depart;
            PhoneNumber = phoneNumber;
            Email = email;
        }
    }
}


