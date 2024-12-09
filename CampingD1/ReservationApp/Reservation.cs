using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationApp
{
    public class Reservation
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int PlaceNumber { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime Depart { get; set; }
    }
}
