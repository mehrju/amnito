using Nop.Plugin.Orders.MultiShipping.Models.Tbl_Extra_Status_Field_Shipment;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models.Extra_Status_Field_Shipment
{
    public class TblExtraFiledShipmentMap : EntityTypeConfiguration<TblExtraFiledShipment>
    {
        public TblExtraFiledShipmentMap()
        {
            ToTable("Tbl_ExtraFiledShipment");
            HasKey(m => m.Id);
            Property(m => m.ShippingId);
            Property(m => m.DateInsert);
            Property(m => m.IdUserInsert);
            Property(m => m.OrderNoteId);
            Property(m => m.Type);
            Property(m => m.value);

        }
    }
}
