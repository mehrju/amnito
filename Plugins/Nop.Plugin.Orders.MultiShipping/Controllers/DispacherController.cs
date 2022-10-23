using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Controllers
{
    public class DispacherController : BasePublicController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DispacherController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            string host = _httpContextAccessor.HttpContext.Request.Host.Host.ToLower();
            if (host.Contains("postbar") || host.Contains("postbaar"))
                return RedirectToRoute("PostbarHome");
            else if (_httpContextAccessor.HttpContext.Request.Host.Host.ToLower().Contains("shipito"))
                return RedirectToRoute("_ShipitoHome");
            return RedirectToRoute("_ShipitoHome");
        }
    }
}
