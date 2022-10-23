using Microsoft.AspNetCore.Mvc;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Models.NotificationModel;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Kendoui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Controllers
{
    public class NotifConfigController : BaseAdminController
    {
        private readonly INotificationService _notificationService;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IStoreService _storeService;

        public NotifConfigController(INotificationService notificationService, IExtendedShipmentService extendedShipmentService, IStoreService storeService)
        {
            _notificationService = notificationService;
            _extendedShipmentService = extendedShipmentService;
            _storeService = storeService;
        }
        public IActionResult Index()
        {
            List<PostBarcodeGeneratorOutputModel> items = new List<PostBarcodeGeneratorOutputModel>();
            var data1 = _extendedShipmentService.NewGenerateBarcodeFromPost(501273, 4, 802041);//سفارشی غیر همجوار
            items.Add(data1);
            //var data2 = _extendedShipmentService.NewGenerateBarcodeFromPost(421825, 4, 795406);//سفارشی همجوار
            //items.Add(data2);
            //var data3 = _extendedShipmentService.NewGenerateBarcodeFromPost(383346, 4, 722248);//سفارشی درون استانی
            //items.Add(data3);

            //var data4 = _extendedShipmentService.NewGenerateBarcodeFromPost(375906, 4, 720918);//پیشتاز غیر همجوار
            //items.Add(data4);
            //var data5 = _extendedShipmentService.NewGenerateBarcodeFromPost(410693, 4, 777655);//پیشتاز همجوار
            //items.Add(data5);
            //var data6 = _extendedShipmentService.NewGenerateBarcodeFromPost(383345, 4, 722248);//پیشتاز درون استانی
            //items.Add(data6);

            //var data7 = _extendedShipmentService.NewGenerateBarcodeFromPost(494519, 4, 799785);//ویژه غیر همجوار
            //items.Add(data7);
            //var data8 = _extendedShipmentService.NewGenerateBarcodeFromPost(409008, 4, 774900);//ویژه همجوار
            //items.Add(data8);
            //var data9 = _extendedShipmentService.NewGenerateBarcodeFromPost(496763, 4, 917943);//ویژه درون استانی
            //items.Add(data9);
            string data = Newtonsoft.Json.JsonConvert.SerializeObject(items);
            common.Log("بارکد های جدید", data);
            return View("/Plugins/Orders.ExtendedShipment/Views/NotifConfiguration/NotifConfig.cshtml");
        }
        [HttpPost]
        public IActionResult getNotifItemsList(DataSourceRequest command, int NotifTypeId)
        {
            var data = _notificationService.getNotifItem(NotifTypeId);
            var gridModel = new DataSourceResult
            {
                Data = data,
                Total = data.Count
            };
            return Json(gridModel);
        }

        [HttpGet]
        public IActionResult GetPopupNotifications()
        {
            var data = _notificationService.GetPopupNotifications(0, int.MaxValue);
            var gridModel = new DataSourceResult
            {
                Data = data.Select(p => new PopupNotificationModel()
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    FromDate = p.FromDate,
                    IsActive = p.IsActive,
                    ToDate = p.ToDate
                }).ToList(),
                Total = data.Count
            };
            return Json(gridModel);
        }

        public IActionResult GetPopupNotification(int id)
        {
            PopupNotificationModel model = _notificationService.GetPopupNotificationById(id);
            if(model == null)
            {
                return BadRequest();
            }
            return Ok(model);
        }

        [HttpPost]
        public IActionResult SavePopupNotification(PopupNotificationModel model)
        {
            _notificationService.SavePopupNotification(model);
            return Ok();
        }
        [HttpPost]
        public IActionResult SaveNotifItems(NotifItemsModel model)
        {
            string StrMsg = "";
            if (_notificationService.SaveNotifItems(model, out StrMsg))
            {
                return Json(new { message = "درج با موفقیت انجام شد", success = true });
            }
            return Json(new { message = StrMsg, success = false });
        }
        public IActionResult getNotifTitleList()
        {
            var list = _notificationService.getNofitTitleList();
            return Json(list);
        }
        [HttpPost]
        public IActionResult UpdateNotifTitleActiveStatus(int[] NotifTitleIds)
        {
            _notificationService.updateNotifTitle(NotifTitleIds.ToList());
            return Json(new { success = true, message = "به روز رسانی انجام شد" });
        }

        public IActionResult GetAllStores()
        {
            var stores = _storeService.GetAllStores();
            return Ok(stores.Select(p => new { Text = p.Name, Value = p.Id }));
        }
    }
}
