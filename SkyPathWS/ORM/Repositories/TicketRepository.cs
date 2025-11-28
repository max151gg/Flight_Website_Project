using ModelSkyPath.Models;
using SkyPath_Models.Models;
using System.Data;

namespace SkyPathWS.ORM.Repositories
{
    public class TicketRepository : Repository, IRepository<Ticket>
    {
        public TicketRepository(DbHelperOleDb helperOleDb, ModelCreators modelCreators) : base(helperOleDb, modelCreators)
        {
        }
        public bool Create(Ticket model)
        {
            string sql = $@"Insert into Ticket
                            (
                            User_Id, Flight_Id, Purchase_Date,
                            Status
                            )
                            values
                            (
                                @User_Id, @Flight_Id, @Purchase_Date, @Status
                            )";
            helperOleDb.AddParameter("@User_Id", model.User_Id);
            helperOleDb.AddParameter("@Flight_Id", model.Flight_Id);
            helperOleDb.AddParameter("@Purchase_Date", model.Purchase_Date);
            helperOleDb.AddParameter("@Status", model.Status);
            return helperOleDb.Insert(sql) > 0;
        }

        public bool Delete(string id)
        {
            string sql = @"Delete from Ticket where Ticket_Id=@Ticket_Id";
            helperOleDb.AddParameter("@Ticket_Id", id);
            return helperOleDb.Delete(sql) > 0;
        }

        public List<Ticket> GetALL()
        {
            string sql = "Select * from Ticket";

            List<Ticket> tickets = new List<Ticket>();
            using (IDataReader reader = helperOleDb.Select(sql))
            {
                while (reader.Read())
                {
                    tickets.Add(modelCreators.TicketCreator.CreateModel(reader));
                }
            }
            return tickets;
        }

        public Ticket GetById(string id)
        {
            string sql = "Select * from Ticket where Ticket_Id=@Ticket_Id";
            helperOleDb.AddParameter("@Ticket_Id", id);
            using (IDataReader reader = helperOleDb.Select(sql))
            {
                reader.Read();
                return modelCreators.TicketCreator.CreateModel(reader);
            }
        }
        public List<Ticket> GetByUserId(string user_id)
        {
            string sql = "Select * from Ticket where User_Id=@User_Id";
            helperOleDb.AddParameter("@User_Id", user_id);
            List<Ticket> tickets = new List<Ticket>();
            using (IDataReader reader = helperOleDb.Select(sql))
            {
                while (reader.Read())
                {
                    tickets.Add(modelCreators.TicketCreator.CreateModel(reader));
                }
            }
            return tickets;
        }

        public bool Update(Ticket model)
        {
            string sql = @"Update Ticket set 
                            User_Id=@User_Id, Flight_Id=@Flight_Id, Purchase_Date=@Purchase_Date,
                            Status=@Status";
            helperOleDb.AddParameter("@User_Id", model.User_Id);
            helperOleDb.AddParameter("@Flight_Id", model.Flight_Id);
            helperOleDb.AddParameter("@Purchase_Date", model.Purchase_Date);
            helperOleDb.AddParameter("@Status", model.Status);
            return helperOleDb.Insert(sql) > 0;
        }
    }
}