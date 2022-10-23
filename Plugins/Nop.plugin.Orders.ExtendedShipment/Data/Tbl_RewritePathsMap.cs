using Nop.plugin.Orders.ExtendedShipment.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.plugin.Orders.ExtendedShipment.Data
{
    class Tbl_RewritePathsMap : EntityTypeConfiguration<Tbl_RewritePaths>
    {
        public Tbl_RewritePathsMap()
        {
            ToTable("Tbl_RewritePaths");
            HasKey(p => p.Id);
        }
    }
}
