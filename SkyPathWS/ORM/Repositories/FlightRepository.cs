using ModelSkyPath.Models;
using SkyPath_Models;
using SkyPath_Models.Models;
using System.Data;
using System.Reflection;

namespace SkyPathWS.ORM.Repositories
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
        public List<Flight> GetFlightsByDeparture(string departureCityId)
        {
            string sql = @"Select * from Flight where Departure_Id=@Departure_Id";
            this.helperOleDb.AddParameter("@Departure_Id", departureCityId);
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
        public List<Flight> GetFlightsByArrival(string arrivalCityId)
        {
            string sql = @"Select * from Flight where Arrival_Id=@Arrival_Id";
            this.helperOleDb.AddParameter("@Arrival_Id", arrivalCityId);
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
        public List<Flight> GetFlightsByDepartureAndArrival(string arrivalCityId, string departureCityId)
        {
            string sql = @"Select * from Flight where Departure_Id=@Departure_Id AND Arrival_Id=@Arrival_Id";
            this.helperOleDb.AddParameter("@Departure_Id", departureCityId);
            this.helperOleDb.AddParameter("@Arrival_Id", arrivalCityId);
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
            this.helperOleDb.AddParameter("@Departure_Time", departure_Time);
            this.helperOleDb.AddParameter("@Arrival_Time", arrival_Time);
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
        public List<Flight> GetFlightsByPage(int page)
        {
            int flightsPerPage = 10;
            List<Flight> flights = this.GetALL();
            return flights.Skip(flightsPerPage * (page - 1)).Take(flightsPerPage).ToList();
        }
        public int GetFlightCount()
        {
            string sql = @"Select Count(Flight_Id) as FlightCount from Flights";
            using (IDataReader reader = this.helperOleDb.Select(sql))
            {
                reader.Read();
                return Convert.ToInt32(reader["FlightCount"]);
            }
        }
    }
}