using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class Sep_StartupModel
    {
        public string authToken { get; set; }
        public string platform { get; set; }
        public string appVersion { get; set; }
        public string lang { get; set; }
        public object additionalData { get; set; }
        public string config { get; set; }
    }
}
