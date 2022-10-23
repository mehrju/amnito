using Nop.Plugin.Misc.PrintedReports_Requirements.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public enum PostArea
    {
        Internal,
        Foreign,
        Heavy
    }
    public class NewCheckoutViewModel
    {
        public bool IsAgent { get; set; }
        public bool IsCod { get; set; }
        public bool IsInCodRole { get; set; }
        public string ApStartupConfig { get; set; }
        public PostArea postArea { get; set; }
        public bool IsSafeBuy { get; set; }
        public bool HideForItSaz { get; set; }
        public vmServiceProvider_Index vmServiceProvider_Index { get; set; }
        //public List<ItemServiceProvider> List_Provider { get; set; }

    }
}
