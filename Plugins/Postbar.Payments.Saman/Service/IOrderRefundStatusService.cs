using NopFarsi.Payments.SepShaparak.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NopFarsi.Payments.SepShaparak.Service
{
    public interface IOrderRefundStatusService
    {
        void InsertOrderRefundStatus(OrderRefundStatus orderRefundStatus);

        bool IsSuccessfulRefund(int orderId);
    }
}
