using Nop.Plugin.Misc.Ticket.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{
    public class vmDetailDamages
    {
        public vmDetailDamages()
        {
            Tbl_Damages = new Ticket.Domain.Tbl_Damages();
            vmDamages_Detail = new List<vmDamages_Detail>();
        }
        public Ticket.Domain.Tbl_Damages Tbl_Damages { get; set; }
        public List<vmDamages_Detail> vmDamages_Detail { get; set; }
        public string Status { get; set; }
        public string TypeGoods { get; set; }
        public string NameCustomer { get; set; }
    }
    public class vmDamages_Detail
    {
        public string NameStaff { get; set; }
        public Tbl_Damages_Detail List_Detail { get; set; }
    }
}
