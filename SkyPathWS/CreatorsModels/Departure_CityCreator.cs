using SkyPath_Models.Models;
using System.Data;

namespace SkyPathWS.CreatorsModels
{
    public class Departure_CityCreator : IModelCreator<Departure_City>
    {
        public Departure_City CreateModel(IDataReader dataReader)
        {
            Departure_City departure_City = new Departure_City
            {
                Departure_Id = Convert.ToString(dataReader["Departure_Id"]),
                City_Name = Convert.ToString(dataReader["City_Name"])
            };
            return departure_City;
        }
    }
}
