using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{
    public class SupportModel
    {
        public int SupportId { get; set; }

        public string Issue { get; set; }
        public string NameDep { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
        public string OrderId { get; set; }
        public string TrackingCode { get; set; }
    }
}
