using Nop.Core;
using Nop.plugin.Orders.ExtendedShipment.Models.Tozico.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Domain
{
    public class Tbl_CustomerVehicle : BaseEntity
    {
        public int CustomerId { get; set; }

        public int BikerCustomerId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }

        public VehicleTypeEnum VehicleTypeEnum { get; set; }

        public string Description { get; set; }

        public long CapacityVolume { get; set; }

        public long CapacityWeight { get; set; }

        public long CapacityCount { get; set; }

        public long CapacityPercent { get; set; }
    }
}
