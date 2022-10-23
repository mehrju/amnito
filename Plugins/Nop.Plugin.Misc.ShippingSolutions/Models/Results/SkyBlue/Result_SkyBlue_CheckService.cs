using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.SkyBlue
{
    public class Result_SkyBlue_CheckService
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public Detail_CheckService Detail_CheckService { get; set; }

    }
    public class Detail_CheckService
    {
        public string CountryCode { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
    }
}
