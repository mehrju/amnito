using BS.Plugin.NopStation.MobileWebApi.Models.PhoneOrder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.plugin.Orders.ExtendedShipment.Enums;
using Nop.plugin.Orders.ExtendedShipment.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    public class PhoneOrderController : BaseApiController
    {
        public PhoneOrderController()
        {

        }


        public IActionResult GetPhoneOrderStatus()
        {
            var enumList = Utility.GetEnumList<OrderShipmentStatusEnum>();
            return Ok(enumList.Select(p => new { Text = p.DisplayName, Value = p.Value }));
        }

        //[HttpPost]
        //public IActionResult PhoneOrderCollectInfo(PhoneOrderCollectInfo model, IFormCollection formCollection)
        //{

        //}

    }
}
