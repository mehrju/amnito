using Nop.Plugin.Misc.PostbarDashboard.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Data
{
    class Tbl_CheckAvatarCustomerMap : EntityTypeConfiguration<Tbl_CheckAvatarCustomer>
    {
        public Tbl_CheckAvatarCustomerMap()
        {
            ToTable("Tbl_CheckAvatarCustomer");
            HasKey(m => m.Id);
            Property(m => m.CustomerAvatarId);
            Property(m => m.CustomerId);
            Property(m => m.DateInsert);
            Property(m => m.DateVerify);
            Property(m => m.IdUserVerify);
            Property(m => m.StateVerify);
            Property(m => m.IdTicket);

        }
    }
}
