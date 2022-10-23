using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Components
{
    [ViewComponent(Name = "PostbarFooter")]
    public class PostbarFooterViewComponent: NopViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;
        public PostbarFooterViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            return View("~/Plugins/Orders.MultiShipping/Views/Components/Footer/Default.cshtml", _commonModelFactory.PrepareFooterModel());
        }
    }
}
