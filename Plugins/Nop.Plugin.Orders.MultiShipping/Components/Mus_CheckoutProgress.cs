using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Plugin.Orders.MultiShipping.Models.Checkout;

namespace Nop.Plugin.Orders.MultiShipping.Components
{
    [ViewComponent(Name = "Mus_CheckoutProgress")]
    public class Mus_CheckoutProgressViewComponent : NopViewComponent
    {
        private readonly ICheckoutModelFactory _checkoutModelFactory;

        public Mus_CheckoutProgressViewComponent(ICheckoutModelFactory checkoutModelFactory)
        {
            this._checkoutModelFactory = checkoutModelFactory;
        }

        public IViewComponentResult Invoke(Mus_CheckoutProgressStep step)
        {
            var model = new Mus_CheckoutProgressModel { CheckoutProgressStep = step };
            return View("~/Plugins/Orders.MultiShipping/Views/Components/CheckoutProgress/Default.cshtml", model);
        }
    }
}
