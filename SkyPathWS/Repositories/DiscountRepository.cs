using ModelSkyPath.Models;
using SkyPath_Models.Models;
using System.Data;

namespace SkyPathWS.Repositories
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
                            Valid_To
                            )
                            values
                            (
                                @Description, @Percentage, @Valid_From, @Valid_To
                            )";
            this.helperOleDb.AddParameter("@Description", model.Description);
            this.helperOleDb.AddParameter("@Percentage", model.Percentage);
            this.helperOleDb.AddParameter("@Valid_From", model.Valid_From);
            this.helperOleDb.AddParameter("@Valid_To", model.Valid_To);
            return this.helperOleDb.Insert(sql) > 0;
        }

        public bool Delete(string id)
        {
            string sql = @"Delete from Discount where Discount_Id=@Discount_Id";
            this.helperOleDb.AddParameter("@Discount_Id", id);
            return this.helperOleDb.Delete(sql) > 0;
        }

        public List<Discount> GetALL()
        {
            string sql = "Select * from Discount";

            List<Discount> discounts = new List<Discount>();
            using (IDataReader reader = this.helperOleDb.Select(sql))
            {
                while (reader.Read())
                {
                    discounts.Add(this.modelCreators.DiscountCreator.CreateModel(reader));
                }
            }
            return discounts;
        }

        public Discount GetById(string id)
        {
            string sql = "Select * from Discount where Discount_Id=@Discount_Id";
            this.helperOleDb.AddParameter("@Discount_Id", id);
            using (IDataReader reader = this.helperOleDb.Select(sql))
            {
                reader.Read();
                return this.modelCreators.DiscountCreator.CreateModel(reader);
            }
        }

        public bool Update(Discount model)
        {
            string sql = @"Update Discount set 
                            Description=@Description, Percentage=@Percentage, Valid_From=@Valid_From,
                            Valid_to=@Valid_To";
            this.helperOleDb.AddParameter("@Description", model.Description);
            this.helperOleDb.AddParameter("@Percentage", model.Percentage);
            this.helperOleDb.AddParameter("@Valid_From", model.Valid_From);
            this.helperOleDb.AddParameter("@Valid_to", model.Valid_To);
            return this.helperOleDb.Insert(sql) > 0;
        }
    }
}