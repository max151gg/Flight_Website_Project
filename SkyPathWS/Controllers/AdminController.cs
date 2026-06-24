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
    // API used by the WPF admin app. Each method does one admin job:
    // managing users, flights, tickets, discounts and announcements in the database.
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
        public bool UpdateUserRole(string user_id, string role_id)
        {
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                return this.repositoryUOW.UserRepository.UpdateUserRole(user_id, role_id);
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }
        [HttpGet]
        public List<Ticket> GetAllTickets()
        {
            this.repositoryUOW.HelperOleDb.OpenConnection();
            try
            {
                return this.repositoryUOW.TicketRepository.GetALL();
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
        public List<Discount> GetAllDiscounts()
        {
            this.repositoryUOW.HelperOleDb.OpenConnection();
            try
            {
                return this.repositoryUOW.DiscountRepository.GetALL();
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
        public bool UpdateTicketStatus(string ticket_id, bool status)
        {
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                return this.repositoryUOW.TicketRepository.UpdateTicketStatus(ticket_id, status);
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }
        [HttpGet]
        public bool BanUser(string user_id, bool user_ban)
        {
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                return this.repositoryUOW.UserRepository.BanUser(user_id, user_ban);
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }

        // Deletes a user from the database and also removes their saved profile image file.
        [HttpGet]
        public bool DeleteUser(string user_id, [FromServices] IWebHostEnvironment env)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user_id))
                {
                    return false;
                }

                this.repositoryUOW.HelperOleDb.OpenConnection();
                bool deleted = this.repositoryUOW.UserRepository.Delete(user_id);

                if (!deleted)
                {
                    return false;
                }

                string folder = Path.Combine(env.WebRootPath, "images", "profiles");
                string baseName = $"profile_{user_id}";
                string[] variants = { ".png", ".jpg", ".jpeg", ".gif", ".jfif" };

                foreach (string v in variants)
                {
                    string p = Path.Combine(folder, baseName + v);
                    if (System.IO.File.Exists(p))
                    {
                        System.IO.File.Delete(p);
                    }
                }

                return true;
            }
            catch (Exception)
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
        // Cancels or reactivates a flight (it is never physically deleted, which keeps it
        // safe even when the flight has tickets).
        //   isActive = false -> mark the flight cancelled AND cancel all of its tickets.
        //   isActive = true  -> mark the flight active again (its tickets are left as they are;
        //                       the admin can reactivate individual tickets on the Tickets page).
        // Returns true only if the flight status was successfully updated.
        [HttpGet]
        public bool SetFlightActiveStatus(string flight_id, bool isActive)
        {
            try
            {
                this.repositoryUOW.HelperOleDb.OpenConnection();
                bool updated = this.repositoryUOW.FlightRepository.UpdateFlightStatus(flight_id, isActive);
                if (updated && !isActive)
                {
                    // Only when cancelling: also cancel every ticket for this flight.
                    this.repositoryUOW.TicketRepository.CancelTicketsByFlightId(flight_id);
                }
                return updated;
            }
            catch (Exception)
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


        // Adds a new flight (used by the "Add New Flight" page in the admin app).
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

        [HttpPost]
        public bool UpdateFlight(Flight flight)
        {
            try
            {
                repositoryUOW.HelperOleDb.OpenConnection();
                return repositoryUOW.FlightRepository.Update(flight);
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
                return list.OrderByDescending(a => DateTime.TryParseExact(a.Announcement_Date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt) ? dt : DateTime.MinValue).ToList();

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
