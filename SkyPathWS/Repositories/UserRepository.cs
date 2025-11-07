using SkyPath_Models;
using SkyPath_Models.Models;
using System.Data;

namespace SkyPathWS.Repositories
{
    public class UserRepository : Repository, IRepository<User>
    { 
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
            //                    '{model.User_Adress}', '{model.UserFullName}', {model.Role_Id}
            //                )";

            string sql = $@"Insert into User
                            (
                            UserName, Email, Password,
                            User_Telephone, User_Adress, User_FullName, Role_Id
                            )
                            values
                            (
                                @UserName, @Email, @Password, @User_Telephone, @User_Adress,
                                @ User_FullName, @Role_Id
                            )";
                            this.helperOleDb.AddParameter("@UserName", model.UserName);
                            this.helperOleDb.AddParameter("@Email", model.Email);
                            this.helperOleDb.AddParameter("@Password", model.Password);
                            this.helperOleDb.AddParameter("@User_Telephone", model.User_Telephone);
                            this.helperOleDb.AddParameter("@User_Adress", model.User_Adress);
                            this.helperOleDb.AddParameter("@User_FullName", model.UserFullName);
                            this.helperOleDb.AddParameter("@Role_Id", model.Role_Id);
                            return this.helperOleDb.Insert(sql) > 0;
        }

        public bool Delete(string id)
        {
            string sql = @"Delete from User where User_Id=@User_Id";
            this.helperOleDb.AddParameter("@User_Id", id);
            return this.helperOleDb.Delete(sql) > 0;
        }

        public List<User> GetALL()
        {
            string sql = "Select * from User";

            List<User> users = new List<User>();
            using(IDataReader reader = this.helperOleDb.Select(sql))
            {
                while (reader.Read())
                {
                    users.Add(this.modelCreators.UserCreator.CreateModel(reader));
                }
            }
            return users;
        }

        public User GetById(string id)
        {
            string sql = "Select * from User where User_Id=@User_Id";
            this.helperOleDb.AddParameter("@User_Id", id);
            using(IDataReader reader = this.helperOleDb.Select(sql))
            {
                reader.Read();
                return this.modelCreators.UserCreator.CreateModel(reader);
            }
        }

        public bool Update(User model)
        {
            string sql = @"Update User set 
                            UserName=@UserName, Email=@Email, Password=@Password,
                            User_Telephone=@User_Telephone, User_Adress=@User_Adress,
                            User_FullName=@User_FullName, Role_Id=@Role_Id";
            this.helperOleDb.AddParameter("@UserName", model.UserName);
            this.helperOleDb.AddParameter("@Email", model.Email);
            this.helperOleDb.AddParameter("@Password", model.Password);
            this.helperOleDb.AddParameter("@User_Telephone", model.User_Telephone);
            this.helperOleDb.AddParameter("@User_Adress", model.User_Adress);
            this.helperOleDb.AddParameter("@User_FullName", model.UserFullName);
            this.helperOleDb.AddParameter("@Role_Id", model.Role_Id);
            return this.helperOleDb.Update(sql) > 0;
        }

        public string Login(string username, string password) 
        {
            string sql = @"select User_Id from Users
                           where UserName=@UserName AND Password=@Password";
            this.helperOleDb.AddParameter("@UserName", username);
            this.helperOleDb.AddParameter("@Password", password);
            using (IDataReader user = this.helperOleDb.Select(sql))
            {
                if (user.Read() == true)
                    return user["User_Id"].ToString();
                return null;
            }
        }
    }
}
