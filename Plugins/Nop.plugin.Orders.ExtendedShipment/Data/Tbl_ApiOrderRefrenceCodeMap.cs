using Nop.plugin.Orders.ExtendedShipment.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Data
{
    public class Tbl_ApiOrderRefrenceCodeMap : EntityTypeConfiguration<Tbl_ApiOrderRefrenceCode>
    {
        public Tbl_ApiOrderRefrenceCodeMap()
        {
            HasKey(p => p.Id);
            ToTable("Tbl_ApiOrderRefrenceCode");
        }
    }
}
