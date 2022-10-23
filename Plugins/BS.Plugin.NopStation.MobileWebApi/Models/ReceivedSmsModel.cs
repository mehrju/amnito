using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models
{
    public class ReceivedSmsModel
    {
        public string UserNameFirstPart { get; set; }
        public string UserNameSecondPart { get; set; }
        public int Amount { get; set; }
        public string RefrenceNumber { get; set; }
        public int CustomerDepositId { get; set; }
    }
}
