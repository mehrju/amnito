using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NopFarsi.Payments.SepShaparak.Domain
{
    public class OrderRefundStatus : BaseEntity
    {
        public int OrderId { get; set; }
        public long RefundRefrenceId { get; set; }
        public RefundStatus RefundStatus { get; set; }
    }

    public enum RefundStatus
    {
        None = 0,
        Start = 1,
        InProgress = 2,
        Ok = 3,
        Fail = 4,
        Cancel = 5,
    }
}