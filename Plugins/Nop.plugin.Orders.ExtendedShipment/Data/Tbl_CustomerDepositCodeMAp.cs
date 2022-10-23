using Nop.plugin.Orders.ExtendedShipment.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Data
{
    public class Tbl_CustomerDepositCodeMap : EntityTypeConfiguration<Tbl_CustomerDepositCode>
    {
        public Tbl_CustomerDepositCodeMap()
        {
            ToTable("Tbl_CustomerDepositCode");
            HasKey(m => m.Id);
        }
    }
}
