using BS.Plugin.Orders.ExtendedShipment.Services;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.plugin.Orders.ExtendedShipment.Models.PhoneOrder;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.plugin.Orders.ExtendedShipment.Services.PhoneOrder;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;

namespace Nop.plugin.Orders.ExtendedShipment.Controllers
{
    [AdminAntiForgery(true)]
    public class ManagePhoneOrderController : BaseAdminController
    {
        private readonly IPhoneOrderService _phoneOrderService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly INotificationService _notificationService;
        private readonly IOptimeApiService _optimeApiService;
        private readonly ICustomerService _customerService;
        //private readonly IApService _apService;

        public ManagePhoneOrderController(IPhoneOrderService phoneOrderService,
            IStateProvinceService stateProvinceService,
            INotificationService notificationService,
            IOptimeApiService optimeApiService,
            ICustomerService customerService
           // IApService apService
            )
        {
            _customerService = customerService;
            _phoneOrderService = phoneOrderService;
            _stateProvinceService = stateProvinceService;
            _notificationService = notificationService;
            _optimeApiService = optimeApiService;
           // _apService = apService;
        }

        [HttpGet]
        public virtual IActionResult Index()
        {
            return View("/Plugins/Orders.ExtendedShipment/Views/PhoneOrder/Index.cshtml", new PhoneOrderModel());
        }

        //[HttpPost]
        public IActionResult Edit(int id)
        {
            var phoneOrder = _phoneOrderService.GetPhoneOrderById(id);
            if (phoneOrder == null)
            {
                return View("Index");
            }
            var city = _stateProvinceService.GetStateProvinceById(phoneOrder.CityId);
            return View("/Plugins/Orders.ExtendedShipment/Views/PhoneOrder/Index.cshtml", new PhoneOrderModel()
            {
                Address = phoneOrder.Address,
                CityId = phoneOrder.CityId,
                FirstName = phoneOrder.FirstName,
                IsCarRequired = phoneOrder.IsCarRequired,
                LastName = phoneOrder.LastName,
                Latitude = phoneOrder.Latitude,
                Longitude = phoneOrder.Longitude,
                PhoneNumber = phoneOrder.PhoneNumber,
                PostalCode = phoneOrder.PostalCode,
                TellNumber = phoneOrder.TellNumber,
                Id = phoneOrder.Id,
                StateId = city.CountryId,
                ServiceId = phoneOrder.ServiceId
            });
        }

        [HttpPost("Post")]
        public IActionResult Post(PhoneOrderModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("/Plugins/Orders.ExtendedShipment/Views/PhoneOrder/Index.cshtml", model);
            }
            int? collectorId = _phoneOrderService.CanRegisterPhoneOrder(model.CityId);
            if (!collectorId.HasValue || collectorId.Value == 0)
            {
                model.Error = "در این شهر نماینده ای وجود ندارد و امکان ثبت سفارش تلفنی وجود ندارد";
                return View("/Plugins/Orders.ExtendedShipment/Views/PhoneOrder/Index.cshtml", model);
            }
            model.Error = "";
            var _customer = _customerService.GetCustomerByUsername(model.PhoneNumber);
            if (_customer == null || !_customer.IsRegistered())
            {
                string msg = "";
                //_customer = _apService.RegisterUnknown(model.PhoneNumber, out msg, 0, true, true);
            }
            model.RelatedCustomerId = _customer.Id;
            _phoneOrderService.InsertPhoneOrder(model);

            if (!model.HasLocation)
            {
                var planResponse = _optimeApiService.NewFakePlanForPhoneOrder(collectorId.Value, new BS.Plugin.Orders.ExtendedShipment.Models.Optime.ToolApiCall.CallRequestModel()
                {
                    Owner = "postex",
                    PlanName = model.FullName,
                    PlanConfigDto = new BS.Plugin.Orders.ExtendedShipment.Models.Optime.ToolApiCall.PlanConfigDto()
                    {
                        Config = new List<BS.Plugin.Orders.ExtendedShipment.Models.Optime.ToolApiCall.Config>()
                    {
                        new BS.Plugin.Orders.ExtendedShipment.Models.Optime.ToolApiCall.Config()
                        {
                            Name = "postex",
                            Volume = 2,
                            Weight = 2
                        }
                    },
                        Option = new List<BS.Plugin.Orders.ExtendedShipment.Models.Optime.ToolApiCall.Option>()
                    {
                        new BS.Plugin.Orders.ExtendedShipment.Models.Optime.ToolApiCall.Option()
                    }
                    },
                    FileContent = new List<BS.Plugin.Orders.ExtendedShipment.Models.Optime.ToolApiCall.FileContent>()
                    {
                        new BS.Plugin.Orders.ExtendedShipment.Models.Optime.ToolApiCall.FileContent()
                        {
                            Address = model.Address,
                            CustomerName = model.FullName,
                            CustomerPhoneNumber = model.PhoneNumber,
                            //model sabt shode va id por shode ast.
                            Id = model.Id.ToString()
                        }
                    }
                }, model.Id);
            }

            _notificationService.sendSms($"مشترک گرامی، سفارش تلفنی شما ثبت گردید{Environment.NewLine}منتظر جمع آوری مرسوله خود توسط نماینده شرکت باشید{Environment.NewLine}با سپاس{Environment.NewLine}امنیتو ", model.PhoneNumber);

            return RedirectToAction("List");
        }

        [HttpPost("Put")]
        public IActionResult Put(PhoneOrderModel model)
        {
            var phoneOrder = _phoneOrderService.GetPhoneOrderById(model.Id);


            phoneOrder.Address = model.Address;
            phoneOrder.CityId = model.CityId;
            phoneOrder.FirstName = model.FirstName;
            phoneOrder.IsCarRequired = model.IsCarRequired;
            phoneOrder.LastName = model.LastName;
            phoneOrder.Latitude = model.Latitude;
            phoneOrder.Longitude = model.Longitude;
            phoneOrder.PhoneNumber = model.PhoneNumber;
            phoneOrder.PostalCode = model.PostalCode;
            phoneOrder.TellNumber = model.TellNumber;
            phoneOrder.ServiceId = model.ServiceId;

            _phoneOrderService.UpdatePhoneOrder(phoneOrder);

            return RedirectToAction("List");
        }

        public IActionResult List()
        {
            return View("/Plugins/Orders.ExtendedShipment/Views/PhoneOrder/List.cshtml");
        }

        public IActionResult GetList([FromQuery] string fromDate, [FromQuery] string toDate, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var phoneOrders = _phoneOrderService.GetPagedPhoneOrders(fromDate, toDate, pageIndex, pageSize);
            var gridModel = new DataSourceResult
            {
                Data = phoneOrders,
                Total = phoneOrders.Count
            };
            return Ok(gridModel);
        }

    }
}
