using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyPath_Models.Model;

namespace SkyPath_Models.ViewModel
{
    public class BrowseViewModel
    {
        public List<Flight> flights { get; set; }
        public List<Arrival_City> arrival_Cities { get; set; }
        public List<Departure_City> departure_Cities { get; set; }
    }
}
