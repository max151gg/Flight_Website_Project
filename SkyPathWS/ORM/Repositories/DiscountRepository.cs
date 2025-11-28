using ModelSkyPath.Models;
using SkyPath_Models.Models;
using System.Data;

namespace SkyPathWS.ORM.Repositories
{
    public class DiscountRepository : Repository, IRepository<Discount>
    {
        public DiscountRepository(DbHelperOleDb helperOleDb, ModelCreators modelCreators) : base(helperOleDb, modelCreators)
        {
        }
        public bool Create(Discount model)
        {
            string sql = $@"Insert into Discount
                            (
                            Description, Percentage, Valid_From,
                            Valid_To, User_Id
                            )
                            values
                            (
                                @Description, @Percentage, @Valid_From, @Valid_To, @User_Id
                            )";
            helperOleDb.AddParameter("@Description", model.Description);
            helperOleDb.AddParameter("@Percentage", model.Percentage);
            helperOleDb.AddParameter("@Valid_From", model.Valid_From);
            helperOleDb.AddParameter("@Valid_To", model.Valid_To);
            helperOleDb.AddParameter("@User_Id", model.User_Id);
            return helperOleDb.Insert(sql) > 0;
        }

        public bool Delete(string id)
        {
            string sql = @"Delete from Discount where Discount_Id=@Discount_Id";
            helperOleDb.AddParameter("@Discount_Id", id);
            return helperOleDb.Delete(sql) > 0;
        }

        public List<Discount> GetALL()
        {
            string sql = "Select * from Discount";

            List<Discount> discounts = new List<Discount>();
            using (IDataReader reader = helperOleDb.Select(sql))
            {
                while (reader.Read())
                {
                    discounts.Add(modelCreators.DiscountCreator.CreateModel(reader));
                }
            }
            return discounts;
        }

        public Discount GetById(string id)
        {
            string sql = "Select * from Discount where Discount_Id=@Discount_Id";
            helperOleDb.AddParameter("@Discount_Id", id);
            using (IDataReader reader = helperOleDb.Select(sql))
            {
                reader.Read();
                return modelCreators.DiscountCreator.CreateModel(reader);
            }
        }
        public List<Discount> GetByUserId(string user_id)
        {
            string sql = "Select * from Discount where User_Id=@User_Id";
            helperOleDb.AddParameter("@User_Id", user_id);
            List<Discount> discounts = new List<Discount>();
            using (IDataReader reader = helperOleDb.Select(sql))
            {
                while (reader.Read())
                {
                    discounts.Add(modelCreators.DiscountCreator.CreateModel(reader));
                }
            }
            return discounts;
        }

        public bool Update(Discount model)
        {
            string sql = @"Update Discount set 
                            Description=@Description, Percentage=@Percentage, Valid_From=@Valid_From,
                            Valid_to=@Valid_To, User_Id=@User_Id";
            helperOleDb.AddParameter("@Description", model.Description);
            helperOleDb.AddParameter("@Percentage", model.Percentage);
            helperOleDb.AddParameter("@Valid_From", model.Valid_From);
            helperOleDb.AddParameter("@Valid_to", model.Valid_To);
            helperOleDb.AddParameter("@User_Id", model.User_Id);
            return helperOleDb.Insert(sql) > 0;
        }
    }
}