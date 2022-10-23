using Nop.plugin.Orders.ExtendedShipment.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Data
{
   public class Tbl_ChargeWalletFailsMap : EntityTypeConfiguration<Tbl_ChargeWalletFails>
    {
        public Tbl_ChargeWalletFailsMap()
        {
            HasKey(p => p.Id);
        }
    }
}
