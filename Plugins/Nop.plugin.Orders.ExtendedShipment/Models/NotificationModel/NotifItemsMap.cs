using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models.NotificationModel
{
    public class NotifItemsMap:EntityTypeConfiguration<NotifItemsModel>
    {
        public NotifItemsMap()
        {
            ToTable("Tb_NotifItems");
            Property(p => p.IsActive);
            Property(p => p.NotifCategoryId);
            Property(p => p.NotifTamplate);
            Property(p => p.NotifTitleId);
            Property(p => p.NotifTypeId);
        }
    }
}
