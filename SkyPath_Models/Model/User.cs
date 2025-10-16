using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyPath_Models.Model
{
    public class User
    {
        string user_Id;
        string userName;
        string email;
        string password;
        string user_Telephone;
        string user_Adress;
        string user_FullName;
        string role_Id;

        public string User_Id
        {
            get { return user_Id; }
            set { user_Id = value; }
        }
        [Required(ErrorMessage = "UserName is required")]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Must set proper email adress")]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        [Required(ErrorMessage = "Password is required")]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Must be proper phone number")]
        public string User_Telephone
        {
            get { return user_Telephone; }
            set { user_Telephone = value; }
        }
        [Required(ErrorMessage = "Adress is required")]
        public string User_Adress
        {
            get { return user_Adress; }
            set { user_Adress = value; }
        }
        [Required(ErrorMessage = "Full name is required")]
        [FirstLetterCapital(ErrorMessage = "Name must start with a capital letter")]
        public string UserFullName
        {
            get { return user_FullName; }
            set { user_FullName = value; }
        }
        public string Role_Id
        {
            get { return role_Id; }
            set { role_Id = value; }
        }
    }
}
