using SkyPath_Models.Models;

namespace SkyPath_Models.ViewModel
{
    public class CheckoutViewModel
    {
        public string UserId { get; set; }
        public string OutboundFlightId { get; set; }
        public string DiscountCode { get; set; }
        public Flight OutboundFlight { get; set; }
    }
}