using Nop.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class PostCoordinationMap : EntityTypeConfiguration<PostCoordinationModel>
    {
        public PostCoordinationMap()
        {
            ToTable("PostCoordination");
            HasKey(p => p.Id);
            Property(p => p.orderId);
            Property(p => p.CoordinationDate);
            Property(p => p.Desc);
        }
    }
}
