using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Orders.MultiShipping.Components
{
    [ViewComponent(Name = "PostbarNewsletterBox")]
    public class PostbarNewsletterBoxViewComponent : NopViewComponent
    {
        private readonly INewsletterModelFactory _newsletterModelFactory;
        public PostbarNewsletterBoxViewComponent(INewsletterModelFactory newsletterModelFactory)
        {
            _newsletterModelFactory = newsletterModelFactory;
        }
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            return View("~/Plugins/Orders.MultiShipping/Views/Components/PostbarNewsletterBox/Default.cshtml", _newsletterModelFactory.PrepareNewsletterBoxModel());
        }
    }
}
