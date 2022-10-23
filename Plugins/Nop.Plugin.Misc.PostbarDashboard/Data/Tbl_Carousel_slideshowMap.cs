using Nop.Plugin.Misc.PostbarDashboard.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Data
{
    public class Tbl_Carousel_slideshowMap : EntityTypeConfiguration<Tbl_Carousel_slideshow>
    {
        public Tbl_Carousel_slideshowMap()
        {
            ToTable("Tbl_Carousel_slideshow");

            HasKey(m => m.Id);
            Property(m => m.Title_Carousel_slideshow);
            Property(m => m.Discrition_Carousel_slideshow).IsOptional();
            Property(m => m.UrlImage).IsOptional();
            Property(m => m.UrlImageMobile).IsOptional();
            Property(m => m.UrlPage).IsOptional();
            Property(m => m.IsActive);
            Property(m => m.TimeInterval);
            Property(m => m.LimitedStore);

            Property(m => m.DateExpire).IsOptional();
            Property(m => m.DateInsert);
            Property(m => m.IsVideo);
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();
        }
    }
}
