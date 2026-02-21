using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyPath_Models;

namespace ModelSkyPath.Models
{
    public class Announcement : Model
    {
        string announcement_Id;
        string admin_Id;
        string title;
        string content;
        string announcement_Date;
        public string user_Id;



        public string Announcement_Id
        {
            get { return announcement_Id; }
            set { announcement_Id = value;}
        }
        public string Admin_Id
        {
            get { return admin_Id; }
            set { admin_Id = value; }
        }
        [FirstLetterCapital(ErrorMessage = "Title must start with a capital letter")]
        [Required(ErrorMessage = "Title is required")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Title must be no longer than 20 characters and no less than 2")]
        public string Title
        {
            get { return title; }
            set { title = value; ValidateProperty(value, "Title"); }
        }
        [StringLength(500, ErrorMessage = "Content must be no longer than 500 characters")]
        public string Content
        {
            get { return content; }
            set { content = value; }
        }
        [Required(ErrorMessage = "Date is required")]
        public string Announcement_Date
        {
            get { return announcement_Date; }
            set { announcement_Date = value; ValidateProperty(value, "Announcement_Id"); }
        }
        public string User_Id
        {
            get { return user_Id; }
            set { user_Id = value; }
        }
    }
}
