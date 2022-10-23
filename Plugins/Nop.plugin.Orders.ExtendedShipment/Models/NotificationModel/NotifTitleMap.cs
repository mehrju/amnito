using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models.NotificationModel
{
    public class NotifTitleMap:EntityTypeConfiguration<NotifTitleModel>
    {
        public NotifTitleMap()
        {
            ToTable("Tb_NotifTitle");
            Property(p => p.IsActive);
            Property(p => p.NotifTitleName);
        }
    }
}
