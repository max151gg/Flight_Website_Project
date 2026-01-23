using ModelSkyPath.Models;
using SkyPath_Models.Models;
using System.Data;

namespace SkyPathWS.ORM.Repositories
{
    public class AnnouncementRepository : Repository, IRepository<Announcement>
    {
        public AnnouncementRepository(DbHelperOleDb helperOleDb, ModelCreators modelCreators) : base(helperOleDb, modelCreators)
        {
        }
        public bool Create(Announcement model)
        {
            string sql = $@"Insert into Announcement
                            (
                            Admin_Id, Title, Content,
                            Announcement_Date, User_Id
                            )
                            values
                            (
                                @Admin_Id, @Title, @Content, @Announcement_Date, @User_Id
                            )";
            helperOleDb.AddParameter("@Admin_Id", model.Admin_Id);
            helperOleDb.AddParameter("@Title", model.Title);
            helperOleDb.AddParameter("@Content", model.Content);
            helperOleDb.AddParameter("@Announcement_Date", model.Announcement_Date);
            helperOleDb.AddParameter("@User_Id", model.User_Id);
            return helperOleDb.Insert(sql) > 0;
        }

        public bool Delete(string id)
        {
            string sql = @"Delete from Announcement where Announcement_Id=@Announcement_Id";
            helperOleDb.AddParameter("@Announcement_Id", id);
            return helperOleDb.Delete(sql) > 0;
        }

        public List<Announcement> GetALL()
        {
            string sql = "Select * from Announcement";

            List<Announcement> announcements = new List<Announcement>();
            using (IDataReader reader = helperOleDb.Select(sql))
            {
                while (reader.Read())
                {
                    announcements.Add(modelCreators.AnnouncementCreator.CreateModel(reader));
                }
            }
            return announcements;
        }

        public Announcement GetById(string id)
        {
            string sql = "Select * from Announcement where Announcement_Id=@Announcement_Id";
            helperOleDb.AddParameter("@Announcement_Id", id);
            using (IDataReader reader = helperOleDb.Select(sql))
            {
                reader.Read();
                return modelCreators.AnnouncementCreator.CreateModel(reader);
            }
        }
        public List<Announcement> GetByUserId(string user_id)
        {
            
         
            string sql = "Select * from Announcement where User_Id=@User_Id";
            helperOleDb.AddParameter("@User_Id", user_id);
            List<Announcement> announcements = new List<Announcement>();
            using (IDataReader reader = helperOleDb.Select(sql))
            {
                while (reader.Read())
                {
                    announcements.Add(modelCreators.AnnouncementCreator.CreateModel(reader));
                }
            }
            return announcements; 
        }
        public bool Update(Announcement model)
        {
            string sql = @"Update Announcement set 
                            Admin_Id=@Admin_Id, Title=@Title, Content=@Content,
                            Announcement_Date=@Announcement_Date, User_Id=@User_Id";
            helperOleDb.AddParameter("@Admin_Id", model.Admin_Id);
            helperOleDb.AddParameter("@Title", model.Title);
            helperOleDb.AddParameter("@Content", model.Content);
            helperOleDb.AddParameter("@Announcement_Date", model.Announcement_Date);
            helperOleDb.AddParameter("@User_Id", model.User_Id);
            return helperOleDb.Insert(sql) > 0;
        }
    }
}
