using ModelSkyPath.Models;
using SkyPath_Models.Models;
using System.Data;

namespace SkyPathWS.Repositories
{
    public class TicketRepository : Repository, IRepository<Ticket>
    {
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
            this.helperOleDb.AddParameter("@User_Id", model.User_Id);
            this.helperOleDb.AddParameter("@Flight_Id", model.Flight_Id);
            this.helperOleDb.AddParameter("@Purchase_Date", model.Purchase_Date);
            this.helperOleDb.AddParameter("@Status", model.Status.ToString());
            return this.helperOleDb.Insert(sql) > 0;
        }

        public bool Delete(string id)
        {
            string sql = @"Delete from Ticket where Ticket_Id=@Ticket_Id";
            this.helperOleDb.AddParameter("@Ticket_Id", id);
            return this.helperOleDb.Delete(sql) > 0;
        }

        public List<Ticket> GetALL()
        {
            string sql = "Select * from Ticket";

            List<Ticket> tickets = new List<Ticket>();
            using (IDataReader reader = this.helperOleDb.Select(sql))
            {
                while (reader.Read())
                {
                    tickets.Add(this.modelCreators.TicketCreator.CreateModel(reader));
                }
            }
            return tickets;
        }

        public Ticket GetById(string id)
        {
            string sql = "Select * from Ticket where Ticket_Id=@Ticket_Id";
            this.helperOleDb.AddParameter("@Ticket_Id", id);
            using (IDataReader reader = this.helperOleDb.Select(sql))
            {
                reader.Read();
                return this.modelCreators.TicketCreator.CreateModel(reader);
            }
        }

        public bool Update(Ticket model)
        {
            string sql = @"Update Ticket set 
                            User_Id=@User_Id, Flight_Id=@Flight_Id, Purchase_Date=@Purchase_Date,
                            Status=@Status";
            this.helperOleDb.AddParameter("@User_Id", model.User_Id);
            this.helperOleDb.AddParameter("@Flight_Id", model.Flight_Id);
            this.helperOleDb.AddParameter("@Purchase_Date", model.Purchase_Date);
            this.helperOleDb.AddParameter("@Status", model.Status.ToString());
            return this.helperOleDb.Insert(sql) > 0;
        }
    }
}