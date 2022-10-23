using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Domain
{
    public class Tbl_CODRequestLog : BaseEntity
    {
        public int CODRequestId { get; set; }
        public bool RobotSucceed { get; set; }
        public bool EmailSucceed { get; set; }
        public string Message { get; set; } = "";
    }
}
