using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Models
{
    public class VmDetailRequestCOD
    {
        public VmDetailRequestCOD()
        {
            Tbl_RequestCODCustomer = new Ticket.Domain.Tbl_RequestCODCustomer();
        }
        public Ticket.Domain.Tbl_RequestCODCustomer Tbl_RequestCODCustomer { get; set; }
        public string Status { get; set; }
        public string NameCustomer { get; set; }
        public string NameStaffAnswer { get; set; }
        [Description("پایان اعتبار")]
        public DateTime EndDate { get; set; }
    }
}
