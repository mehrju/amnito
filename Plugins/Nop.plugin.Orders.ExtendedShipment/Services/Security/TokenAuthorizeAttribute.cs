using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JWT;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Nop.plugin.Orders.ExtendedShipment.Extensions;

namespace Nop.plugin.Orders.ExtendedShipment.Services.Security
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
            public void OnAuthorization(AuthorizationFilterContext actionContext)
            {

                var identity = ParseAuthorizationHeader(actionContext);
                if (identity == false)
                {
                    Challenge(actionContext);
                    return;
                }

                //base.OnAuthorization(actionContext);
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
