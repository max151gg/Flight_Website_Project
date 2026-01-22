using Microsoft.AspNetCore.Mvc;
using SkyPath_Models.Models;
using SkyPath_Models.ViewModel;
using SkyPathWSClient;

namespace SkyPathWebApp.Controllers
{
    public class UserController : Controller
    {

        [HttpGet]
        public async Task<IActionResult> Ticket()
        {
            string userId = "1"; // replace with session/claims later

            // 1) Tickets (and flights) for this user
            var client = new ApiClient<TicketViewModel>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/User/GetTicketByUserId"
            };
            client.SetQueryParameter("user_id", userId);

            TicketViewModel ticketViewModel = await client.GetAsync();

            // 2) Cities dict
            var cityClient = new ApiClient<List<City>>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/City/GetAll"
            };

            List<City> cities = await cityClient.GetAsync();
            ViewBag.CityDict = (cities ?? new List<City>())
                .ToDictionary(c => c.CityId, c => c.CityName);

            // 3) Single user (NEW) — requires a WS endpoint like api/User/GetById
            var userClient = new ApiClient<User>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/User/GetById" // <-- implement this in WS if you don't have it yet
            };
            userClient.SetQueryParameter("user_id", userId);

            User user = await userClient.GetAsync();
            ViewBag.PassengerFullName = user?.User_FullName ?? "Unknown";

            return View(ticketViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Discount()
        {
            string userId = "5"; // replace with session/claims later


            var client = new ApiClient<DiscountViewModel>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/User/GetDiscountByUserId"
            };
            client.SetQueryParameter("user_id", userId);

            DiscountViewModel discountViewModel = await client.GetAsync();
            var userClient = new ApiClient<User>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/User/GetById" // <-- implement this in WS if you don't have it yet
            };
            userClient.SetQueryParameter("user_id", userId);

            User user = await userClient.GetAsync();
            ViewBag.PassengerFullName = user?.User_FullName ?? "Unknown";

            return View(discountViewModel);

        }
    }
}
