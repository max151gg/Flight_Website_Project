using ModelSkyPath.Models;
using SkyPath_Models.Models;
using System.Data;

namespace SkyPathWS.ORM.CreatorsModels
{
    public class CityCreator : IModelCreator<City>
    {
        public City CreateModel(IDataReader dataReader)
        {
            City City = new City
            {
                CityId = Convert.ToString(dataReader["CityId"]),
                CityName = Convert.ToString(dataReader["CityName"])
            };
            return City;
        }
    }
}
