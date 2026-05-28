using Microsoft.AspNetCore.Mvc;
using SkyPath_Models.Models;
using SkyPath_Models.ViewModel;
using SkyPathWSClient;
using System.Net.Http.Headers;


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

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LoginHome", "Guest");
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

        // ─── GET: /User/Checkout?flight_id=X ────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Checkout(string flight_id)
        {
            string userId = HttpContext.Session.GetString("user_Id");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("LoginHome", "Guest");

            if (string.IsNullOrEmpty(flight_id))
                return RedirectToAction("Browse");

            // Load the selected outbound flight
            var flightClient = new ApiClient<Flight>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Guest/GetFlightDetails"
            };
            flightClient.SetQueryParameter("flight_id", flight_id);
            Flight outboundFlight = await flightClient.GetAsync();

            if (outboundFlight == null)
                return RedirectToAction("Browse");

            // Load all flights to find possible return flights
            // Return flights go from the arrival city back to the departure city
            var allFlightsClient = new ApiClient<List<Flight>>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/User/GetAllFlights"
            };
            List<Flight> allFlights = await allFlightsClient.GetAsync() ?? new List<Flight>();

            List<Flight> returnFlights = allFlights
                .Where(f => f.Departure_Id == outboundFlight.Arrival_Id &&
                            f.Arrival_Id == outboundFlight.Departure_Id &&
                            f.Seats_Available > 0)
                .ToList();

            // Load cities for display names
            var cityClient = new ApiClient<List<City>>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/City/GetAll"
            };
            List<City> cities = await cityClient.GetAsync() ?? new List<City>();
            ViewBag.CityDict = cities.ToDictionary(c => c.CityId, c => c.CityName);

            var vm = new CheckoutViewModel
            {
                OutboundFlightId = flight_id,
                OutboundFlight = outboundFlight,
                AvailableReturnFlights = returnFlights
            };

            return View(vm);
        }

        // ─── POST: /User/Purchase ────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Purchase(CheckoutViewModel model)
        {
            string userId = HttpContext.Session.GetString("user_Id");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("LoginHome", "Guest");

            if (string.IsNullOrEmpty(model.OutboundFlightId))
                return RedirectToAction("Browse");

            model.UserId = userId;

            var client = new ApiClient<CheckoutViewModel>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/User/PurchaseTicket"
            };

            bool ok = await client.PostAsyncReturn<CheckoutViewModel, bool>(model);

            if (ok)
            {
                TempData["BookingSuccess"] = "Your booking was confirmed! Check your tickets below.";
                return RedirectToAction("Ticket");
            }

            TempData["BookingError"] = "Booking failed. The flight may be sold out. Please try again.";
            return RedirectToAction("Browse");
        }

        [HttpGet]
        public IActionResult AboutUs()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Account()
        {
            string userId = HttpContext.Session.GetString("user_Id");
            var client = new ApiClient<User>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/User/GetById"
            };
            client.SetQueryParameter("user_id", userId);

            User user = await client.GetAsync();
            if (user == null)
                return RedirectToAction("LoginHome", "Guest");

            // refresh session values so header stays correct
            HttpContext.Session.SetString("user_FullName", user.User_FullName ?? "My Account");
            HttpContext.Session.SetString("user_Image", user.User_Image ?? "");


            return View(user);
        }


        [HttpGet]
        public async Task<IActionResult> GetProfileImage(string user_id)
        {
            if (string.IsNullOrWhiteSpace(user_id))
                return BadRequest();

            using var http = new HttpClient();
            var url = $"http://localhost:5125/api/User/GetProfileImage?user_id={Uri.EscapeDataString(user_id)}";

            var resp = await http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return NotFound();

            var bytes = await resp.Content.ReadAsByteArrayAsync();
            var contentType = resp.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

            return File(bytes, contentType);
        }



        [HttpPost]
        public async Task<IActionResult> UpdateProfileImage(IFormFile profileImage)
        {
            Console.WriteLine(">>> UpdateProfileImage HIT");
            string userId = HttpContext.Session.GetString("user_Id");
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToAction("LoginHome", "Guest");

            if (profileImage == null || profileImage.Length == 0)
                return RedirectToAction("Account");

            using var http = new HttpClient();
            using var form = new MultipartFormDataContent();

            var streamContent = new StreamContent(profileImage.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(profileImage.ContentType);

            // WS reads Request.Form.Files[0] so any name is fine; "file" is clean
            form.Add(streamContent, "file", profileImage.FileName);

            var wsUrl = $"http://localhost:5125/api/User/UpdateProfileImage?user_id={Uri.EscapeDataString(userId)}";
            var resp = await http.PostAsync(wsUrl, form);


            var body = await resp.Content.ReadAsStringAsync();

            Console.WriteLine("WS STATUS: " + (int)resp.StatusCode);
            Console.WriteLine("WS BODY: " + body);

            if (!resp.IsSuccessStatusCode)
            {
                TempData["UploadError"] = $"WS error {(int)resp.StatusCode}: {body}";
                return RedirectToAction("Account");
            }

            // body already contains the returned path
            string newPath = body.Trim('"', ' ', '\n', '\r');
            HttpContext.Session.SetString("user_Image", newPath);


            // Update session
            HttpContext.Session.SetString("user_Image", newPath);

            // OPTIONAL but recommended: cache-buster version for fixed filenames
            HttpContext.Session.SetString("user_Image_V", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());

            return RedirectToAction("Account");

        }


        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
        {
            string userId = HttpContext.Session.GetString("user_Id");
            model.User_Id = userId;

            var client = new ApiClient<UpdateProfileViewModel>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/User/UpdateProfile"
            };

            bool ok = await client.PostAsync(model);
            if (ok)
            {
                HttpContext.Session.SetString("user_FullName", model.User_FullName ?? "My Account");
            }

            return RedirectToAction("Account");
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(ChangePasswordViewModel model)
        {
            string userId = HttpContext.Session.GetString("user_Id");

            if (string.IsNullOrWhiteSpace(model.NewPassword) || model.NewPassword != model.ConfirmNewPassword)
                return RedirectToAction("Account");

            model.User_Id = userId;

            var client = new ApiClient<ChangePasswordViewModel>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/User/UpdatePassword"
            };

            bool ok = await client.PostAsync(model);
            TempData["PasswordStatus"] = ok ? "Password changed successfully." : "Could not change password.";
            return RedirectToAction("Account");
        }
    }
}
