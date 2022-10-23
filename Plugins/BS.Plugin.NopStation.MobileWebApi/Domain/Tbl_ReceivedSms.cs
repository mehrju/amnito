using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Domain
{
    public class Tbl_ReceivedSms : BaseEntity
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        public string MessageId { get; set; }
        public int? TicketId { get; set; }
        public int? RewardPointHistoryId { get; set; }
        public string RefrenceNumber { get; set; }
    }
}
