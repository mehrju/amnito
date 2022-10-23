using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Orders.BulkOrder.Models;
using Nop.Plugin.Orders.BulkOrder.Services;
using Nop.Plugin.Orders.MultiShipping.Services;
using Nop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.BulkOrder.Controllers
{
    public class OrderPriceController : BasePublicController
    {
        private readonly INewCheckout _newCheckout;
        private readonly IBulkOrderService _bulkOrderService;

        public OrderPriceController(INewCheckout newCheckout,
            IBulkOrderService bulkOrderService)
        {
            _newCheckout = newCheckout;
            _bulkOrderService = bulkOrderService;
        }

        [HttpGet("[controller]/index")]
        public IActionResult QuickOrderPrice()
        {
            return View("~/Plugins/Orders.BulkOrder/Views/QuickOrderPricePage.cshtml");
        }

        [HttpPost]
        public IActionResult QuickOrderPrice([FromForm]OrderPriceInput orderPriceInput)
        {
            //if (!orderPriceInput.IsValid(out string msg))
            //{
            //    return BadRequest();
            //}
            if (orderPriceInput.SenderCityId == 0 || orderPriceInput.Weight == 0)
            {
                return BadRequest();
            }
            if (string.IsNullOrEmpty(orderPriceInput.ReceiverCountry) && orderPriceInput.ReceiverCityId == 0)
            {
                return BadRequest();
            }
            if (!string.IsNullOrEmpty(orderPriceInput.ReceiverCountry))
            {
                orderPriceInput.ReceiverCityId = 0;
            }
            //if (!string.IsNullOrEmpty(orderPriceInput.PackingDimension.CartonName))
            //{
            //    var dimensions = _newCheckout.getDimentionByName(orderPriceInput.PackingDimension.CartonName);
            //    orderPriceInput.PackingDimension.Width = dimensions.Width;
            //    orderPriceInput.PackingDimension.Height = dimensions.Height;
            //    orderPriceInput.PackingDimension.Length = dimensions.Length;
            //}

            //var priceModel = _bulkOrderService.GetCheckoutAttributePriceSeparately(orderPriceInput.Weight,
            //    orderPriceInput.InsuranceName,
            //    orderPriceInput.PackingDimension.Length,
            //    orderPriceInput.PackingDimension.Width,
            //    orderPriceInput.PackingDimension.Height,
            //    false,
            //    orderPriceInput.PrintBill,
            //    orderPriceInput.SendSms,
            //    orderPriceInput.PrintLogo,
            //    orderPriceInput.NeedCartoon,
            //    0,
            //    orderPriceInput.SenderCityId,
            //    orderPriceInput.ReceiverCityId,
            //    0);
            if (string.IsNullOrEmpty(orderPriceInput.InsuranceName))
            {
                orderPriceInput.InsuranceName = "غرامت تا سقف 300 هزار تومان";
            }
            var foreignId = string.IsNullOrEmpty(orderPriceInput.ReceiverCountry) ? "0" : orderPriceInput.ReceiverCountry.Split('|')[0];
            var priceModel = _bulkOrderService.GetCheckoutAttributePriceSeparately(orderPriceInput.Weight,
                orderPriceInput.InsuranceName,
                22,
                11,
                2,
                false,
                orderPriceInput.PrintBill,
                orderPriceInput.SendSms,
                orderPriceInput.PrintLogo,
                orderPriceInput.NeedCartoon,
                0,
                orderPriceInput.SenderCityId,
                orderPriceInput.ReceiverCityId,
                0,
                Convert.ToInt32(foreignId));

            return Ok(priceModel);
        }

    }
}
