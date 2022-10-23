using Nop.Core.Configuration;

namespace BS.Plugin.Orders.ExtendedShipment.Infrastructure
{
    public class OptimeSetting : ISettings
    {
        public string BaseUrl { get; set; } = "https://dashboard2.optime-ai.com/api";
        public string Username { get; set; } = "dc_postex";
        public string Password { get; set; } = "postex2021";
    }
}
