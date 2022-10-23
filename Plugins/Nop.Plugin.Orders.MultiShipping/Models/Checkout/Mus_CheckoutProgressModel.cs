using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Orders.MultiShipping.Models.Checkout
{
    public class Mus_CheckoutProgressModel : BaseNopModel
    {
        public Mus_CheckoutProgressStep CheckoutProgressStep { get; set; }
    }

    public enum Mus_CheckoutProgressStep
    {
        Cart,
        Address,
        Shipment,
        Shipping,
        Payment,
        Confirm,
        Complete
    }
}