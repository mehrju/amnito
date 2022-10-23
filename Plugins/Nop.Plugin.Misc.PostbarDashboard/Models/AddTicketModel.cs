using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{
    public class AddTicketModel
    {
        public int OrderCode { get; set; }
        public string TrackingCode { get; set; }
        public int ProrityId { get; set; }
        public int DepartmentId { get; set; }
        public string Issue { get; set; }
        public string Description { get; set; }
        public int IdCategoryTicket { get; set; }

        public int TypeTicket { get; set; }
    }
}
