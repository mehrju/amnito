using Nop.plugin.Orders.ExtendedShipment.Domain;
using Nop.plugin.Orders.ExtendedShipment.Models;
using System.Data.Entity.ModelConfiguration;

namespace Nop.plugin.Orders.ExtendedShipment.Data
{
    class Tbl_ContractItemDetailMap : EntityTypeConfiguration<Tbl_ContractItemDetail>
    {
        public Tbl_ContractItemDetailMap()
        {
            ToTable("Tbl_ContractItemDetail");
            HasKey(p => p.Id);
        }
    }
}
