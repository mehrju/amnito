using Nop.Core;
using Nop.Plugin.Orders.MultiShipping.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Domain
{
    public class Tbl_Comment : BaseEntity
    {
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public string TrackingCode { get; set; }
        public string Description { get; set; }
        public CommentRateEnum Rate { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
