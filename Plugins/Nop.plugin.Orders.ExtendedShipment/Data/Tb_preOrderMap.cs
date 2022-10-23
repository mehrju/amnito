using Nop.plugin.Orders.ExtendedShipment.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Data
{
    public class Tb_preOrderMap : EntityTypeConfiguration<Tb_preOrder>
    {
        public Tb_preOrderMap()
        {
            ToTable("Tb_preOrder");
            HasKey(p=>p.Id);
        }
    }
}
