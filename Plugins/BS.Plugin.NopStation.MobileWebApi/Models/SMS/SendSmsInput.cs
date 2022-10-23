using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models.SMS
{
    public class SendSmsInput
    {
        public string UserName { get; set; }
        public string Passwrod { get; set; }
        public string Receiver { get; set; }
        public string Message { get; set; }
    }
}
