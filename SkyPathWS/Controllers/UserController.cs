using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkyPath_Models.Models;
using SkyPath_Models.ViewModel;
using SkyPathWS.ORM.Repositories;

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
        [HttpGet]
        public BrowseViewModel GetFlightCatalog(string flight_id = null, int page = 0, string departure_id = null, string arrival_id = null)
        {
            BrowseViewModel browseViewModel = new BrowseViewModel();
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();

                if (page == 0 && departure_id == null && arrival_id == null)
                {
                    browseViewModel.flights = this.repositoryUOW.FlightRepository.GetALL();
                    return browseViewModel;
                }
                else if (page == 0 && departure_id != null && arrival_id == null)
                {
                    browseViewModel.flights = this.repositoryUOW.FlightRepository.GetFlightsByDeparture(departure_id);
                    return browseViewModel;
                }
                else if (page == 0 && departure_id == null && arrival_id != null)
                {
                    browseViewModel.flights = this.repositoryUOW.FlightRepository.GetFlightsByArrival(arrival_id);
                    return browseViewModel;
                }
                else if (page != 0 && departure_id == null && arrival_id == null)
                {
                    browseViewModel.flights = this.repositoryUOW.FlightRepository.GetFlightsByPage(page);
                    return browseViewModel;
                }
                else if (page != 0 && departure_id != null && arrival_id == null)
                {
                    int flightsPerPage = 10;
                    browseViewModel.flights = this.repositoryUOW.FlightRepository.GetFlightsByDeparture(departure_id);
                    browseViewModel.flights.Skip(flightsPerPage * (page - 1)).Take(flightsPerPage).ToList();
                }
                else if (page != 0 && departure_id == null && arrival_id != null)
                {
                    int flightsPerPage = 10;
                    browseViewModel.flights = this.repositoryUOW.FlightRepository.GetFlightsByArrival(arrival_id);
                    browseViewModel.flights.Skip(flightsPerPage * (page - 1)).Take(flightsPerPage).ToList();
                }
                else if (page == 0 && departure_id != null && arrival_id != null)
                {
                    int flightsPerPage = 10;
                    browseViewModel.flights = this.repositoryUOW.FlightRepository.GetFlightsByDepartureAndArrival(arrival_id, departure_id);
                    browseViewModel.flights.Skip(flightsPerPage * (page - 1)).Take(flightsPerPage).ToList();
                }
                else if (page != 0 && departure_id != null && arrival_id != null)
                {
                    int flightsPerPage = 10;
                    browseViewModel.flights = this.repositoryUOW.FlightRepository.GetFlightsByDepartureAndArrival(arrival_id, departure_id);
                    browseViewModel.flights.Skip(flightsPerPage * (page - 1)).Take(flightsPerPage).ToList();
                }
                this.repositoryUOW.HelperOleDb.CloseConnection();
                return browseViewModel;
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
        public TicketViewModel GetTicketByUserId(string user_id = null)
        {
            TicketViewModel ticketViewModel = new TicketViewModel();
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                ticketViewModel.tickets = this.repositoryUOW.TicketRepository.GetByUserId(user_id);
                return ticketViewModel;
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
        public DiscountViewModel GetDiscountByUserId(string user_id = null)
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
    }
}
