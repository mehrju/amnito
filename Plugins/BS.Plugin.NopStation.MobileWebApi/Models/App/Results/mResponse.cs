using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models.App.Results
{
    public class mResponse
    {
        public int id { get; set; }
        public int Status { get; set; }
        public string message_en { get; set; }
        public string message_fa { get; set; }
        public object data { get; set; }
        public int pagepumber = 0;
        public bool haspage = false;
    }
}
