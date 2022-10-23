using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Domain
{
    public class Tbl_Damages_Detail : BaseEntity
    {
        public Tbl_Damages_Detail()
        {
        }
        [ScaffoldColumn(false)]
        public int IdDamages { get; set; }
        public Tbl_Damages Damages { get; set; }

        //0 customer  1 staff
        [ScaffoldColumn(false)]
        public bool Type { get; set; }


        [ScaffoldColumn(false)]
        public int? StaffId { get; set; }

        [ScaffoldColumn(false)]
        public DateTime DateInsert { get; set; }

        [ScaffoldColumn(false)]
        public string Description { get; set; }

        [ScaffoldColumn(false)]
        public String UrlFileCardMeli { get; set; }
        [ScaffoldColumn(false)]
        public String UrlFile1 { get; set; }
        [ScaffoldColumn(false)]
        public String UrlFile2 { get; set; }
        [ScaffoldColumn(false)]
        public String UrlFile3 { get; set; }
        [ScaffoldColumn(false)]
        public String UrlFile4 { get; set; }
    }
}
