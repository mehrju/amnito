using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class BarcodeRepositoryMap: EntityTypeConfiguration<BarcodeRepositoryModel>
    {
        public BarcodeRepositoryMap()
        {
            ToTable("BarcodeRepository");
            HasKey(p => p.Id);
            Property(p => p.ShipmentId);
            Property(p => p.Barcode);
        }
    }
}
