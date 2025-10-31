using SkyPath_Models.Models;
using System.Data;

namespace SkyPathWS.CreatorsModels
{
    public class DiscountCreator : IModelCreator<Discount>
    {
        public Discount CreateModel(IDataReader dataReader)
        {
            Discount discount = new Discount
            {
                Discount_Id = Convert.ToString(dataReader["Discount_Id"]),
                Description = Convert.ToString(dataReader["Description"]),
                Percentage = Convert.ToInt16(dataReader["Percentage"]),
                Valid_From = Convert.ToString(dataReader["Valid_From"]),
                Valid_To = Convert.ToString(dataReader["Valid_To"])
            };
            return discount;
        }
    }
}
