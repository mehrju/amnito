using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran
{
   public class Params_Safiran_CreateManifest
    {
        public OrderCreateManifest order { get; set; }
    }
    public class Geo
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class OrderCreateManifest
    {
        public List<string> consignment_no { get; set; }
        public Geo geo { get; set; }
        public string status { get; set; }
        public int current_user { get; set; }
    }
}
