using Nop.Plugin.Misc.Ticket.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{
    public class vmDetailTicket
    {
        public vmDetailTicket()
        {
            Tbl_Ticket = new Ticket.Domain.Tbl_Ticket();
            vmTicket_Detail = new List<vmTicket_Detail>();
        }

        public Ticket.Domain.Tbl_Ticket Tbl_Ticket { get; set; }
        public string NameCustomer { get; set; }
        public string NameDep { get; set; }
        public string Proirity { get; set; }
        public string Status { get; set; }
        public string NameCategory { get; set; }
        public List<vmTicket_Detail> vmTicket_Detail { get; set; }
    }
    public class vmTicket_Detail
    {
        public string NameStaff { get; set; }
        public Tbl_Ticket_Detail List_Detail { get; set; }
    }
}
