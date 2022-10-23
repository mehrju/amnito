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
    public class Tbl_PatternAnswer_Ticket_Damages_RequestCOD : BaseEntity
    {

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.TitlePatternAnswer")]
        [DataType(DataType.Text)]
        public String TitlePatternAnswer { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.DescriptipnPatternAnswer")]
        [DataType(DataType.Text)]
        public String DescriptipnPatternAnswer { get; set; }


        // 1 ticket
        //2 damages
        //3 Request COD
        [ScaffoldColumn(false)]
        public int Type { get; set; }

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
