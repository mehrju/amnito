using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Orders.MultiShipping.Components
{
    [ViewComponent(Name = "ShipitoNewsletterBox")]
    public class ShipitoNewsletterBoxViewComponent : NopViewComponent
    {
        private readonly INewsletterModelFactory _newsletterModelFactory;
        public ShipitoNewsletterBoxViewComponent(INewsletterModelFactory newsletterModelFactory)
        {
            _newsletterModelFactory = newsletterModelFactory;
        }
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            return View("~/Plugins/Orders.MultiShipping/Views/Components/ShipitoNewsletterBox/Default.cshtml", _newsletterModelFactory.PrepareNewsletterBoxModel());
        }
    }
}
