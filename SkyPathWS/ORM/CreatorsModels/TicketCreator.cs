using SkyPath_Models.Models;
using System.Data;

namespace SkyPathWS.ORM.CreatorsModels
{
    public class TicketCreator : IModelCreator<Ticket>
    {
        public Ticket CreateModel(IDataReader dataReader)
        {
            Ticket ticket = new Ticket
            {
                Ticket_Id = Convert.ToString(dataReader["Ticket_Id"]),
                User_Id = Convert.ToString(dataReader["User_Id"]),
                Flight_Id = Convert.ToString(dataReader["Flight_Id"]),
                Purchase_Date = Convert.ToString(dataReader["Purchase_Date"]),
                Status = Convert.ToBoolean(dataReader["Status"])
            };
            return ticket;
        }
    }
}
