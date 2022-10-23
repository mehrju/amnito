using Microsoft.AspNetCore.Mvc;
using Nop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Controllers
{
    public class ManageRouteProviderController : BasePublicController
    {
        public ManageRouteProviderController()
        {
        }
        public virtual IActionResult GetCOD(int Id)
        {
            //Search_Customer search_Customer = new Search_Customer();
            //search_Customer.ActiveSearch = true;
            //search_Customer.SearchId = Id;
            //TempData["_search_Customer"] = JsonConvert.SerializeObject(search_Customer);
            return RedirectToAction("RelationController", "ManageRequestCOD", new { Id = Id, Area = "Admin" });
            //return RedirectToAction("RelationController", new RouteValueDictionary(
            //   new { controller = "Admin/ManageAvatarCustomer", action = "RelationController", Id = Id }));
        }
        public virtual IActionResult GetDamages(int Id)
        {
            return RedirectToAction("Edit", "ManageDamages", new {  Id, Area = "Admin" });


        }
    }
}
