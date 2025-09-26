using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyPath_Models
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
            get { return this.flight_Id; }
            set { this.flight_Id = value; }
        }
    }
}
