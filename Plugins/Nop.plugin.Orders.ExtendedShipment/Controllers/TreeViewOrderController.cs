using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Controllers
{

    public class TreeViewOrderController : BaseAdminController
    {
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerService _customerService;
        private readonly ICategoryService _CategoryService;
        private readonly IStateProvinceService _StateProvinceService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICountryService _CountryService;
        public TreeViewOrderController
            (
             IWorkContext workContext,
            IDbContext dbContext,
            IPermissionService permissionService,
            ICategoryService CategoryService,
            IStaticCacheManager cacheManager,
            IStateProvinceService StateProvinceService,
             ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ICustomerService customerService,
            ICountryService CountryService

            )
        {
            _workContext = workContext;
            _dbContext = dbContext;
            _permissionService = permissionService;
            _CategoryService = CategoryService;
            _cacheManager = cacheManager;
            _StateProvinceService = StateProvinceService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _customerService = customerService;
            _CountryService = CountryService;
        }

        //[Produces("application/json")]
        //[Route("api/TreeViewOrder")]
        [HttpGet]
        [Route("DataSource")]
        [Area(AreaNames.Admin)]
        public JsonResult GetCountry_user_order()
        {
            var obj = new[]
           {
               new {
               name = " اصفهان",
               value = 1,
               items = new[] {
               new {
                   name = " شاهین شهر",
                   value = 2,
                   items = new dynamic [] {
                       new {
                           name = " مشتریان",
                           value=3,
                           items= new[]
                           {
                               new {
                                name = " احمد",
                                value=4,
                                    },
                                new {
                                name = " حسین",
                                value=5,
                                    }
                           }

                       }

                       },
                   },


           },
               },
           };
            return Json(obj);
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
            //    return AccessDeniedView();
            //var DataJson = new List<Result_Json_JSTree>();
            //var CountyAll = _CountryService.GetAllCountries();
            //if (CountyAll.Count > 0)
            //{
            //    foreach (var item in CountyAll)
            //    {
            //        Result_Json_JSTree temp = new Result_Json_JSTree();
            //        temp.id = item.Id;
            //        temp.text = item.Name;
            //        temp.icon =  @"< i class='fa fa-navicon'></i>";
            //        temp.children = new List<Child>();
            //        DataJson.Add(temp);
            //    }
            //}
            //return Json(DataJson);
        }
    }
}
