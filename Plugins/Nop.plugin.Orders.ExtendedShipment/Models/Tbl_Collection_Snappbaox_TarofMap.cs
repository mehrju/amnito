using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class Tbl_Collection_Snappbaox_TarofMap : EntityTypeConfiguration<Tbl_Collection_Snappbaox_Tarof>
    {
        public Tbl_Collection_Snappbaox_TarofMap()
        {
            ToTable("Tbl_Collection_Snappbaox_Tarof");

            HasKey(m => m.Id);
            Property(m => m.Id_Request);
            Property(m => m.TypeRequest);
            Property(m => m.ShipmentId);
            Property(m => m.DateInsert);
            Property(m => m.UserIdInsert);
            Property(m => m.Status);
            Property(m => m.Date_Statuse);
            Property(m => m.Mablagh_Induery);
            Property(m => m.orderId).IsOptional();
            Property(m => m.Snapp_bikerName).IsOptional();
            Property(m => m.Snapp_bikerPhone).IsOptional();
            Property(m => m.Snapp_distance).IsOptional();
            Property(m => m.Snapp_eta).IsOptional();
            Property(m => m.Snapp_orderAcceptedAt).IsOptional();
            Property(m => m.Description_log).IsOptional();

            
        }
    }
}
