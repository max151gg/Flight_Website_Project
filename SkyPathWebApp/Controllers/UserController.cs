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
            ApiClient<List<Flight>> flightsClient = new ApiClient<List<Flight>>();
            flightsClient.Scheme = "http";
            flightsClient.Host = "localhost";
            flightsClient.Port = 5125;
            flightsClient.Path = "api/User/GetAllFlights";

            List<Flight> flights = await flightsClient.GetAsync() ?? new List<Flight>();

            BrowseViewModel browseViewModel = new BrowseViewModel
            {
                flights = flights
            };

            ApiClient<List<City>> cityClient = new ApiClient<List<City>>();
            cityClient.Scheme = "http";
            cityClient.Host = "localhost";
            cityClient.Port = 5125;
            cityClient.Path = "api/City/GetAll";

            List<City> cities = await cityClient.GetAsync() ?? new List<City>();
            ViewBag.CityDict = cities.ToDictionary(c => c.CityId, c => c.CityName);

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
            string userId = HttpContext.Session.GetString("user_Id");

            var client = new ApiClient<TicketViewModel>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/User/GetTicketByUserId"
            };
            client.SetQueryParameter("user_id", userId);

            TicketViewModel ticketViewModel = await client.GetAsync();

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

            var userClient = new ApiClient<User>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/User/GetById"
            };
            userClient.SetQueryParameter("user_id", userId);

            User user = await userClient.GetAsync();
            ViewBag.PassengerFullName = user?.User_FullName ?? "Unknown";

            return View(ticketViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Discount()
        {
            string userId = HttpContext.Session.GetString("user_Id");

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
                Path = "api/User/GetById"
            };
            userClient.SetQueryParameter("user_id", userId);

            User user = await userClient.GetAsync();
            ViewBag.PassengerFullName = user?.User_FullName ?? "Unknown";

            return View(discountViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Announcement()
        {
            string userId = HttpContext.Session.GetString("user_Id");

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
                Path = "api/User/GetFlightCatalog"
            };

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
        public async Task<IActionResult> FlightDetail(string flight_id)
        {
            if (string.IsNullOrEmpty(flight_id))
                return RedirectToAction("Browse");

            var allFlightsClient = new ApiClient<List<Flight>>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/User/GetAllFlights"
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

        [HttpGet]
        public async Task<IActionResult> Checkout(string flight_id)
        {
            string userId = HttpContext.Session.GetString("user_Id");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("LoginHome", "Guest");
            if (string.IsNullOrEmpty(flight_id))
                return RedirectToAction("Browse");

            var allFlightsClient = new ApiClient<List<Flight>>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/User/GetAllFlights"
            };
            List<Flight> allFlights = await allFlightsClient.GetAsync() ?? new List<Flight>();

            Flight outboundFlight = allFlights.FirstOrDefault(f => f.Flight_Id == flight_id);
            if (outboundFlight == null)
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

            var vm = new CheckoutViewModel
            {
                UserId = userId,
                OutboundFlightId = flight_id,
                OutboundFlight = outboundFlight
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ValidateDiscount(string code)
        {
            string userId = HttpContext.Session.GetString("user_Id");
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
                return Json(new { valid = false });

            var client = new ApiClient<DiscountViewModel>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/User/GetDiscountByUserId"
            };
            client.SetQueryParameter("user_id", userId);

            DiscountViewModel vm = await client.GetAsync();

            var discount = vm?.discounts?.FirstOrDefault(d => d.Discount_Id == code);
            if (discount == null)
                return Json(new { valid = false });

            return Json(new { valid = true, percentage = discount.Percentage, description = discount.Description });
        }

        [HttpPost]
        public async Task<IActionResult> Purchase(CheckoutViewModel vm)
        {
            string userId = HttpContext.Session.GetString("user_Id");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("LoginHome", "Guest");

            vm.UserId = userId;

            var client = new ApiClient<CheckoutViewModel>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/User/PurchaseTicket"
            };

            bool ok = await client.PostAsync(vm);
            if (ok)
                return RedirectToAction("Ticket");

            TempData["PurchaseError"] = "Booking failed. Please try again or choose a different flight.";
            return RedirectToAction("Checkout", new { flight_id = vm.OutboundFlightId });
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

            string newPath = body.Trim('"', ' ', '\n', '\r');
            HttpContext.Session.SetString("user_Image", newPath);
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