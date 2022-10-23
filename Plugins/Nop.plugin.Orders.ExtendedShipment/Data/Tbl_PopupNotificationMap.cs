using Nop.plugin.Orders.ExtendedShipment.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Data
{
    public class Tbl_PopupNotificationMap : EntityTypeConfiguration<Tbl_PopupNotification>
    {
        public Tbl_PopupNotificationMap()
        {
            ToTable("Tbl_PopupNotification");
            HasKey(m => m.Id);
        }
    }
}
