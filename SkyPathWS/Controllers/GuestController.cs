using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelSkyPath.Models;
using SkyPath_Models.Models;
using SkyPath_Models.ViewModel;
using SkyPathWS.ORM.Repositories;

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
        public BrowseViewModel GetFlightCatalog(string flight_id = null, int page = 0, string departure_id = null, string arrival_id = null)
        {
            this.repositoryUOW.HelperOleDb.OpenConnection();
            BrowseViewModel browseViewModel = new BrowseViewModel();
            try
            {

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
                //int books = this.repositoryUOW.FlightRepository.
                //browseViewModel.pageCount = f
                //if (flights)
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

