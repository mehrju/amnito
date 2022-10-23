using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Params_SnappBox_API
{
   public class Params_SnappBox_Webhok
    {
        public int? timeStamp { get; set; }
        public string allocationStatus { get; set; }
        public int? sequenceNumber { get; set; }
        public int? customerRefId { get; set; }
        public string webhookType { get; set; }
        public string orderId { get; set; }
        public string orderStatus { get; set; }
        public string bikerName { get; set; }
        public string bikerPhone { get; set; }
        public double? distance { get; set; }
        public int? eta { get; set; }
        public string orderAcceptedAt { get; set; }
        public string bikerPhotoUrl { get; set; }
        public int? notificationId { get; set; }
    }
}
