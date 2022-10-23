using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models.Optime.ToolApiCall
{
    public class SpFileContent
    {
        public int Id { get; set; }

        public string Address { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string District { get; set; }

        public string MissionType { get; set; }

        public int Vehicle { get; set; }

        public double Weight { get; set; }

        public double Volume { get; set; }

        public string CustomerName { get; set; }

        public string CustomerPhoneNumber { get; set; }
        public string CustomerTimeWindow { get; set; }
    }
}
