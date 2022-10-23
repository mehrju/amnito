using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Order
{
    public class OrderCancelResultModel
    {
        public bool Success { get; set; }
        public int ResultCode { get; set; }
        public string Message { get; set; }
    }
}
