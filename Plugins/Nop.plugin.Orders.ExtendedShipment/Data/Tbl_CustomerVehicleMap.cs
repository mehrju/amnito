using Nop.plugin.Orders.ExtendedShipment.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Data
{
    public class Tbl_CustomerVehicleMap : EntityTypeConfiguration<Tbl_CustomerVehicle>
    {
        public Tbl_CustomerVehicleMap()
        {
            HasKey(p => p.Id);

        }
    }
}
