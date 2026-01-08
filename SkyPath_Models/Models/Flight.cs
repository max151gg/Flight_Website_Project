using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyPath_Models;

namespace SkyPath_Models.Models
{
    public class Flight : Model
    {
        string flight_Id;
        string flight_Number;
        string airline;
        string departure_Id;
        string arrival_Id;
        string departure_Time;
        string arrival_Time;
        double price;
        short seats_Available;
        string departure_Date;
        string arrival_Date;

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
    }
}
