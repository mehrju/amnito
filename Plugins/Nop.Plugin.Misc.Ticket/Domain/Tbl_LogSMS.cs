using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Domain
{
    public class Tbl_LogSMS : BaseEntity
    {
       
        public int Type { get; set; }
        public string Mobile { get; set; }
        public DateTime DateSend { get; set; }
        public string TextMessage { get; set; }
        public int StoreId { get; set; }
        public int Status { get; set; }
    }
}
