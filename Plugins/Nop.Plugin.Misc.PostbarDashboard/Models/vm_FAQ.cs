using Nop.Plugin.Misc.Ticket.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{
   public class vm_FAQ
    {
        
        public List<listFAQ> ListFAQ { get; set; }

        
    }
    public class listFAQ
    {
        public string NameCategory { get; set; }

        public List<Tbl_FAQ> Listfaq { get; set; }
    }

}
