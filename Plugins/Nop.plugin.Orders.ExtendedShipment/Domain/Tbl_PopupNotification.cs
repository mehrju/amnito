using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Domain
{
    public class Tbl_PopupNotification : BaseEntity
    {
        public string Title { get; set; }
        public DateTime? FromDate  { get; set; }
        public DateTime? ToDate { get; set; }
        public bool IsActive { get; set; }
        public string Content { get; set; }
        public string StoreIds { get; set; }
    }
}
