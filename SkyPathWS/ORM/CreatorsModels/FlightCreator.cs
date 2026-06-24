using SkyPath_Models.Models;
using System;
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
                Arrival_Date = Convert.ToString(dataReader["Arrival_Date"]),
                // Read the cancel/active flag. Access stores Yes/No as a boolean.
                IsActive = ReadIsActive(dataReader)
            };
            return flight;
        }

        // Reads the IsActive column safely. If the column does not exist yet (old database)
        // or is empty, we treat the flight as active (true) so nothing breaks.
        private static bool ReadIsActive(IDataReader dataReader)
        {
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                if (string.Equals(dataReader.GetName(i), "IsActive", StringComparison.OrdinalIgnoreCase))
                {
                    object value = dataReader.GetValue(i);
                    if (value == null || value == DBNull.Value) return true;
                    return Convert.ToBoolean(value);
                }
            }
            return true;
        }
    }

}
