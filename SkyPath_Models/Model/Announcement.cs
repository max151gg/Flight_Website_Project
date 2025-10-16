using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyPath_Models.Model
{
    public class Announcement
    {
        string announcement_Id;
        string admin_Id;
        string title;
        string content;
        string announcement_Date;


        public string Announcement_Id
        {
            get { return announcement_Id; }
            set { announcement_Id = value; }
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
            set { title = value; }
        }
        public string Content
        {
            get { return content; }
            set { content = value; }
        }
        [Required(ErrorMessage = "Date is required")]
        public string Announcement_Date
        {
            get { return announcement_Date; }
            set { announcement_Date = value; }
        }
    }
}
