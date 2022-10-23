using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.SkyBlue
{
    public class Result_SkyBlue_Tracking
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public List<Detail_sk_Tracking> Detail_sk_Tracking { get; set; }
    }
    public class Detail_sk_Tracking
    {
        public string OrderNumber { get; set; }
        public string Location { get; set; }
        public string DateTime { get; set; }
        public string Status { get; set; }
        public string Details { get; set; }
    }
}
