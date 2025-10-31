using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyPath_Models;

namespace SkyPath_Models.Models
{
    public class Arrival_City : Model
    {
        string arrival_Id;
        string city_Name;

        public string Arrival_Id
        {
            get { return arrival_Id; }
            set { arrival_Id = value; }
        }
        [FirstLetterCapital(ErrorMessage = "City name must start with a capital letter")]
        public string City_Name
        {
            get { return city_Name; }
            set { city_Name = value; ValidateProperty(value, "City_Name"); }
        }
    }
}
