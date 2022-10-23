using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Components
{
    [ViewComponent(Name = "QuickOrderPrice")]
    public class QuickOrderPrice : NopViewComponent
    {

        public QuickOrderPrice()
        {

        }

        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Orders.MultiShipping/Views/Components/QuickOrderPrice/Default.cshtml");
        }
    }
}