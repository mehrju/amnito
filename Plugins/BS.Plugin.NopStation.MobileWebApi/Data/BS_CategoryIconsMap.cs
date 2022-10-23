using BS.Plugin.NopStation.MobileWebApi.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Data
{
    public partial class BS_CategoryIconsMap : EntityTypeConfiguration<BS_CategoryIcons>
    {
        public BS_CategoryIconsMap()
        {
            this.ToTable("BS_CategoryIcons");
            this.HasKey(x => x.Id);
        }
    }
}
