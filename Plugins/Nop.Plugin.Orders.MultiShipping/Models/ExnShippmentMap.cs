using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class ExnShippmentMap : EntityTypeConfiguration<ExnShippmentModel>
    {
        public ExnShippmentMap()
        {
            ToTable("XtnShippment");
            HasKey(p => p.Id);
            Property(p => p.ShipmentId);
            Property(p => p.ShippmentAddressId);
            Property(p => p.ShippmentMethod);
            Property(p => p.ShipmentTempId);

        }
    }
       
}
