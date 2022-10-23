using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models.NotificationModel
{
    public class NotifItemsModel : BaseEntity
    {
        [DisplayName("عنوان پیام")]
        public int NotifTitleId { get; set; }
        [NotMapped]
        public string NotifTitle { get; set; }
        [DisplayName("نوع سرویس")]
        public int NotifCategoryId { get; set; }
        [NotMapped]
        public string NotifCategoryName { get; set; }
        [DisplayName("روش اطلاع رسانی")]
        public int NotifTypeId { get; set; }
        [NotMapped]
        public string NotifTypeName { get; set; }
        [DisplayName("متن پیام")]
        public string NotifTamplate { get; set; }
        [DisplayName("فعال")]
        public bool IsActive { get; set; }
        [NotMapped]
        public List<SelectListItem> AvailableNotifTitle { get; set; }
        [NotMapped]
        public List<SelectListItem> AvailableCategory { get; set; }
        [NotMapped]
        public string Str_IsActive { get; set; }
    }
}
