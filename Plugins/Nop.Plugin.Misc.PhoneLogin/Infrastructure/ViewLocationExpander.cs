using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Nop.Plugin.Misc.PhoneLogin.Infrastructure
{
    public class ViewLocationExpander : IViewLocationExpander
    {

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            //if (context.ViewName == "Register")
            //{
            //    viewLocations = new string[] { $"/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/Register.cshtml" }.Concat(viewLocations);
            //}
           
            return viewLocations;
        }
    }
}
