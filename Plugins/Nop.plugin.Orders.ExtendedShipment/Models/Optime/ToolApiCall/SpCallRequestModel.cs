using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models.Optime.ToolApiCall
{
    public class SpCallRequestModel
    {
        public int VehicleTypeEnum { get; set; }
        public int customerId { get; set; }
        public string carName { get; set; }
        public string description { get; set; }

        public string fileExtension { get; set; } 

        public string owner { get; set; }

        public string planName { get; set; }

        public string toolName { get; set; }

        public List<string> shiftsCode { get => new List<string> { ShiftsCodeStr }; }

        public string ShiftsCodeStr { get; set; }
        public long CapacityVolume { get; set; }
        public long CapacityWeight { get; set; }

        public bool driverComeBack { get; set; }
    }
}
