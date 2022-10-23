using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Controllers
{
    [AdminAntiForgery(true)]
    public class ManageCancelOrderReasonController : BaseAdminController
    {
        private readonly IRepository<Tbl_CancelReason_Order> _repositoryTbl_CancelReason_Order;
        private readonly IRepository<Tbl_CancellReason> _repositoryTbl_CancellReason;

        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly IRepository<Order> _repository_Order;
        private readonly IOrderStatusTrackingService _orderStatusTrackingService;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly Services.Tozico.ITozicoService _tozicoService;
        private readonly ICacheManager _cacheManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomAuthenticationService _customAuthenticationService;

        public ManageCancelOrderReasonController
            (
            IRepository<Tbl_CancelReason_Order> repositoryTbl_CancelReason_Order,
            IRepository<Tbl_CancellReason> repositoryTbl_CancellReason,
            IWorkContext workContext,
            IDbContext dbContext,
            IPermissionService permissionService,
            ICustomerService customerService,
            IHttpContextAccessor httpContextAccessor,
            IOrderService orderService,
            IRepository<Order> repository_Order,
            IOrderStatusTrackingService orderStatusTrackingService,
            IExtendedShipmentService extendedShipmentService,
            Services.Tozico.ITozicoService tozicoService,
            ICustomAuthenticationService customAuthenticationService
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _repositoryTbl_CancellReason = repositoryTbl_CancellReason;
            _repositoryTbl_CancellReason = repositoryTbl_CancellReason;
            _repositoryTbl_CancelReason_Order = repositoryTbl_CancelReason_Order;
            _workContext = workContext;
            _dbContext = dbContext;
            _permissionService = permissionService;
            _customerService = customerService;
            _orderService = orderService;
            _repository_Order = repository_Order;
            _orderStatusTrackingService = orderStatusTrackingService;
            _extendedShipmentService = extendedShipmentService;
            _tozicoService = tozicoService;
            _customAuthenticationService = customAuthenticationService;
        }

        [Area(AreaNames.Admin)]
        [AdminAntiForgery]
        [AuthorizeAdmin]
        [HttpPost]
        public IActionResult PreCancelOrder(int IdOrder, int idDescription, List<CancelOrderCostInfo> cancelOrderCostInfos)
        {
            try
            {
                var order = _orderService.GetOrderById(IdOrder);
                if (order.Shipments.Any(p => !string.IsNullOrEmpty(p.TrackingNumber)))
                {
                    //string SecurtyCode = _httpContextAccessor.HttpContext.Session.GetString("secCod");
                    ////string SecurtyCode = _cacheManager.Get<string>("secCod_" + _workContext.CurrentCustomer.Username);
                    //if (string.IsNullOrEmpty(SecurtyCode))
                   // if (!_customAuthenticationService.IsSupperAdmin())
                   if(_workContext.CurrentCustomer.Id != 6046671)
                        return Json(new { success = true, message = "سفارش مورد نظر دارای بارکد پستی می باشد و امکان کنسل کردن آن با توجه به دسترسی شما وجود ندارد" });
                }
                string RefoundItem = "";
                if (cancelOrderCostInfos != null && cancelOrderCostInfos.Any())
                    RefoundItem = Newtonsoft.Json.JsonConvert.SerializeObject(cancelOrderCostInfos); ;
                Tbl_CancelReason_Order temp = new Tbl_CancelReason_Order();
                temp.id_Description = idDescription;
                temp.OrderId = IdOrder;
                temp.DateInsert = DateTime.Now;
                temp.IdUserInsert = _workContext.CurrentCustomer.Id;
                temp.RefoundItem = RefoundItem;
                _repositoryTbl_CancelReason_Order.Insert(temp);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { success = false, status = 400 });
            }
        }


        [Area(AreaNames.Admin)]
        [AdminAntiForgery]
        [AuthorizeAdmin]
        public IActionResult GetListPatternAnswer()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();//login
            var listdis = _repositoryTbl_CancellReason.Table.Where(p => p.IsActive).Select(p => new
            {
                Value = p.Id,
                Text = p.Description
            }).ToList();
            return Json(listdis);

        }


        public IActionResult SetOrderToComplite(int orderId)
        {
            var isOrderCanceled = _repository_Order.Table.Any(p => p.Id == orderId && p.OrderStatusId == 40);
            if (isOrderCanceled)
            {
                var query = $"update [Order] set OrderStatusId = 30 where Id = {orderId}";
                _dbContext.ExecuteSqlCommand(query);
                _orderStatusTrackingService.Insert(orderId, 30);
                return Ok(true);
            }
            return Ok(false);
        }
    }
}
