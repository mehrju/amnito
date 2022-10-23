using Nop.Plugin.Misc.PostbarDashboard.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Data
{
    public class Tbl_ServiceProviderDashboardMap : EntityTypeConfiguration<Tbl_ServiceProviderDashboard>
    {
        public Tbl_ServiceProviderDashboardMap()
        {
            ToTable("Tbl_ServiceProviderDashboard");

            HasKey(m => m.Id);
            Property(m => m.TitleServiceProviderDashboard);
            Property(m => m.UrlImage);
            Property(m => m.UrlPageDiscreption);
            Property(m => m.IsActive);
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();
        }
    }
}
