using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkyPath_Models.Models;
using SkyPath_Models.ViewModel;
using SkyPathWS.ORM.Repositories;
using System.Globalization;

namespace SkyPathWS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        RepositoryUOW repositoryUOW;
        public UserController()
        {
            this.repositoryUOW = new RepositoryUOW();
        }

        private static List<Flight> ApplySort(List<Flight> flights, string sort)
        {
            if (flights == null) return new List<Flight>();
            sort = (sort ?? "").Trim().ToLowerInvariant();

            return sort switch
            {
                "price_asc" => flights.OrderBy(f => TryParseDecimal(f.Price)).ToList(),
                "price_desc" => flights.OrderByDescending(f => TryParseDecimal(f.Price)).ToList(),
                "duration_asc" => flights.OrderBy(f => TryParseDurationMinutes(f.DurationDisplay)).ToList(),
                "dep_time_asc" => flights.OrderBy(f => TryParseDepartureDateTime(f.Departure_Date, f.Departure_Time)).ToList(),
                _ => flights // default: keep repository order (or sort by newest/soonest if you want)
            };
        }

        private static decimal TryParseDecimal(object price)
        {
            if (price == null) return decimal.MaxValue;
            if (price is decimal d) return d;

            return decimal.TryParse(price.ToString(), out var result)
                ? result
                : decimal.MaxValue;
        }

        // If DurationDisplay looks like "3h 20m" or "200" etc.
        private static int TryParseDurationMinutes(string durationDisplay)
        {
            if (string.IsNullOrWhiteSpace(durationDisplay)) return int.MaxValue;

            // Try plain minutes
            if (int.TryParse(durationDisplay.Trim(), out int mins)) return mins;

            // Try "Xh Ym"
            int total = 0;
            var s = durationDisplay.ToLowerInvariant();

            // very forgiving parsing
            var hIndex = s.IndexOf('h');
            if (hIndex > 0 && int.TryParse(s.Substring(0, hIndex).Trim(), out int h))
                total += h * 60;

            var mIndex = s.IndexOf('m');
            if (mIndex > 0)
            {
                // take substring between 'h' and 'm' if exists, otherwise from start to 'm'
                var start = hIndex >= 0 ? hIndex + 1 : 0;
                var part = s.Substring(start, mIndex - start).Trim();
                if (int.TryParse(part, out int m))
                    total += m;
            }

            return total > 0 ? total : int.MaxValue;
        }

        private static DateTime TryParseDepartureDateTime(string date, string time)
        {
            // Your DB date format: "dd-MM-yyyy"
            // Your time might be "HH:mm" (example: "23:40")
            if (string.IsNullOrWhiteSpace(date)) return DateTime.MaxValue;

            var dtString = string.IsNullOrWhiteSpace(time) ? date : $"{date} {time}";
            var formats = string.IsNullOrWhiteSpace(time)
                ? new[] { "dd-MM-yyyy" }
                : new[] { "dd-MM-yyyy HH:mm", "dd-MM-yyyy H:mm" };

            return DateTime.TryParseExact(dtString, formats,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
                ? dt
                : DateTime.MaxValue;
        }

        [HttpGet]
        public BrowseViewModel GetFlightCatalog(
            string flight_id = null,
            int page = 1,
            string departure_id = null,
            string arrival_id = null,
            string departure_date = null,     // From (yyyy-MM-dd)
            string departure_date_to = null,  // To   (yyyy-MM-dd)
            string sort = null,
            string openFlightId = null)

        {
            this.repositoryUOW.HelperOleDb.OpenConnection();

            try
            {
                DateTime? ParseBrowserDate(string s)
                {
                    if (string.IsNullOrWhiteSpace(s)) return null;
                    // browser sends yyyy-MM-dd
                    if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
                        return d.Date;
                    return null;
                }

                DateTime? ParseDbDate(string s)
                {
                    if (string.IsNullOrWhiteSpace(s)) return null;
                    // DB stores dd-MM-yyyy
                    if (DateTime.TryParseExact(s, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
                        return d.Date;
                    return null;
                }

                const int flightsPerPage = 12;

                // 1) Build filtered list
                List<Flight> filtered;

                if (!string.IsNullOrEmpty(departure_id) && !string.IsNullOrEmpty(arrival_id))
                {
                    // NOTE: your repository call order looks suspicious (arrival_id, departure_id).
                    // Keep as-is to avoid breaking your existing logic, but verify the repository signature.
                    filtered = this.repositoryUOW.FlightRepository.GetFlightsByDepartureAndArrival(arrival_id, departure_id);
                }
                else if (!string.IsNullOrEmpty(departure_id))
                {
                    filtered = this.repositoryUOW.FlightRepository.GetFlightsByDeparture(departure_id);
                }
                else if (!string.IsNullOrEmpty(arrival_id))
                {
                    filtered = this.repositoryUOW.FlightRepository.GetFlightsByArrival(arrival_id);
                }
                else
                {
                    filtered = this.repositoryUOW.FlightRepository.GetALL();
                }
                filtered = ApplySort(filtered, sort);

                // Optional single-flight filter (if you actually use it)
                if (!string.IsNullOrEmpty(flight_id))
                    filtered = filtered.Where(f => f.Flight_Id == flight_id).ToList();

                // 2) If openFlightId is present, force page to the one containing that flight
                if (!string.IsNullOrEmpty(openFlightId))
                {
                    int index = filtered.FindIndex(f => f.Flight_Id == openFlightId);
                    if (index >= 0)
                    {
                        page = (index / flightsPerPage) + 1; // 1-based page
                    }
                    else
                    {
                        // If the flight isn't in filtered results, fallback to page 1
                        page = 1;
                    }
                }
                var from = ParseBrowserDate(departure_date);
                var to = ParseBrowserDate(departure_date_to);

                // If user accidentally swaps them, normalize
                if (from.HasValue && to.HasValue && from.Value > to.Value)
                {
                    var tmp = from;
                    from = to;
                    to = tmp;
                }

                if (from.HasValue || to.HasValue)
                {
                    filtered = filtered.Where(f =>
                    {
                        var dep = ParseDbDate(f.Departure_Date);
                        if (!dep.HasValue) return false;

                        if (from.HasValue && dep.Value < from.Value) return false;
                        if (to.HasValue && dep.Value > to.Value) return false;

                        return true;
                    }).ToList();
                }

                // 3) Normalize page
                if (page < 1) page = 1;

                // 4) Build VM
                var vm = new BrowseViewModel
                {
                    TotalCount = filtered.Count,
                    CurrentPage = page,
                    pageCount = (int)Math.Ceiling(filtered.Count / (double)flightsPerPage),
                    flights = filtered
                        .Skip(flightsPerPage * (page - 1))
                        .Take(flightsPerPage)
                        .ToList()
                };

                return vm;
            }
            catch
            {
                return null;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }
        [HttpGet]
        public AnnouncementViewModel GetAnnouncement()
        {
            AnnouncementViewModel announcementViewModel = new AnnouncementViewModel();
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                announcementViewModel.announcements = this.repositoryUOW.AnnouncementRepository.GetALL();
                return announcementViewModel;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }
        [HttpGet]
        public TicketViewModel GetTicketByUserId(string user_id)
        {
            TicketViewModel ticketViewModel = new TicketViewModel();
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();

                ticketViewModel.tickets = this.repositoryUOW.TicketRepository.GetByUserId(user_id);

                // Add: load flights for those tickets
                var flightIds = ticketViewModel.tickets
                    .Select(t => t.flight_Id)
                    .Distinct()
                    .ToList();

                ticketViewModel.flights = this.repositoryUOW.FlightRepository
                    .GetALL()
                    .Where(f => flightIds.Contains(f.flight_Id))
                    .ToList();

                return ticketViewModel;
            }
            catch
            {
                return null;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }

        [HttpGet]
        public DiscountViewModel GetDiscountByUserId(string user_id)
        {
            DiscountViewModel discountViewModel = new DiscountViewModel();
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                discountViewModel.discounts = this.repositoryUOW.DiscountRepository.GetByUserId(user_id);
                return discountViewModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }

        [HttpGet]
        public AnnouncementViewModel GetAnnouncementByUserId(string user_id)
        {
            AnnouncementViewModel announcementViewModel = new AnnouncementViewModel();
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                announcementViewModel.announcements = this.repositoryUOW.AnnouncementRepository.GetByUserId(user_id);
                announcementViewModel.publicAnnouncements = this.repositoryUOW.AnnouncementRepository.GetByUserId("0");

                // NEW: sort newest -> oldest by dd-MM-yyyy
                announcementViewModel.announcements = announcementViewModel.announcements
                    .OrderByDescending(a => DateTime.ParseExact(
                        a.Announcement_Date,   // <-- change property name if different
                        "dd-MM-yyyy",
                        CultureInfo.InvariantCulture))
                    .ToList();

                announcementViewModel.publicAnnouncements = announcementViewModel.publicAnnouncements
                    .OrderByDescending(a => DateTime.ParseExact(
                        a.Announcement_Date,   // <-- change property name if different
                        "dd-MM-yyyy",
                        CultureInfo.InvariantCulture))
                    .ToList();


                return announcementViewModel;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }
        [HttpGet]
        public List<Flight> GetAllFlights()
        {
            this.repositoryUOW.HelperOleDb.OpenConnection();
            try
            {
                return this.repositoryUOW.FlightRepository.GetALL();
            }
            catch
            {
                return null;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }

        [HttpGet]
        public User GetById(string user_id)
        {
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                return this.repositoryUOW.UserRepository.GetById(user_id); // implement if missing
            }
            catch
            {
                return null;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }
    }
}
