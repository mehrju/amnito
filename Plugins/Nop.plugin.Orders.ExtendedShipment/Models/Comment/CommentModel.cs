using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models.Comment
{
    public class CommentModel
    {
        public int Id { get; set; }
        public string TrackingCode { get; set; }
        public string Description { get; set; }
        public int Rate { get; set; }
        public bool IsPublished { get; set; }
    }
}
