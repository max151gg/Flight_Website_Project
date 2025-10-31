using SkyPathWS.CreatorsModels;

namespace SkyPathWS
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
                if(this.UserCreator == null)
                {
                    userCreator = new UserCreator();
                }
                return this.userCreator;
            }
        }
        public FlightCreator FlightCreator
        {
            get
            {
                if (this.flightCreator == null)
                {
                    flightCreator = new FlightCreator();
                }
                return this.flightCreator;
            }
        }
        public AnnouncementCreator AnnouncementCreator
        {
            get
            {
                if (this.announcementCreator == null)
                {
                    announcementCreator = new AnnouncementCreator();
                }
                return this.announcementCreator;
            }
        }
        public DiscountCreator DiscountCreator
        {
            get
            {
                if (this.discountCreator == null)
                {
                    discountCreator = new DiscountCreator();
                }
                return this.discountCreator;
            }
        }
        public Arrival_CityCreator Arrival_CityCreator
        {
            get
            {
                if (this.arrival_CityCreator == null)
                {
                    arrival_CityCreator = new Arrival_CityCreator();
                }
                return this.arrival_CityCreator;
            }
        }
        public Departure_CityCreator Departure_CityCreator
        {
            get
            {
                if (this.departure_CityCreator == null)
                {
                    departure_CityCreator = new Departure_CityCreator();
                }
                return this.departure_CityCreator;
            }
        }
        public TicketCreator TicketCreator
        {
            get
            {
                if (this.ticketCreator == null)
                {
                    ticketCreator = new TicketCreator();
                }
                return this.ticketCreator;
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
