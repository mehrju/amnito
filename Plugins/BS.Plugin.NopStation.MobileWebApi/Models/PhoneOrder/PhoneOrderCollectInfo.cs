using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models.PhoneOrder
{
    public class PhoneOrderCollectInfo
    {
        public int PhoneOrderId { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
    }
}
