using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Chapar
{
    public class Params_Chapar_HistoryReport
    {
        public User user { get; set; }
        public Date date { get; set; }
        public int maximum_records { get; set; }
    }
   

    public class Date
    {
        public string from { get; set; }
        public string to { get; set; }
    }
}
