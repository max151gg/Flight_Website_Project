using SkyPath_Models;
using SkyPath_Models.Models;
using System.Data;

namespace SkyPathWS.ORM.Repositories
{
    public class UserRepository : Repository, IRepository<User>
    {
        public UserRepository(DbHelperOleDb helperOleDb, ModelCreators modelCreators) : base(helperOleDb, modelCreators)
        {
        }
        public bool Create(User model)
        {
            //string sql = $@"Insert into User
            //                (
            //                UserName, Email, Password,
            //                User_Telephone, User_Adress, User_FullName, Role_Id
            //                )
            //                values
            //                (
            //                    '{model.UserName}', '{model.Email}', '{model.Password}', '{model.User_Telephone}',
            //                    '{model.User_Adress}', '{model.User_FullName}', {model.Role_Id}
            //                )";

            string sql = $@"Insert into [User]
                            (
                            UserName, Email, [Password],
                            User_Telephone, User_Adress, User_FullName, Role_Id
                            )
                            values
                            (
                                @UserName, @Email, @Password, @User_Telephone, @User_Adress,
                                @User_FullName, @Role_Id
                            )";
                            helperOleDb.AddParameter("@UserName", model.UserName);
                            helperOleDb.AddParameter("@Email", model.Email);
                            helperOleDb.AddParameter("@Password", model.Password);
                            helperOleDb.AddParameter("@User_Telephone", model.User_Telephone);
                            helperOleDb.AddParameter("@User_Adress", model.User_Adress);
                            helperOleDb.AddParameter("@User_FullName", model.User_FullName);
                            helperOleDb.AddParameter("@Role_Id", model.Role_Id);
                            return helperOleDb.Insert(sql) > 0;
        }

        public bool Delete(string id)
        {
            string sql = @"Delete from User where User_Id=@User_Id";
            helperOleDb.AddParameter("@User_Id", id);
            return helperOleDb.Delete(sql) > 0;
        }

        public List<User> GetALL()
        {
            string sql = "Select * from [User]";

            List<User> users = new List<User>();
            using(IDataReader reader = helperOleDb.Select(sql))
            {
                while (reader.Read())
                {
                    users.Add(modelCreators.UserCreator.CreateModel(reader));
                }
            }
            return users;
        }

        public User GetById(string id)
        {
            string sql = "Select * from User where User_Id=@User_Id";
            helperOleDb.AddParameter("@User_Id", id);
            using(IDataReader reader = helperOleDb.Select(sql))
            {
                reader.Read();
                return modelCreators.UserCreator.CreateModel(reader);
            }
        }

        public bool Update(User model)
        {
            string sql = @"Update User set 
                            UserName=@UserName, Email=@Email, Password=@Password,
                            User_Telephone=@User_Telephone, User_Adress=@User_Adress,
                            User_FullName=@User_FullName, Role_Id=@Role_Id";
            helperOleDb.AddParameter("@UserName", model.UserName);
            helperOleDb.AddParameter("@Email", model.Email);
            helperOleDb.AddParameter("@Password", model.Password);
            helperOleDb.AddParameter("@User_Telephone", model.User_Telephone);
            helperOleDb.AddParameter("@User_Adress", model.User_Adress);
            helperOleDb.AddParameter("@User_FullName", model.User_FullName);
            helperOleDb.AddParameter("@Role_Id", model.Role_Id);
            return helperOleDb.Update(sql) > 0;
        }

        public string Login(string username, string password) 
        {
            string sql = @"select User_Id from Users
                           where UserName=@UserName AND Password=@Password";
            helperOleDb.AddParameter("@UserName", username);
            helperOleDb.AddParameter("@Password", password);
            using (IDataReader user = helperOleDb.Select(sql))
            {
                if (user.Read() == true)
                    return user["User_Id"].ToString();
                return null;
            }
        }
    }
}
