using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelSkyPath.Models;
using SkyPath_Models.Models;
using SkyPath_Models.ViewModel;
using SkyPathWS.ORM.Repositories;
using System.Globalization;
using System.Linq;


namespace SkyPathWS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GuestController : ControllerBase
    {
        RepositoryUOW repositoryUOW;
        public GuestController()
        {
            this.repositoryUOW = new RepositoryUOW();
        }

        [HttpGet]
        public AnnouncementViewModel Announcement()
        {
            this.repositoryUOW.HelperOleDb.OpenConnection();
            AnnouncementViewModel announcementViewModel = new AnnouncementViewModel();
            try
            {
                announcementViewModel.announcements = this.repositoryUOW.AnnouncementRepository.GetALL();

                // NEW: sort newest -> oldest by dd-MM-yyyy
                announcementViewModel.announcements = announcementViewModel.announcements
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
        public BrowseViewModel GetFlightCatalog(string flight_id = null, int page = 0, string departure_id = null, string arrival_id = null)
        {
            this.repositoryUOW.HelperOleDb.OpenConnection();
            BrowseViewModel browseViewModel = new BrowseViewModel();
            try
            {

                int flightsPerPage = 12;

                // 1) Get the FULL filtered list (for total count)
                List<Flight> filtered;

                if (departure_id != null && arrival_id != null)
                {
                    filtered = this.repositoryUOW.FlightRepository.GetFlightsByDepartureAndArrival(arrival_id, departure_id);
                }
                else if (departure_id != null)
                {
                    filtered = this.repositoryUOW.FlightRepository.GetFlightsByDeparture(departure_id);
                }
                else if (arrival_id != null)
                {
                    filtered = this.repositoryUOW.FlightRepository.GetFlightsByArrival(arrival_id);
                }
                else
                {
                    filtered = this.repositoryUOW.FlightRepository.GetALL();
                }

                // 2) Build the view model
                BrowseViewModel vm = new BrowseViewModel();
                vm.TotalCount = filtered.Count;

                // 3) Normalize page (your current default is 0; we convert it to 1)
                if (page <= 0) page = 1;
                vm.CurrentPage = page;

                // 4) Slice the list for the current page
                vm.flights = filtered
                    .Skip(flightsPerPage * (page - 1))
                    .Take(flightsPerPage)
                    .ToList();

                // 5) Optional: total pages (handy for UI)
                vm.pageCount = (int)Math.Ceiling(vm.TotalCount / (double)flightsPerPage);

                return vm;

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
        [HttpPost]
        public bool Register(User user)
        {
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                return this.repositoryUOW.UserRepository.Create(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }
        [HttpPost]
        public string Login(string userName, string password)
        {
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                string id = this.repositoryUOW.UserRepository.Login(userName, password);
                if (id != null)
                    return id;
                else
                    return "No User";
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

    }
}

