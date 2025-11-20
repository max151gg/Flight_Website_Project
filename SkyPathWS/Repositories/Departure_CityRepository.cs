using SkyPath_Models.Models;
using System.Data;

namespace SkyPathWS.Repositories
{
    public class Departure_CityRepository : Repository, IRepository<Departure_City>
    {
        public Departure_CityRepository(DbHelperOleDb helperOleDb, ModelCreators modelCreators) : base(helperOleDb, modelCreators)
        {
        }
        public bool Create(Departure_City model)
        {
            string sql = $@"Insert into Departure_City
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
            string sql = @"Delete from Departure_City where departure_Id=@departure_Id";
            this.helperOleDb.AddParameter("@departure_Id", id);
            return this.helperOleDb.Delete(sql) > 0;
        }
        public List<Departure_City> GetALL()
        {
            string sql = "Select * from Departure_City";

            List<Departure_City> departure_Cities = new List<Departure_City>();
            using (IDataReader reader = this.helperOleDb.Select(sql))
            {
                while (reader.Read())
                {
                    departure_Cities.Add(this.modelCreators.Departure_CityCreator.CreateModel(reader));
                }
            }
            return departure_Cities;
        }
        public Departure_City GetById(string id)
        {
            string sql = "Select * from Departure_Ciry where departure_Id=@departure_Id";
            this.helperOleDb.AddParameter("@departure_Id", id);
            using (IDataReader reader = this.helperOleDb.Select(sql))
            {
                reader.Read();
                return this.modelCreators.Departure_CityCreator.CreateModel(reader);
            }
        }
        public bool Update(Departure_City model)
        {
            string sql = @"Update Departure-City set 
                            City_Name=@City_Name";
            this.helperOleDb.AddParameter("@City_Name", model.City_Name);
            return this.helperOleDb.Insert(sql) > 0;
        }
    }
}