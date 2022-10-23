using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class ExtendedShipmentSettingModel
    {
        [DisplayName(displayName: "نقش مدیر:")]
        public int StoreAdminRoleId { get; set; }
        [DisplayName(displayName: "نقش پستچی:")]
        public int PostmanRoleId { get; set; }
        [DisplayName(displayName: "نقش ناظر پست:")]
        public int PostAdminRoleId { get; set; }
        public List<SelectListItem> RoleList { get; set; }
        [DisplayName(displayName: "نام کاربری سرویس:")]
        public string PostUserName { get; set; }
        [DisplayName(displayName: "پسورد سرویس:")]
        public string PostPassword { get; set; }
        [DisplayName(displayName: "کد نوع پست قراردادی:")]
        public string PostType { get; set; }
        [DisplayName(displayName: "متن پیام برای مدیر:")]
        public string StoreAdminMessageTemplate { get; set; }
        [DisplayName(displayName: "متن پیام برای ناظر پست:")]
        public string PostAdminMessageTemplate { get; set; }
        [DisplayName(displayName: "متن پیام برای پستچی:")]
        public string PostmanMessageTemplate { get; set; }
        [DisplayName(displayName: "بازه زمانی به روز رسانی از پست(دقیقه):")]
        public int UpdateFromPostInterval { get; set; }
        [DisplayName(displayName: "متن پیام برای مشتری:")]
        public string CustomerMessageTemplate { get; set; }
        public int TaskId { get; set; }
        [NotMapped]
        public List<SelectListItem> AllCategory { get; set; }
    }
}
