using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{
   public class Tbl_ViewVideoCustomer : BaseEntity
    {
        public int CustomerId { get; set; }
        public DateTime DateView { get; set; }
        public bool IsActive { get; set; }
        public string IPCustomer { get; set; }
    }
}
