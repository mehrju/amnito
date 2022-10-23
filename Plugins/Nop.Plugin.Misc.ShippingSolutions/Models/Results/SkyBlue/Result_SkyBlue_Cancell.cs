using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.SkyBlue
{
    public class Result_SkyBlue_Cancell
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public Detail_sk_Camcell Detail_sk_Camcell { get; set; }
    }
    public class Detail_sk_Camcell
    {
        public string Result { get; set; }
        public string OrderNumber { get; set; }
        public string Message { get; set; }
    }
}
