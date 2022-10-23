using Nop.Core;
using Nop.Core.Infrastructure;
using System;
using Nop.Plugin.Misc.PhoneLogin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core.Data;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Nop.Plugin.Misc.PhoneLogin.ActionFilters
{
    /// <summary>
    /// Represents filter attribute that validates customer password expiration
    /// </summary>
    public class CustomerLoginAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public CustomerLoginAttribute() : base(typeof(CustomerLoginFilter))
        {
        }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that validates customer password expiration
        /// </summary>
        private class CustomerLoginFilter : IActionFilter
        {
            #region Fields

            private readonly IUrlHelperFactory _urlHelperFactory;
            private readonly IWorkContext _workContext;
            private readonly PhoneLoginSettings phoneLoginSettings;

            #endregion

            #region Ctor

            public CustomerLoginFilter(IUrlHelperFactory urlHelperFactory, IWorkContext workContext)
            {
                this._urlHelperFactory = urlHelperFactory;
                this._workContext = workContext;
                phoneLoginSettings = EngineContext.Current.Resolve<PhoneLoginSettings>();
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called before the action executes, after model binding is complete
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (phoneLoginSettings.Enabled)
                {
                    var myController = EngineContext.Current.Resolve<PhoneLoginController>();

                    if (context == null)
                        throw new ArgumentNullException(nameof(context));

                    if (context.HttpContext.Request == null)
                        return;

                    if (!DataSettingsHelper.DatabaseIsInstalled())
                        return;

                    //get action and controller names
                    var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                    var actionName = actionDescriptor?.ActionName;
                    var controllerName = actionDescriptor?.ControllerName;

                    if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
                        return;
                    var httpMethod = context.HttpContext.Request.Method;
                    var isValid = (context.Controller as Controller).ViewData.ModelState.IsValid;

                    if (actionName.Equals("Login", StringComparison.InvariantCultureIgnoreCase)
                        && httpMethod.Equals("GET", StringComparison.InvariantCultureIgnoreCase))
                    {
                        bool? checkoutAsGuest = null;

                        if (context.RouteData.Values["checkoutAsGuest"] != null)
                        {
                            checkoutAsGuest = bool.Parse(context.RouteData.Values["checkoutAsGuest"].ToString());
                        }
                        context.Result = myController.Login(checkoutAsGuest);
                    }
                    else if (actionName.Equals("Register", StringComparison.InvariantCultureIgnoreCase)
                        && httpMethod.Equals("GET", StringComparison.InvariantCultureIgnoreCase)
                        && isValid)
                    {
                        context.Result = myController.Register();
                    }
                }
            }

            /// <summary>
            /// Called after the action executes, before the action result
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuted(ActionExecutedContext context)
            {
                //do nothing
            }

            #endregion
        }

        #endregion
    }
}
