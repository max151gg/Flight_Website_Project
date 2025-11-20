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

        DbHelperOleDb helperOleDb;
        ModelCreators modelCreators;
        public RepositoryUOW()
        {
            this.helperOleDb = new DbHelperOleDb();
            this.modelCreators = new ModelCreators();
        }
        public DbHelperOleDb HelperOleDb
        {
            get
            {
                return this.helperOleDb;
            }
        }

        public AnnouncementRepository AnnouncementRepository
        {
            get
            {
                if (this.announcementRepository == null)
                {
                    this.announcementRepository = new AnnouncementRepository(this.helperOleDb, this.modelCreators);
                }
                return this.announcementRepository;
            }
        }
        public Arrival_CityRepository Arrival_CityRepository
        {
            get
            {
                if (this.Arrival_CityRepository == null)
                {
                    this.arrival_CityRepository = new Arrival_CityRepository(this.helperOleDb, this.modelCreators);
                }
                return this.arrival_CityRepository;
            }
        }
        public Departure_CityRepository Departure_CityRepository
        {
            get
            {
                if (this.Departure_CityRepository == null)
                {
                    this.departure_CityRepository = new Departure_CityRepository(this.helperOleDb, this.modelCreators);
                }
                return this.departure_CityRepository;
            }
        }
        public DiscountRepository DiscountRepository
        {
            get
            {
                if (this.discountRepository == null)
                {
                    this.discountRepository = new DiscountRepository(this.helperOleDb, this.modelCreators);
                }
                return this.discountRepository;
            }
        }
        public FlightRepository FlightRepository
        {
            get
            {
                if (this.flightRepository == null)
                {
                    this.flightRepository = new FlightRepository(this.helperOleDb, this.modelCreators);
                }
                return this.flightRepository;
            }
        }
        public TicketRepository TicketRepository
        {
            get
            {
                if (this.ticketRepository == null)
                {
                    this.ticketRepository = new TicketRepository(this.helperOleDb, this.modelCreators);
                }
                return this.ticketRepository;
            }
        }
        public UserRepository UserRepository
        {
            get
            {
                if (this.userRepository == null)
                {
                    this.userRepository = new UserRepository(this.helperOleDb, this.modelCreators);
                }
                return this.userRepository;
            }
        }
    }
}
