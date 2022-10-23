using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class ExtendedOrderListModel: OrderListModel
    {
        public int SenderStateProvinceId { get; set; }
        public int ReciverCountryId { get; set; }
        public int ReciverStateProvinceId { get; set; }
        public string ReciverName { get; set; }
        public string SenderName { get; set; }
        public int OrderId { get; set; }
        public bool IsOrderOutDate { get; set; }
        public int orderState { get; set; }
        public DateTime? TimeFrom { get; set; }
        public DateTime? TimeTo { get; set; }
        public int treeCountryId { get; set; }
        public int treeStateId { get; set; }
        public int treeCustomerId { get; set; }
    }
}
