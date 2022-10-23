using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models.NotificationModel
{
    public class NotifTitleModel : BaseEntity
    {
        public string NotifTitleName { get; set; }
        public bool IsActive { get; set; }
        [NotMapped]
        public NotifTitle? notifTitle
        {
            get
            {
                return (NotifTitle)this.Id;
            }
        }
    }
    public enum NotifTitle
    {
        none = 0,
        NewOrder_Manger = 1,
        NewOrder_Sender = 2,
        NewOrder_Reciver = 3,
        NotifyPost_PostSupervisor = 4,
        NotifyPost_Sender = 5,
        NotifyPost_Reciver = 6,
        SendToPostMan = 7,
        Collect_Sender = 8,
        Collect_Reciver = 9,
        shipped_Sender = 10,
        shipped_Reciver = 11,
        delivered_Sender = 12,
        delivered_Reciver = 13,
        Cancel_Sender = 14,
        Delivered_Reciver = 15,
        Return_Sender = 16,
        Return_Reciver = 17,
        WaitingBox_Serncer = 18,
        WaitingBox_Reciver = 19
    }
}
