using BS.Plugin.NopStation.MobileWebApi.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Data
{
    public class Tbl_ReceivedSmsMap : EntityTypeConfiguration<Tbl_ReceivedSms>
    {
        public Tbl_ReceivedSmsMap()
        {
            this.ToTable("Tbl_ReceivedSms");
            this.HasKey(x => x.Id);
        }
    }
}
