using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Nop.Plugin.Misc.PrintedReports_Requirements.Models.Search;
using Nop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nop.Plugin.Misc.PrintedReports_Requirements.Controllers.ManageAvatarCustomerController;

namespace Nop.Plugin.Misc.PrintedReports_Requirements.Controllers
{
    public class ManageRouteProviderController : BasePublicController
    {
        public ManageRouteProviderController()
        {
        }

        public object ManageAvatarCustomer { get; private set; }

        public virtual IActionResult GetCustomer(int Id)
        {
            //Search_Customer search_Customer = new Search_Customer();
            //search_Customer.ActiveSearch = true;
            //search_Customer.SearchId = Id;
            //TempData["_search_Customer"] = JsonConvert.SerializeObject(search_Customer);
            return RedirectToAction("RelationController", "ManageAvatarCustomer", new { Id = Id, Area = "Admin" });
            //return RedirectToAction("RelationController", new RouteValueDictionary(
            //   new { controller = "Admin/ManageAvatarCustomer", action = "RelationController", Id = Id }));
        }
    }
}
