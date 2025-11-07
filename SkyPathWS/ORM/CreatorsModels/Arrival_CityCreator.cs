using ModelSkyPath.Models;
using SkyPath_Models.Models;
using System.Data;

namespace SkyPathWS.ORM.CreatorsModels
{
    public class Arrival_CityCreator : IModelCreator<Arrival_City>
    {
        public Arrival_City CreateModel(IDataReader dataReader)
        {
            Arrival_City arrival_City = new Arrival_City
            {
                Arrival_Id = Convert.ToString(dataReader["Arrival_Id"]),
                City_Name = Convert.ToString(dataReader["City_Name"])
            };
            return arrival_City;
        }
    }
}
