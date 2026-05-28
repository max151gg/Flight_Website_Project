using Microsoft.AspNetCore.Mvc;
using SkyPath_Models;
using SkyPath_Models.Models;
using SkyPath_Models.ViewModel;
using SkyPathWSClient;

namespace SkyPathWebApp.Controllers
{
    public class GuestController : Controller
    {
        // ─── Shared helper: loads flights + cities, sets ViewBag.CityDict ────────
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
            ViewBag.CityDict = cities.ToDictionary(c => c.CityId, c => c.CityName);

            return new BrowseViewModel { flights = flights };
        }

        // ─── GET: /Guest/HomePage ────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> HomePage()
        {
            BrowseViewModel vm = await LoadBrowseViewModelAsync();
            vm.user = new User();
            return View(vm);
        }

        // ─── GET: /Guest/LoginHome ───────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> LoginHome()
        {
            BrowseViewModel vm = await LoadBrowseViewModelAsync();
            vm.LoginViewModel = new LoginViewModel();
            return View(vm);
        }

        // ─── POST: /Guest/Login ──────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginviewmodel)
        {
            // 1) Server-side validation — shows field errors if Email/Password are bad
            if (!ModelState.IsValid)
            {
                BrowseViewModel vm = await LoadBrowseViewModelAsync();
                vm.LoginViewModel = loginviewmodel;
                return View("LoginHome", vm);
            }

            // 2) Call the API
            var client = new ApiClient<LoginViewModel>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Guest/Login"
            };

            User user = await client.PostAsyncReturn<LoginViewModel, User>(loginviewmodel);

            // 3) Wrong credentials — show a form-level error message
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                BrowseViewModel vm = await LoadBrowseViewModelAsync();
                vm.LoginViewModel = loginviewmodel;
                return View("LoginHome", vm);
            }

            // 4) Banned account — do not create a session
            if (user.User_Ban)
            {
                ModelState.AddModelError("", "Your account has been banned. Please contact support.");
                BrowseViewModel vmBan = await LoadBrowseViewModelAsync();
                vmBan.LoginViewModel = loginviewmodel;
                return View("LoginHome", vmBan);
            }

            // 5) Successful login
            HttpContext.Session.SetString("user_Id", user.User_Id);
            HttpContext.Session.SetString("user_Image", user.User_Image ?? "");
            HttpContext.Session.SetString("user_FullName", user.User_FullName ?? "");
            return RedirectToAction("HomePage", "User");
        }

        // ─── POST: /Guest/Register ───────────────────────────────────────────────
        //[HttpPost]
        //public async Task<IActionResult> Register(User user)
        //{
        //    if (user == null)
        //        return RedirectToAction("HomePage");

        //    // Set these before ModelState runs — they are not form fields
        //    if (string.IsNullOrWhiteSpace(user.Role_Id))
        //        user.Role_Id = "1";
        //    user.User_Image = "";
        //    user.User_Id = "";
        //    user.UserSalt = "";
        //    user.User_Ban = false;

        //    //Server - side field validation
        //    if (!ModelState.IsValid)
        //    {
        //        BrowseViewModel vm = await LoadBrowseViewModelAsync();
        //        vm.user = user;
        //        return View("HomePage", vm);
        //    }

        //    // Call API to create the user
        //    var registerClient = new ApiClient<User>
        //    {
        //        Scheme = "http",
        //        Host = "localhost",
        //        Port = 5125,
        //        Path = "api/Guest/Register"
        //    };
        //    bool ok = await registerClient.PostAsync(user);

        //    if (!ok)
        //    {
        //        ModelState.AddModelError("", "Registration failed. Please try again.");
        //        BrowseViewModel vm = await LoadBrowseViewModelAsync();
        //        vm.user = user;
        //        return View("HomePage", vm);
        //    }

        //    // Auto-login after successful registration
        //    var loginClient = new ApiClient<LoginViewModel>
        //    {
        //        Scheme = "http",
        //        Host = "localhost",
        //        Port = 5125,
        //        Path = "api/Guest/Login"
        //    };
        //    var loginVm = new LoginViewModel { Email = user.Email, Password = user.Password };
        //    User loggedInUser = await loginClient.PostAsyncReturn<LoginViewModel, User>(loginVm);

        //    if (loggedInUser != null)
        //    {
        //        HttpContext.Session.SetString("user_Id", loggedInUser.User_Id);
        //        HttpContext.Session.SetString("user_FullName", loggedInUser.User_FullName ?? "My Account");
        //        HttpContext.Session.SetString("user_Image", loggedInUser.User_Image ?? "");
        //        return RedirectToAction("HomePage", "User");
        //    }

        //    return RedirectToAction("LoginHome");
        //}
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

            // These are server-managed fields, not user-entered form fields.
            // Remove possible validation errors that were created before this method ran.
            
            user.Validate(); // This will populate ModelState with any validation errors based on data annotations in the User model.
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

            bool ok = await registerClient.PostAsync(user);

            if (!ok)
            {
                ModelState.AddModelError("", "Registration failed. Please check the API/database error.");
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

        // ─── GET: /Guest/Browse ──────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Browse(
            string flight_id = null, int page = 1,
            string departure_id = null, string arrival_id = null,
            string departure_date = null, string departure_date_to = null,
            string sort = null, string openFlightId = null)
        {
            var client = new ApiClient<BrowseViewModel>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Guest/GetFlightCatalog"
            };

            if (!string.IsNullOrEmpty(flight_id)) client.SetQueryParameter("flight_id", flight_id);
            if (!string.IsNullOrEmpty(departure_id)) client.SetQueryParameter("departure_id", departure_id);
            if (!string.IsNullOrEmpty(arrival_id)) client.SetQueryParameter("arrival_id", arrival_id);
            if (!string.IsNullOrEmpty(departure_date)) client.SetQueryParameter("departure_date", departure_date);
            if (!string.IsNullOrEmpty(departure_date_to)) client.SetQueryParameter("departure_date_to", departure_date_to);
            if (!string.IsNullOrEmpty(sort)) client.SetQueryParameter("sort", sort);
            if (page < 1) page = 1;
            client.SetQueryParameter("page", page.ToString());
            if (!string.IsNullOrEmpty(openFlightId)) client.SetQueryParameter("openFlightId", openFlightId);

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
            ViewBag.OpenFlightId = openFlightId;
            ViewBag.DepartureDate = departure_date;
            ViewBag.DepartureDateTo = departure_date_to;
            ViewBag.Sort = sort;

            return View(browseViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> FlightDetails(string flight_id)
        {
            var client = new ApiClient<Flight>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Guest/GetFlightDetails"
            };
            client.SetQueryParameter("flight_id", flight_id);
            Flight flight = await client.GetAsync();
            return View(flight);
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
    }
}