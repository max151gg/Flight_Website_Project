using SkyPath_Models.Models;
using System.Data;
namespace SkyPathWS.ORM.CreatorsModels
{
    // Turns one row from the Flight table into a Flight object.
    // Input: a DataReader sitting on a flight row. Output: a filled Flight.
    // Used by FlightRepository every time it reads flights from the database.
    public class FlightCreator : IModelCreator<Flight>
    {
        public Flight CreateModel(IDataReader dataReader)
        {
            // Read each column by its name and convert it to the matching C# type.
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
