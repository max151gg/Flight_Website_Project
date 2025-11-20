using ModelSkyPath.Models;
using SkyPath_Models.Models;
using System.Data;

namespace SkyPathWS.Repositories
{
    public class FlightRepository : Repository, IRepository<Flight>
    {
        public FlightRepository(DbHelperOleDb helperOleDb, ModelCreators modelCreators) : base(helperOleDb, modelCreators)
        {
        }
        public bool Create(Flight model)
        {
            string sql = $@"Insert into Flight
                            (
                            Flight_Number, Airline, Departure_Id,
                            Arrival_Id, Departure_Time, Arrival_Time,
                            Price, Seats_Available
                            )
                            values
                            (
                                @Flight_Number, @Airline, @Departure_Id,
                                @Arrival_Id, @Departure_Time, @Arrival_Time,
                                @Price, @Seats_Available
                            )";
            this.helperOleDb.AddParameter("@Flight_Number", model.Flight_Number);
            this.helperOleDb.AddParameter("@Airline", model.Airline);
            this.helperOleDb.AddParameter("@Departure_Id", model.Departure_Id);
            this.helperOleDb.AddParameter("@Arrival_Id", model.Arrival_Id);
            this.helperOleDb.AddParameter("@Departure_Time", model.Departure_Time);
            this.helperOleDb.AddParameter("@Arrival_Time", model.Arrival_Time);
            this.helperOleDb.AddParameter("@Price", model.Price);
            this.helperOleDb.AddParameter("@Seats_Available", model.Seats_Available);
            return this.helperOleDb.Insert(sql) > 0;
        }

        public bool Delete(string id)
        {
            string sql = @"Delete from Flight where Flight_Id=@Flight_Id";
            this.helperOleDb.AddParameter("@Flight_Id", id);
            return this.helperOleDb.Delete(sql) > 0;
        }

        public List<Flight> GetALL()
        {
            string sql = "Select * from Flight";

            List<Flight> flights = new List<Flight>();
            using (IDataReader reader = this.helperOleDb.Select(sql))
            {
                while (reader.Read())
                {
                    flights.Add(this.modelCreators.FlightCreator.CreateModel(reader));
                }
            }
            return flights;
        }

        public Flight GetById(string id)
        {
            string sql = "Select * from Flight where Flight_Id=@Flight_Id";
            this.helperOleDb.AddParameter("@Flight_Id", id);
            using (IDataReader reader = this.helperOleDb.Select(sql))
            {
                reader.Read();
                return this.modelCreators.FlightCreator.CreateModel(reader);
            }
        }

        public bool Update(Flight model)
        {
            string sql = @"Update Flight set 
                            Flight_Number=@Flight_Number, Airline=@Airline, Departure_Id=@Departure_Id,
                            Arrival_Id=@Arrival_Id, Departure_Time=@Departure_Time, Arrival_Time=@Arrival_Time,
                            Price=@Price, Seats_Available=@Seats_Available";
            this.helperOleDb.AddParameter("@Flight_Number", model.Flight_Number);
            this.helperOleDb.AddParameter("@Airline", model.Airline);
            this.helperOleDb.AddParameter("@Departure_Id", model.Departure_Id);
            this.helperOleDb.AddParameter("@Arrival_Id", model.Arrival_Id);
            this.helperOleDb.AddParameter("@Departure_Time", model.Departure_Time);
            this.helperOleDb.AddParameter("@Arrival_Time", model.Arrival_Time);
            this.helperOleDb.AddParameter("@Price", model.Price);
            this.helperOleDb.AddParameter("@Seats_Available", model.Seats_Available);
            return this.helperOleDb.Insert(sql) > 0;
        }
        //מעתיקים GET ALL FUNCTION ומשנים את משפטי SQL לפי הUSE CASE כגון(FILTER BY DATE, BY PRICE...)
        public List<Flight> GetFlightsByDepartureAndArrival(string departureCityId, string arrivalCityId)
        {
            string sql = @"Select * from Flight where Departure_Id=@Departure_Id AND Arrival_Id=@Arrival_Id";
            List<Flight> flights = new List<Flight>();
            using (IDataReader reader = this.helperOleDb.Select(sql))
            {
                while (reader.Read())
                {
                    flights.Add(this.modelCreators.FlightCreator.CreateModel(reader));
                }
            }
            return flights;
        }
        public List<Flight> GetFlightsByDepartureTimeAndArrivalTime(string departure_Time, string arrival_Time)
        {
            string sql = @"Select * from Flight where Departure_Time=@Departure_Time AND Arrival_Time=@Arrival_Time";
            List<Flight> flights = new List<Flight>();
            using (IDataReader reader = this.helperOleDb.Select(sql))
            {
                while (reader.Read())
                {
                    flights.Add(this.modelCreators.FlightCreator.CreateModel(reader));
                }
            }
            return flights;
        }
    }
}