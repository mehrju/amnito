using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Infrastructure;
using ActionFilterAttribute = Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using BS.Plugin.NopStation.MobileWebApi.Models.NstSettingsModel;
using Nop.Services.Stores;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace BS.Plugin.NopStation.MobileWebApi.Extensions
{

   
    public class NstAuthorizationAttribute : TypeFilterAttribute
    {
        #region Ctor
        public NstAuthorizationAttribute() : base(typeof(NstAuthorization))
        {
            
        }
        

        #endregion

        #region Nested filter
        public class NstAuthorization : IActionFilter
        {
            public  void OnActionExecuting(ActionExecutingContext filterContext)
            {

                var identity = ParseNstAuthorizationHeader(filterContext);
                if (identity == false)
                {
                    CreateNstAccessResponceMessage(filterContext);
                    return;
                }


            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
                //do nothing
            }


            protected virtual bool ParseNstAuthorizationHeader(ActionExecutingContext actionContext)
            {
               var httpContext = EngineContext.Current.Resolve<IHttpContextAccessor>().HttpContext;
                StringValues keyFound; 
                    httpContext.Request.Headers.TryGetValue(Constant.NST,out keyFound);
                var requestkey =  keyFound.FirstOrDefault();
                try
                {

                    var settingService = EngineContext.Current.Resolve<ISettingService>();
                    var storeService = EngineContext.Current.Resolve<IStoreService>();
                    var workContext = EngineContext.Current.Resolve<IWorkContext>();
                    var storeScope = 0;
                    if (storeService.GetAllStores().Count < 2)
                    {
                        storeScope = 0;
                    }
                    else
                    {
                        var storeId = workContext.CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.AdminAreaStoreScopeConfiguration);
                        var store = storeService.GetStoreById(storeId);
                        storeScope = store?.Id ?? 0;
                    }
                    var nstSettings = settingService.LoadSetting<NstSettingsModel>(storeScope);

                    var load = JWT.JsonWebToken.DecodeToObject(requestkey, nstSettings.NST_SECRET) as IDictionary<string, object>;
                    if (load != null)
                    {
                        if (!string.IsNullOrEmpty((string)(load[Constant.NST_KEY])))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch(Exception ex)
                {

                    return false;
                }
                return false;

            }
            void CreateNstAccessResponceMessage(ActionExecutingContext actionContext)
            {
                // var host = actionContext.Request.RequestUri.DnsSafeHost;
                var host = actionContext.HttpContext.Request.Host;
                var response = new BaseResponse
                {
                    StatusCode = (int)ErrorType.AuthenticationError,
                    ErrorList = new List<string>
                    {
                        "Nst Token Not Valid"
                    }
                };
           actionContext.Result = new BadRequestObjectResult(response);
                // actionContext.HttpContext.Response.StatusCode = 400;
                // actionContext.HttpContext.Response.WriteAsync("Nst Token Not Valid");
                return;
                // actionContext.HttpContext.Response.Body = actionContext.HttpContext.Response.Body(HttpStatusCode.Forbidden, response);
                // actionContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", host));
            }
        }

       
        #endregion

       
    }
   
}
