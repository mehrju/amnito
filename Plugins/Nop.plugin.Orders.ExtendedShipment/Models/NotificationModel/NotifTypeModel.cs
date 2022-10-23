using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models.NotificationModel
{
    public class NotifTypeModel:BaseEntity
    {
        public string NotifTypeName { get; set; }
        public bool IsFree { get; set; }
    }
}
