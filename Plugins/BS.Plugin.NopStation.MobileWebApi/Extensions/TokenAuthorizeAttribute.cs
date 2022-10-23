using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;


namespace BS.Plugin.NopStation.MobileWebApi.Extensions
{

    public class TokenAuthorizeAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public TokenAuthorizeAttribute() : base(typeof(TokenAuthorizeAttributeFilter))
        {
        }

        #endregion

        #region nested class
        public class TokenAuthorizeAttributeFilter : IAuthorizationFilter
        {
            public  void OnAuthorization(AuthorizationFilterContext actionContext)
            {

                var identity = ParseAuthorizationHeader(actionContext);
                if (identity == false)
                {
                    Challenge(actionContext);
                    return;
                }

               // base.OnAuthorization(actionContext);
            }

            protected virtual bool ParseAuthorizationHeader(AuthorizationFilterContext actionContext)
            {
                bool check = false;
                StringValues checkToken;
                if (actionContext.HttpContext.Request.Headers.TryGetValue(Constant.TokenName, out checkToken))
                {
                    var token = checkToken.FirstOrDefault();
                    var secretKey = Constant.SecretKey;
                    try
                    {
                        var payload = JWT.JsonWebToken.DecodeToObject(token, secretKey) as IDictionary<string, object>;
                        check = true;
                    }
                    catch
                    {
                        check = false;
                    }
                }

                return check;
            }
            void Challenge(AuthorizationFilterContext actionContext)
            {
                var host = actionContext.HttpContext.Request.Host;
                actionContext.Result = new UnauthorizedResult();
                var response = new BaseResponse
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    ErrorList = new List<string>
                    {
                        "Token Invalid or Expired.Please Login Again"
                    }
                };
                actionContext.HttpContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", host));
                actionContext.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(response));
                //actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, response);

            }

        }
        #endregion
    }

   
}

