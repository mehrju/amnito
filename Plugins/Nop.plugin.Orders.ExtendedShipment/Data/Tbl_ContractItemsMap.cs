using Nop.plugin.Orders.ExtendedShipment.Domain;
using Nop.plugin.Orders.ExtendedShipment.Models;
using System.Data.Entity.ModelConfiguration;

namespace Nop.plugin.Orders.ExtendedShipment.Data
{
    class Tbl_ContractItemsMap : EntityTypeConfiguration<Tbl_ContractItems>
    {
        public Tbl_ContractItemsMap()
        {
            ToTable("Tbl_ContractItems");
            HasKey(p => p.Id);
        }
    }
}
