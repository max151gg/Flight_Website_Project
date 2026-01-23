using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using SkyPath_Models;
using SkyPath_Models.Models;
using SkyPath_Models.ViewModel;
using SkyPathWSClient;
using System.Runtime.InteropServices;

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
            string userId = "0"; // replace with session/claims later


            var client = new ApiClient<AnnouncementViewModel>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Guest/GetAnnouncementByUserId"
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
                Path = "api/Guest/GetFlightCatalog"
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


        [HttpGet]
        public async Task<IActionResult> ViewRegisterForm(User user)
        {
            //SignUpViewModel signUpViewModel = new SignUpViewModel();
            //signUpViewModel.user = null;
            //ApiClient<List<User>> client = new ApiClient<List<User>>();
            //client.Scheme = "http";
            //client.Host = "localhost";
            //client.Port = 5125;
            //client.Path = "api/Guest/";
            //signUpViewModel.cities = await client.GetAsync();
            //return View(signUpViewModel);
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Register(User user)
        //{
        //    if (ModelState.IsValid == false)
        //    {
        //        ApiClient<User> client1 = new ApiClient<User>();
        //        client1.Scheme = "http";
        //        client1.Host = "localhost";
        //        client1.Port = 5125;
        //        client1.Path = "api/Guest/Register";
        //        User user1 = await client1.GetAsync();
        //        return View("HomePage", user1);
        //    }
        //    ApiClient<User> client = new ApiClient<User>();
        //    client.Scheme = "http";
        //    client.Host = "localhost";
        //    client.Port = 5125;
        //    client.Path = "api/Guest/Register";
        //    bool ok = await client.PostAsync(user, (Stream)null);
        //    if (ok)
        //    {
        //        HttpContext.Session.SetString("user_Id", user.User_Id);
        //        return RedirectToAction("GetAnnouncement", "User");
        //    }
        //    ApiClient<User> client1 = new ApiClient<User>();
        //    client1.Scheme = "http";
        //    client1.Host = "localhost";
        //    client1.Port = 5125;
        //    client1.Path = "api/Guest/Register";
        //    User user1 = await client1.GetAsync();
        //    ViewBag["Error"] = true;
        //    return View("HomePage", user1);
        //}


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginviewmodel)
        {
            ApiClient<LoginViewModel> client = new ApiClient<LoginViewModel>();
            client.Scheme = "http";
            client.Host = "localhost";
            client.Port = 5125;
            client.Path = "api/Guest/Login";


            User user = await client.PostAsyncReturn<LoginViewModel, User>(loginviewmodel);

            
            if (user != null)
            {
                string user_id = user.User_Id;
                HttpContext.Session.SetString("user_Id", user_id);
                return RedirectToAction("HomePage", "User");
            }

            return RedirectToAction("LoginHome");
        }

        //[HttpPost]
        //public async Task<IActionResult> SignUp(Registered reg)
        //{
        //    ApiClient<Registered> client = new ApiClient<Registered>();
        //    client.Scheme = "http";
        //    client.Host = "localhost";
        //    client.Port = 5049;
        //    client.Path = "api/Guest/SignUp";

        //    reg.Role = "User";
        //    reg.RegisteredID = "6";
        //    reg.RegisteredSalt = " ";
        //    reg.ImagePath = "None";
        //    ApiResultModel<Registered> success = client.PostAsyncRet<Registered, Registered>(reg).Result;

        //    //888888888888888888888888888888888888

        //    //888888888888888888888888888888888888
        //    //ApiResultModel<string> success = client.PostAsync(reg).Result;
        //    await Console.Out.WriteLineAsync("here****************************************");
        //    await Console.Out.WriteLineAsync(success.Success + "HEREHREREHREHEHREHRHEEH");

        //    Console.WriteLine($@"{success.Data.RegisteredID}");
        //    if (success.Success && success.Data != null)
        //    {

        //        HttpContext.Session.SetString("RegisteredID", success.Data.RegisteredID);
        //        return RedirectToAction("RegisteredHomePage", "Registered");
        //    }

        //    return RedirectToAction("ViewSignUpPage");
        //    //if (!success)
        //    //    return View("Failed to sign up.");
        //    //return View("User has been signed up.");

        //}


        [HttpGet]
        public IActionResult AboutUs()
        {
            return View();
        }


        //[HttpPost]
        //public async Task<ActionResult> Login(string Email, string Password)
        //{
        //    ApiClient<User> client = new ApiClient<User>();
        //    client.Scheme = "http";
        //    client.Host = "localhost";
        //    client.Port = 5125;
        //    client.Path = "api/Guest/Login";
        //    client.SetQueryParameter("userName", UserName);
        //    client.SetQueryParameter("password", Password);
        //    User user = await client.GetAsync();
        //    if(user == null)
        //    {
        //        ViewBag.ErrorMessage = "Invalid email or password.";
        //        return View("HomePage");
        //    }
        //    return View(user);
        //}
    }
}
