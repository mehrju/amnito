using Nop.Data.Mapping;
using NopFarsi.Payments.SepShaparak.Domain;

namespace NopFarsi.Payments.SepShaparak.Data
{
    public class OrderRefundStatusMap : NopEntityTypeConfiguration<OrderRefundStatus>
    {
        public OrderRefundStatusMap()
        {
            ToTable("OrderRefundStatus");
            HasKey(tr => tr.Id);
        }
    }
}
