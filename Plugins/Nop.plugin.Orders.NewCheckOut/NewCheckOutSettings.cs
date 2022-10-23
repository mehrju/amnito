using Nop.Core.Configuration;

namespace Nop.plugin.Orders.NewCheckOut
{
    /// <summary>
    /// Manual payment processor
    /// </summary>
    public class NewCheckOutSettings : ISettings
    {
        public NewCheckOutSettings()
        {
        }
        public long GlodPriceInGram { get; set; }
        public long DollerPrice { get; set; }
        public int RoleId { get; set; }
    }
}
