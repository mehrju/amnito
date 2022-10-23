using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{
    public class Tbl_CheckAvatarCustomer : BaseEntity
    {
        public int CustomerId { get; set; }
        public int CustomerAvatarId { get; set; }
        // 0 new  1 notok 2 ok
        public int StateVerify { get; set; }
        public DateTime? DateVerify { get; set; }
        public int? IdUserVerify { get; set; }
        public DateTime DateInsert { get; set; }
       
        public int IdTicket { get; set; }
    }
}
