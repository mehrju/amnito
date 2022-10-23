using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.YarBox
{
    public class Result_YarBox_Tracking
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public Detail_Tracking Detail_Tracking { get; set; }
    }
    public class Detail_Tracking
    {
        public string status { get; set; }
    }
}
