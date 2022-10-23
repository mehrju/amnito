using Nop.Core;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Domain
{
   public class Tbl_Ticket_Priority : BaseEntity
    {
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.PriorityName")]
        [DataType(DataType.Text)]
        public String Name { get; set; }

        [ScaffoldColumn(false)]
        public int Sort { get; set; }



        [ScaffoldColumn(false)]
        public bool IsActive { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DateInsert { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? DateUpdate { get; set; }

        [ScaffoldColumn(false)]
        public int IdUserInsert { get; set; }

        [ScaffoldColumn(false)]
        public int? IdUserUpdate { get; set; }
    }
}
