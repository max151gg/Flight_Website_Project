using Microsoft.AspNetCore.Mvc;
using SkyPath_Models;
using SkyPath_Models.Models;
using SkyPath_Models.ViewModel;
using SkyPathWSClient;

namespace SkyPathWebApp.Controllers
{
    public class GuestController : Controller
    {
        // Loads the data the home/login pages need: all flights plus a city id->name lookup.
        private async Task<BrowseViewModel> LoadBrowseViewModelAsync()
        {
            var flightsClient = new ApiClient<List<Flight>>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Guest/GetAllFlights"
            };
            List<Flight> flights = await flightsClient.GetAsync() ?? new List<Flight>();

            var cityClient = new ApiClient<List<City>>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/City/GetAll"
            };
            List<City> cities = await cityClient.GetAsync() ?? new List<City>();
            // CityDict: Key = city id, Value = city name. The pages store city ids on flights,
            // so this lets the views show the city name without searching the list each time.
            ViewBag.CityDict = cities.ToDictionary(c => c.CityId, c => c.CityName);

            return new BrowseViewModel { flights = flights };
        }

        [HttpGet]
        public async Task<IActionResult> HomePage()
        {
            BrowseViewModel vm = await LoadBrowseViewModelAsync();
            vm.user = new User();
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> LoginHome()
        {
            BrowseViewModel vm = await LoadBrowseViewModelAsync();
            vm.LoginViewModel = new LoginViewModel();
            return View(vm);
        }

        // Handles the login form. On success it saves the user in the session and goes to
        // the user home page; on failure it shows an error on the login page.
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginviewmodel)
        {
            if (!ModelState.IsValid)
            {
                BrowseViewModel vm = await LoadBrowseViewModelAsync();
                vm.LoginViewModel = loginviewmodel;
                return View("LoginHome", vm);
            }

            var client = new ApiClient<LoginViewModel>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Guest/Login"
            };

            User user = await client.PostAsyncReturn<LoginViewModel, User>(loginviewmodel);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                BrowseViewModel vm = await LoadBrowseViewModelAsync();
                vm.LoginViewModel = loginviewmodel;
                return View("LoginHome", vm);
            }

            if (user.User_Ban)
            {
                ModelState.AddModelError("", "Your account has been banned. Please contact support.");
                BrowseViewModel vmBan = await LoadBrowseViewModelAsync();
                vmBan.LoginViewModel = loginviewmodel;
                return View("LoginHome", vmBan);
            }

            HttpContext.Session.SetString("user_Id", user.User_Id);
            HttpContext.Session.SetString("user_Image", user.User_Image ?? "");
            HttpContext.Session.SetString("user_FullName", user.User_FullName ?? "");
            return RedirectToAction("HomePage", "User");
        }

        // Handles the sign-up form: validates the input, sends the new user to the API,
        // and (if it worked) logs them in automatically.
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (user == null)
                return RedirectToAction("HomePage");

            user.Role_Id = string.IsNullOrWhiteSpace(user.Role_Id) ? "1" : user.Role_Id;
            user.User_Image = "";
            user.User_Id = "";
            user.UserSalt = "";
            user.User_Ban = false;

            user.Validate();
            if (!user.IsValid)
            {
                BrowseViewModel vm = await LoadBrowseViewModelAsync();
                vm.user = user;
                return View("HomePage", vm);
            }

            var registerClient = new ApiClient<User>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Guest/Register"
            };

            // Send the new user to the API. PostAsync returns false if the API rejected it
            // (for example a duplicate email/username); LastError holds the API's message.
            bool ok = await registerClient.PostAsync(user);

            if (!ok)
            {
                ModelState.AddModelError("", registerClient.LastError ?? "Registration failed. Please try again.");
                BrowseViewModel vm = await LoadBrowseViewModelAsync();
                vm.user = user;
                return View("HomePage", vm);
            }

            var loginClient = new ApiClient<LoginViewModel>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Guest/Login"
            };

            var loginVm = new LoginViewModel
            {
                Email = user.Email,
                Password = user.Password
            };

            User loggedInUser = await loginClient.PostAsyncReturn<LoginViewModel, User>(loginVm);

            if (loggedInUser != null)
            {
                HttpContext.Session.SetString("user_Id", loggedInUser.User_Id);
                HttpContext.Session.SetString("user_FullName", loggedInUser.User_FullName ?? "My Account");
                HttpContext.Session.SetString("user_Image", loggedInUser.User_Image ?? "");
                return RedirectToAction("HomePage", "User");
            }

            ModelState.AddModelError("", "Registration succeeded, but automatic login failed. Please log in manually.");
            BrowseViewModel failedLoginVm = await LoadBrowseViewModelAsync();
            failedLoginVm.user = user;
            return View("HomePage", failedLoginVm);
        }

        [HttpGet]
        public async Task<IActionResult> Browse(
            int page = 1,
            string departure_id = null,
            string arrival_id = null,
            string departure_date = null,
            string departure_date_to = null,
            string sort = null)
        {
            var client = new ApiClient<BrowseViewModel>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Guest/GetFlightCatalog"
            };

            if (!string.IsNullOrEmpty(departure_id)) client.SetQueryParameter("departure_id", departure_id);
            if (!string.IsNullOrEmpty(arrival_id)) client.SetQueryParameter("arrival_id", arrival_id);
            if (!string.IsNullOrEmpty(departure_date)) client.SetQueryParameter("departure_date", departure_date);
            if (!string.IsNullOrEmpty(departure_date_to)) client.SetQueryParameter("departure_date_to", departure_date_to);
            if (!string.IsNullOrEmpty(sort)) client.SetQueryParameter("sort", sort);
            if (page < 1) page = 1;
            client.SetQueryParameter("page", page.ToString());

            BrowseViewModel browseViewModel = await client.GetAsync();

            var cityClient = new ApiClient<List<City>>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/City/GetAll"
            };
            List<City> cities = await cityClient.GetAsync() ?? new List<City>();
            ViewBag.CityDict = cities.ToDictionary(c => c.CityId, c => c.CityName);

            ViewBag.DepartureId = departure_id;
            ViewBag.ArrivalId = arrival_id;
            ViewBag.DepartureDate = departure_date;
            ViewBag.DepartureDateTo = departure_date_to;
            ViewBag.Sort = sort;

            return View(browseViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Announcement()
        {
            var client = new ApiClient<AnnouncementViewModel>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Guest/GetAnnouncementByUserId"
            };
            client.SetQueryParameter("user_id", "0");
            AnnouncementViewModel vm = await client.GetAsync();
            return View(vm);
        }

        [HttpGet]
        public IActionResult AboutUs() => View();

        [HttpGet]
        public IActionResult ViewRegisterForm() => View();


        [HttpGet]
        public async Task<IActionResult> FlightDetail(string flight_id)
        {
            if (string.IsNullOrEmpty(flight_id))
                return RedirectToAction("Browse");

            var allFlightsClient = new ApiClient<List<Flight>>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Guest/GetAllFlights"
            };
            List<Flight> allFlights = await allFlightsClient.GetAsync() ?? new List<Flight>();

            Flight flight = allFlights.FirstOrDefault(f => f.Flight_Id == flight_id);
            if (flight == null)
                return RedirectToAction("Browse");

            var cityClient = new ApiClient<List<City>>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/City/GetAll"
            };
            List<City> cities = await cityClient.GetAsync() ?? new List<City>();
            ViewBag.CityDict = cities.ToDictionary(c => c.CityId, c => c.CityName);

            return View(flight);
        }
    }

}

