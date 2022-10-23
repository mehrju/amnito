using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{
    public class FullCODRequestResultModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int CODRequestId { get; set; }
    }
}
