using System;
using System.Collections.Generic;

namespace Database.Types
{
    public enum ReservationStatus
    {
        cancelled,   // Reserves are cancelled and should not block availability
        repaid,      // Reserves that were repaid (same logic as cancelled)
        ongoing,     // Reserves that are currently active
        awaiting,    // Reserves that are awaiting (perhaps confirmed but not yet active)
        finished     // Completed reservations
    }

    public class Reservation
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PlaceNumber { get; set; } // The spot number
        public DateTime Arrival { get; set; }
        public DateTime Depart { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string SpotName { get; set; }
        public ReservationStatus Status { get; set; }

        // Translated status as a read-only property
        public string TranslatedStatus => Translate(Status);

        // Translations for status
        public static readonly Dictionary<ReservationStatus, string> StatusTranslations = new()
        {
            { ReservationStatus.cancelled, "Geannuleerd" },
            { ReservationStatus.repaid, "Terugbetaald" },
            { ReservationStatus.ongoing, "Lopend" },
            { ReservationStatus.awaiting, "In afwachting" },
            { ReservationStatus.finished, "Afgerond" }
        };

        public Reservation(int id, string firstName, string lastName, int placeNumber, string spotName, DateTime arrival, DateTime depart, string phoneNumber, string email, ReservationStatus status)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            PlaceNumber = placeNumber;
            SpotName = spotName;
            Arrival = arrival;
            Depart = depart;
            PhoneNumber = phoneNumber;
            Email = email;
            Status = status;
        }

        // Checks if a given date range overlaps with the reservation period
        public bool OverlapsWith(DateTime arrivalDate, DateTime departureDate)
        {
            return Arrival < departureDate && Depart > arrivalDate;
        }

        // Returns if the reservation is available during the selected date range
        public bool IsAvailable(DateTime selectedFromDate, DateTime selectedToDate)
        {
            // Check if the reservation overlaps with the selected dates and ensure it's not cancelled or repaid
            return !OverlapsWith(selectedFromDate, selectedToDate) || Status == ReservationStatus.cancelled || Status == ReservationStatus.repaid;
        }

        // Translate reservation status to a human-readable form
        public static string Translate(ReservationStatus status)
        {
            return StatusTranslations.TryGetValue(status, out string translatedStatus)
                ? translatedStatus
                : "Onbekend"; // Fallback for unknown status
        }
    }
}
