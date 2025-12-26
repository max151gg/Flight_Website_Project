using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyPath_Models.Models
{
    public class City : Model
    {
        string cityId;
        string cityName;

        public string CityId
        {
            get { return cityId; }
            set { cityId = value; }
        }
        [FirstLetterCapital(ErrorMessage = "City name must start with a capital letter")]
        public string CityName
        {
            get { return cityName; }
            set { cityName = value; ValidateProperty(value, "CityName"); }
        }
    }
}
