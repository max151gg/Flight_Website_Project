using SkyPath_Models.Models;
using System.Data;

namespace SkyPathWS.ORM.CreatorsModels
{
    public class UserCreator : IModelCreator<User>
    {
        public User CreateModel(IDataReader dataReader)
        {
            //User user = new User();
            //user.User_Id = Convert.ToString(dataReader["User_Id"]);
            //user.UserName = Convert.ToString(dataReader["UserName"]);
            //user.UserFullName = Convert.ToString(dataReader["UserFullName"]);
            //user.Email = Convert.ToString(dataReader["Email"]);
            //user.Password = Convert.ToString(dataReader["Password"]);
            //user.User_Telephone = Convert.ToString(dataReader["User_Telephone"]);
            //user.User_Adress = Convert.ToString(dataReader["User_Adress"]);
            //user.Role_Id = Convert.ToString(dataReader["Role_Id"]);
            //return user;


            User user = new User
            {
                User_Id = Convert.ToString(dataReader["User_Id"]),
                UserName = Convert.ToString(dataReader["UserName"]),
                User_FullName = Convert.ToString(dataReader["User_FullName"]),
                Email = Convert.ToString(dataReader["Email"]),
                Password = Convert.ToString(dataReader["Password"]),
                User_Telephone = Convert.ToString(dataReader["User_Telephone"]),
                User_Adress = Convert.ToString(dataReader["User_Adress"]),
                Role_Id = Convert.ToString(dataReader["Role_Id"]),
                UserSalt = Convert.ToString(dataReader["UserSalt"])
            };
            return user;
        }
    }
}
