using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyPath_Models;

namespace SkyPath_Models.Models
{
    public class Departure_City : Model
    {
        public string departure_Id;
        string city_Name;

        public string Departure_Id
        {
            get { return departure_Id; }
            set { departure_Id = value; }
        }
        [FirstLetterCapital(ErrorMessage = "City name must start with a capital letter")]
        public string City_Name
        {
            get { return city_Name; }
            set { city_Name = value; ValidateProperty(value, "City_Name"); }
        }
    }
}
