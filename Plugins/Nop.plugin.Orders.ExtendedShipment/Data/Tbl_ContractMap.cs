using Nop.plugin.Orders.ExtendedShipment.Domain;
using Nop.plugin.Orders.ExtendedShipment.Models;
using System.Data.Entity.ModelConfiguration;

namespace Nop.plugin.Orders.ExtendedShipment.Data
{
    class Tbl_ContractMap : EntityTypeConfiguration<Tbl_Contract>
    {
        public Tbl_ContractMap()
        {
            ToTable("Tbl_Contract");
            HasKey(p => p.Id);
        }
    }
}
