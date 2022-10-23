using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Controllers;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Newtonsoft.Json;
using Nop.Plugin.Widgets.ShipmentTracking.Models;
using Microsoft.AspNetCore.Http;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.ShipmentTracking.Controller
{
    public class ShipmentTrackingController : BaseController
    {
        private readonly IShipmentTrackingService _shipmentTrackingService;
        private readonly IWorkContext _workContext;
        public ShipmentTrackingController(IShipmentTrackingService shipmentTrackingService, IWorkContext workContext)
        {
            _shipmentTrackingService = shipmentTrackingService;
            _workContext = workContext;
        }

        [HttpPost]
        public IActionResult GetTrackingResult( string trackingNumber, int orderId, string MobileNo, int IdServiceProvider)
        {
            return _GetTrackingResult(trackingNumber,  orderId,  MobileNo, IdServiceProvider);

        }

        private IActionResult _GetTrackingResult(  string trackingNumber,  int orderId,  string MobileNo, int IdServiceProvider)
        {
            //if (!string.IsNullOrEmpty(TrackingAllInput))
            //{
            //    if (TrackingAllInput.Length == 11 && TrackingAllInput.StartsWith("09"))
            //    {
            //        MobileNo = TrackingAllInput;
            //        orderId = 0;
            //        trackingNumber = null;
            //    }
            //    else if (TrackingAllInput.Length >= 8)
            //    {
            //        MobileNo = null;
            //        orderId = 0;
            //        trackingNumber = TrackingAllInput;
            //    }
            //    else if (TrackingAllInput.Length <= 7)
            //    {
            //        MobileNo = null;
            //        orderId = int.Parse(TrackingAllInput);
            //        trackingNumber = null;
            //    }
            //    else
            //    {
            //        return Json(new { message = "اطلاعات وارد شده جهت رهگیری نامعتبر می باشد" });
            //    }
            //}
            if (string.IsNullOrEmpty(trackingNumber))
            {
                if (!_workContext.CurrentCustomer.IsRegistered())
                {
                    return Json(new { message = "برای رهگیری مرسوله پستی با شماره سفارش و یا شماره تلفن همراه ابتدا باید وارد حساب کاربری خود در سامانه شوید" });
                }
            }
            string msg = "";
            var data = _shipmentTrackingService.getLastShipmentTracking(trackingNumber, orderId, MobileNo, _workContext.CurrentCustomer.Id, IdServiceProvider, out msg);
            if (msg == "404")
            {
                return Json(new { message = "کاربر گرامی لطفا سرویس پستی خود را مشخص بفرمایید", State = 404 });
            }
            if (data == null)
                return Json(new { message = "اطلاعات رهگیری دریافتی یافت نشد" });
            if (data != null && data.Count > 0)
            {
                if (data.Count == 0)
                    return Json(new { message = "اطلاعات رهگیری دریافتی یافت نشد" });
                for (int i = 0; i < data.Count; i++)
                {
                    data[i].Id = i;
                }
                return Json(new { data = data, State = 200 });
            }
            else
            {
                return Json(new { Success = false, Message = "خطایی در رهگیری مرسوله اتفاق افتاده است، با پشتیبانی تماس حاصل فرمایید" });
            }
        }

        [HttpPost]
        [ValidateCaptcha]
        public IActionResult GetTrackingResultLanding(string TrackingAllInput, string trackingNumber, int orderId, string MobileNo, int IdServiceProvider, bool captchaValid)
        {
            if (!captchaValid)
            {
                var session = HttpContext.Session.GetString("TrackingRequestData");
                if (!string.IsNullOrEmpty(session))
                {
                    var requestData = JsonConvert.DeserializeObject<List<TrackingRequestData>>(session);
                    var fiveMin = DateTime.Now.AddMinutes(-2);
                    if (requestData.Count(p => p.RequestDateTime >= fiveMin) >= 5)
                    {
                        AddNewRowToSession(requestData);
                        return Json(new { message = "کپچا نامعتبر است", reload = true });
                    }
                    AddNewRowToSession(requestData);
                }
                else
                {
                    var requestData = new List<TrackingRequestData>();
                    AddNewRowToSession(requestData);
                }
            }

            return _GetTrackingResult(trackingNumber,  orderId,  MobileNo, IdServiceProvider);
        }

        private void AddNewRowToSession(List<TrackingRequestData> requestData)
        {
            requestData.Add(new TrackingRequestData()
            {
                RequestDateTime = DateTime.Now
            });
            HttpContext.Session.SetString("TrackingRequestData", JsonConvert.SerializeObject(requestData));
        }


        [HttpPost]
        public IActionResult getOrderShipment(int orderId)
        {
            return Json(_shipmentTrackingService.getAllShipmentByOrderId(orderId));
        }
        [HttpPost]
        public IActionResult GetshipmentTrackingDetails(string TrackingNumber, int OrderId, int ShipmentId)
        {
            string error = "";
            var resul = _shipmentTrackingService.getShipmentDetails(TrackingNumber, OrderId, ShipmentId, out error);

            return Json(new { success = string.IsNullOrEmpty(error), message = error, data = resul });
        }

        [HttpGet]
        public IActionResult ShipmentTrackingFrame()
        {
            //return Content("سرویس رهگیری در حال حاضر درسترسی نیست");
            return View("~/Plugins/Widgets.ShipmentTracking/Views/Landing_TrackingInfo.cshtml");
        }
    }
}
