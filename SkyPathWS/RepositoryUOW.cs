using SkyPathWS.Repositories;

namespace SkyPathWS
{
    public class RepositoryUOW
    {
        AnnouncementRepository announcementRepository;
        Arrival_CityRepository arrival_CityRepository;
        Departure_CityRepository departure_CityRepository;
        DiscountRepository discountRepository;
        FlightRepository flightRepository;
        TicketRepository ticketRepository;
        UserRepository userRepository;


        public AnnouncementRepository AnnouncementRepository
        {
            get
            {
                if (this.announcementRepository == null)
                {
                    this.announcementRepository = new AnnouncementRepository();
                }
                return this.announcementRepository;
            }
        }
    }
}
