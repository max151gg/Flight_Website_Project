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
        public IActionResult HomePage()
        {
            return View();
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
