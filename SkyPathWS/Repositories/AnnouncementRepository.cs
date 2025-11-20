using ModelSkyPath.Models;
using SkyPath_Models.Models;
using System.Data;

namespace SkyPathWS.Repositories
{
    public class AnnouncementRepository : Repository, IRepository<Announcement>
    {
        public AnnouncementRepository(DbHelperOleDb helperOleDb, ModelCreators modelCreators) : base(helperOleDb, modelCreators)
        {
        }
        public bool Create(Announcement model)
        {
            string sql = $@"Insert into User
                            (
                            Admin_Id, Title, Content,
                            Announcement_Date
                            )
                            values
                            (
                                @Admin_Id, @Title, @Content, @Announcement_Date
                            )";
            this.helperOleDb.AddParameter("@Admin_Id", model.Admin_Id);
            this.helperOleDb.AddParameter("@Title", model.Title);
            this.helperOleDb.AddParameter("@Content", model.Content);
            this.helperOleDb.AddParameter("@Announcement_Date", model.Announcement_Date);
            return this.helperOleDb.Insert(sql) > 0;
        }

        public bool Delete(string id)
        {
            string sql = @"Delete from Announcement where Announcement_Id=@Announcement_Id";
            this.helperOleDb.AddParameter("@Announcement_Id", id);
            return this.helperOleDb.Delete(sql) > 0;
        }

        public List<Announcement> GetALL()
        {
            string sql = "Select * from Announcement";

            List<Announcement> announcements = new List<Announcement>();
            using (IDataReader reader = this.helperOleDb.Select(sql))
            {
                while (reader.Read())
                {
                    announcements.Add(this.modelCreators.AnnouncementCreator.CreateModel(reader));
                }
            }
            return announcements;
        }

        public Announcement GetById(string id)
        {
            string sql = "Select * from Announcement where Announcement_Id=@Announcement_Id";
            this.helperOleDb.AddParameter("@Announcement_Id", id);
            using (IDataReader reader = this.helperOleDb.Select(sql))
            {
                reader.Read();
                return this.modelCreators.AnnouncementCreator.CreateModel(reader);
            }
        }

        public bool Update(Announcement model)
        {
            string sql = @"Update Announcement set 
                            Admin_Id=@Admin_Id, Title=@Title, Content=@Content,
                            Announcement_Date=@Announcement_Date";
            this.helperOleDb.AddParameter("@Admin_Id", model.Admin_Id);
            this.helperOleDb.AddParameter("@Title", model.Title);
            this.helperOleDb.AddParameter("@Content", model.Content);
            this.helperOleDb.AddParameter("@Announcement_Date", model.Announcement_Date);
            return this.helperOleDb.Insert(sql) > 0;
        }
    }
}
