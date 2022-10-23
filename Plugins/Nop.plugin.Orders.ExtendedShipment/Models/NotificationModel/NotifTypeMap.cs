using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models.NotificationModel
{
    public class NotifTypeMap: EntityTypeConfiguration<NotifTypeModel>
    {
        public NotifTypeMap()
        {
            ToTable("Tb_NotifType");
            Property(p => p.NotifTypeName);
            Property(p => p.IsFree);
        }
    }
}
