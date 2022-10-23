using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.PhoneLogin.ActionFilters;
using Nop.Web.Controllers;

namespace Nop.Plugin.Misc.PhoneLogin.FilterProvider
{
    class CustomerLoginFilterProvider : IFilterProvider
    {
        private readonly IActionFilter _actionFilter;

        public CustomerLoginFilterProvider(IActionFilter actionFilter)
        {
            _actionFilter = actionFilter;
        }


        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            if (/*actionDescriptor.ControllerDescriptor.ControllerType == typeof(CustomerController) &&*/
                      (actionDescriptor.ActionName.Equals("Login")) || actionDescriptor.ActionName.Equals("Register"))
            {
                return new Filter[]
                    {
                        new Filter(_actionFilter, FilterScope.Action, null)
                    };
            }



            return new Filter[] { };

        }

    }
}
