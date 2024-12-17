using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Types
{
    public enum ReservationStatus
    {
        cancelled,
        repaid,
        ongoing,
        awaiting,
        finished
    }
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
        public string SpotName { get; set; }
        public ReservationStatus Status { get; set; }
        public string TranslatedStatus => Translate(Status);

        public static readonly Dictionary<ReservationStatus, string> StatusTranslations = new()
        {
            { ReservationStatus.cancelled, "Geannuleerd" },
            { ReservationStatus.repaid, "Terugbetaald" },
            { ReservationStatus.ongoing, "Lopend" },
            { ReservationStatus.awaiting, "In afwachting" },
            { ReservationStatus.finished, "Afgerond" }
        };
        private int v;
        private int campingSpot;
        private DateTime fromDate;
        private DateTime toDate;
        private string phone;

        public Reservation(int id, string firstName, string lastName, int placeNumber, string spotName, DateTime arrival, DateTime depart, string phoneNumber, string email, ReservationStatus status)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            PlaceNumber = placeNumber;
            Arrival = arrival;
            Depart = depart;
            PhoneNumber = phoneNumber;
            Email = email;
            Status = status;
            SpotName = spotName;
        }

        public static string Translate(ReservationStatus status)
        {
            return StatusTranslations.TryGetValue(status, out string translatedStatus)
                ? translatedStatus
                : "Onbekend"; // Fallback
        }
    }
}


