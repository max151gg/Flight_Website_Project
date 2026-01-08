using SkyPath_Models.Models;
using System.Data;
namespace SkyPathWS.ORM.CreatorsModels
{
    public class FlightCreator : IModelCreator<Flight>
    {
        public Flight CreateModel(IDataReader dataReader)
        {
            Flight flight = new Flight
            {
                Flight_Id = Convert.ToString(dataReader["Flight_Id"]),
                Flight_Number = Convert.ToString(dataReader["Flight_Number"]),
                Airline = Convert.ToString(dataReader["Airline"]),
                Departure_Id = Convert.ToString(dataReader["Departure_Id"]),
                Arrival_Id = Convert.ToString(dataReader["Arrival_Id"]),
                Departure_Time = Convert.ToString(dataReader["Departure_Time"]),
                Arrival_Time = Convert.ToString(dataReader["Arrival_Time"]),
                Price = Convert.ToDouble(dataReader["Price"]),
                Seats_Available = Convert.ToInt16(dataReader["Seats_Available"]),
                Departure_Date = Convert.ToString(dataReader["Departure_Date"]),
                Arrival_Date = Convert.ToString(dataReader["Arrival_Date"])
            };
            return flight;
        }
    }

}
