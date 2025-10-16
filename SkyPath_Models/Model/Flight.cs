using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyPath_Models.Model
{
    public class Flight
    {
        string flight_Id;
        string flight_Number;
        string airline;
        string departure_Id;
        string arrival_Id;
        string departure_Time;
        string arrival_Time;
        double price;
        int seats_Available;

        public string Flight_Id
        {
            get { return flight_Id; }
            set { flight_Id = value; }
        }
        [Required(ErrorMessage = "Flight number is required")]
        public string Flight_Number
        {
            get { return flight_Number; }
            set { flight_Number = value; }
        }
        [Required(ErrorMessage = "Airline is required")]
        public string Airline
        {
            get { return airline; }
            set { airline = value; }
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
            set { departure_Time = value; }
        }
        [Required(ErrorMessage = "Arrival time is required")]
        public string Arrival_Time
        {
            get { return arrival_Time; }
            set { arrival_Time = value; }
        }
        [Required(ErrorMessage = "Price is required")]
        [IsDigits(ErrorMessage = "Must be digits only")]
        public double Price
        {
            get { return price; }
            set { price = value; }
        }
        [Required(ErrorMessage = "its required to set the number of seats available")]
        [IsDigits(ErrorMessage = "Must be digits only")]
        public int Seats_Available
        {
            get { return seats_Available; }
            set { seats_Available = value; }
        }
    }
}
