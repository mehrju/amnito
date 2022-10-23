using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.BulkOrder.Models
{
    public class BulkOrderMap : EntityTypeConfiguration<BulkOrderModel>
    {
        public BulkOrderMap()
        {
            ToTable("BulkOrder");
            HasKey(p => p.Id);
            Property(p => p.CreateDate);
            Property(p => p.CustomerId);
            Property(p => p.FileName);
            Property(p => p.OrderId);
            Property(p => p.OrderStatusId);
            Property(p => p.OrderTotal);
            Property(p => p.PaymentStatusId);
            Property(p => p.OrderCount);
            Property(p => p.IsCod);
            Property(p => p.discountCouponCode);
            Property(p => p.Deleted);
            Property(p => p.FileType);
            Property(p => p.ServiceSort);
            Property(p => p.SendSms);
            Property(p => p.PrintLogo);
            Property(p => p.OrderIds);
            Property(p => p.IsInProcceing);
            Property(p => p.ServiceId);
        }
    }
}
