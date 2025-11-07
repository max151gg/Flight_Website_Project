using ModelSkyPath.Models;
using SkyPath_Models.Models;
using System.Data;

namespace SkyPathWS.Repositories
{
    public class Arrival_CityRepository : Repository, IRepository<Arrival_City>
    {
        public bool Create(Arrival_City model)
        {
            string sql = $@"Insert into Arrival_City
                            (
                            City_Name
                            )
                            values
                            (
                                @City_Name
                            )";
            this.helperOleDb.AddParameter("@City_Name", model.City_Name);
            return this.helperOleDb.Insert(sql) > 0;
        }
        public bool Delete(string id)
        {
            string sql = @"Delete from Arrival_City where Arrival_Id=@Arrival_Id";
            this.helperOleDb.AddParameter("@Arrival_Id", id);
            return this.helperOleDb.Delete(sql) > 0;
        }
        public List<Arrival_City> GetALL()
        {
            string sql = "Select * from Arrival_City";

            List<Arrival_City> arrival_Cities = new List<Arrival_City>();
            using (IDataReader reader = this.helperOleDb.Select(sql))
            {
                while (reader.Read())
                {
                    arrival_Cities.Add(this.modelCreators.Arrival_CityCreator.CreateModel(reader));
                }
            }
            return arrival_Cities;
        }
        public Arrival_City GetById(string id)
        {
            string sql = "Select * from Arrival_City where Arrival_Id=@Arrival_Id";
            this.helperOleDb.AddParameter("@Arrival_Id", id);
            using (IDataReader reader = this.helperOleDb.Select(sql))
            {
                reader.Read();
                return this.modelCreators.Arrival_CityCreator.CreateModel(reader);
            }
        }
        public bool Update(Arrival_City model)
        {
            string sql = @"Update Arrival_City set 
                            City_Name=@City_Name";
            this.helperOleDb.AddParameter("@City_Name", model.City_Name);
            return this.helperOleDb.Insert(sql) > 0;
        }
    }
}