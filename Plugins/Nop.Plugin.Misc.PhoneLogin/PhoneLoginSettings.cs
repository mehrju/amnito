
using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.PhoneLogin
{
    public class PhoneLoginSettings : ISettings
    {
        public string LineNumber { get; set; }

        public string ApiKey { get; set; }

        public bool Enabled { get; set; }
    }
}