using ModelSkyPath.Models;
using SkyPath_Models.Models;
using System.Data;

namespace SkyPathWS.CreatorsModels
{
    public class AnnouncementCreator : IModelCreator<Announcement>
    {
        public Announcement CreateModel(IDataReader dataReader)
        {
            Announcement announcement = new Announcement
            {
                Announcement_Id = Convert.ToString(dataReader["Announcement_Id"]),
                Admin_Id = Convert.ToString(dataReader["Admin_Id"]),
                Title = Convert.ToString(dataReader["Title"]),
                Content = Convert.ToString(dataReader["Content"]),
                Announcement_Date = Convert.ToString(dataReader["Announcement_Date"])
            };
            return announcement;
        }
    }
}
