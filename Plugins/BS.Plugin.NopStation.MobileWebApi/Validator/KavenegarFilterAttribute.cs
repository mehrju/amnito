using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Validator
{
    public class KavenegarFilterAttribute : IActionFilter
    {
        private readonly IWebHelper _webHelper;

        //آی پی های کاوه نگار
        private string[] validIps =
        {
            "37.130.202.18",
            "37.130.202.19",
            "37.130.202.20",
            "37.130.202.17",
            "37.130.202.25",
            "79.175.172.9",
            "79.175.172.10",
            "79.175.172.11",
            "79.175.172.12",
            "79.175.172.13",
            "79.175.172.14",
            "79.175.172.15",
            "79.175.172.16",
            "79.175.172.17",
            "104.26.11.225",
            "104.26.11.226",
            "79.175.166.177",
            "79.175.166.182"
        };

        public KavenegarFilterAttribute(IWebHelper webHelper)
        {
            _webHelper = webHelper;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var ip = _webHelper.GetCurrentIpAddress();
            if (!validIps.Contains(ip))
            {
                context.Result = new BadRequestResult();
            }
        }
    }
}
