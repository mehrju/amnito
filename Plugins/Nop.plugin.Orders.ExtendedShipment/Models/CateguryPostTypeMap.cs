using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class CateguryPostTypeMap: EntityTypeConfiguration<CateguryPostTypeModel>
    {
        public CateguryPostTypeMap()
        {
            ToTable("CateguryPostType");
            HasKey(p => p.Id);
            Property(p => p.CateguryId);
            Property(p => p.CateguryName);
            Property(p => p.PostType);
        }
    }
}
