using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyPath_Models.Models;

namespace SkyPath_Models.ViewModel
{
    public class BrowseViewModel
    {
        public List<Flight> flights { get; set; }
        public List<Arrival_City> arrival_Cities { get; set; }
        public List<Departure_City> departure_Cities { get; set; }
        public int page { get; set; }
        public int pagePerPage { get; set; }
        public int pageCount { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }

        public User User { get; set; }
    }
}
