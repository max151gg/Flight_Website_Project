using System.Collections.Generic;
using SkyPath_Models.Models;

namespace SkyPath_Models.ViewModel
{
    public class CheckoutViewModel
    {
        public string UserId { get; set; }
        public string OutboundFlightId { get; set; }
        public string? DiscountId { get; set; }
        public Flight? OutboundFlight { get; set; }
        public List<Discount>? Discounts { get; set; }
    }
}