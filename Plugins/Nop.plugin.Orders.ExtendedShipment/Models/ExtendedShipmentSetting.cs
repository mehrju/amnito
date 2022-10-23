using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class ExtendedShipmentSetting: ISettings
    {
        public int PostmanRoleId { get; set; }
        public int PostAdminRoleId { get; set; }
        public string PostUserName { get; set; }
        public string PostPassword { get; set; }
        public string PostType { get; set; }
        public int UpdateFromPostInterval { get; set; }
        public string PostmanMessageTemplate { get; set; }
        public string PostAdminMessageTemplate { get; set; }
        public string StoreAdminMessageTemplate { get; set; }
        public int StoreAdminRoleId { get; set; }
        public string CustomerMessageTemplate { get; set; }
        public int? TaskId { get; set; }
    }
}
