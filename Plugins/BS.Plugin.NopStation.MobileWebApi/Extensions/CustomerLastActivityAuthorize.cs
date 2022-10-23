using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;

namespace BS.Plugin.NopStation.MobileWebApi.Extensions
{
    public class CustomerLastActivityAuthorizeAttribute : TypeFilterAttribute
    {
        #region ctor
        public CustomerLastActivityAuthorizeAttribute() : base(typeof(CustomerLastActivityAuthorize))
        {
        }
        #endregion

        public class CustomerLastActivityAuthorize : IAuthorizationFilter
        {
            public void OnActionExecuting(AuthorizationFilterContext filterContext)
            {
                if (!DataSettingsHelper.DatabaseIsInstalled())
                    return;

                if (filterContext?.HttpContext.Request == null)
                    return;

                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                var customer = workContext.CurrentCustomer;

                //update last activity date
                if (customer.LastActivityDateUtc.AddMinutes(1.0) < DateTime.UtcNow)
                {
                    var customerService = EngineContext.Current.Resolve<ICustomerService>();
                    customer.LastActivityDateUtc = DateTime.UtcNow;
                    customerService.UpdateCustomer(customer);
                }
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                //do nothing
            }
        }



    }

}


