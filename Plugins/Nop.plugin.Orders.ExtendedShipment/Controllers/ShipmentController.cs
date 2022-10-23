using Microsoft.AspNetCore.Mvc;
using Nop.Data;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Kendoui;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Controllers
{
    public class ShipmentsController: BaseAdminController
    {
        private readonly IDbContext _dbContext;
        private readonly IStoreService _storeService;
        public ShipmentsController(IDbContext dbContext
            , IStoreService storeService)
        {
            _storeService = storeService;
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult getShipmentList(DataSourceRequest command)
        {
            string query = "EXEC dbo.Sp_Shipments @pageIndex ,@PageSize";
            SqlParameter P_pageIndex = new SqlParameter()
            {
                ParameterName = "pageIndex",
                SqlDbType = SqlDbType.Int,
                Value = (object)command.Page
            };
            SqlParameter P_PageSize = new SqlParameter()
            {
                ParameterName = "PageSize",
                SqlDbType = SqlDbType.Int,
                Value = (object)command.PageSize
            };
            List<SqlParameter> Lst_prm = new List<SqlParameter>() { P_pageIndex, P_PageSize };
            var shipments= _dbContext.SqlQuery<ShipmentsLsit>(query, Lst_prm).ToList();
            var gridModel = new DataSourceResult
            {
                Data = shipments.Select(x =>
                {
                    var store = _storeService.GetStoreById(x.StoreId);
                    return new
                    {
                        x.OrderId,
                        x.Id,
                        x.RegisterUser,
                        x.SenderFullName,
                        x.PhoneNumber,
                        x.Address1,
                        x.TrackingNumber,
                        x.LastShipmentState,
                        x.LastShipmentStateDate,
                        x.CreateDateTime_Sh,
                        x.OrderStatusId,
                        x.ServiceName,
                        x.IsFreePost,
                        x.NeedCarton,
                        x.NeedPrint,
                        x.CartonSizeName

                    };
                }),
                Total = (shipments.Any()?shipments.First().Total:0)
            };
            return Json(gridModel);
        }
    }
    public class ShipmentsLsit
    {
        public int OrderId { get; set; }
        public int StoreId { get; set; }
        public int Id { get; set; }
        public string RegisterUser { get; set; }
        public string SenderFullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address1 { get; set; }
        public string TrackingNumber { get; set; }
        public string LastShipmentState { get; set; }
        public string LastShipmentStateDate { get; set; }
        public string CreateDateTime_Sh { get; set; }
        public int OrderStatusId { get; set; }
        public string ServiceName { get; set; }
        public bool IsFreePost { get; set; }
        public bool NeedCarton { get; set; }
        public bool NeedPrint { get; set; }
        public string CartonSizeName { get; set; }
        public int Total { get; set; }
    }
}
