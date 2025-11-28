using SkyPathWS.ORM.CreatorsModels;

namespace SkyPathWS.ORM
{
    public class ModelCreators
    {
        UserCreator userCreator;
        FlightCreator flightCreator;
        AnnouncementCreator announcementCreator;
        DiscountCreator discountCreator;
        Arrival_CityCreator arrival_CityCreator;
        Departure_CityCreator departure_CityCreator;
        TicketCreator ticketCreator;

        public UserCreator UserCreator 
        {
            get
            { 
                if(userCreator == null)
                {
                    userCreator = new UserCreator();
                }
                return userCreator;
            }
        }
        public FlightCreator FlightCreator
        {
            get
            {
                if (flightCreator == null)
                {
                    flightCreator = new FlightCreator();
                }
                return flightCreator;
            }
        }
        public AnnouncementCreator AnnouncementCreator
        {
            get
            {
                if (announcementCreator == null)
                {
                    announcementCreator = new AnnouncementCreator();
                }
                return announcementCreator;
            }
        }
        public DiscountCreator DiscountCreator
        {
            get
            {
                if (discountCreator == null)
                {
                    discountCreator = new DiscountCreator();
                }
                return discountCreator;
            }
        }
        public Arrival_CityCreator Arrival_CityCreator
        {
            get
            {
                if (arrival_CityCreator == null)
                {
                    arrival_CityCreator = new Arrival_CityCreator();
                }
                return arrival_CityCreator;
            }
        }
        public Departure_CityCreator Departure_CityCreator
        {
            get
            {
                if (departure_CityCreator == null)
                {
                    departure_CityCreator = new Departure_CityCreator();
                }
                return departure_CityCreator;
            }
        }
        public TicketCreator TicketCreator
        {
            get
            {
                if (ticketCreator == null)
                {
                    ticketCreator = new TicketCreator();
                }
                return ticketCreator;
            }
        }


        //public ModelCreators()
        //{
        //    userCreator = new UserCreator();
        //    flightCreator = new FlightCreator();
        //    announcementCreator = new AnnouncementCreator();
        //    discountCreator = new DiscountCreator();
        //    arrival_CityCreator = new Arrival_CityCreator();
        //    departure_CityCreator = new Departure_CityCreator();
        //}
    }
}
