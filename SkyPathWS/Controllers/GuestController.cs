using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkyPath_Models.Models;
using SkyPath_Models.ViewModel;

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
        }//ToDo
        //i added controller for guest, need to add more "filters" like above, the "filters" are connected to Repositories, for example if i want to do:"
        //else if (page != 0 && departure_id != null && arrival_id != null)
        //        {
        //            int flightsPerPage = 10;
        //browseViewModel.flights = this.repositoryUOW.FlightRepository.GetFlightsByDepartureAndArrival(arrival_id, departure_id);
        //            browseViewModel.flights.Skip(flightsPerPage* (page - 1)).Take(flightsPerPage).ToList();
        //
        //I need to have the "GetFlightsByDepartureAndArrival" on the FlightRepository page
        //
        //need to check hte filters on swagger which turns on when you run the site, need to create more filters like on usecase
    }
}

