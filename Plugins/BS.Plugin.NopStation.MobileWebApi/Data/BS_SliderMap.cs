using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.Domain;

namespace BS.Plugin.NopStation.MobileWebApi.Data
{
   public partial class BS_SliderMap : EntityTypeConfiguration<BS_Slider>
   {
       public BS_SliderMap()
       {
           this.ToTable("BS_Slider");
           this.HasKey(x => x.Id);
       }
    }
}
