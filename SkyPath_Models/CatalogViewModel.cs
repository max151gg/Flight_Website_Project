using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyPath_Models
{
    public class CatalogViewModel
    {
        public List<Flight> flights { get; set; }
        public List<Arrival_City> arrival_Cities { get; set; }
        public List<Departure_City> departure_Cities { get; set; }
    }
}
