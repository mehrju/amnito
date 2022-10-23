using Microsoft.AspNetCore.Mvc;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Controllers
{
    public class gatewayShopController : BaseAdminController
    {
        private readonly IDbContext _dbContext;
        private readonly IStoreService _storeService;
        private readonly ICodService _codService;
        public gatewayShopController(IDbContext dbContext
            , IStoreService storeService
            , ICodService codService)
        {
            _storeService = storeService;
            _dbContext = dbContext;
            _codService = codService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Area(AreaNames.Admin)]
        [AuthorizeAdmin]
        public IActionResult getgetwayShopList(DataSourceRequest command)
        {
            var gateway_result = _codService.GetShopList();
            var orderedData = gateway_result.OrderByDescending(p => p.ShopID).ToList();
            if (gateway_result != null)
            {
                var paged_Data = orderedData.Skip((command.Page - 1) * command.PageSize).Take(command.PageSize).ToList();
                var gridModel = new DataSourceResult
                {
                    Data = paged_Data.Select(x =>
                {
                    var store = _storeService.GetStoreById(5);
                    return new
                    {
                        x.ShopID,
                        x.ShopName,
                        x.ShopUsername,
                        x.ShopStatus
                    };
                }),
                    Total = (gateway_result.Any() ? gateway_result.Count : 0)
                };
                return Json(gridModel);
            }
            return Json(new DataSourceResult());
        }
    }
    public class ShopinfoResult
    {
        public List<ShopInfo> ShopInfos { get; set; }
        public string Message { get; set; }
        public bool UnhandledMessage { get; set; }
        public int ReposnceStatusCode { get; set; }
    }
    public class ShopInfo
    {
        public int ShopID { get; set; }
        public int ShopStatusCode { get; set; }
        public string ShopStatus { get; set; }
        public string ShopName { get; set; }
        public string ShopUsername { get; set; }
    }
}
