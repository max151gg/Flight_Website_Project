using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyPath_Models.Model
{
    public class Departure_City
    {
        string departure_Id;
        string departure_City;

        public string Departure_Id
        {
            get { return departure_Id; }
            set { departure_Id = value; }
        }
        [FirstLetterCapital(ErrorMessage = "City name must start with a capital letter")]
        public string ArrivalCity
        {
            get { return departure_City; }
            set { departure_City = value; }
        }
    }
}
