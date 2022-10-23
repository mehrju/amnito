using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class AgentAmountRuleModel : BaseEntity
    {
        [DisplayName("عنوان")]
        public string Name { get; set; }
        [DisplayName("کالا")]
        public int ProductId { get; set; }
        [DisplayName("عنوان ویژگی")]
        public int ProductAttributeId { get; set; }
        [DisplayName("مقدار ویژگی")]
        public int ProductAttributeValueId { get; set; }
        [DisplayName("حق السهم نمایندگی")]
        public int Price { get; set; }
        [DisplayName("تاریخ ایجاد")]
        public DateTime? CreateDate { get; set; }
        [DisplayName("کاربر ایجاد کننده")]
        public int CreateCustomerId { get; set; }
        [DisplayName("تاریخ حدف")]
        public DateTime? DeletedDate { get; set; }
        [DisplayName("کاربر حذف کننده")]
        public int? DeleteCustomerId { get; set; }

        [DisplayName("تعداد از")]
        public int MinCount { get; set; }

        [DisplayName("تعداد تا")]
        public int MaxCount { get; set; }

        [NotMapped]
        public List<SelectListItem> AvailableProduct { get; set; }
        [NotMapped]
        public string ProductName { get; set; }
        [NotMapped]
        public string ProductAttributeName { get; set; }
        [NotMapped]
        public string ProductAttributeValueName { get; set; }
    }
}
