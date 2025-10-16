using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyPath_Models.Model
{
    public class Ticket
    {
        string ticket_Id;
        string user_Id;
        string flight_Id;
        string purchase_Date;
        bool status;

        public string Ticket_Id
        {
            get { return ticket_Id; }
            set { ticket_Id = value; }
        }
        public string User_Id
        {
            get { return user_Id; }
            set { user_Id = value; }
        }
        public string Flight_Id
        {
            get { return flight_Id; }
            set { flight_Id = value; }
        }
        [Required(ErrorMessage = "Purchase date is required")]
        public string Purchase_Date
        {
            get { return purchase_Date; }
            set { purchase_Date = value; }
        }
        [Required(ErrorMessage = "Ticket status is required")]
        public bool Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}
