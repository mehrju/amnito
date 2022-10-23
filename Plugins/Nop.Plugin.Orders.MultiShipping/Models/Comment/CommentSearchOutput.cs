using Nop.Plugin.Orders.MultiShipping.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models.Comment
{
    public class CommentSearchOutput
    {
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public string TrackingCode { get; set; }
        public string Description { get; set; }
        public CommentRateEnum Rate { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
    }
}
