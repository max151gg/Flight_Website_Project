using Microsoft.AspNetCore.Mvc;
using SkyPath_Models.Models;
using SkyPath_Models.ViewModel;
using SkyPathWSClient;

namespace SkyPathWebApp.Controllers
{
    public class UserController : Controller
    {

        [HttpGet]
        public async Task<IActionResult> HomePage()
        {
            // 1) Call WS endpoint that returns ALL flights (no pagination)
            ApiClient<List<Flight>> flightsClient = new ApiClient<List<Flight>>();
            flightsClient.Scheme = "http";
            flightsClient.Host = "localhost";
            flightsClient.Port = 5125;
            flightsClient.Path = "api/User/GetAllFlights";

            List<Flight> flights = await flightsClient.GetAsync() ?? new List<Flight>();

            // 2) Create BrowseViewModel LOCALLY (THIS is what I meant)
            BrowseViewModel browseViewModel = new BrowseViewModel
            {
                flights = flights
            };

            // 3) Get cities (same as Browse page)
            ApiClient<List<City>> cityClient = new ApiClient<List<City>>();
            cityClient.Scheme = "http";
            cityClient.Host = "localhost";
            cityClient.Port = 5125;
            cityClient.Path = "api/City/GetAll";

            List<City> cities = await cityClient.GetAsync() ?? new List<City>();

            ViewBag.CityDict = cities.ToDictionary(c => c.CityId, c => c.CityName);

            // 4) Return model to Home view
            return View(browseViewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Ticket()
        {
            string userId = HttpContext.Session.GetString("user_Id"); // replace with session/claims later

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
            string userId = HttpContext.Session.GetString("user_Id"); // replace with session/claims later


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

        [HttpGet]
        public async Task<IActionResult> Announcement()
        {
            string userId = HttpContext.Session.GetString("user_Id"); // replace with session/claims later


            var client = new ApiClient<AnnouncementViewModel>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/User/GetAnnouncementByUserId"
            };
            client.SetQueryParameter("user_id", userId);

            AnnouncementViewModel announcementViewModel = await client.GetAsync();

            return View(announcementViewModel);

        }

        [HttpGet]
        public async Task<IActionResult> Browse(
           string flight_id = null,
           int page = 1,
           string departure_id = null,
           string arrival_id = null,
           string departure_date = null,
           string departure_date_to = null,
           string sort = null,
           string openFlightId = null)


        {
            var client = new ApiClient<BrowseViewModel>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/User/GetFlightCatalog"
            };

            if (!string.IsNullOrEmpty(flight_id))
                client.SetQueryParameter("flight_id", flight_id);

            if (!string.IsNullOrEmpty(departure_id))
                client.SetQueryParameter("departure_id", departure_id);

            if (!string.IsNullOrEmpty(arrival_id))
                client.SetQueryParameter("arrival_id", arrival_id);

            if (!string.IsNullOrEmpty(departure_date))
                client.SetQueryParameter("departure_date", departure_date);

            if (!string.IsNullOrEmpty(departure_date_to))
                client.SetQueryParameter("departure_date_to", departure_date_to);

            if (!string.IsNullOrEmpty(sort))
                client.SetQueryParameter("sort", sort);



            // Always send a normalized page
            if (page < 1) page = 1;
            client.SetQueryParameter("page", page.ToString());

            // Forward openFlightId so WS can compute correct page
            if (!string.IsNullOrEmpty(openFlightId))
                client.SetQueryParameter("openFlightId", openFlightId);

            BrowseViewModel browseViewModel = await client.GetAsync();

            // Cities (for NameOrUnknown)
            var cityClient = new ApiClient<List<City>>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/City/GetAll"
            };

            List<City> cities = await cityClient.GetAsync() ?? new List<City>();
            ViewBag.CityDict = cities.ToDictionary(c => c.CityId, c => c.CityName);

            // Keep these so the view can preserve filters in pagination links
            ViewBag.DepartureId = departure_id;
            ViewBag.ArrivalId = arrival_id;
            ViewBag.OpenFlightId = openFlightId;
            ViewBag.DepartureDate = departure_date;
            ViewBag.DepartureDateTo = departure_date_to;
            ViewBag.Sort = sort;


            return View(browseViewModel);
        }


        [HttpGet]
        public IActionResult AboutUs()
        {
            return View();
        }
    }
}
