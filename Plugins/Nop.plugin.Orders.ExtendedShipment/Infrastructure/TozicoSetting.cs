using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment
{
    public class TozicoSetting : ISettings
    {
        public string BaseAddress { get; set; }
        public string AccessToken { get; set; }
    }
}
