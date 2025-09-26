using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyPath_Models
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
            get { return this.announcement_Id; }
            set { this.announcement_Id = value;}
        }
        public string Admin_Id
        {
            get { return this.admin_Id; }
            set { this.admin_Id = value; }
        }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(20,MinimumLength =2,ErrorMessage ="Title must be no longer than 20 characters")]
        public string Title
        {
            get { return this.title; }
            set { this.title = value; }
        }
        public string Content
        {
            get { return this.content; }
            set { this.content = value; }
        }
        public string Announcement_Date
        {
            get { return this.announcement_Date; }
            set { this.announcement_Date = value; }
        }
    }
}
