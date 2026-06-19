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
        // Adds a new user to the database. The password is never stored as plain text:
        // we make a random salt and save the SHA256 hash of (password + salt).
        public bool Create(User model)
        {
            string sql = $@"Insert into [User]
                            (
                            UserName, Email, [Password],
                            User_Telephone, User_Adress, User_FullName, Role_Id, UserSalt, User_Image, User_Ban
                            )
                            values
                            (
                                @UserName, @Email, @Password, @User_Telephone, @User_Adress,
                                @User_FullName, @Role_Id, @UserSalt, @User_Image, @User_Ban
                            )";
                            helperOleDb.AddParameter("@UserName", model.UserName);
                            helperOleDb.AddParameter("@Email", model.Email);
                            string salt = GenerateSalt(GetRandomNumber());
                            helperOleDb.AddParameter("@Password", GetHash(model.Password, salt));
                            helperOleDb.AddParameter("@User_Telephone", model.User_Telephone);
                            helperOleDb.AddParameter("@User_Adress", model.User_Adress);
                            helperOleDb.AddParameter("@User_FullName", model.User_FullName);
                            helperOleDb.AddParameter("@Role_Id", Convert.ToInt32(model.Role_Id));
                            helperOleDb.AddParameter("@UserSalt", salt);
                            helperOleDb.AddParameter("@User_Image", model.User_Image);
                            helperOleDb.AddParameter("@User_Ban", model.User_Ban);
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
        // Returns true if a user with this email already exists (email must be unique).
        public bool EmailExists(string email)
        {
            string sql = "Select User_Id from [User] where Email=@Email";
            helperOleDb.AddParameter("@Email", email);
            using (IDataReader reader = helperOleDb.Select(sql))
            {
                return reader.Read();
            }
        }

        // Returns true if a user with this username already exists (username must be unique).
        public bool UserNameExists(string userName)
        {
            string sql = "Select User_Id from [User] where UserName=@UserName";
            helperOleDb.AddParameter("@UserName", userName);
            using (IDataReader reader = helperOleDb.Select(sql))
            {
                return reader.Read();
            }
        }

        public bool BanUser(string user_Id, bool user_Ban)
        {
            string sql = @"Update [User] set User_Ban=@User_Ban where User_Id=@User_Id";
            helperOleDb.AddParameter("@User_Ban", user_Ban);
            helperOleDb.AddParameter("@User_Id", user_Id);
            return helperOleDb.Update(sql) > 0;
        }
        public bool Delete(string id)
        {
            string sql = @"Delete from [User] where User_Id=@User_Id";
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
            string sql = "Select * from [User] where User_Id=@User_Id";
            helperOleDb.AddParameter("@User_Id", id);
            using(IDataReader reader = helperOleDb.Select(sql))
            {
                if (!reader.Read()) return null;
                return modelCreators.UserCreator.CreateModel(reader);
            }
        }

        public bool UpdatePassword(string userId, string newPassword)
        {
            string salt = GenerateSalt(GetRandomNumber());
            string hash = GetHash(newPassword, salt);

            string sql = @"UPDATE [User]
                   SET [Password]=@Password, UserSalt=@UserSalt
                   WHERE User_Id=@User_Id";

            helperOleDb.AddParameter("@Password", hash);
            helperOleDb.AddParameter("@UserSalt", salt);
            helperOleDb.AddParameter("@User_Id", userId);

            return helperOleDb.Update(sql) > 0;
        }

        public bool UpdateProfile(User model)
        {
            string sql = @"UPDATE [User]
                   SET UserName=@UserName,
                       User_Telephone=@User_Telephone,
                       User_Adress=@User_Adress,
                       User_FullName=@User_FullName
                   WHERE User_Id=@User_Id";

            helperOleDb.AddParameter("@UserName", model.UserName);
            helperOleDb.AddParameter("@User_Telephone", model.User_Telephone);
            helperOleDb.AddParameter("@User_Adress", model.User_Adress);
            helperOleDb.AddParameter("@User_FullName", model.User_FullName);
            helperOleDb.AddParameter("@User_Id", model.User_Id);

            return helperOleDb.Update(sql) > 0;
        }

        public bool Update(User model)
        {
            string sql = @"Update User set 
                            UserName=@UserName, Email=@Email, Password=@Password,
                            User_Telephone=@User_Telephone, User_Adress=@User_Adress,
                            User_FullName=@User_FullName, Role_Id=@Role_Id, UserSalt=@UserSalt, User_Image=@User_Image, User_Ban=@User_Ban";
            helperOleDb.AddParameter("@UserName", model.UserName);
            helperOleDb.AddParameter("@Email", model.Email);
            string salt = GenerateSalt(GetRandomNumber());
            helperOleDb.AddParameter("@Password", GetHash(model.Password, salt));
            helperOleDb.AddParameter("@User_Telephone", model.User_Telephone);
            helperOleDb.AddParameter("@User_Adress", model.User_Adress);
            helperOleDb.AddParameter("@User_FullName", model.User_FullName);
            helperOleDb.AddParameter("@Role_Id", model.Role_Id);
            helperOleDb.AddParameter("@UserSalt", salt);
            helperOleDb.AddParameter("@User_Image", model.User_Image);
            helperOleDb.AddParameter("@User_Ban", model.User_Ban);
            return helperOleDb.Update(sql) > 0;
        }


        
        // Checks a login. We hash the typed password with the user's saved salt and
        // compare it to the saved hash. Returns the User_Id on success, or null if wrong.
        public string Login(string Email, string password)
        {
            string sql = @"Select UserSalt, User_Id, Password from [User]
                   where [Email]=@Email";
            helperOleDb.AddParameter("@Email", Email);
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

        public bool UpdateUserRole(string userId, string roleId)
        {
            string sql = @"UPDATE [User] SET Role_Id=@Role_Id WHERE User_Id=@User_Id";
            helperOleDb.AddParameter("@Role_Id", roleId);
            helperOleDb.AddParameter("@User_Id", userId);
            return helperOleDb.Update(sql) > 0;
        }

        public bool UpdateUserImage(string userId, string imagePath)
        {
            string sql = @"UPDATE [User] 
                   SET User_Image=@User_Image 
                   WHERE User_Id=@User_Id";

            helperOleDb.AddParameter("@User_Image", imagePath);
            helperOleDb.AddParameter("@User_Id", userId);

            return helperOleDb.Update(sql) > 0;
        }


    }
}
