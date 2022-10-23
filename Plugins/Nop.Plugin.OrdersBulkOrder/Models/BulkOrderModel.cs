using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Orders.MultiShipping.Models;

namespace Nop.Plugin.Orders.BulkOrder.Models
{
    public class BulkOrderModel : BaseEntity
    {
        public string FileName { get; set; }
        [DisplayName("وضعیت سفارش")]
        public int OrderStatusId { get; set; }
        [DisplayName("وضعیت پرداخت")]
        public int PaymentStatusId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CustomerId { get; set; }
        public decimal OrderTotal { get; set; }
        public int OrderId { get; set; }
        [DisplayName("نوع فایل سفارش")]
        public bool IsCod { get; set; }
        public int OrderCount { get; set; }
        [NotMapped]
        public OrderStatus OrderStatus
        {
            get => (OrderStatus)OrderStatusId;
            set => OrderStatusId = (int)value;
        }
        [NotMapped]
        public PaymentStatus PaymentStatus
        {
            get => (PaymentStatus)PaymentStatusId;
            set => PaymentStatusId = (int)value;
        }
        [NotMapped]
        [DisplayName("نام مشتری")]
        public string CustomerName { get; set; }
        [NotMapped]
        [DisplayName("تاریخ سفارش از")]
        [UIHint("DateNullable")]
        public DateTime? CreateDateFrom { get; set; }
        [NotMapped]
        [DisplayName("تاریخ سفارش تا")]
        [UIHint("DateNullable")]
        public DateTime? CreateDateTo { get; set; }
        [DisplayName("کوپن تخفیف")]
        public string discountCouponCode { get; set; }
        public bool Deleted { get; set; }
        public int? FileType { get; set; }
        public bool? PrintLogo { get; set; }
        public bool? SendSms { get; set; }
        public int? ServiceSort { get; set; }
        public bool? HasAccessToPrinter { get; set; }
        public string OrderIds { get; set; }
        public bool? IsInProcceing { get; set; }
        public int ServiceId { get; set; }
        [NotMapped]
        public string CategoryName { get; set; }

        [NotMapped]
        public string Sender_FristName { get; set; }
        [NotMapped]
        public string Sender_LastName { get; set; }
        [NotMapped]
        public string Sender_mobile { get; set; }
        [NotMapped]
        public string Sender_Country { get; set; }
        [NotMapped]
        public string Sender_State { get; set; }
        [NotMapped]
        public string Sender_City { get; set; }
        [NotMapped]
        public string Sender_PostCode { get; set; }
        [NotMapped]
        public string Sender_Address { get; set; }
        [NotMapped]
        public string Sender_Email { get; set; }
        [NotMapped]
        public float? Sender_Lat { get; set; }
        [NotMapped]
        public float? Sender_Lon { get; set; }
        [NotMapped]
        public bool _isPeyk { get; set; }
        [NotMapped]
        public phoneOrderRegisterOrderModel PhoneOrdermodel { get; set; }
    }

}
