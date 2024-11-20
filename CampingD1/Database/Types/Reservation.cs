using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Types
{
    public struct Reservation
    {
        public int id;
        public string firstName;
        public string lastName;
        public int placenumber;
        public DateTime arrival;
        public DateTime depart;

        public Reservation(int id, string firstName, string lastName, int placenumber, DateTime arrival, DateTime depart)
        {
            this.id = id;
            this.firstName = firstName;
            this.lastName = lastName;
            this.placenumber = placenumber;
            this.arrival = arrival;
            this.depart = depart;
        }
    }
}
