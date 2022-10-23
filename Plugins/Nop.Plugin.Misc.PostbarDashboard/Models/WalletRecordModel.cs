using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{
    public class WalletRecord
    {
        public int Id { get; set; }
        public int Points { get; set; }
        public int PointsBalance { get; set; }
        public string Message { get; set; }
        public string CreateDate { get; set; }
    }

}
