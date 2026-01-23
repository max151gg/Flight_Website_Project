using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyPath_Models;
using System.Globalization;


namespace SkyPath_Models.Models
{
    public class Flight : Model
    {
        public string flight_Id;
        public string flight_Number;
        public string airline;
        public string departure_Id;
        public  string arrival_Id;
        public string departure_Time;
        public string arrival_Time;
        public double price;
        public short seats_Available;
        public string departure_Date;
        public string arrival_Date;

        public string Flight_Id
        {
            get { return flight_Id; }
            set { flight_Id = value; }
        }
        [Required(ErrorMessage = "Flight number is required")]
        public string Flight_Number
        {
            get { return flight_Number; }
            set { flight_Number = value; ValidateProperty(value, "Flight_Number"); }
        }
        [Required(ErrorMessage = "Airline is required")]
        public string Airline
        {
            get { return airline; }
            set { airline = value; ValidateProperty(value, "Airline"); }
        }
        public string Departure_Id
        {
            get { return departure_Id; }
            set { departure_Id = value; }
        }
        public string Arrival_Id
        {
            get { return arrival_Id; }
            set { arrival_Id = value; }
        }
        [Required(ErrorMessage = "Departure time is required")]
        public string Departure_Time
        {
            get { return departure_Time; }
            set { departure_Time = value; ValidateProperty(value, "Departure_Time"); }
        }
        [Required(ErrorMessage = "Arrival time is required")]
        public string Arrival_Time
        {
            get { return arrival_Time; }
            set { arrival_Time = value; ValidateProperty(value, "Arrival_Time"); }
        }
        [Required(ErrorMessage = "Price is required")]
        [IsDigits(ErrorMessage = "Must be digits only")]
        public double Price
        {
            get { return price; }
            set { price = value; ValidateProperty(value, "Price"); }
        }
        [Required(ErrorMessage = "its required to set the number of seats available")]
        [IsDigits(ErrorMessage = "Must be digits only")]
        public short Seats_Available
        {
            get { return seats_Available; }
            set { seats_Available = value; ValidateProperty(value, "Seats_Available"); }
        }
        [Required(ErrorMessage = "Departure Date is required")]
        public string Departure_Date
        {
            get { return departure_Date; }
            set { departure_Date = value; ValidateProperty(value, "Departure_Date"); }
        }
        [Required(ErrorMessage = "Arrival Date is required")]
        public string Arrival_Date
        {
            get { return arrival_Date; }
            set { arrival_Date = value; ValidateProperty(value, "Arrival_Date"); }
        }


        public DateTime? DepartureDateTime
        {
            get
            {
                return TryBuildDateTime(Departure_Date, Departure_Time, out var dt)
                    ? dt
                    : (DateTime?)null;
            }
        }

        public DateTime? ArrivalDateTime
        {
            get
            {
                return TryBuildDateTime(Arrival_Date, Arrival_Time, out var dt)
                    ? dt
                    : (DateTime?)null;
            }
        }

        public TimeSpan? Duration
        {
            get
            {
                if (DepartureDateTime is null || ArrivalDateTime is null)
                    return null;

                var dep = DepartureDateTime.Value;
                var arr = ArrivalDateTime.Value;

                // Safety net: if arrival is earlier than departure, assume arrival is next day.
                // (Shouldn't happen if dates are correct, but it prevents negative durations from bad data.)
                if (arr < dep)
                    arr = arr.AddDays(1);

                return arr - dep;
            }
        }

        public string DurationDisplay
        {
            get
            {
                if (Duration is null) return "—";

                var ts = Duration.Value;
                int hours = (int)ts.TotalHours;
                int minutes = ts.Minutes;

                if (hours <= 0) return $"{minutes}m";
                return $"{hours}h {minutes:00}m";
            }
        }

        private static bool TryBuildDateTime(string dateText, string timeText, out DateTime dateTime)
        {
            dateTime = default;

            if (!TryParseDate(dateText, out var date)) return false;
            if (!TryParseTime(timeText, out var time)) return false;

            dateTime = date.Date.Add(time);
            return true;
        }

        private static bool TryParseDate(string input, out DateTime date)
        {
            date = default;
            if (string.IsNullOrWhiteSpace(input)) return false;

            // example: "08-01-2026"
            // Treat as dd-MM-yyyy.
            string[] formats = { "dd-MM-yyyy", "d-M-yyyy" };

            return DateTime.TryParseExact(
                input.Trim(),
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date
            );
        }

        private static bool TryParseTime(string input, out TimeSpan time)
        {
            time = default;
            if (string.IsNullOrWhiteSpace(input)) return false;

            string[] formats = { @"hh\:mm", @"h\:mm" };

            return TimeSpan.TryParseExact(
                input.Trim(),
                formats,
                CultureInfo.InvariantCulture,
                out time
            );
        }
    }
}
