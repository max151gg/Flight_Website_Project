using Microsoft.AspNetCore.Mvc;
using SkyPathWSClient;
using SkyPath_Models;
using SkyPath_Models.ViewModel;
using System.Runtime.InteropServices;
using SkyPath_Models.Models;

namespace SkyPathWebApp.Controllers
{
    public class GuestController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> HomePage()
        {
            // 1) Call WS endpoint that returns ALL flights (no pagination)
            ApiClient<List<Flight>> flightsClient = new ApiClient<List<Flight>>();
            flightsClient.Scheme = "http";
            flightsClient.Host = "localhost";
            flightsClient.Port = 5125;
            flightsClient.Path = "api/Guest/GetAllFlights";

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
        public async Task<IActionResult> LoginHome()
        {
            // 1) Call WS endpoint that returns ALL flights (no pagination)
            ApiClient<List<Flight>> flightsClient = new ApiClient<List<Flight>>();
            flightsClient.Scheme = "http";
            flightsClient.Host = "localhost";
            flightsClient.Port = 5125;
            flightsClient.Path = "api/Guest/GetAllFlights";

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
        public async Task<IActionResult> Announcement()
        {
            ApiClient<AnnouncementViewModel> client = new ApiClient<AnnouncementViewModel>();
            client.Scheme = "http";
            client.Host = "localhost";
            client.Port = 5125;
            client.Path = "api/Guest/Announcement";
            AnnouncementViewModel announcementViewModel = await client.GetAsync();
            return View(announcementViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Browse(string flight_id = null, int page = 0, string departure_id = null, string arrival_id = null)
        {
            ApiClient<BrowseViewModel> client = new ApiClient<BrowseViewModel>();
            client.Scheme = "http";
            client.Host = "localhost";
            client.Port = 5125;
            client.Path = "api/Guest/GetFlightCatalog";
            if (flight_id != null)
                client.SetQueryParameter("flight_id", flight_id);
            if (departure_id != null)
                client.SetQueryParameter("departure_id", departure_id);
            if (arrival_id != null)
                client.SetQueryParameter("arrival_id", arrival_id);
            if (page != 0)
                client.SetQueryParameter("page", page.ToString());
            BrowseViewModel browseViewModel = await client.GetAsync();

            ApiClient<List<City>> cityClient = new ApiClient<List<City>>();
            cityClient.Scheme = "http";
            cityClient.Host = "localhost";
            cityClient.Port = 5125;
            cityClient.Path = "api/City/GetAll";

            List<City> cities = await cityClient.GetAsync();

            var cityDict = (cities ?? new List<City>())
                .ToDictionary(c => c.CityId, c => c.CityName);

            ViewBag.CityDict = cityDict;


            return View(browseViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> FlightDetails(string flight_id)
        {
            ApiClient<Flight> client = new ApiClient<Flight>();
            client.Scheme = "http";
            client.Host = "localhost";
            client.Port = 5125;
            client.Path = "api/Guest/GetFlightDetails";
            client.SetQueryParameter("flight_id", flight_id);
            Flight flight = await client.GetAsync();
            return View(flight);
        }


        [HttpPost]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AboutUs()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Login(string UserName, string Password)
        {
            ApiClient<User> client = new ApiClient<User>();
            client.Scheme = "http";
            client.Host = "localhost";
            client.Port = 5125;
            client.Path = "api/Guest/Login";
            client.SetQueryParameter("userName", UserName);
            client.SetQueryParameter("password", Password);
            User user = await client.GetAsync();
            if(user == null)
            {
                ViewBag.ErrorMessage = "Invalid username or password.";
                return View("HomePage");
            }
            return View(user);
        }
    }
}
