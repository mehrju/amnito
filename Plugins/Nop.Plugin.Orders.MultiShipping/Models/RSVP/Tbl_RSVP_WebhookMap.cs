using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Orders.MultiShipping.Models;

namespace Nop.Plugin.Orders.MultiShipping.Models.RSVP
{
    public class Tbl_RSVP_WebhookMap : EntityTypeConfiguration<Tbl_RSVP_Webhook>
    {
        public Tbl_RSVP_WebhookMap()
        {
            ToTable("Tbl_RSVP_Webhook");
            HasKey(m => m.Id);
            Property(m => m.Mobile);
            Property(m => m.DateInsert);

        }
    }
}
