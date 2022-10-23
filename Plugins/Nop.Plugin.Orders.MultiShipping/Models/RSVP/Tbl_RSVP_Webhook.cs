using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models.RSVP
{
   public class Tbl_RSVP_Webhook : BaseEntity
    {
        public string Mobile { get; set; }
        public DateTime DateInsert { get; set; }

    }
}
