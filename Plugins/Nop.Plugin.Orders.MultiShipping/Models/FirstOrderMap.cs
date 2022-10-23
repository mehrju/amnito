using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    class FirstOrderMap: EntityTypeConfiguration<FirstOrderModel>
    {
        public FirstOrderMap()
        {
            ToTable("FirstOrder");
            HasKey(p => p.Id);
            Property(p => p.OrderId);
        }
    }
}
