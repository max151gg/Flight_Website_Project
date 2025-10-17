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
        string arrival_City;

        public string Arrival_Id
        {
            get { return arrival_Id; }
            set { arrival_Id = value; }
        }
        [FirstLetterCapital(ErrorMessage = "City name must start with a capital letter")]
        public string ArrivalCity
        {
            get { return arrival_City; }
            set { arrival_City = value; ValidateProperty(value, "ArrivalCity"); }
        }
    }
}
