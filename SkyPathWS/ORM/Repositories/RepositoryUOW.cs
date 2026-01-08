namespace SkyPathWS.ORM.Repositories
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
        CityRepository cityRepository;

        DbHelperOleDb helperOleDb;
        ModelCreators modelCreators;

        public RepositoryUOW()
        {
            helperOleDb = new DbHelperOleDb();
            modelCreators = new ModelCreators();
        }

        public DbHelperOleDb HelperOleDb => helperOleDb;

        public CityRepository CityRepository
        {
            get
            {
                if (cityRepository == null)
                    cityRepository = new CityRepository(helperOleDb, modelCreators);
                return cityRepository;
            }
        }

        public AnnouncementRepository AnnouncementRepository
        {
            get
            {
                if (announcementRepository == null)
                    announcementRepository = new AnnouncementRepository(helperOleDb, modelCreators);

                return announcementRepository;
            }
        }

        public Arrival_CityRepository Arrival_CityRepository
        {
            get
            {
                if (arrival_CityRepository == null)      // FIXED
                    arrival_CityRepository = new Arrival_CityRepository(helperOleDb, modelCreators);

                return arrival_CityRepository;
            }
        }

        public Departure_CityRepository Departure_CityRepository
        {
            get
            {
                if (departure_CityRepository == null)    // FIXED
                    departure_CityRepository = new Departure_CityRepository(helperOleDb, modelCreators);

                return departure_CityRepository;
            }
        }

        public DiscountRepository DiscountRepository
        {
            get
            {
                if (discountRepository == null)
                    discountRepository = new DiscountRepository(helperOleDb, modelCreators);

                return discountRepository;
            }
        }

        public FlightRepository FlightRepository
        {
            get
            {
                if (flightRepository == null)
                    flightRepository = new FlightRepository(helperOleDb, modelCreators);

                return flightRepository;
            }
        }

        public TicketRepository TicketRepository
        {
            get
            {
                if (ticketRepository == null)
                    ticketRepository = new TicketRepository(helperOleDb, modelCreators);

                return ticketRepository;
            }
        }

        public UserRepository UserRepository
        {
            get
            {
                if (userRepository == null)
                    userRepository = new UserRepository(helperOleDb, modelCreators);

                return userRepository;
            }
        }
    }
}
