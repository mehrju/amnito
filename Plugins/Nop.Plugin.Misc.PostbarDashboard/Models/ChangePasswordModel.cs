using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{
    public class ChangePasswordModel
    {
        public string CustomerId { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
    }
}
