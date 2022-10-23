using BS.Plugin.NopStation.MobileWebApi.Models.Params_SnappBox_API;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.Services.Logging;
using Nop.Services.Shipping;
using Nop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{

    public class SnappBoxController : BasePublicController
    {
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly IShipmentService _shipmentService;
        private readonly ISnapboxWebhook _snapboxWebhook;
        private readonly IRepository<Nop.plugin.Orders.ExtendedShipment.Models.Tbl_Collection_Snappbaox_Tarof> _repositoryTbl_Collection_Snappbaox_Tarof;
        public SnappBoxController
            (
            ILogger logger, IWorkContext workContext,
            IShipmentService shipmentService,
            IRepository<Nop.plugin.Orders.ExtendedShipment.Models.Tbl_Collection_Snappbaox_Tarof> repositoryTbl_Collection_Snappbaox_Tarof,
            ISnapboxWebhook snapboxWebhook
            )
        {
            _snapboxWebhook = snapboxWebhook;
            _logger = logger;
            _workContext = workContext;
            _shipmentService = shipmentService;
            _repositoryTbl_Collection_Snappbaox_Tarof = repositoryTbl_Collection_Snappbaox_Tarof;
        }

        //1
        [Route("api/SnappBox/Webhook")]
        [HttpPost]
        public IActionResult Webhook([FromBody]Params_SnappBox_Webhok input)
        {
            try
            {
                //string webhookString = Newtonsoft.Json.JsonConvert.SerializeObject(json);
                string webhookString = Newtonsoft.Json.JsonConvert.SerializeObject(input);
                _logger.InsertLog(Nop.Core.Domain.Logging.LogLevel.Information, "مدل فراخوانی شده در وب هوک اسنپ", webhookString);
                if(!input.customerRefId.HasValue)
                {
                    return Json(new { State = false });
                }
                int orderid = Convert.ToInt32(input.customerRefId);
                if (orderid > 0)
                {
                    switch (input.orderStatus)
                    {
                        case "ARRIVIED_AT_PICKUP":
                            _snapboxWebhook.SnapboxARRIVIED(orderid, input.bikerPhone);
                            break;
                        case "CANCELLED":
                            _snapboxWebhook.SnapboxBikerCancel(orderid);
                            break;

                        case "ACCEPTED":
                            {
                                _snapboxWebhook.SnapBoxAccepted(orderid, input.orderId, input.bikerPhone, input.bikerName);
                                break;
                            }
                        case "PICKED_UP":
                            {
                                _snapboxWebhook.SnapboxPickup(orderid, input.bikerPhone);
                                break;
                            }
                        case "DELIVERED":
                            {
                                _snapboxWebhook.SnapboxDeliver(orderid);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.InsertLog(Nop.Core.Domain.Logging.LogLevel.Error, "خطا در وب هوک اسنپ", "");
                LogException(ex);
            }






            bool State = true;
            return Json(new { State = State });
        }
    }
}
