using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Http.Extensions;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Plugin.Orders.MultiShipping.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Web.Controllers;
using Nop.Web.Extensions;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using Nop.Web.Models.Checkout;
using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using Nop.Services.Helpers;
using Nop.Services.Security;
using Nop.Services.Shipping.Tracking;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Kendoui;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Orders.MultiShipping.Models.Extra_Status_Field_Shipment;
using Nop.Plugin.Orders.MultiShipping.Models.Tbl_Extra_Status_Field_Shipment;
using Nop.plugin.Orders.ExtendedShipment.Models;

namespace Nop.Plugin.Orders.MultiShipping.Controllers
{
    #region InternalClass

    public class ShipmentValues
    {
        public string ItemNumber { get; set; }
        public string ShipmentNumber { get; set; }
        public int QtyNumber { get; set; }
    }

    public class AddressValues
    {
        public string Address { get; set; }
        public string ShipmentNumber { get; set; }
    }

    public class ShippingMethodValues
    {
        public string ShippingMethod { get; set; }
        public string ShipmentNumber { get; set; }
        public string DeliveryDate { get; set; }
    }

    public class MultiShipmentInfo
    {
        public int ShipmentNumber { get; set; }
        public int orginalShipmentNumber { get; set; }
        public List<int> ShoppingCartIds { get; set; }
        public int? ShippingAddressId { get; set; }
        public string DeliveryDate { get; set; }
        public ShippingOption SelectedShippingOption { get; set; }
        public CheckoutShippingMethodModel ShippingMethod { get; set; }
    }

    #endregion

    public class MultiShippingCheckoutController : BasePublicController
    {
        #region Fields
        private readonly IRepository<TblExtraFiledShipment> _repositoryTblExtraFiledShipment;
        private readonly IRepository<Tbl_CancelReason_Order> _repositoryTbl_CancelReason_Order;
        private readonly IRepository<Tbl_CancellReason> _repositoryTbl_CancellReason;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly ICodService _codService;
        private readonly IAccountingService _accountingService;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IShippingService _shippingService;
        private readonly IPaymentService _paymentService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IWebHelper _webHelper;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IPermissionService _permissionService;
        private readonly OrderSettings _orderSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly AddressSettings _addressSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IAddressService _addressService;
        private readonly IExtnOrderProcessingService _extnOrderProcessingService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;
        private readonly IShipmentService _shipmentService;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IPackingRequestService _packingRequestService;
        private readonly INotificationService _notificationService;
        private readonly IOrderCostService _orderCostService;
        private readonly Nop.Data.IDbContext _dbContext;
        private readonly IAttributeEditorService _attributeEditorService;
        #endregion

        #region ctor

        public MultiShippingCheckoutController(
            IRepository<Tbl_CancelReason_Order> repositoryTbl_CancelReason_Order,
            IRepository<Tbl_CancellReason> repositoryTbl_CancellReason,
            IRepository<TblExtraFiledShipment> repositoryTblExtraFiledShipment,
            IRepository<OrderItem> orderItemRepository,
            IAccountingService accountingService,
            ICodService codService,
                IRepository<OrderNote> orderNoteRepository,
                IShipmentService shipmentService,
                ICheckoutModelFactory checkoutModelFactory,
                IWorkContext workContext,
                IStoreContext storeContext,
                IShoppingCartService shoppingCartService,
                ILocalizationService localizationService,
                IProductAttributeParser productAttributeParser,
                IProductService productService,
                IOrderProcessingService orderProcessingService,
                ICustomerService customerService,
                IGenericAttributeService genericAttributeService,
                ICountryService countryService,
                IStateProvinceService stateProvinceService,
                IShippingService shippingService,
                IPaymentService paymentService,
                IPluginFinder pluginFinder,
                ILogger logger,
                IOrderService orderService,
                IWebHelper webHelper,
                IAddressAttributeParser addressAttributeParser,
                IAddressAttributeService addressAttributeService,
                OrderSettings orderSettings,
                RewardPointsSettings rewardPointsSettings,
                PaymentSettings paymentSettings,
                ShippingSettings shippingSettings,
                AddressSettings addressSettings,
                CustomerSettings customerSettings,
                IAddressModelFactory addressModelFactory,
                IAddressService addressService,
                IExtnOrderProcessingService extnOrderProcessingService
                , IPermissionService permissionService
                , IDateTimeHelper dateTimeHelper
                , IMeasureService measureService
                , MeasureSettings measureSettings
                , IExtendedShipmentService extendedShipmentService,
                IPackingRequestService packingRequestService,
                INotificationService notificationService,
                IOrderCostService orderCostService,
               Nop.Data.IDbContext dbContext,
               IAttributeEditorService attributeEditorService
                )
        {
            _repositoryTbl_CancelReason_Order = repositoryTbl_CancelReason_Order;
            _repositoryTbl_CancellReason = repositoryTbl_CancellReason;
            _repositoryTblExtraFiledShipment = repositoryTblExtraFiledShipment;
            _extendedShipmentService = extendedShipmentService;
            _packingRequestService = packingRequestService;
            _notificationService = notificationService;
            _orderCostService = orderCostService;
            _dbContext = dbContext;
            _attributeEditorService = attributeEditorService;
            _accountingService = accountingService;
            _codService = codService;
            _orderNoteRepository = orderNoteRepository;
            _shipmentService = shipmentService;
            _dateTimeHelper = dateTimeHelper;
            _measureService = measureService;
            _measureSettings = measureSettings;
            _permissionService = permissionService;
            _extnOrderProcessingService = extnOrderProcessingService;
            _addressService = addressService;
            _customerSettings = customerSettings;
            _checkoutModelFactory = checkoutModelFactory;
            _workContext = workContext;
            _storeContext = storeContext;
            _shoppingCartService = shoppingCartService;
            _localizationService = localizationService;
            _productAttributeParser = productAttributeParser;
            _productService = productService;
            _orderProcessingService = orderProcessingService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _shippingService = shippingService;
            _paymentService = paymentService;
            _pluginFinder = pluginFinder;
            _logger = logger;
            _orderService = orderService;
            _webHelper = webHelper;
            _addressAttributeParser = addressAttributeParser;
            _addressAttributeService = addressAttributeService;

            _orderSettings = orderSettings;
            _rewardPointsSettings = rewardPointsSettings;
            _paymentSettings = paymentSettings;
            _shippingSettings = shippingSettings;
            _addressSettings = addressSettings;
            _customerSettings = customerSettings;
            _addressModelFactory = addressModelFactory;
            _orderItemRepository = orderItemRepository;
        }

        #endregion
        [Area(AreaNames.Admin)]
        [AdminAntiForgery]
        [AuthorizeAdmin]
        [HttpPost]
        public IActionResult getNewTrackingNumber(int shipmentId)
        {
            var shipment = _shipmentService.GetShipmentById(shipmentId);
            if (shipment == null)
            {
                return Json(new { success = false, message = "محموله مورد نظر یافت نشد" });
            }
            if (string.IsNullOrEmpty(shipment.TrackingNumber))
            {
                return Json(new { success = false, message = "محموله مورد نظر فاقد بارکد می باشد برای دریافت بارکد در تب اطلاعات در همین صفحه از کلید تغییر وضعیت استفاده کنید" });
            }

            if (shipment.ShippedDateUtc.HasValue || shipment.DeliveryDateUtc.HasValue)
            {
                return Json(new { success = false, message = "محموله مورد وارد فرآیند پستی شده و امکان تغییر بارکد وجود ندارد" });
            }
            //if (_extnOrderProcessingService.HasDateCollect(shipment))
            //{
            //    return Json(new { success = false, message = "برای محموله مورد نظر توسط پست اقدام برای جمع آوری صورت گرفته و امکان تغییر بارکد وجود ندارد. با مسئول فنی تماس بگیرید" });
            //}
            if (!_extnOrderProcessingService.InsertCanceledShipment(shipment))
            {
                return Json(new { success = false, message = "خطا در زمان ثبت کنسلی بارکد برای اطلاعات بیشتر به لاگ سیستم مراجعه کنید" });
            }


            var order = shipment.Order;
            CleanToSendDataToPostAgain(order);
            shipment.TrackingNumber = "";
            _shipmentService.UpdateShipment(shipment);
            order.OrderStatus = OrderStatus.Complete;
            _orderService.UpdateOrder(order);
            return Json(new { success = true, message = "عملیات با موفقیت انجام شد" });
        }
        private void CleanToSendDataToPostAgain(Order order)
        {
            var orderNotes = order.OrderNotes.FirstOrDefault(p => p.Note.Contains("SendDataToPost"));
            if (orderNotes != null)
                _orderNoteRepository.Delete(orderNotes);
        }
        [Area(AreaNames.Admin)]
        [AdminAntiForgery]
        [AuthorizeAdmin]
        [HttpPost]
        public IActionResult DeleteShipment(int shipmentId, int orderItemId, int idDescription)
        {
            var shipment = _shipmentService.GetShipmentById(shipmentId);
            if (shipment == null)
            {
                return Json(new { success = false, message = "محموله مورد نظر یافت نشد" });
            }


            var order = shipment.Order;
            if (order.Shipments.Count == 1)
            {
                return Json(new { success = false, message = "امکان حذف محموله وجود ندارد برای این مورد می بایست سفارش مورد نظر را کنسل کنید" });
            }
            var orderItem = _orderItemRepository.GetById(shipment.ShipmentItems.First().OrderItemId);
            var CATinFO = _extendedShipmentService.GetCategoryInfo(orderItem.Product);
            bool isCOD = CATinFO.IsCod;
            if (shipment.ShippedDateUtc.HasValue || shipment.DeliveryDateUtc.HasValue)
            {
                return Json(new { success = false, message = "محموله مورد وارد فرآیند پستی شده و امکان حذف محموله وجود ندارد" });
            }
            if (_extnOrderProcessingService.HasDateCollect(shipment))
            {
                return Json(new { success = false, message = "برای محموله مورد نظر توسط پست اقدام برای جمع آوری صورت گرفته و امکان حذف محموله وجود ندارد. با مسئول فنی تماس بگیرید" });
            }

            if (isCOD && (CATinFO.CategoryId == 667 || CATinFO.CategoryId == 670))
            {
                string resultMsg = "";
                if (!_codService.ChangeStatus(1, shipment.TrackingNumber, out resultMsg))
                {
                    return Json(new { success = false, message = resultMsg });
                }
            }

            if (!string.IsNullOrEmpty(shipment.TrackingNumber))
            {
                if (!_extnOrderProcessingService.InsertCanceledShipment(shipment))
                {
                    return Json(new { success = false, message = "خطا در زمان ثبت کنسلی بارکد برای اطلاعات بیشتر به لاگ سیستم مراجعه کنید" });
                }
            }
            if (shipment.Order.Customer.IsInCustomerRole("mini-Administrators"))
            {
                _accountingService.RecoverAgentAmountMonyForShipment(shipment);
            }

            var OI = _orderItemRepository.Table.FirstOrDefault(p => p.Id == orderItemId);
            if (OI != null)
            {
                OI.Quantity -= OI.Quantity;
                _orderItemRepository.Update(OI);
            }
            _shipmentService.DeleteShipment(shipment);
            _extnOrderProcessingService.InsertOrderNote("محموله شماره " + shipment.Id + " از لیست این سفارش حذف شد. جهت عودت مبلغ بررسی شود", order.Id);

            #region insert in to table cancel
            Tbl_CancelReason_Order temp = new Tbl_CancelReason_Order();
            temp.id_Description = idDescription;
            temp.OrderId = orderItemId;
            temp.DateInsert = DateTime.Now;
            temp.IdUserInsert = _workContext.CurrentCustomer.Id;
            _repositoryTbl_CancelReason_Order.Insert(temp);


            #endregion

            return Json(new { success = true, message = "عملیات با موفقیت انجام شد،جهت عودت مبلغ بررسی شود" });
        }
        [Area(AreaNames.Admin)]
        [AdminAntiForgery]
        [AuthorizeAdmin]
        public IActionResult GetListCancellDescriptionAnswer()
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
        [HttpPost]
        [PublicAntiForgery]
        [HttpsRequirement(SslRequirement.Yes)]
        public IActionResult AddressDelete(List<int> LstAddressId)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();
            var customer = _workContext.CurrentCustomer;
            foreach (var addressId in LstAddressId)
            {
                //find address (ensure that it belongs to the current customer)
                var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
                if (address != null)
                {
                    customer.RemoveAddress(address);
                    _customerService.UpdateCustomer(customer);
                    //now delete the address record
                    _addressService.DeleteAddress(address);
                }
            }

            //redirect to the address list page
            return Json(new
            {
                redirect = Url.RouteUrl("CustomerAddresses")
            });
        }

        [HttpPost]
        public IActionResult getShipmentAddress(int shipmentId)
        {
            var shippingAddress = _extnOrderProcessingService.getShippingAddres(shipmentId);
            return Json(new { address = shippingAddress });
        }

        [HttpPost]
        [Area(AreaNames.Admin)]
        [HttpsRequirement(SslRequirement.Yes)]
        [AdminAntiForgery]
        [ValidateIpAddress]
        [AuthorizeAdmin]
        [ValidateVendor]
        public IActionResult getShipmentAddressForEdit(int shipmentId)
        {
            var shippingAddress = _extnOrderProcessingService.getShippingAddres(shipmentId);
            return Json(new { address = shippingAddress });
        }
        [HttpPost]
        [Area(AreaNames.Admin)]
        [HttpsRequirement(SslRequirement.Yes)]
        [AdminAntiForgery]
        [ValidateIpAddress]
        [AuthorizeAdmin]
        [ValidateVendor]
        public IActionResult ShipmentsByOrder(int orderId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();

            var order = _orderService.GetOrderById(orderId);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToOrder(order))
                return Content("");
            //shipments
            int shipmentsCount = 0;
            var data = _extnOrderProcessingService.Get_OrderDetails_Shipping(orderId, command.PageSize, command.Page - 1, out shipmentsCount);
            var gridModel = new DataSourceResult
            {
                Data = data,
                Total = shipmentsCount
            };

            return Json(gridModel);
        }
        [HttpPost]
        [Area(AreaNames.Admin)]
        [HttpsRequirement(SslRequirement.Yes)]
        [AdminAntiForgery]
        [ValidateIpAddress]
        [AuthorizeAdmin]
        [ValidateVendor]
        public IActionResult EditShipment(ModelPassDataToEditShipment _data)
        {
            try
            {
                if (!string.IsNullOrEmpty(_data.DeliveryDate) && string.IsNullOrEmpty(_data.ShippedDate))
                {
                    return Json(new { success = true, message = "کاربر محترم شما نمیتواند در زمانی که تاریخ تحویل دارای مقدار می باشد تاریخ ارسال را حذف کنید" });
                }
                if (_data.shipmentId == 0)
                    return Json(new { success = false, message = "محموله مورد نظر یافت نشد" });
                var shipment = _shipmentService.GetShipmentById(_data.shipmentId);
                if (shipment == null)
                    return Json(new { success = false, message = "محموله مورد نظر یافت نشد" });
                DateTime? shippdate = null;
                DateTime? deliverdate = null;
                if (!string.IsNullOrEmpty(_data.ShippedDate))
                    shippdate = Convert.ToDateTime(_data.ShippedDate);
                if (!string.IsNullOrEmpty(_data.DeliveryDate))
                    deliverdate = Convert.ToDateTime(_data.DeliveryDate);
                DateTime? _CollectDate = null;
                if (!string.IsNullOrEmpty(_data.CollectDate))
                    _CollectDate = Convert.ToDateTime(_data.CollectDate);
                if (shippdate.HasValue && deliverdate.HasValue && DateTime.Compare(shippdate.Value, deliverdate.Value) >= 0)
                {
                    return Json(new { success = true, message = "تاریخ ارسال نمیتواند هم زمان یا بعد از تاریخ تحویل باشد" });
                }
                if (shippdate.HasValue && _CollectDate.HasValue && DateTime.Compare(_CollectDate.Value, shippdate.Value) >= 0)
                {
                    return Json(new { success = true, message = "تاریخ جمع آوری نمیتواند هم زمان یا بعد از تاریخ ارسال باشد" });
                }
                if (deliverdate.HasValue && _CollectDate.HasValue && DateTime.Compare(_CollectDate.Value, deliverdate.Value) >= 0)
                {
                    return Json(new { success = true, message = "تاریخ جمع آوری نمیتواند هم زمان یا بعد از تاریخ تحویل باشد" });
                }


                int enteredWight = -1;
                var orderItemId = shipment.ShipmentItems.FirstOrDefault().OrderItemId;
                var OI = _orderItemRepository.Table.FirstOrDefault(p => p.Id == orderItemId);
                var prAttrMapp = _productAttributeParser.ParseProductAttributeMappings(OI.AttributesXml);


                if (!string.IsNullOrEmpty(_data.Weight))
                {
                    if (!int.TryParse(_data.Weight, out enteredWight) || enteredWight <= 0)
                    {
                        return Json(new { success = false, message = "لطفا وزن را صحیح وارد کنید" });
                    }

                    var productCategory = OI.Product.ProductCategories.FirstOrDefault();
                    if (productCategory == null || (productCategory.CategoryId != 654 && productCategory.CategoryId != 655 && productCategory.CategoryId != 662 && productCategory.CategoryId != 722 && productCategory.CategoryId != 723))
                    {
                        return Json(new { success = false, message = "تغییر وزن فقط برای سفارش های شرکت پست امکان پذیر می باشد" });
                    }
                    #region بررسی امکان تغییر وزن شرکت پست
                    //var attrPackingWeight = prAttrMapp.First(p => p.ProductAttribute.Name.Contains("وزن بسته"));
                    //var attrWeightAdjustment = _productAttributeParser.ParseProductAttributeValues(OI.AttributesXml, attrPackingWeight.Id).FirstOrDefault();
                    int minWeight = 0, maxWeight = 500;

                    var exactWeightStr = _dbContext.SqlQuery<string>($"SELECT toir.ExactWeight FROM dbo.Tb_OrderItemsRecord AS toir WHERE toir.OrderItemId = {OI.Id}").FirstOrDefault();
                    if (!string.IsNullOrEmpty(exactWeightStr))
                    {
                        int exactWeight = Convert.ToInt32(exactWeightStr);
                        //if (exactWeight >= 0 && exactWeight <= 500)
                        //{
                        //    minWeight = 0;
                        //    maxWeight = 500;
                        //}
                        //else
                        if (exactWeight > 500 && exactWeight <= 1000)
                        {
                            minWeight = 500;
                            maxWeight = 1000;
                        }
                        else if (exactWeight > 1000 && exactWeight <= 2000)
                        {
                            minWeight = 1000;
                            maxWeight = 2000;
                        }
                        else if (exactWeight > 2000 && exactWeight <= 3000)
                        {
                            minWeight = 2000;
                            maxWeight = 3000;
                        }
                        else if (exactWeight > 3000 && exactWeight <= 5000)
                        {
                            minWeight = 3000;
                            maxWeight = 5000;
                        }
                        else if (exactWeight > 5000 && exactWeight <= 10000)
                        {
                            minWeight = 5000;
                            maxWeight = 10000;
                        }
                        else if (exactWeight > 10000 && exactWeight <= 15000)
                        {
                            minWeight = 10000;
                            maxWeight = 15000;
                        }
                        else if (exactWeight > 15000 && exactWeight <= 20000)
                        {
                            minWeight = 15000;
                            maxWeight = 20000;
                        }
                        else if (exactWeight > 20000 && exactWeight <= 25000)
                        {
                            minWeight = 20000;
                            maxWeight = 25000;
                        }
                        else if (exactWeight > 25000 && exactWeight <= 30000)
                        {
                            minWeight = 25000;
                            maxWeight = 30000;
                        }

                        #region commented
                        //switch (exactWeight.Value)
                        //{
                        //    case "500 تا 1 کیلوگرم":

                        //        break;
                        //    case "1 تا 2 کیلوگرم":
                        //        minWeight = 1000;
                        //        maxWeight = 2000;
                        //        break;
                        //    case "2 تا 3 کیلوگرم":
                        //        minWeight = 2000;
                        //        maxWeight = 3000;
                        //        break;

                        //    case "3 تا 5 کیلوگرم":
                        //        minWeight = 3000;
                        //        maxWeight = 5000;
                        //        break;

                        //    case "5 تا 10 کیلوگرم":
                        //        minWeight = 5000;
                        //        maxWeight = 10000;
                        //        break;

                        //    case "10 تا 15 کیلوگرم":
                        //        minWeight = 10000;
                        //        maxWeight = 15000;
                        //        break;

                        //    case "15 تا 20 کیلوگرم":
                        //        minWeight = 15000;
                        //        maxWeight = 20000;
                        //        break;
                        //    case "20 تا 25 کیلوگرم":
                        //        minWeight = 20000;
                        //        maxWeight = 25000;
                        //        break;
                        //    case "25 تا 30 کیلوگرم":
                        //        minWeight = 25000;
                        //        maxWeight = 30000;
                        //        break;
                        //    default:
                        //        break;
                        //}
                        #endregion

                        if (enteredWight <= minWeight || enteredWight > maxWeight)
                        {
                            return Json(new
                            {
                                success = false,
                                message = $"وزن وارد شده خارج از بازه مجاز برای این محموله می باشد{Environment.NewLine}آیا برای مشتری پیامکی برای پرداخت ما به تفاوت ارسال شود ؟",
                                confirm = true
                            });
                        }
                    }

                    #endregion
                }

                shipment.DeliveryDateUtc = (deliverdate.HasValue ? deliverdate.Value.ToUniversalTime() : (DateTime?)null);
                shipment.ShippedDateUtc = (shippdate.HasValue ? shippdate.Value.ToUniversalTime() : (DateTime?)null); ;
                _shipmentService.UpdateShipment(shipment);

                _extendedShipmentService.EditCollectDate(shipment.Id, _CollectDate);
                //=====================================================================================================//
                #region for insert data to table Extra Field Shipment
                //=type in table
                //1 مفقود شده 
                //2 غرامت خسارت
                //3 غرامت مفقودی
                //4 غرامت تاخیر
                //5 شکایت

                string TempelteNote = "";// ".نظر کاربر: " + _workContext.CurrentCustomer.GetFullName() + "در تاریخ: " + DateTime.Now.ToLongDateString() + "با توضیحات:";

                TblExtraFiledShipment temp = new TblExtraFiledShipment();
                var t = _shipmentService.GetShipmentById(_data.shipmentId).OrderId;
                Order _order = _orderService.GetOrderById(t);
                if (_data.Value_mafgho > 0)
                {
                    //1
                    _order.OrderNotes.Add(new OrderNote
                    {
                        Note = TempelteNote + _data.Des_mafgho,
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _orderService.UpdateOrder(_order);


                    //2

                    temp.Type = 1;
                    temp.value = _data.Value_mafgho;
                    temp.ShippingId = _data.shipmentId;
                    temp.OrderNoteId = _order.OrderNotes.OrderByDescending(p => p.Id).FirstOrDefault().Id;
                    temp.IdUserInsert = _workContext.CurrentCustomer.Id;
                    temp.DateInsert = DateTime.Now;
                    _repositoryTblExtraFiledShipment.Insert(temp);
                }
                if (_data.Value_khesarat > 0)
                {
                    //1
                    _order.OrderNotes.Add(new OrderNote
                    {
                        Note = TempelteNote + _data.Des_khesarat,
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _orderService.UpdateOrder(_order);
                    //2

                    temp.Type = 2;
                    temp.value = _data.Value_khesarat;
                    temp.ShippingId = _data.shipmentId;
                    temp.OrderNoteId = _order.OrderNotes.OrderByDescending(p => p.Id).FirstOrDefault().Id;
                    temp.IdUserInsert = _workContext.CurrentCustomer.Id;
                    temp.DateInsert = DateTime.Now;
                    _repositoryTblExtraFiledShipment.Insert(temp);
                }
                if (_data.Value_gheramatmafghod > 0)
                {
                    //1
                    _order.OrderNotes.Add(new OrderNote
                    {
                        Note = TempelteNote + _data.Des_gheramatmafghod,
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _orderService.UpdateOrder(_order);
                    //2

                    temp.Type = 3;
                    temp.value = _data.Value_gheramatmafghod;
                    temp.ShippingId = _data.shipmentId;
                    temp.OrderNoteId = _order.OrderNotes.OrderByDescending(p => p.Id).FirstOrDefault().Id;
                    temp.IdUserInsert = _workContext.CurrentCustomer.Id;
                    temp.DateInsert = DateTime.Now;
                    _repositoryTblExtraFiledShipment.Insert(temp);
                }
                if (_data.Value_takhir > 0)
                {
                    //1
                    _order.OrderNotes.Add(new OrderNote
                    {
                        Note = TempelteNote + _data.Des_takhir,
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _orderService.UpdateOrder(_order);

                    //2

                    temp.Type = 4;
                    temp.value = _data.Value_takhir;
                    temp.ShippingId = _data.shipmentId;
                    temp.OrderNoteId = _order.OrderNotes.OrderByDescending(p => p.Id).FirstOrDefault().Id;
                    temp.IdUserInsert = _workContext.CurrentCustomer.Id;
                    temp.DateInsert = DateTime.Now;
                    _repositoryTblExtraFiledShipment.Insert(temp);
                }
                if (_data.Value_shekayat > 0)
                {
                    //1
                    _order.OrderNotes.Add(new OrderNote
                    {
                        Note = TempelteNote + _data.Des_shekayat,
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _orderService.UpdateOrder(_order);



                    //2

                    temp.Type = 5;
                    temp.value = _data.Value_shekayat;
                    temp.ShippingId = _data.shipmentId;
                    temp.OrderNoteId = _order.OrderNotes.OrderByDescending(p => p.Id).FirstOrDefault().Id;
                    temp.IdUserInsert = _workContext.CurrentCustomer.Id;
                    temp.DateInsert = DateTime.Now;
                    _repositoryTblExtraFiledShipment.Insert(temp);
                }


                #endregion
                //================================================
                if (enteredWight != -1)
                {

                    //var attr = prAttrMapp.First(p => p.ProductAttribute.Name.Contains("وزن دقیق"));

                    //var finalPrAttr = _productAttributeParser.AddProductAttribute(_productAttributeParser.RemoveProductAttribute(OI.AttributesXml, attr), attr, _data.Weight);
                    if (OI.Quantity <= 1)
                    {
                        // OI.AttributesXml = finalPrAttr;

                        //var i = _dbContext.ExecuteSqlCommand($"UPDATE OrderItem SET AttributesXml = N'{finalPrAttr}' where Id = {OI.Id}");
                        _attributeEditorService.EditOrderItemAttributes(OI.Product, OI.Order.Customer, OI.AttributesXml, OI.Id, new Dictionary<string, string>() { { "وزن دقیق", enteredWight.ToString() } });

                        _dbContext.ExecuteSqlCommand($"UPDATE [Tb_OrderItemsRecord] SET [ExactWeight] = '{enteredWight}' WHERE [OrderItemId] = {OI.Id}");
                        _dbContext.ExecuteSqlCommand($"UPDATE [Tb_OrderItemAttributeValue] SET [PropertyAttrValueText] = '{enteredWight}' WHERE [OrderItemId] = {OI.Id} AND [PropertyAttrName] like N'%وزن دقیق%'");
                    }
                    else
                    {
                        //OI.Quantity -= OI.Quantity;
                        //_orderItemRepository.Update(OI);
                        _dbContext.ExecuteSqlCommand($"UPDATE OrderItem SET Quantity = {(OI.Quantity - 1)} where Id = {OI.Id}");

                        OrderItem newOItem = new OrderItem()
                        {
                            AttributesXml = OI.AttributesXml,
                            AttributeDescription = OI.AttributeDescription,
                            DiscountAmountExclTax = OI.DiscountAmountExclTax,
                            DiscountAmountInclTax = OI.DiscountAmountInclTax,
                            ItemWeight = OI.ItemWeight,
                            OrderId = OI.OrderId,
                            OriginalProductCost = OI.OriginalProductCost,
                            PriceExclTax = OI.PriceExclTax,
                            PriceInclTax = OI.PriceInclTax,
                            Quantity = 1,
                            UnitPriceExclTax = OI.UnitPriceExclTax,
                            UnitPriceInclTax = OI.UnitPriceInclTax,
                            ProductId = OI.ProductId
                        };

                        _orderItemRepository.Insert(newOItem);
                        _attributeEditorService.EditOrderItemAttributes(OI.Product, OI.Order.Customer, OI.AttributesXml, newOItem.Id, new Dictionary<string, string>() { { "وزن دقیق", enteredWight.ToString() } });

                        _dbContext.ExecuteSqlCommand($"UPDATE [Tb_OrderItemsRecord] SET [ExactWeight] = '{enteredWight}' WHERE [OrderItemId] = {newOItem.Id}");
                        _dbContext.ExecuteSqlCommand($"UPDATE [Tb_OrderItemAttributeValue] SET [PropertyAttrValueText] = '{enteredWight}' WHERE [OrderItemId] = {newOItem.Id} AND [PropertyAttrName] like N'%وزن دقیق%'");

                        var shipmentItem = shipment.ShipmentItems.FirstOrDefault();
                        shipmentItem.OrderItemId = newOItem.Id;
                        _shipmentService.UpdateShipmentItem(shipmentItem);

                    }


                }


                return Json(new { success = true, message = "به روز رسانی با موفقیت انجام شد" });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { success = false, message = "خطا در زمان به روز رسانی. با پشتیبانی فنی تماس بگیرید" });
            }
        }

        [Area(AreaNames.Admin)]
        public IActionResult SendCancelOrderSms([FromQuery]int shipmentId)
        {
            var shipment = _shipmentService.GetShipmentById(shipmentId);
            var customer = _customerService.GetCustomerById(shipment.Order.CustomerId);
            if (_notificationService._sendSms(customer.Username, $"کاربر گرامی وزن محموله شماره {shipmentId} شما اشتباه انتخاب شده است، از این رو، لطفا با مراجعه به قسمت \"حساب من - سفارش ها\"، نسبت به پرداخت ما به تفاوت این سفارش اقدام نمایید{Environment.NewLine}امنیتو"))
            {
                return Ok(new { success = true, message = "پیامک با موفقیت ارسال شد" });
            }
            else
            {
                return Ok(new { success = false, message = "عملیات ارسال پیامک با خطا مواجه شده است" });
            }
        }

        protected virtual bool HasAccessToShipment(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var hasVendorProducts = false;
            var vendorId = _workContext.CurrentVendor.Id;
            foreach (var shipmentItem in shipment.ShipmentItems)
            {
                var orderItem = _orderService.GetOrderItemById(shipmentItem.OrderItemId);
                if (orderItem != null)
                {
                    if (orderItem.Product.VendorId == vendorId)
                    {
                        hasVendorProducts = true;
                        break;
                    }
                }
            }
            return hasVendorProducts;
        }
        protected virtual bool HasAccessToOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = _workContext.CurrentVendor.Id;
            var hasVendorProducts = order.OrderItems.Any(orderItem => orderItem.Product.VendorId == vendorId);
            return hasVendorProducts;
        }
        public IActionResult FetchAddress(string searchtext)
        {
            var Addresses = _workContext.CurrentCustomer.Addresses.Where(
                p => ((p.FirstName ?? "") + " " + (p.LastName ?? "") + " " + (p.PhoneNumber ?? "") + " " +
                      (p.Country?.Name ?? "") + " "
                      + (p.StateProvince?.Name ?? "") + " " + (p.Address1 ?? "") + " " + (p.ZipPostalCode ?? "") + " " +
                      (p.Company ?? "")).Contains(searchtext)
            ).Select(p => new
            {
                id = p.Id,
                text = (p.FirstName ?? "") + " " + (p.LastName ?? "") + " " + (p.PhoneNumber ?? "") + " " +
                       (p.Country?.Name ?? "") + " "
                       + (p.StateProvince?.Name ?? "") + " " + (p.Address1 ?? "") + " " + (p.ZipPostalCode ?? "") +
                       " " + (p.Company ?? "")
            }).ToList();
            return Json(new { results = Addresses });
        }

        public IActionResult Mus_SelectBillingAddress(int addressId, bool shipToSameAddress = false,
            bool multipleShipment = false)
        {
            //validation
            var address = _workContext.CurrentCustomer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                return RedirectToRoute("CheckoutBillingAddress");

            _workContext.CurrentCustomer.BillingAddress = address;
            _customerService.UpdateCustomer(_workContext.CurrentCustomer);

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            if (!shipToSameAddress
                && multipleShipment
                // && _shoppingCartService.ShoppingCartRequiresShipping(cart) 
                && address.Country.AllowsShipping)
                return RedirectToRoute("CheckoutShipment", new { ShipmentStep = 1 });

            //ship to the same address?
            if (_shippingSettings.ShipToSameAddress
                && shipToSameAddress
                && !multipleShipment
                // && _shoppingCartService.ShoppingCartRequiresShipping(cart) 
                && address.Country.AllowsShipping)
            {
                _workContext.CurrentCustomer.ShippingAddress = _workContext.CurrentCustomer.BillingAddress;
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                //reset selected shipping method (in case if "pick up in store" was selected)
                _genericAttributeService.SaveAttribute<ShippingOption>(_workContext.CurrentCustomer,
                    SystemCustomerAttributeNames.SelectedShippingOption, null, _storeContext.CurrentStore.Id);
                _genericAttributeService.SaveAttribute<PickupPoint>(_workContext.CurrentCustomer,
                    SystemCustomerAttributeNames.SelectedPickupPoint, null, _storeContext.CurrentStore.Id);
                //limitation - "Ship to the same address" doesn't properly work in "pick up in store only" case (when no shipping plugins are available) 
                return RedirectToRoute("CheckoutShippingMethod");
            }

            return RedirectToRoute("CheckoutShippingAddress");
        }

        public IActionResult Shipment(int ShipmentStep)
        {
            try
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();

                if (!cart.Any())
                    return RedirectToRoute("ShoppingCart");

                if (_orderSettings.OnePageCheckoutEnabled && _workContext.OriginalCustomerIfImpersonated != null)
                    return RedirectToRoute("CheckoutOnePage");

                if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                    return Challenge();


                if (ShipmentStep == 1)
                {
                    //#region Can Remove
                    var shipmentValues = new List<ShipmentValues>();

                    foreach (var shoppingCartItem in cart)
                    {
                        var shipmentValue = new ShipmentValues
                        {
                            ItemNumber = shoppingCartItem.Id.ToString(),
                            ShipmentNumber = "1"
                        };
                        shipmentValues.Add(shipmentValue);
                    }

                    HttpContext.Session.Set("ShipmentValues", shipmentValues);
                }

                if (ShipmentStep == 2)
                {
                    var listOfAddresses = new List<AddressModel>();

                    foreach (var curAddress in _workContext.CurrentCustomer.Addresses)
                    {
                        if (curAddress.Address1 == null) curAddress.Address1 = string.Empty;
                        if (curAddress.Address2 == null) curAddress.Address2 = string.Empty;
                        if (curAddress.City == null) curAddress.City = string.Empty;
                        if (curAddress.ZipPostalCode == null) curAddress.ZipPostalCode = string.Empty;
                        if (curAddress.Company == null) curAddress.Company = string.Empty;

                        var output = new AddressModel();

                        _addressModelFactory.PrepareAddressModel(output,
                            curAddress,
                            false,
                            _addressSettings,
                            () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));

                        listOfAddresses.Add(output);
                    }

                    HttpContext.Session.Set("Addresses", listOfAddresses.ToList());
                }

                if (ShipmentStep == 3)
                {
                    //model
                    var multiShipmentInfo = HttpContext.Session.Get<List<MultiShipmentInfo>>("MultiShipmentInfo");
                    if (multiShipmentInfo != null)
                    {
                        foreach (var item in multiShipmentInfo)
                            item.ShippingMethod = _checkoutModelFactory.PrepareShippingMethodModel(
                                cart.Where(p => item.ShoppingCartIds.Contains(p.Id)).ToList()
                                , _addressService.GetAddressById(item.ShippingAddressId ?? 0));
                        HttpContext.Session.Set("MultiShipmentInfo", multiShipmentInfo);
                    }
                    else
                    {
                        ShipmentStep = 1;
                    }
                }

                HttpContext.Session.SetInt32("ShipmentStep", ShipmentStep);

                return View("~/Plugins/Orders.MultiShipping/Views/Checkout/Shipment.cshtml");
            }
            catch (Exception ex)
            {
                Log(ex.Message + ex.InnerException != null ? "-->" + ex.InnerException.Message : "", "");
                HttpContext.Session.SetInt32("ShipmentStep", 1);
                return View("~/Plugins/Orders.MultiShipping/Views/Checkout/Shipment.cshtml");
            }
        }

        [HttpPost]
        public void SaveShipmentNumbers(List<ShipmentValues> shipmentValues)
        {
            HttpContext.Session.Set("ShipmentValues", shipmentValues);
        }

        [HttpPost]
        public void SaveShipmentAddresses(List<AddressValues> addressValues)
        {
            try
            {
                HttpContext.Session.Set("addressValues", addressValues);
                var shipmentList = HttpContext.Session.Get<List<ShipmentValues>>("ShipmentValues");
                var multiShipmentInfo = new List<MultiShipmentInfo>();
                var i = 1;
                foreach (var shipmentItem in shipmentList)
                {
                    var addressId = int.Parse(addressValues.First(p => p.ShipmentNumber == shipmentItem.ShipmentNumber)
                        .Address);
                    multiShipmentInfo.Add(new MultiShipmentInfo
                    {
                        ShipmentNumber = i,
                        ShippingAddressId = addressId,
                        orginalShipmentNumber = int.Parse(shipmentItem.ShipmentNumber),
                        ShoppingCartIds = shipmentList.Where(p => p.ShipmentNumber.Equals(shipmentItem.ShipmentNumber))
                            .Select(p => int.Parse(p.ItemNumber)).Distinct().ToList()
                    });
                    i++;
                }

                //var multiShipmentInfo = shipmentList.Select(v => new MultiShipmentInfo()
                //{
                //    ShipmentNumber = int.Parse(v.ShipmentNumber),
                //    ShoppingCartIds = shipmentList.Where(p => p.ShipmentNumber.Equals(v.ShipmentNumber)).Select(p => int.Parse(p.ItemNumber)).ToList()
                //}).ToList();
                //    .GroupBy(p => p.ShipmentNumber).Select(g => g.First()).ToList();
                HttpContext.Session.Set("MultiShipmentInfo", multiShipmentInfo);
            }
            catch (Exception ex)
            {
                Log(ex.Message + ex.InnerException != null ? "-->" + ex.InnerException.Message : "", "");
                HttpContext.Session.SetInt32("ShipmentStep", 1);
                //eturn View("~/Plugins/Orders.MultiShipping/Views/Checkout/Shipment.cshtml");
            }
        }

        [HttpPost]
        public void SaveShipmentMethods(List<ShippingMethodValues> shippingMethodValues)
        {
            try
            {
                var multiShipmentInfo = HttpContext.Session.Get<List<MultiShipmentInfo>>("MultiShipmentInfo");
                foreach (var item in multiShipmentInfo)
                {
                    var shippmentMethod = shippingMethodValues.FirstOrDefault(p =>
                        p.ShipmentNumber == item.orginalShipmentNumber.ToString());
                    item.DeliveryDate = shippmentMethod.DeliveryDate;
                    foreach (var shippingOption in item.ShippingMethod.ShippingMethods)
                        if (shippmentMethod.ShippingMethod.Contains(shippingOption.Name))
                        {
                            shippingOption.Selected = true;
                            item.SelectedShippingOption = shippingOption.ShippingOption;
                        }
                        else
                        {
                            shippingOption.Selected = false;
                        }
                }

                HttpContext.Session.Set("MultiShipmentInfo", multiShipmentInfo);
                var multiShipmentShippingOptions = new List<MultiShipmentShippingOptions>();
                foreach (var item in multiShipmentInfo)
                    multiShipmentShippingOptions.Add(
                        new MultiShipmentShippingOptions
                        {
                            ShipmentNumber = item.ShipmentNumber,
                            OrginalShipmentNumber = item.orginalShipmentNumber,
                            ShippingAddressId = item.ShippingAddressId,
                            ShoppingCartIds = item.ShoppingCartIds,
                            DeliveryDate = item.DeliveryDate,
                            SelectedShippingOption = item.SelectedShippingOption
                        });
                //save
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    "SelectedMultiShipmentShippingOptionsAttribute",
                    JsonConvert.SerializeObject(multiShipmentShippingOptions), _storeContext.CurrentStore.Id);
            }
            catch (Exception ex)
            {
                Log(ex.Message + ex.InnerException != null ? "-->" + ex.InnerException.Message : "", "");
                HttpContext.Session.SetInt32("ShipmentStep", 1);
                //return View("~/Plugins/Orders.MultiShipping/Views/Checkout/Shipment.cshtml");
            }
        }

        public void Log(string header, string Message)
        {
            var logger =
                (DefaultLogger)EngineContext.Current
                    .Resolve<ILogger>();
            logger.InsertLog(LogLevel.Information, header, Message, null);
        }

        public IActionResult ShowNewBillingAddress()
        {
            return PartialView("~/Plugins/Orders.ExtendedShipment/Views/_CreateOrUpdateAddress.cshtml", new AddressModel
            {
                CityEnabled = true,
                CompanyEnabled = true,
                CityRequired = true,
                CountryEnabled = true,
                FaxEnabled = true,
                PhoneEnabled = true,
                StateProvinceEnabled = true,
                StreetAddressEnabled = true,
                ZipPostalCodeEnabled = true,
                AvailableCountries = _countryService.GetAllCountries().Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList(),
                AvailableStates = _stateProvinceService.GetStateProvinces().Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList()
            });
        }

        public IActionResult AddNewAddress(AddressModel newAddress)
        {
            if (ModelState.IsValid)
            {
                //try to find an address with the same values (don't duplicate records)
                var address = _workContext.CurrentCustomer.Addresses.ToList().FindAddress(
                    newAddress.FirstName, newAddress.LastName, newAddress.PhoneNumber,
                    newAddress.Email, newAddress.FaxNumber, newAddress.Company,
                    newAddress.Address1, newAddress.Address2, newAddress.City,
                    newAddress.StateProvinceId, newAddress.ZipPostalCode,
                    newAddress.CountryId, null);
                if (address == null)
                {
                    //address is not found. let's create a new one
                    address = newAddress.ToEntity();
                    address.CustomAttributes = null;
                    address.CreatedOnUtc = DateTime.UtcNow;
                    //some validation
                    if (address.CountryId == 0)
                        address.CountryId = null;
                    if (address.StateProvinceId == 0)
                        address.StateProvinceId = null;
                    _workContext.CurrentCustomer.Addresses.Add(address);

                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                    return Json(new
                    {
                        result = true,
                        message = "آدرس ذخیره شد",
                        address = (address.FirstName ?? "") + " " + (address.LastName ?? "") + " " +
                                  (address.PhoneNumber ?? "") + " " + (address.Country?.Name ?? "") + " "
                                  + (address.StateProvince?.Name ?? "") + " " + (address.Address1 ?? "") + " " +
                                  (address.ZipPostalCode ?? "") + " " + (address.Company ?? ""),
                        Id = address.Id.ToString()
                    });
                }

                return Json(new { result = false, message = "آدرس وارد شده موجود می باشد" });
            }

            return Json(new { result = false, message = "لطفا اطلاعات آدرس را به درستی وارد نمایید" });
        }

        public IActionResult PaymentMethod()
        {
            try
            {
                if (HttpContext.Session.GetInt32("ShipmentStep") > 0)
                    HttpContext.Session.SetInt32("ShipmentStep", 4);
                //validation

                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                if (!cart.Any())
                    return RedirectToRoute("ShoppingCart");

                if (_orderSettings.OnePageCheckoutEnabled && _workContext.OriginalCustomerIfImpersonated != null)
                    return RedirectToRoute("CheckoutOnePage");

                if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                    return Challenge();

                //Check whether payment workflow is required
                //we ignore reward points during cart total calculation
                var isPaymentWorkflowRequired = _orderProcessingService.IsPaymentWorkflowRequired(cart, false);
                if (!isPaymentWorkflowRequired)
                {
                    _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
                        "SelectedPaymentMethod", null, _storeContext.CurrentStore.Id);
                    return RedirectToRoute("Mus_CheckoutPaymentInfo");
                }

                //filter by country
                var filterByCountryId = 0;
                if (_addressSettings.CountryEnabled &&
                    _workContext.CurrentCustomer.BillingAddress != null &&
                    _workContext.CurrentCustomer.BillingAddress.Country != null)
                    filterByCountryId = _workContext.CurrentCustomer.BillingAddress.Country.Id;

                //model
                var paymentMethodModel = _checkoutModelFactory.PreparePaymentMethodModel(cart, filterByCountryId);

                if (_paymentSettings.BypassPaymentMethodSelectionIfOnlyOne &&
                    paymentMethodModel.PaymentMethods.Count == 1 && !paymentMethodModel.DisplayRewardPoints)
                {
                    //if we have only one payment method and reward points are disabled or the current customer doesn't have any reward points
                    //so customer doesn't have to choose a payment method

                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                        "SelectedPaymentMethod",
                        paymentMethodModel.PaymentMethods[0].PaymentMethodSystemName,
                        _storeContext.CurrentStore.Id);
                    return RedirectToRoute("Mus_CheckoutPaymentInfo");
                }

                return View("~/Plugins/Orders.MultiShipping/Views/Checkout/PaymentMethod.cshtml", paymentMethodModel);
            }
            catch (Exception ex)
            {
                Log(ex.Message + ex.InnerException != null ? "-->" + ex.InnerException.Message : "", "");
                HttpContext.Session.SetInt32("ShipmentStep", 1);
                return View("~/Plugins/Orders.MultiShipping/Views/Checkout/Shipment.cshtml");
            }
        }

        [HttpPost]
        [ActionName("PaymentMethod")]
        [FormValueRequired("nextstep")]
        public IActionResult SelectPaymentMethod(string paymentmethod, CheckoutPaymentMethodModel model)
        {
            try
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                if (!cart.Any())
                    return RedirectToRoute("ShoppingCart");

                if (_orderSettings.OnePageCheckoutEnabled && _workContext.OriginalCustomerIfImpersonated != null)
                    return RedirectToRoute("CheckoutOnePage");

                if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                    return Challenge();

                //reward points
                if (_rewardPointsSettings.Enabled)
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                        "UseRewardPointsDuringCheckout", model.UseRewardPoints,
                        _storeContext.CurrentStore.Id);

                //Check whether payment workflow is required
                var isPaymentWorkflowRequired = _orderProcessingService.IsPaymentWorkflowRequired(cart);
                if (!isPaymentWorkflowRequired)
                {
                    _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
                        "SelectedPaymentMethod", null, _storeContext.CurrentStore.Id);
                    return RedirectToRoute("Mus_CheckoutPaymentInfo");
                }

                //payment method 
                if (string.IsNullOrEmpty(paymentmethod))
                    return PaymentMethod();

                var paymentMethodInst = _paymentService.LoadPaymentMethodBySystemName(paymentmethod);
                if (paymentMethodInst == null ||
                    //!_paymentService. IsPaymentMethodActive(paymentMethodInst) ||
                    !_pluginFinder.AuthenticateStore(paymentMethodInst.PluginDescriptor,
                        _storeContext.CurrentStore.Id) ||
                    !_pluginFinder.AuthorizedForUser(paymentMethodInst.PluginDescriptor, _workContext.CurrentCustomer))
                    return PaymentMethod();

                //save
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    "SelectedPaymentMethod", paymentmethod, _storeContext.CurrentStore.Id);
                return RedirectToRoute("Mus_CheckoutPaymentInfo");
            }
            catch (Exception ex)
            {
                Log(ex.Message + ex.InnerException != null ? "-->" + ex.InnerException.Message : "", "");
                HttpContext.Session.SetInt32("ShipmentStep", 1);
                return View("~/Plugins/Orders.MultiShipping/Views/Checkout/Shipment.cshtml");
            }
        }

        public IActionResult PaymentInfo()
        {
            try
            {
                //validation

                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                if (!cart.Any())
                    return RedirectToRoute("ShoppingCart");

                if (_orderSettings.OnePageCheckoutEnabled && _workContext.OriginalCustomerIfImpersonated != null)
                    return RedirectToRoute("CheckoutOnePage");

                if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                    return Challenge();

                //Check whether payment workflow is required
                var isPaymentWorkflowRequired = _orderProcessingService.IsPaymentWorkflowRequired(cart);
                if (!isPaymentWorkflowRequired) return RedirectToRoute("Mus_CheckoutConfirm");

                //load payment method
                var paymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>("SelectedPaymentMethod",
                    _genericAttributeService
                    , _storeContext.CurrentStore.Id);
                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(paymentMethodSystemName);
                if (paymentMethod == null)
                    return RedirectToRoute("Mus_PaymentMethod");

                //Check whether payment info should be skipped
                if (paymentMethod.SkipPaymentInfo ||
                    paymentMethod.PaymentMethodType == PaymentMethodType.Redirection &&
                    _paymentSettings.SkipPaymentInfoStepForRedirectionPaymentMethods)
                {
                    //skip payment info page
                    var paymentInfo = new ProcessPaymentRequest();

                    //session save
                    HttpContext.Session.Set("OrderPaymentInfo", paymentInfo);
                    HttpContext.Session.SetInt32("ShipmentStep", 5);
                    return RedirectToRoute("Mus_CheckoutConfirm");
                }

                //model
                var model = _checkoutModelFactory.PreparePaymentInfoModel(paymentMethod);

                //model.CreditCards = _creditCardService.GetCreditCardsByCustomerId(_workContext.CurrentCustomer.Id);

                return View("~/Plugins/Orders.MultiShipping/Views/Checkout/PaymentInfo.cshtml", model);
            }
            catch (Exception ex)
            {
                Log(ex.Message + ex.InnerException != null ? "-->" + ex.InnerException.Message : "", "");
                HttpContext.Session.SetInt32("ShipmentStep", 1);
                return View("~/Plugins/Orders.MultiShipping/Views/Checkout/Shipment.cshtml");
            }
        }

        [HttpPost]
        [ActionName("PaymentInfo")]
        [FormValueRequired("nextstep")]
        public IActionResult EnterPaymentInfo(IFormCollection form)
        {
            try
            {
                //validation
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                if (!cart.Any())
                    return RedirectToRoute("ShoppingCart");

                if (_orderSettings.OnePageCheckoutEnabled && _workContext.OriginalCustomerIfImpersonated != null)
                    return RedirectToRoute("CheckoutOnePage");

                if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                    return Challenge();

                //Check whether payment workflow is required
                var isPaymentWorkflowRequired = _orderProcessingService.IsPaymentWorkflowRequired(cart);
                if (!isPaymentWorkflowRequired) return RedirectToRoute("Mus_CheckoutConfirm");

                //load payment method
                var paymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>("SelectedPaymentMethod",
                    _genericAttributeService
                    , _storeContext.CurrentStore.Id);
                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(paymentMethodSystemName);
                if (paymentMethod == null)
                    return RedirectToRoute("Mus_PaymentMethod");

                var warnings = paymentMethod.ValidatePaymentForm(form);
                foreach (var warning in warnings)
                    ModelState.AddModelError("", warning);
                if (ModelState.IsValid)
                {
                    //get payment info
                    var paymentInfo = paymentMethod.GetPaymentInfo(form);

                    //session save
                    HttpContext.Session.Set("OrderPaymentInfo", paymentInfo);
                    return RedirectToRoute("Mus_CheckoutConfirm");
                }

                //If we got this far, something failed, redisplay form
                //model
                var model = _checkoutModelFactory.PreparePaymentInfoModel(paymentMethod);
                return View("~/Plugins/Orders.MultiShipping/Views/Checkout/PaymentInfo.cshtml", model);
            }
            catch (Exception ex)
            {
                Log(ex.Message + ex.InnerException != null ? "-->" + ex.InnerException.Message : "", "");
                HttpContext.Session.SetInt32("ShipmentStep", 1);
                return View("~/Plugins/Orders.MultiShipping/Views/Checkout/Shipment.cshtml");
            }
        }

        public IActionResult Confirm()
        {
            try
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                if (!cart.Any())
                    return RedirectToRoute("ShoppingCart");

                if (_orderSettings.OnePageCheckoutEnabled && _workContext.OriginalCustomerIfImpersonated != null)
                    return RedirectToRoute("CheckoutOnePage");

                if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                    return Challenge();

                //model
                var model = _checkoutModelFactory.PrepareConfirmOrderModel(cart);
                return View("~/Plugins/Orders.MultiShipping/Views/Checkout/Confirm.cshtml", model);
            }
            catch (Exception ex)
            {
                Log(ex.Message + ex.InnerException != null ? "-->" + ex.InnerException.Message : "", "");
                HttpContext.Session.SetInt32("ShipmentStep", 1);
                return View("~/Plugins/Orders.MultiShipping/Views/Checkout/Shipment.cshtml");
            }
        }

        [HttpPost]
        [ActionName("Confirm")]
        public IActionResult ConfirmOrder()
        {
            var devDate = HttpContext.Session.GetString("SingleDeliveryDate");
            //Is it multiple shipment?
            if (HttpContext.Session.GetInt32("ShipmentStep") > 0) return ConfirmOrderMultipleShipment();

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled && _workContext.OriginalCustomerIfImpersonated != null)
                return RedirectToRoute("CheckoutOnePage");

            if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //model
            var model = _checkoutModelFactory.PrepareConfirmOrderModel(cart);
            try
            {
                var processPaymentRequest = HttpContext.Session.Get<ProcessPaymentRequest>("OrderPaymentInfo");
                if (processPaymentRequest == null)
                {
                    //Check whether payment workflow is required
                    if (_orderProcessingService.IsPaymentWorkflowRequired(cart))
                        return RedirectToRoute("Mus_CheckoutPaymentInfo");

                    processPaymentRequest = new ProcessPaymentRequest();
                }

                //prevent 2 orders being placed within an X seconds time frame
                if (!IsMinimumOrderPlacementIntervalValid(_workContext.CurrentCustomer))
                    throw new Exception(_localizationService.GetResource("Checkout.MinOrderPlacementInterval"));

                //place order
                processPaymentRequest.StoreId = _storeContext.CurrentStore.Id;
                processPaymentRequest.CustomerId = _workContext.CurrentCustomer.Id;
                processPaymentRequest.PaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    "SelectedPaymentMethod", _genericAttributeService
                    , _storeContext.CurrentStore.Id);

                var shipment = new List<ExnShippmentModel>
                {
                    new ExnShippmentModel
                    {
                        shipment = new Shipment
                        {
                            DeliveryDateUtc = DateTimeOffset.Parse(devDate).UtcDateTime
                        }
                    }
                };
                var placeOrderResult = _extnOrderProcessingService.PlaceOrder(processPaymentRequest, shipment);
                if (placeOrderResult.Success)
                {
                    HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", null);
                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };

                    _paymentService.PostProcessPayment(postProcessPaymentRequest);

                    if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone) return Content("Redirected");

                    return RedirectToRoute("CheckoutCompleted", new { orderId = placeOrderResult.PlacedOrder.Id });
                }

                foreach (var error in placeOrderResult.Errors)
                    model.Warnings.Add(error);
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc);
                model.Warnings.Add(exc.Message);
            }

            //If we got this far, something failed, redisplay form
            return View("~/Plugins/Orders.MultiShipping/Views/Checkout/Confirm.cshtml", model);
        }

        public IActionResult ConfirmOrderMultipleShipment()
        {
            //validation
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled && _workContext.OriginalCustomerIfImpersonated != null)
                return RedirectToRoute("CheckoutOnePage");

            if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //model
            var model = _checkoutModelFactory.PrepareConfirmOrderModel(cart);
            try
            {
                var processPaymentRequest = HttpContext.Session.Get<ProcessPaymentRequest>("OrderPaymentInfo");
                if (processPaymentRequest == null)
                {
                    //Check whether payment workflow is required
                    if (_orderProcessingService.IsPaymentWorkflowRequired(cart))
                        return RedirectToRoute("Mus_CheckoutPaymentInfo");

                    processPaymentRequest = new ProcessPaymentRequest();
                }

                //prevent 2 orders being placed within an X seconds time frame
                if (!IsMinimumOrderPlacementIntervalValid(_workContext.CurrentCustomer))
                    throw new Exception(_localizationService.GetResource("Checkout.MinOrderPlacementInterval"));

                //place order
                processPaymentRequest.StoreId = _storeContext.CurrentStore.Id;
                processPaymentRequest.CustomerId = _workContext.CurrentCustomer.Id;
                processPaymentRequest.PaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    "SelectedPaymentMethod", _genericAttributeService
                    , _storeContext.CurrentStore.Id);

                //MultipleShipment Tasks
                var multiShipmentInfo = HttpContext.Session.Get<List<MultiShipmentInfo>>("MultiShipmentInfo");
                var Lst_ExShippments = new List<ExnShippmentModel>();
                foreach (var multiShipment in multiShipmentInfo)
                {
                    var newShipment = new Shipment
                    {
                        DeliveryDateUtc = string.IsNullOrEmpty(multiShipment.DeliveryDate)
                            ? (DateTime?)null
                            : DateTime.Parse(multiShipment.DeliveryDate).ToUniversalTime()
                    };
                    //foreach (var item in multiShipment.ShoppingCartIds)
                    //{
                    var newItem = new ShipmentItem
                    {
                        OrderItemId = multiShipment.ShoppingCartIds[0],//cart.Where(x => x.Id.Equals().First().ProductId,
                        Quantity = 1 //cart.Where(x => x.Id.Equals(item)).First().Quantity,
                    };
                    newShipment.ShipmentItems.Add(newItem);
                    //}

                    Lst_ExShippments.Add(new ExnShippmentModel
                    {
                        shipment = newShipment,
                        ShippmentAddressId = _addressService.GetAddressById(multiShipment.ShippingAddressId ?? 0).Id,
                        ShippmentMethod = multiShipment.SelectedShippingOption.Name
                    });
                }

                var placeOrderResult = _extnOrderProcessingService.PlaceOrder(processPaymentRequest, Lst_ExShippments);


                if (placeOrderResult.Success)
                {
                    HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", null);
                    HttpContext.Session.Set<List<ShipmentValues>>("ShipmentValues", null);
                    HttpContext.Session.Set<List<AddressValues>>("addressValues", null);
                    HttpContext.Session.Set<List<ShippingMethodValues>>("shippingMethodsValues", null);
                    HttpContext.Session.Set<List<MultiShipmentInfo>>("MultiShipmentInfo", null);

                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };

                    _paymentService.PostProcessPayment(postProcessPaymentRequest);

                    if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone) return Content("Redirected");

                    return RedirectToRoute("CheckoutCompleted", new { orderId = placeOrderResult.PlacedOrder.Id });
                }

                foreach (var error in placeOrderResult.Errors)
                    model.Warnings.Add(error);
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc);
                model.Warnings.Add(exc.Message);
            }

            //If we got this far, something failed, redisplay form
            return View("~/Plugins/Orders.MultiShipping/Views/Checkout/Confirm.cshtml", model);
        }

        #region Utilities

        protected bool IsMinimumOrderPlacementIntervalValid(Customer customer)
        {
            //prevent 2 orders being placed within an X seconds time frame
            if (_orderSettings.MinimumOrderPlacementInterval == 0)
                return true;

            var lastOrder = _orderService.SearchOrders(_storeContext.CurrentStore.Id,
                    customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
                .FirstOrDefault();
            if (lastOrder == null)
                return true;

            var interval = DateTime.UtcNow - lastOrder.CreatedOnUtc;
            return interval.TotalSeconds > _orderSettings.MinimumOrderPlacementInterval;
        }

        #endregion
        protected virtual ShipmentModel PrepareShipmentModel(Shipment shipment, bool prepareProducts, bool prepareShipmentEvent = false)
        {
            //measures
            var baseWeight = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId);
            var baseWeightIn = baseWeight != null ? baseWeight.Name : "";
            var baseDimension = _measureService.GetMeasureDimensionById(_measureSettings.BaseDimensionId);
            var baseDimensionIn = baseDimension != null ? baseDimension.Name : "";

            var model = new ShipmentModel
            {
                Id = shipment.Id,
                OrderId = shipment.OrderId,
                TrackingNumber = shipment.TrackingNumber,
                TotalWeight = shipment.TotalWeight.HasValue ? $"{shipment.TotalWeight:F2} [{baseWeightIn}]" : "",
                ShippedDate = shipment.ShippedDateUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc).ToString() : _localizationService.GetResource("Admin.Orders.Shipments.ShippedDate.NotYet"),
                ShippedDateUtc = shipment.ShippedDateUtc,
                CanShip = !shipment.ShippedDateUtc.HasValue,
                DeliveryDate = shipment.DeliveryDateUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc).ToString() : _localizationService.GetResource("Admin.Orders.Shipments.DeliveryDate.NotYet"),
                DeliveryDateUtc = shipment.DeliveryDateUtc,
                CanDeliver = shipment.ShippedDateUtc.HasValue && !shipment.DeliveryDateUtc.HasValue,
                AdminComment = shipment.AdminComment,
                CustomOrderNumber = shipment.Order.CustomOrderNumber
            };

            if (prepareProducts)
            {
                foreach (var shipmentItem in shipment.ShipmentItems)
                {
                    var orderItem = _orderService.GetOrderItemById(shipmentItem.OrderItemId);
                    if (orderItem == null)
                        continue;

                    //quantities
                    var qtyInThisShipment = shipmentItem.Quantity;
                    var maxQtyToAdd = orderItem.GetTotalNumberOfItemsCanBeAddedToShipment();
                    var qtyOrdered = orderItem.Quantity;
                    var qtyInAllShipments = orderItem.GetTotalNumberOfItemsInAllShipment();

                    var warehouse = _shippingService.GetWarehouseById(shipmentItem.WarehouseId);
                    var shipmentItemModel = new ShipmentModel.ShipmentItemModel
                    {
                        Id = shipmentItem.Id,
                        OrderItemId = orderItem.Id,
                        ProductId = orderItem.ProductId,
                        ProductName = orderItem.Product.Name,
                        Sku = orderItem.Product.FormatSku(orderItem.AttributesXml, _productAttributeParser),
                        AttributeInfo = orderItem.AttributeDescription,
                        ShippedFromWarehouse = warehouse != null ? warehouse.Name : null,
                        ShipSeparately = orderItem.Product.ShipSeparately,
                        ItemWeight = orderItem.ItemWeight.HasValue ? $"{orderItem.ItemWeight:F2} [{baseWeightIn}]" : "",
                        ItemDimensions = $"{orderItem.Product.Length:F2} x {orderItem.Product.Width:F2} x {orderItem.Product.Height:F2} [{baseDimensionIn}]",
                        QuantityOrdered = qtyOrdered,
                        QuantityInThisShipment = qtyInThisShipment,
                        QuantityInAllShipments = qtyInAllShipments,
                        QuantityToAdd = maxQtyToAdd,
                    };
                    //rental info
                    if (orderItem.Product.IsRental)
                    {
                        var rentalStartDate = orderItem.RentalStartDateUtc.HasValue ? orderItem.Product.FormatRentalDate(orderItem.RentalStartDateUtc.Value) : "";
                        var rentalEndDate = orderItem.RentalEndDateUtc.HasValue ? orderItem.Product.FormatRentalDate(orderItem.RentalEndDateUtc.Value) : "";
                        shipmentItemModel.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                            rentalStartDate, rentalEndDate);
                    }

                    model.Items.Add(shipmentItemModel);
                }
            }

            if (!prepareShipmentEvent || string.IsNullOrEmpty(shipment.TrackingNumber))
                return model;

            var shipmentTracker = shipment.GetShipmentTracker(_shippingService, _shippingSettings);
            if (shipmentTracker == null)
                return model;

            model.TrackingNumberUrl = shipmentTracker.GetUrl(shipment.TrackingNumber);
            if (!_shippingSettings.DisplayShipmentEventsToStoreOwner)
                return model;

            var shipmentEvents = shipmentTracker.GetShipmentEvents(shipment.TrackingNumber);
            if (shipmentEvents == null)
                return model;

            foreach (var shipmentEvent in shipmentEvents)
            {
                var shipmentStatusEventModel = new ShipmentModel.ShipmentStatusEventModel();
                var shipmentEventCountry = _countryService.GetCountryByTwoLetterIsoCode(shipmentEvent.CountryCode);
                shipmentStatusEventModel.Country = shipmentEventCountry != null
                    ? shipmentEventCountry.GetLocalized(x => x.Name)
                    : shipmentEvent.CountryCode;
                shipmentStatusEventModel.Date = shipmentEvent.Date;
                shipmentStatusEventModel.EventName = shipmentEvent.EventName;
                shipmentStatusEventModel.Location = shipmentEvent.Location;
                model.ShipmentStatusEvents.Add(shipmentStatusEventModel);
            }

            return model;
        }

        [HttpPost]
        [Area(AreaNames.Admin)]
        //[HttpsRequirement(SslRequirement.Yes)]
        //[AdminAntiForgery]
        //[ValidateIpAddress]
        //[AuthorizeAdmin]
        //[ValidateVendor]
        public IActionResult RegisterPackingRequest(PackingRequestModel model)
        {
            if (_packingRequestService.ShipmentHasPackingRequest(model.ShipmentId))
            {
                return BadRequest(new { message = "درخواست بسته بندی قبلا ارسال شده است" });
            }
            var packingReq = _packingRequestService.SearchPackingRequests(model.ShipmentId, model.KartonSizeItemName, model.Length,
                model.Width, model.Height, isSmsSent: false).FirstOrDefault();
            if (packingReq == null)
            {
                packingReq = new Domain.Tbl_ShipmentPackingRequest()
                {
                    Height = model.Height,
                    Width = model.Width,
                    Length = model.Length,
                    KartonSizeItemName = model.KartonSizeItemName,
                    ShipmentId = model.ShipmentId,
                    CustomerPhone = model.CustomerPhoneNumber,
                    Status = Enums.ShipmentPackingRequestStatus.Requested
                };
                _packingRequestService.InsertPackingRequest(packingReq);
            }
            try
            {
                //var order = _orderService.GetOrderById(model.OrderId);
                //var customer = _customerService.GetCustomerById(order.CustomerId);
                var msg = $"مشتری گرامی محموله شماره {model.ShipmentId} شما نیاز به بسته بندی {model.KartonSizeItemName} دارد، لطفا نسبت به خرید بسته بندی از طریق سایت برای این محموله اقدام نمایید {Environment.NewLine} امنیتو";
                packingReq.IsSmsSent = _notificationService._sendSms(model.CustomerPhoneNumber, msg);
                _packingRequestService.UpdatePackingRequest(packingReq);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "درخواست بسته بندی ثبت شد اما پیامک ارسال نشد" });
            }


            return Ok();
        }

        [Area(AreaNames.Admin)]
        public IActionResult GetOrderItemsCost(int orderId, int shipmentId)
        {
            var items = _orderCostService.GetOrderItemsCost(orderId, shipmentId);

            return Ok(items);
        }
    }
}