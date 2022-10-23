using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class OrderStatusTrackingMap: EntityTypeConfiguration<OrderStatusTrackingModel>
    {
        public OrderStatusTrackingMap()
        {
            ToTable("OrderStatusTracking");
            HasKey(p => p.Id);
            Property(p => p.OrderstatusId);
            Property(p => p.chageDate);
            Property(p => p.orderId);
            Property(p => p.CustomerId);
            Property(p => p.ClientIp);
        }
    }
}
