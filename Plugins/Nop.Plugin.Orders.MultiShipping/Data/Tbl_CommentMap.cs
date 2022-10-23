using Nop.Plugin.Orders.MultiShipping.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Data
{
    public class Tbl_CommentMap : EntityTypeConfiguration<Tbl_Comment>
    {
        public Tbl_CommentMap()
        {
            ToTable("Tbl_Comment");
        }
    }
}
