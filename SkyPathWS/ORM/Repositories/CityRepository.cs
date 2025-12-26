using ModelSkyPath.Models;
using SkyPath_Models.Models;
using System.Data;

namespace SkyPathWS.ORM.Repositories
{
    public class CityRepository : Repository, IRepository<City>
    {
        public CityRepository(DbHelperOleDb helperOleDb, ModelCreators modelCreators) : base(helperOleDb, modelCreators)
        {
        }
        public bool Create(City model)
        {
            string sql = $@"Insert into City
                            (
                            CityName
                            )
                            values
                            (
                                @CityName
                            )";
            helperOleDb.AddParameter("@CityName", model.CityName);
            return helperOleDb.Insert(sql) > 0;
        }
        public bool Delete(string id)
        {
            string sql = @"Delete from City where CityId=@CityId";
            helperOleDb.AddParameter("@CityId", id);
            return helperOleDb.Delete(sql) > 0;
        }
        public List<City> GetALL()
        {
            string sql = "Select * from City";

            List<City> Cities = new List<City>();
            using (IDataReader reader = helperOleDb.Select(sql))
            {
                while (reader.Read())
                {
                    Cities.Add(modelCreators.CityCreator.CreateModel(reader));
                }
            }
            return Cities;
        }
        public City GetById(string id)
        {
            string sql = "Select * from City where CityId=@CityId";
            helperOleDb.AddParameter("@CityId", id);
            using (IDataReader reader = helperOleDb.Select(sql))
            {
                reader.Read();
                return modelCreators.CityCreator.CreateModel(reader);
            }
        }
        public bool Update(City model)
        {
            string sql = @"Update City set 
                            CityName=@CityName";
            helperOleDb.AddParameter("@CityName", model.CityName);
            return helperOleDb.Insert(sql) > 0;
        }
    }
}