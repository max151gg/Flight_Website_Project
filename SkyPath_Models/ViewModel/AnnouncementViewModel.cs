using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelSkyPath.Models;
using SkyPath_Models.Models;

namespace SkyPath_Models.ViewModel
{
    public class AnnouncementViewModel
    {
        public List<Announcement> announcements { get; set; }
        public List <Announcement> publicAnnouncements { get; set; }
    }
}
