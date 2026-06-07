using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyPath_Models.ViewModel
{
    public class PurchaseTicketRequest
    {
        public string UserId { get; set; }

        public string OutboundFlightId { get; set; }

        public string ReturnFlightId { get; set; }
    }
}
