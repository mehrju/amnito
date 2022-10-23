using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Chapar
{
    public class Result_Chapar_HistoryReport
    {
        public bool result { get; set; }
        public string message { get; set; }
        public Objects objects { get; set; }
    }
    public class Objects
    {
        public object cons { get; set; }
    }
}
