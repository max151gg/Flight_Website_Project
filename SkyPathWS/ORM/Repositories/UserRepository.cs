using SkyPath_Models;
using SkyPath_Models.Models;
using System.Data;
using System.Security.Cryptography;
using System.Text;

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
                            User_Telephone, User_Adress, User_FullName, Role_Id, UserSalt
                            )
                            values
                            (
                                @UserName, @Email, @Password, @User_Telephone, @User_Adress,
                                @User_FullName, @Role_Id, @UserSalt
                            )";
                            helperOleDb.AddParameter("@UserName", model.UserName);
                            helperOleDb.AddParameter("@Email", model.Email);
                            string salt = GenerateSalt(GetRandomNumber());
                            helperOleDb.AddParameter("@Password", GetHash(model.Password, salt));
                            helperOleDb.AddParameter("@User_Telephone", model.User_Telephone);
                            helperOleDb.AddParameter("@User_Adress", model.User_Adress);
                            helperOleDb.AddParameter("@User_FullName", model.User_FullName);
                            helperOleDb.AddParameter("@Role_Id", model.Role_Id);
                            helperOleDb.AddParameter("@UserSalt", salt);
                            return helperOleDb.Insert(sql) > 0;
        }
        private string GetHash(string password, string salt)
        {
            string combine = password + salt;
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(combine);
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
        private int GetRandomNumber()
        {
            Random random = new Random();
            return random.Next(8, 16);
        }
        private string GenerateSalt(int length)
        {
            byte[] bytes = new byte[length];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
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

        public string Login(string userName, string password) 
        {
            string sql = @"Select UserSalt, User_Id, Password from [User]
                   where [UserName]=@UserName";
            helperOleDb.AddParameter("@UserName", userName);
            using (IDataReader user = helperOleDb.Select(sql))
            {
                if (user.Read() == true)
                {
                    string salt = user["UserSalt"].ToString();
                    string hash = user["Password"].ToString();
                    string passwordHash = GetHash(password, salt);
                    if (hash == passwordHash)
                    {
                        return user["User_Id"].ToString();
                    }
                    return null;
                }
                return null;
            }
        }
    }
}
