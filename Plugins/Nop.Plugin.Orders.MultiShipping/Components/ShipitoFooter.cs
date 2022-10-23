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
    [ViewComponent(Name = "ShipitoFooter")]
    public class ShipitoFooterViewComponent : NopViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;
        public ShipitoFooterViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            return View("~/Plugins/Orders.MultiShipping/Views/Components/ShipitoFooter/Default.cshtml", _commonModelFactory.PrepareFooterModel());
        }
    }
}
