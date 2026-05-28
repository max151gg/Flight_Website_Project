using SkyPath_Models.Models;
using System.Collections.Generic;

namespace SkyPath_Models.ViewModel
{
    public class CheckoutViewModel
    {
        public string UserId { get; set; }
        public string OutboundFlightId { get; set; }
        public string ReturnFlightId { get; set; }

        // Used only to display on the Checkout page — NOT sent to the WS
        public Flight OutboundFlight { get; set; }
        public List<Flight> AvailableReturnFlights { get; set; }
    }
}