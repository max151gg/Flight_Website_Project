using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelSkyPath.Models;
using SkyPath_Models.Models;
using SkyPath_Models.ViewModel;
using SkyPathWS.ORM.Repositories;
using System.Globalization;
using System.Text.Json.Serialization;


namespace SkyPathWS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        RepositoryUOW repositoryUOW;
        public AdminController()
        {
            this.repositoryUOW = new RepositoryUOW();
        }
        [HttpGet]
        public UserViewModel GetUser()
        {
            UserViewModel userViewModel = new UserViewModel();
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                userViewModel.users = this.repositoryUOW.UserRepository.GetALL();
                return userViewModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }
        [HttpGet]
        public bool DeleteDiscount(string discount_id)
        {
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                return this.repositoryUOW.DiscountRepository.Delete(discount_id);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }
        [HttpGet]
        public bool DeleteAnnouncement(string announcement_id)
        {
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                return this.repositoryUOW.AnnouncementRepository.Delete(announcement_id);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }
        [HttpGet]
        public bool DeleteUser(string user_id)
        {
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                return this.repositoryUOW.UserRepository.Delete(user_id);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }
        [HttpGet]
        public bool DeleteFlight(string flight_id)
        {
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                return this.repositoryUOW.FlightRepository.Delete(flight_id);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }
        [HttpGet]
        public bool DeleteTicket(string ticket_id)
        {
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                return this.repositoryUOW.TicketRepository.Delete(ticket_id);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }
        [HttpGet]
        public bool DeleteArrival_City(string city_id)
        {
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                return this.repositoryUOW.Arrival_CityRepository.Delete(city_id);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }
        [HttpGet]
        public bool DeleteDeparture_City(string city_id)
        {
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                return this.repositoryUOW.Departure_CityRepository.Delete(city_id);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }
        [HttpPost]
        public bool CreateAnnouncement(Announcement announcement)
        {
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                return this.repositoryUOW.AnnouncementRepository.Create(announcement);
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
        public bool CreateDiscount(Discount discount)
        {
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                return this.repositoryUOW.DiscountRepository.Create(discount);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }
        [HttpPost]
        public bool CreateDepartureCity(Departure_City departure_City)
        {
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                return this.repositoryUOW.Departure_CityRepository.Create(departure_City);
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
        public bool CreateArrival_City(Arrival_City arrival_City)
        {
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                return this.repositoryUOW.Arrival_CityRepository.Create(arrival_City);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }
        [HttpPost]
        public bool CreateTicket(Ticket ticket)
        {
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                return this.repositoryUOW.TicketRepository.Create(ticket);
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
        public bool CreateFlight(Flight flight)
        {
            try
            {
                repositoryUOW.HelperOleDb.OpenConnection();
                return repositoryUOW.FlightRepository.Create(flight);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            finally
            {
                repositoryUOW.HelperOleDb.CloseConnection();
            }
        }

        //[HttpPost]
        //public bool AddNewFlight(Flight flight)
        //{
        //    string json = Request.Form["model"].ToString();
        //    FlightViewModel newflightviewmodel = JsonSerializableAttribute.Deserialize<FlightViewModel>(json);
        //}


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
        public List<Announcement> GetAllAnnouncements()
        {
            this.repositoryUOW.HelperOleDb.OpenConnection();
            try
            {
                var list = this.repositoryUOW.AnnouncementRepository.GetALL() ?? new List<Announcement>();

                // NEW: sort newest -> oldest (dd-MM-yyyy)
                return list.OrderByDescending(a =>DateTime.TryParseExact(a.Announcement_Date, "dd-MM-yyyy",CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)? dt : DateTime.MinValue).ToList();

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
