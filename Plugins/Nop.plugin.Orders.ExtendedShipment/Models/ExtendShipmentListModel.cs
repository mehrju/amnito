using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Web.Areas.Admin.Models.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public enum CustomerType {
        PostMan,
        PostAdmin
    }
         
    public class ExtendShipmentListModel : ShipmentListModel
    {
        
        public int ReciverCountryId { get; set; }
        public int ReciverStateProvinceId { get; set; }
        public string ReciverCity { get; set; }
        public int PostmanId { get; set; }
        public string ReciverName { get; set; }
        public string SenderName { get; set; }
        public int orderId { get; set; }
        public int ShipmentState { get; set; }
        public int ShipmentState2 { get; set; }
        public int CODShipmentEventId { get; set; }
        public int CodTrackingDayCount { get; set; }
        public DateTime? CodEndTime { get; set; }
        public bool HasGoodsPrice { get; set; }
        public int treeCustomerId { get; set; }
        public int treeStateId { get; set; }
        public int treeCountryId { get; set; }
        public int StatusId { get; set; }
    }
}
