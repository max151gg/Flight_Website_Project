using SkyPath_Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyPath_Models.ViewModel
{
    public class TicketViewModel
    {
        public List<Ticket> ticket { get; set; }
        public List<Flight> flights { get; set; }
    }
}
