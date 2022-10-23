using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.plugin.Orders.NewCheckOut.Models
{
    public class OrderDivisionModel
    {
        [DisplayName(displayName: "قیمت هر گرم طلای 18 عیار")]
        public long BaseGoldPriceInGram { get; set; }
        [DisplayName(displayName: "قیمت هر دلار")]
        public long BaseDallerPrice { get; set; }
        [DisplayName(displayName: "مقدار طلای پرداختی")]
        public long AmountOfGold { get; set; }
        [DisplayName(displayName: "مبلغ دلار پرداختی")]
        public long AmountOfDaller { get; set; }
        [DisplayName(displayName: "مبلغ پرداختی به صورت نقدی")]
        public long AmountOfCash { get; set; }
        [DisplayName(displayName: "مبلغ کل")]
        public decimal TotalPrice { get; set; }
        public string Str_TotalPrice { get; set; }
        [DisplayName(displayName: "گروه مورد نظر")]
        public int RoleId { get; set; }
        public List<SelectListItem> RoleList { get; set; }
    }
}
