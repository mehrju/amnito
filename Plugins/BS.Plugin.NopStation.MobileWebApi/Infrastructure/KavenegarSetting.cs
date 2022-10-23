using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Infrastructure
{
    public class KavenegarSetting : ISettings
    {
        public string BaseAddress { get; set; }
        public string ApiKey { get; set; }
        public string LineNumber { get; set; }
    }
}
