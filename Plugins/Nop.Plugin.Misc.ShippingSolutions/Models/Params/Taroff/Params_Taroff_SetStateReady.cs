using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Taroff
{
    public class Params_Taroff_SetStateReady
    {
        public string Token { get; set; }
        public int OrderId { get; set; }
    }
}
