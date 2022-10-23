using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Domain
{
    public class Tbl_OpTimeList : BaseEntity
    {
        public string ListJsonString { get; set; }
        public int CollectorCustomerId { get; set; }
        public DateTime CreateDate { get; set; }
        public string Token { get; set; }
        public bool? IsUpload { get; set; }
        public int? UploadId { get; set; }
        public int? _Status { get; set; }
        public string ResponceJsonString { get; set; }
        public bool IsFake { get; set; }
        public int? PhoneOrderId { get; set; }
    }
}
