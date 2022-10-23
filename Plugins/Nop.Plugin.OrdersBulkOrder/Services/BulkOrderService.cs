using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Orders.BulkOrder.Models;
using Nop.Plugin.Orders.MultiShipping.Model;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Plugin.Orders.MultiShipping.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Extensions;
using Nop.Web.Factories;
using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Nop.Plugin.Orders.BulkOrder.Services
{
    public class BulkOrderService : IBulkOrderService
    {
        #region fields

        private readonly INewCheckout _newCheckout;
        private readonly ILocalizationService _localizationService;
        private readonly IDiscountService _discountService;
        private readonly IRepository<BulkOrderModel> _bulkOrderRepository;

        private readonly ICategoryService _categoryService;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly ICustomerService _customerService;
        private readonly IDbContext _dbContext;
        private readonly IExtnOrderProcessingService _extnOrderProcessingService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IPaymentService _paymentService;
        private readonly IProductService _productService;
        private readonly IRepository<ShippingMethod> _shipingMethodRepository;
        private readonly IRepository<StateProvince> _stateProvinceRepository;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IRepository<Address> _addressRepository;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly IAddressService _addressService;
        private readonly IOrderService _orderService;
        private readonly IRewardPointService _rewardPointService;
        private readonly ICartonService _cartonService;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IWebHelper _webHelper;
        #endregion

        #region ctor
        public BulkOrderService(
            ILocalizationService localizationService,
          IAddressService addressService,
          IWorkContext workContext,
          ICustomerService customerService,
          IStoreContext storeContext,
          IRepository<ShippingMethod> shippingMethodRepository,
          IExtnOrderProcessingService extnOrderProcessingService,
          IPaymentService paymentService,
          IRepository<BulkOrderModel> bulkOrderRepository,
          IRepository<Country> countryRepository,
          IHostingEnvironment hostingEnvironment,
          IRepository<StateProvince> stateProvinceRepository,
          IRepository<Customer> customerRepository,
          IDbContext dbContext,
          ICategoryService categoryService,
          IProductService productService,
          IRepository<Address> addressRepository,
          IShoppingCartService shoppingCartService,
          ICheckoutModelFactory checkoutModelFactory,
          IDiscountService discountService,
        IOrderService orderService,
        INewCheckout newCheckout,
        IRewardPointService rewardPointService,
        ICartonService cartonService,
        IWebHelper webHelper,
        IExtendedShipmentService extendedShipmentService
          )
        {
            _rewardPointService = rewardPointService;
            _cartonService = cartonService;
            _extendedShipmentService = extendedShipmentService;
            _newCheckout = newCheckout;
            _localizationService = localizationService;
            _discountService = discountService;
            _addressService = addressService;
            _checkoutModelFactory = checkoutModelFactory;
            _shoppingCartService = shoppingCartService;
            _addressRepository = addressRepository;
            _dbContext = dbContext;
            _workContext = workContext;
            _customerService = customerService;
            _storeContext = storeContext;
            _shipingMethodRepository = shippingMethodRepository;
            _bulkOrderRepository = bulkOrderRepository;
            _extnOrderProcessingService = extnOrderProcessingService;
            _paymentService = paymentService;
            _countryRepository = countryRepository;
            _stateProvinceRepository = stateProvinceRepository;
            _hostingEnvironment = hostingEnvironment;
            _customerRepository = customerRepository;
            _categoryService = categoryService;
            _productService = productService;
            _orderService = orderService;
            _webHelper = webHelper;
        }
        #endregion

        #region Postkhone

        public List<PlaceOrderResult> ProcessOrderList_PostKhones(BulkOrderModel bulkOrder = null, List<CheckoutItemApi> apiModel = null,
        int customerId = 0, string discountCouponCode = null)
        {
            List<PlaceOrderResult> Lst_placeOrderResult = new List<PlaceOrderResult>();

            if (bulkOrder != null)
            {
                try
                {
                    bulkOrder.IsInProcceing = true;
                    _bulkOrderRepository.Update(bulkOrder);
                    long total = 0;
                    string error = "";
                    var watch = StartStopwatch();
                    var listOfOrderList = ReadexcelContent_Postkhone(bulkOrder);
                    //RestartStopwatch(watch, "خواندن رکورد های فایل اکسل", ref total);
                    List<ListOfOrderList> SeprateService = SpacifyServcie(listOfOrderList, bulkOrder);//, out error);
                    if (SeprateService.Any(p => !string.IsNullOrEmpty(p.error)))
                    {
                        Lst_placeOrderResult.Add(new PlaceOrderResult()
                        {
                            Errors = new List<string>() { SeprateService.Where(p => !string.IsNullOrEmpty(p.error)).First().error }
                        });
                        return Lst_placeOrderResult;
                    }
                    //RestartStopwatch(watch, "مشحص کردن سرویس برای هر رکود اکسل", ref total);

                    int price = 0;
                    foreach (var item in SeprateService)
                    {
                        foreach (var subItem in item.list)
                        {
                            subItem.CalcPrice += getCheckoutAttributePrice(subItem, subItem.ServiceId.Value);
                            price += subItem.CalcPrice;
                            price += 10000;//برای مبالغی که شاید محاسبه نشده باشد
                        }
                    }
                    price += (SeprateService.Count() * 60000);//محض احتیاط
                    int rewardPointsBalance =
                               _rewardPointService.GetRewardPointsBalance(bulkOrder.CustomerId, _storeContext.CurrentStore.Id);
                    #region CheckWallet
                    if (bulkOrder.IsCod)
                    //{
                    //    if (price > rewardPointsBalance)
                    //    {
                    //        Lst_placeOrderResult.Add(new PlaceOrderResult()
                    //        {
                    //            Errors = new List<string>() { "موجودی کیف پول برای این سفارش می بایست حداقل "+  Convert.ToInt32(price).ToString("N0")
                    //                                                                                     + " ريال باشد. موجودی فعلی "+ rewardPointsBalance.ToString("N0") + " erroCode:930" }
                    //        });
                    //        return Lst_placeOrderResult;
                    //    }
                    //}
                    //else
                    {
                        if (rewardPointsBalance < 1500000)
                        {
                            Lst_placeOrderResult.Add(new PlaceOrderResult()
                            {
                                Errors = new List<string>() { "موجوی کیف پول شما باید حداقل 1،500،000 ریال باشد erroCode:940" },
                                PlacedOrder = null
                            });
                            return Lst_placeOrderResult;
                        }
                    }
                    #endregion
                    //RestartStopwatch(watch, "قسمت چک کردن مبلغ کیف پول و محاسبه قیمت تقریبی کل", ref total);
                    if (listOfOrderList == null)
                    {
                        Lst_placeOrderResult.Add(new PlaceOrderResult()
                        {
                            Errors = new List<string>() { error }
                        });
                        return Lst_placeOrderResult;
                    }
                    foreach (var item in SeprateService)
                    {
                        var orderList = item.list;
                        Lst_placeOrderResult.Add(ProccessExcelOrder_Postkhone(orderList, bulkOrder));
                    }
                    //RestartStopwatch(watch, "ثبت سفارش مربوط به فایل اکسل", ref total);
                    //watch.Stop();
                    bulkOrder.IsInProcceing = false;
                    _bulkOrderRepository.Update(bulkOrder);
                    return Lst_placeOrderResult;
                }
                catch (Exception ex)
                {
                    bulkOrder.IsInProcceing = false;
                    _bulkOrderRepository.Update(bulkOrder);
                    _newCheckout.LogException(ex);
                    Lst_placeOrderResult.Add(new PlaceOrderResult()
                    {
                        Errors = new List<string>() { "خطایی در زمان ثبت سفارش شما رخ داده. با پشتیبانی تماس بگیرید" }
                    });
                    return Lst_placeOrderResult;
                }

            }
            else if (apiModel != null)
            {
                Lst_placeOrderResult.Add(ProccessApiOrder_postkhone(apiModel, customerId, discountCouponCode));
            }

            Lst_placeOrderResult.Add(new PlaceOrderResult()
            {
                Errors = new List<string>() { "اطلاعات جهت ثبت سفارش معتبر نمی باشد" }
            });
            return Lst_placeOrderResult;
        }

        private List<ListOfOrderList> SpacifyServcie(ListOfOrderList listOfOrderList, BulkOrderModel bulkOrder)//, out string error)
        {
            OrderList myItem = null;
            int state = 0;
            try
            {
                state = 1;
                string _error = "";
                //var _item = listOfOrderList.list.First();

                int SenderAddressId = ProcessAddress(bulkOrder.Sender_FristName,
                    bulkOrder.Sender_LastName, bulkOrder.Sender_mobile, bulkOrder.Sender_Email, null, null,
                    bulkOrder.Sender_Address, null, null,
                    bulkOrder.Sender_City, bulkOrder.Sender_State, bulkOrder.Sender_PostCode,
                    bulkOrder.CustomerId, bulkOrder.Sender_Lat, bulkOrder.Sender_Lon);
                if (SenderAddressId == 0)
                {
                    _error = "آدرس فرستنده سفارش انبوه نامعتبر می باشد";
                    common.Log(_error, "");
                    return new List<ListOfOrderList>() { new ListOfOrderList() { error = _error } };
                }
                var senderAddress = _addressService.GetAddressById(SenderAddressId);
                if (_newCheckout.isInvalidSernder(senderAddress.CountryId.Value, senderAddress.StateProvinceId.Value))
                {
                    _error = "در حاضر امکان ثبت سفارش از مبدا مورد نظر وجود ندارد";
                    common.Log(_error, "");
                    return new List<ListOfOrderList>() { new ListOfOrderList() { error = _error } };
                }

                int i = 0;
                foreach (var item in listOfOrderList.list)
                {
                    state = 2;
                    i++;
                    myItem = item;
                    item.SenderAddressId = SenderAddressId;
                    //var senderAddress = _addressService.GetAddressById(SenderAddressId);
                    var shippingAddressId = ProcessAddress(item.Reciver_FristName,
                        item.Reciver_LastName, item.Reciver_mobile, item.Reciver_Email, null, null,
                        item.Reciver_Address, null, item.Reciver_City,
                        item.Reciver_State, item.Reciver_Country, item.Reciver_PostCode,
                        bulkOrder.CustomerId);
                    if (shippingAddressId == 0)
                    {
                        _error = $"اطلاعات آدرس گیرنده در رکورد {i} نامعتبر می باشد";
                        common.Log(_error, "");
                        return new List<ListOfOrderList>() { new ListOfOrderList() { error = _error } };
                    }
                    var ReciverAddress = _addressService.GetAddressById(shippingAddressId);
                    item.ReciverAddressId = shippingAddressId;
                    state = 3;
                    if (!string.IsNullOrEmpty(item.Reciver_vilage))
                    {
                        if (!bulkOrder.IsCod)
                        {
                            if (_workContext.CurrentCustomer.Id != 13653617)//اسفندی نماینده تبریز
                            {
                                if (item.Wehight_g > 5000)
                                {
                                    item.ServiceId = 655;
                                }
                                else
                                    item.ServiceId = bulkOrder.ServiceSort.Value == 1 ? 654 : 655;
                            }
                            else
                                item.ServiceId = bulkOrder.ServiceSort.Value == 1 ? 654 : 655;
                        }
                        else
                        {
                            if (_workContext.CurrentCustomer.Id != 13653617)//اسفندی نماینده تبریز
                            {
                                if (item.Wehight_g > 5000)
                                {
                                    item.ServiceId = 670;
                                }
                                else
                                    item.ServiceId = bulkOrder.ServiceSort.Value == 1 ? 667 : 670;
                            }
                            else
                                item.ServiceId = bulkOrder.ServiceSort.Value == 1 ? 667 : 670;
                        }
                        item.CalcPrice = 0;
                    }
                    else
                    {
                        var result = getServiceInfo(new _getServiceInfoModel()
                        {
                            AproximateValue = item.ApproximateValue,
                            boxType = ((item.BoxType == "A") ? (byte)0 : (byte)1),
                            weightItem = item.Wehight_g,
                            Content = item.GoodsType,
                            height = item.height.GetValueOrDefault(0),
                            IsCod = bulkOrder.IsCod,
                            length = item.length.GetValueOrDefault(0),
                            ListType = bulkOrder.ServiceSort.HasValue ? (bulkOrder.ServiceSort.Value == 1 ? 2 : 1) : 2,
                            receiverStateId = ReciverAddress.CountryId.Value,
                            receiverTownId = ReciverAddress.StateProvinceId.Value,
                            senderStateId = senderAddress.CountryId.Value,
                            senderTownId = senderAddress.StateProvinceId.Value,
                            width = item.width.GetValueOrDefault(0),
                            customerId = _workContext.CurrentCustomer.Id,
                            serviceId = bulkOrder.ServiceId
                        }).GetAwaiter().GetResult();

                        if (result == null || !result.Any() || result.All(p => p.ServiceId == 0))
                        {

                            _error = $"سرویسی برای رکورد شماره {i} یافت نشد";
                            common.Log(_error, "");
                            item.ServiceId = bulkOrder.ServiceSort.Value == 1 ? 654 : 655;
                            item.CalcPrice = 0;
                        }

                        if (result.Any())
                        {
                            item.ServiceId = result.First().ServiceId;
                            item.CalcPrice = result.First().Price;
                        }
                    }
                }
                state = 4;
                var serviceIds = listOfOrderList.list.Select(p => p.ServiceId).Distinct().ToList();
                List<ListOfOrderList> seprateList = new List<ListOfOrderList>();
                foreach (var id in serviceIds)
                {
                    if (id == 0)
                        continue;
                    ListOfOrderList newListOforderlist = new ListOfOrderList();
                    newListOforderlist.list = listOfOrderList.list.Where(p => p.ServiceId == id).ToList();
                    seprateList.Add(newListOforderlist);
                }
                //error = "";
                return seprateList;
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                string _error = "";
                return new List<ListOfOrderList>() { new ListOfOrderList() { error = _error } };
            }
        }

        private PlaceOrderResult ProccessApiOrder_postkhone(List<CheckoutItemApi> orderList, int CustomerId, string discountCouponCode)
        {
            long total = 0;

            if (orderList.Select(p => p.ServiceId).Distinct().Count() > 1)
            {
                return new PlaceOrderResult()
                {
                    Errors = new List<string>() { "در هر سفارش از چند سرویس به صورت همزمان نمی توانید استفاده کنید" },
                    PlacedOrder = null
                };
            }
            var BillingAddressRecord = orderList[0];
            var billingAddressId = ProcessAddress(BillingAddressRecord.Sender_FristName,
                BillingAddressRecord.Sender_LastName, BillingAddressRecord.Sender_mobile, BillingAddressRecord.Sender_Email, null, null,
                BillingAddressRecord.Sender_Address, null, BillingAddressRecord.Sender_City,
                BillingAddressRecord.Sender_townId, BillingAddressRecord.Sender_StateId, BillingAddressRecord.Sender_PostCode,
                CustomerId);

            var customer = _customerService.GetCustomerById(CustomerId);
            customer.BillingAddress = _addressService.GetAddressById(billingAddressId);
            _customerService.UpdateCustomer(customer);

            if (customer.ShoppingCartItems.Any())
                CleanShopingCartItem(customer);

            var lstExShipments = new List<ExnShippmentModel>();
            int hagheSabt = 0;
            if (_storeContext.CurrentStore.Id == 5)
            {
                hagheSabt = _newCheckout.CalcHagheSabet(_workContext.CurrentCustomer.Id, orderList.First().ServiceId, 0);
            }
            string shipingMethod = _shipingMethodRepository.Table.OrderBy(p => p.DisplayOrder).First().Name;
            foreach (var orderItem in orderList)
            {
                var shippingAddressId = ProcessAddress(orderItem.Reciver_FristName,
                    orderItem.Reciver_LastName, orderItem.Reciver_mobile, orderItem.Reciver_Email, null, null,
                    orderItem.Reciver_Address, null, orderItem.Reciver_City,
                    orderItem.Reciver_townId, orderItem.Reciver_StateId, orderItem.Reciver_PostCode,
                    CustomerId);
                if (orderItem.receiver_ForeginCountry > 0)
                {
                    var add = _addressService.GetAddressById(shippingAddressId);
                    add.Address2 = orderItem.receiver_ForeginCountry + "|" + orderItem.receiver_ForeginCountryName;
                    _addressService.UpdateAddress(add);
                }
                if (string.IsNullOrEmpty(orderItem.boxType) && _storeContext.CurrentStore.Id == 5 && orderItem.ServiceId != 701)
                {
                    return new PlaceOrderResult()
                    {
                        Errors = new List<string>() { "نوع مرسوله را مشخص نمایید" },
                        PlacedOrder = null
                    };
                }

                var product = DetectProduct(orderItem.ServiceId);
                if (product == null)
                {
                    return new PlaceOrderResult()
                    {
                        Errors = new List<string>() { "درحال حاضر سرویس مورد نظر شما ارائه نمی شود" },
                        PlacedOrder = null
                    };
                }
                if (orderItem.AgentSaleAmount > 0 && !(customer.CustomerRoles.Any(p => p.Active && new int[] { 1, 7 }.Contains(p.Id)) && !string.IsNullOrEmpty(discountCouponCode)))
                {
                    orderItem.AgentSaleAmount = 0;
                }
                var checkoutAttributeXml =
                    getCheckoutAttributeXmlApi(orderItem, product.Id, hagheSabt);
                if (orderItem.Count == 0)
                    orderItem.Count = 1;

                var warnings = _shoppingCartService.AddToCart(customer, product, ShoppingCartType.ShoppingCart,
                      _storeContext.CurrentStore.Id, checkoutAttributeXml,
                      0, automaticallyAddRequiredProductsIfEnabled: false, quantity: orderItem.Count);
                if (warnings.Any())
                {
                    return new PlaceOrderResult()
                    {
                        Errors = warnings,
                        PlacedOrder = null
                    };
                }

                var shoppingCartItem = _shoppingCartService.FindShoppingCartItemInTheCart(customer.ShoppingCartItems.ToList(),
                    ShoppingCartType.ShoppingCart, product, checkoutAttributeXml);

                if (shoppingCartItem == null)
                {
                    return new PlaceOrderResult()
                    {
                        Errors = new List<string>() { "خطا در زمان فرایند سفارش-Error Code:920" },
                        PlacedOrder = null
                    };
                }

                var shoppingCartIemId = shoppingCartItem.Id;
                for (int i = 0; i < orderItem.Count; i++)
                {
                    var newShipment = new Shipment
                    {
                        DeliveryDateUtc = null
                    };

                    var newItem = new ShipmentItem
                    {
                        OrderItemId = shoppingCartIemId,
                        Quantity = 1
                    };
                    newShipment.ShipmentItems.Add(newItem);

                    lstExShipments.Add(new ExnShippmentModel
                    {
                        shipment = newShipment,
                        ShippmentAddressId = shippingAddressId,
                        ShippmentMethod = shipingMethod,
                    });
                }

            }

            if (lstExShipments.Any(p => p.ShippmentAddressId == 0))
                return new PlaceOrderResult()
                {
                    Errors = new List<string>() { "آدرس فرستنده دارای نقص می باشد" },
                    PlacedOrder = null
                };
            var paymentMethodModel = _checkoutModelFactory.PreparePaymentMethodModel(customer.ShoppingCartItems.ToList(), 0);
            string selectedPaymentMethodSystemName = orderList[0].IsCOD ? "Payments.CashOnDelivery" : (string)null;

            var ppr = new ProcessPaymentRequest
            {
                CustomerId = CustomerId,
                StoreId = _storeContext.CurrentStore.Id,
                PaymentMethodSystemName = selectedPaymentMethodSystemName
            };
            if (_workContext.CurrentCustomer.Id == 4144899 || _workContext.CurrentCustomer.AffiliateId == 1149)
            {
                int TotalWeight = orderList.Sum(p => p.Weight * p.Count);

                int Weight_V = 0;
                foreach (var item in orderList)
                {
                    if (!(item.width.HasValue && item.length.HasValue && item.height.HasValue))
                        continue;
                    Weight_V += (((item.length.Value * item.width.Value * item.height.Value) / 6000) * item.Count) * 1000;
                }
                if (Weight_V > TotalWeight)
                    TotalWeight = Weight_V;

                //int HagheMagharPrice = _newCheckout.CalcHagheMaghar(customer.BillingAddress, orderList[0].ServiceId, TotalWeight, _workContext.CurrentCustomer.Id);
                //if (HagheMagharPrice == 25000)
                //    discountCouponCode = "2500bdk";
                //else if (HagheMagharPrice == 30000)
                //    discountCouponCode = "3000bdk";
            }
            if (!string.IsNullOrEmpty(discountCouponCode))
            {
                List<string> Lst_Error = new List<string>();
                ApplyDiscountCoupon(customer, discountCouponCode, out Lst_Error);
                if (Lst_Error.Any())
                {
                    return new PlaceOrderResult()
                    {
                        Errors = Lst_Error,
                        PlacedOrder = null
                    };
                }
            }
            var result = _extnOrderProcessingService.PlaceOrderApi_Postkhone(ppr, lstExShipments, (orderList[0].orderSource > 0 ? orderList[0].orderSource : 3));//OrderRegistrationMethod.Api);
            if (result.Success)
            {
                bool needCheck = false;
                var pishtazResult = RegisterPishtazForForginRequest(result.PlacedOrder, orderList, out needCheck);
                if (needCheck)
                {
                    if (!pishtazResult.Success)
                    {
                        common.InsertOrderNote("سفارش پیشتاز برای پست خارجی ثبت نشد" + string.Join("\r\n", pishtazResult.Errors), result.PlacedOrder.Id);
                    }
                    else
                    {
                        _newCheckout.InsertRelatedOrder(result.PlacedOrder.Id, pishtazResult.PlacedOrder.Id, 0);
                        common.InsertOrderNote($"شماره سفارش پیشتاز متناطر با پست خارجی مورد نظر {pishtazResult.PlacedOrder.Id} می باشد", result.PlacedOrder.Id);
                    }
                }
            }
            return result;
        }


        public async Task<List<ServiceInfoModel>> getServiceInfo(_getServiceInfoModel model)
        {
            try
            {
                var result = new List<_ServiceInfo>();
                if (model.customerId == 0)
                {
                    result = _newCheckout.GetServiceInfoAnonymouse(model.senderStateId, model.senderTownId, model.receiverStateId, model.receiverTownId, model.weightItem,
                          model.AproximateValue, model.height, model.length, model.width, model.boxType.Value, model.Content,
                          (string.IsNullOrEmpty(model.dispatch_date) ? (DateTime?)null : Convert.ToDateTime(model.dispatch_date)),
                          model.PackingLoad, model.TruckType, model.VechileOptions, model.receiver_ForeginCountry, "", true, model.IsCod, model.serviceId
                          ).GetAwaiter().GetResult();
                }
                else
                {
                    result = _newCheckout.GetServiceInfo(model.senderStateId, model.senderTownId, model.receiverStateId, model.receiverTownId, model.weightItem,
                          model.AproximateValue, model.customerId, model.height, model.length, model.width, model.boxType.Value, model.Content,
                          (string.IsNullOrEmpty(model.dispatch_date) ? (DateTime?)null : Convert.ToDateTime(model.dispatch_date)),
                          model.PackingLoad, model.TruckType, model.VechileOptions, model.receiver_ForeginCountry, "", true, model.IsCod, model.serviceId
                          ).GetAwaiter().GetResult();
                }
                if (result == null)
                    return new List<ServiceInfoModel>();
                var data = result.Where(p => p.CanSelect == true).Select(p =>
                  new ServiceInfoModel()
                  {
                      IsCod = p.IsCod,
                      Price = p.Price,
                      ServiceId = p.ServiceId,
                      ServiceName = p.ServiceName,
                      SLA = p.SLA,
                      _Price = p._Price
                  }).ToList();
                if (model.ListType == 1)
                    return new List<ServiceInfoModel>() { data.OrderBy(p => p.SLA).First() };
                else if (model.ListType == 2)
                    return new List<ServiceInfoModel>() { data.OrderBy(p => p.Price).First() };
                return data;
            }
            catch (Exception ex)
            {
                Log("API خطا در زمان محاسبه قیمت برای ", ex.Message + (ex.InnerException != null ? "-->" + ex.InnerException.Message : ""));
                string error = "بروز اشکال در زمان دریافت لیست سرویس ها با پشتیبانی فنی تماس بگیرید";
                return new List<ServiceInfoModel>() { new ServiceInfoModel() { _Price = error } };
            }
        }
        public bool ReadExcelFile_PostKhone(MemoryStream stream
            , string fileName
            , int customerId
            , string discountCouponCode
            , bool PrintLogo
            , bool SendSms
            , int ServiceSort//1 = cheapest , 2 = fastest
            , int FileType//1 = domestic online pay,2 = domestic CashOnDelivery,3 = International
            , bool HasAccessToPrinter
            , out BulkOrderModel bulkOrdermodel,
            string Sender_FirstName,
            string Sender_LastName,
            string Sender_mobile,
            string Sender_Country,
            string Sender_State,
            string Sender_City,
            string Sender_PostCode,
            string Sender_Address,
            string Sender_Email,
            string Sender_lat,
            string Sender_long,
             int ServiceId = 0
            )
        {
            bulkOrdermodel = null;
            try
            {
                #region ExcelProccess

                var uploads = @"Plugins\Orders.BulkOrder\BulkOrderFile\";

                var filePath = Path.Combine(uploads, fileName);
                filePath = Path.Combine(_hostingEnvironment.ContentRootPath, filePath);

                using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
                {
                    stream.CopyTo(fileStream);
                }

                var excelReader = ExcelReaderFactory.CreateReader(stream);
                var result = excelReader.AsDataSet();
                stream.Close();
                var tb1 = result.Tables[1];
                var tb2 = result.Tables[2];
                result.Tables.Remove(tb1);
                result.Tables.Remove(tb2);
                result.DataSetName = "ArrayOfOrderList";
                result.Tables[0].TableName = "ListOfOrderList";

                //result.Tables[0].Columns.Add("Sender_FristName");
                //result.Tables[0].Columns.Add("Sender_LastName");
                //result.Tables[0].Columns.Add("Sender_mobile");
                //result.Tables[0].Columns.Add("Sender_Country");
                //result.Tables[0].Columns.Add("Sender_State");
                //result.Tables[0].Columns.Add("Sender_City");
                //result.Tables[0].Columns.Add("Sender_PostCode");
                //result.Tables[0].Columns.Add("Sender_Address");
                //result.Tables[0].Columns.Add("Sender_Email");
                //result.Tables[0].Columns.Add("Sender_Lat");
                //result.Tables[0].Columns.Add("Sender_Long");


                if (FileType == 2)//domestic CashOnDelivery
                {
                    result.Tables[0].Columns[0].ColumnName = "BoxSize";
                    result.Tables[0].Columns[1].ColumnName = "Wehight_g";
                    result.Tables[0].Columns[2].ColumnName = "NeedCarton";
                    result.Tables[0].Columns[3].ColumnName = "GoodsType";
                    result.Tables[0].Columns[4].ColumnName = "ApproximateValue";
                    result.Tables[0].Columns[5].ColumnName = "GetCodGoodsPrice";
                    result.Tables[0].Columns[6].ColumnName = "Insurance";

                    result.Tables[0].Columns[7].ColumnName = "Reciver_FristName";
                    result.Tables[0].Columns[8].ColumnName = "Reciver_LastName";
                    result.Tables[0].Columns[9].ColumnName = "Reciver_mobile";
                    result.Tables[0].Columns[10].ColumnName = "Reciver_Country";
                    result.Tables[0].Columns[11].ColumnName = "Reciver_State";
                    result.Tables[0].Columns[12].ColumnName = "Reciver_City";
                    result.Tables[0].Columns[13].ColumnName = "Reciver_vilage";
                    result.Tables[0].Columns[14].ColumnName = "Reciver_PostCode";
                    result.Tables[0].Columns[15].ColumnName = "Reciver_Address";
                    result.Tables[0].Columns[16].ColumnName = "Reciver_Email";
                    result.Tables[0].Columns[17].ColumnName = "AgentSaleAmount";

                    //result.Tables[0].Columns[18].ColumnName = "Sender_FristName";
                    //result.Tables[0].Columns[19].ColumnName = "Sender_LastName";
                    //result.Tables[0].Columns[20].ColumnName = "Sender_mobile";
                    //result.Tables[0].Columns[21].ColumnName = "Sender_Country";
                    //result.Tables[0].Columns[22].ColumnName = "Sender_State";
                    //result.Tables[0].Columns[23].ColumnName = "Sender_City";
                    //result.Tables[0].Columns[24].ColumnName = "Sender_PostCode";
                    //result.Tables[0].Columns[25].ColumnName = "Sender_Address";
                    //result.Tables[0].Columns[26].ColumnName = "Sender_Email";
                    //result.Tables[0].Columns[27].ColumnName = "HasAccessToPrinter";
                }
                else if (FileType == 1)//domestic online pay
                {
                    result.Tables[0].Columns[0].ColumnName = "BoxSize";
                    result.Tables[0].Columns[1].ColumnName = "Wehight_g";
                    result.Tables[0].Columns[2].ColumnName = "NeedCarton";
                    result.Tables[0].Columns[3].ColumnName = "GoodsType";
                    result.Tables[0].Columns[4].ColumnName = "ApproximateValue";
                    result.Tables[0].Columns[5].ColumnName = "Insurance";

                    result.Tables[0].Columns[6].ColumnName = "Reciver_FristName";
                    result.Tables[0].Columns[7].ColumnName = "Reciver_LastName";
                    result.Tables[0].Columns[8].ColumnName = "Reciver_mobile";
                    result.Tables[0].Columns[9].ColumnName = "Reciver_Country";
                    result.Tables[0].Columns[10].ColumnName = "Reciver_State";
                    result.Tables[0].Columns[11].ColumnName = "Reciver_City";
                    result.Tables[0].Columns[12].ColumnName = "Reciver_vilage";
                    result.Tables[0].Columns[13].ColumnName = "Reciver_PostCode";
                    result.Tables[0].Columns[14].ColumnName = "Reciver_Address";
                    result.Tables[0].Columns[15].ColumnName = "Reciver_Email";
                    result.Tables[0].Columns[16].ColumnName = "AgentSaleAmount";

                    //result.Tables[0].Columns[16].ColumnName = "Sender_FristName";
                    //result.Tables[0].Columns[17].ColumnName = "Sender_LastName";
                    //result.Tables[0].Columns[18].ColumnName = "Sender_mobile";
                    //result.Tables[0].Columns[19].ColumnName = "Sender_Country";
                    //result.Tables[0].Columns[20].ColumnName = "Sender_State";
                    //result.Tables[0].Columns[21].ColumnName = "Sender_City";
                    //result.Tables[0].Columns[22].ColumnName = "Sender_PostCode";
                    //result.Tables[0].Columns[23].ColumnName = "Sender_Address";
                    //result.Tables[0].Columns[24].ColumnName = "Sender_Email";
                    //result.Tables[0].Columns[26].ColumnName = "HasAccessToPrinter"
                }
                else if (FileType == 3) //International
                {

                }

                var row = result.Tables[0].Rows[0];
                result.Tables[0].Rows.Remove(row);

                //foreach (DataRow item in result.Tables[0].Rows)
                //{
                //    item["Sender_FristName"] = Sender_FirstName;
                //    item["Sender_LastName"] = Sender_LastName;
                //    item["Sender_mobile"] = Sender_mobile;
                //    item["Sender_Country"] = Sender_Country;
                //    item["Sender_State"] = Sender_State;
                //    item["Sender_City"] = Sender_City;
                //    item["Sender_PostCode"] = Sender_PostCode;
                //    item["Sender_Address"] = Sender_Address;
                //    item["Sender_Email"] = Sender_Email;
                //    item["Sender_Lat"] = Sender_lat;
                //    item["Sender_Long"] = Sender_long;
                //}

                string xmlString;
                using (TextWriter writer = new StringWriter())
                {
                    result.Tables[0].WriteXml(writer);
                    xmlString = writer.ToString();
                }

                //stream.Close();

                #endregion

                var serializer = new XmlSerializer(typeof(ListOfOrderList));
                var listOfOrderList = new ListOfOrderList();
                using (TextReader reader = new StringReader(xmlString))
                {
                    listOfOrderList = (ListOfOrderList)serializer.Deserialize(reader);
                }
                if (listOfOrderList.list.Any(p => string.IsNullOrEmpty(p.BoxSize)))
                {
                    return false;
                }
                foreach (var item in listOfOrderList.list)
                {
                    if (item.NeedCarton == "نـــــــــدارم")
                        item.Carton = item.BoxSize;
                    else
                        item.Carton = "کارتن نیاز ندارم.";
                    if (item.BoxSize.Contains("سایر"))
                        continue;
                    var dimantions = item.BoxSize.Split('(')[1].Replace(")", "").Replace("\"", "").Split('*');
                    item.width = (int)float.Parse(dimantions[0]);
                    item.length = (int)float.Parse(dimantions[1]);
                    if (dimantions.Length > 2)
                        item.height = (int)float.Parse(dimantions[2]);
                    else
                        item.height = 2;
                    if (item.GetCodGoodsPrice == "بلی")
                    {
                        item.CodGoodsPrice = item.ApproximateValue;
                    }
                }
                var recordCount = listOfOrderList.list.Count;
                var boModel = new BulkOrderModel
                {
                    CreateDate = DateTime.Now,
                    CustomerId = customerId,
                    FileName = fileName,
                    OrderCount = recordCount,
                    OrderStatusId = (int)OrderStatus.Pending,
                    PaymentStatusId = (int)PaymentStatus.Pending,
                    IsCod = FileType == 2,
                    discountCouponCode = discountCouponCode,
                    FileType = FileType,
                    SendSms = SendSms,
                    PrintLogo = PrintLogo,
                    ServiceSort = ServiceSort,
                    HasAccessToPrinter = HasAccessToPrinter,
                    ServiceId = ServiceId,
                    Sender_Address = Sender_Address,
                    Sender_Country = Sender_Country,
                    Sender_Email = Sender_Email,
                    Sender_FristName = Sender_FirstName,
                    Sender_LastName = Sender_LastName,
                    Sender_Lat = float.Parse(Sender_lat),
                    Sender_Lon = float.Parse(Sender_long),
                    Sender_mobile = Sender_mobile,
                    Sender_PostCode = Sender_PostCode,
                    Sender_State = Sender_State,
                    Sender_City = Sender_City
                };
                InsertBulkOrder(boModel);
                bulkOrdermodel = boModel;
                return true;
            }
            catch (Exception ex)
            {
                Log("خطا در زمان بارگذاری  اکسل سفارش انبوه",
                    ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                return false;
            }
        }

        private PlaceOrderResult ProccessExcelOrder_Postkhone(List<OrderList> orderList, BulkOrderModel bulkorder)
        {
            var Lst_NewCheckoutModel = new List<NewCheckoutModel>();

            foreach (var order1 in orderList)
            {
                #region CreateSpModel
                NewCheckoutModel ApiCheckoutModel = new NewCheckoutModel()
                {
                    AgentSaleAmount = order1.AgentSaleAmount,
                    boxType = order1.BoxType,
                    CartonSizeName = order1.Carton,
                    CodGoodsPrice = order1.CodGoodsPrice,
                    Count = 1,
                    discountCouponCode = bulkorder.discountCouponCode,
                    dispatch_date = null,
                    //getItNow 
                    GoodsType = order1.GoodsType,
                    HasAccessToPrinter = bulkorder.HasAccessToPrinter.GetValueOrDefault(false),
                    hasNotifRequest = bulkorder.SendSms.GetValueOrDefault(false),
                    height = order1.height,
                    width = order1.width,
                    length = order1.length,
                    InsuranceName = order1.Insurance,
                    IsCOD = bulkorder.IsCod,
                    receiver_ForeginCityName = null,
                    receiver_ForeginCountry = order1.receiver_ForeginCountry.GetValueOrDefault(0),
                    receiver_ForeginCountryName = order1.receiver_ForeginCountryName,
                    receiver_ForeginCountryNameEn = order1.receiver_ForeginCountryName,
                    ApproximateValue = order1.ApproximateValue,
                    RequestPrintAvatar = bulkorder.PrintLogo ?? false,
                    ReciverLat = 0,
                    ReciverLon = 0,
                    SenderLat = bulkorder.Sender_Lat,
                    SenderLon = bulkorder.Sender_Lon,
                    UbbarPackingLoad = null,
                    ServiceId = bulkorder.ServiceId,
                    _dispatch_date = null,
                    Weight = order1.Wehight_g,
                    VechileOptions = null,
                    UbbraTruckType = null
                };
                ApiCheckoutModel.billingAddressModel = new Address()
                {
                    //Address1 = order1.Sender_Address,
                    //City = order1.Sender_City,
                    //CountryId = 0,
                    //Country = new Country() {Name = order1.Sender_Country} ,
                    //StateProvinceId = 0,
                    //StateProvince = new StateProvince() {Name = order1.Sender_State },
                    //Email = order1.Sender_Email,
                    //FirstName = order1.Sender_FristName,
                    //LastName = order1.Sender_LastName,
                    //PhoneNumber = order1.Sender_mobile,
                    //ZipPostalCode = order1.Sender_PostCode,
                    Id = order1.SenderAddressId.Value
                };
                ApiCheckoutModel.shippingAddressModel = new Address()
                {
                    //Address1 = order1.Reciver_Address,
                    //City = order1.Reciver_City,
                    //CountryId = 0,
                    //Country = new Country() {Name = order1.Reciver_Country },
                    //StateProvinceId = 0,
                    //StateProvince = new StateProvince() {Name = order1.Reciver_State },
                    //Email = order1.Reciver_Email,
                    //FirstName = order1.Reciver_FristName,
                    //LastName = order1.Reciver_LastName,
                    //PhoneNumber = order1.Reciver_mobile,
                    //ZipPostalCode = order1.Reciver_PostCode,
                    Id = order1.ReciverAddressId.Value
                };
                #endregion
                Lst_NewCheckoutModel.Add(ApiCheckoutModel);
            }
            var inputModel = new NewCheckout_Sp_Input()
            {
                JsonOrderList = JsonConvert.SerializeObject(Lst_NewCheckoutModel),
                JsonData = JsonConvert.SerializeObject(new
                {
                    CustommerId = _workContext.CurrentCustomer.Id,
                    CustommerIp = _webHelper.GetCurrentIpAddress(),
                    StoreId = _storeContext.CurrentStore.Id,
                    BulkOrder = true,
                    FileName = bulkorder.FileName,
                    ServiceSort = bulkorder.ServiceSort,
                    bulkorderId = bulkorder.Id,
                    SourceId = 2
                })
            };
            try
            {
                var ret = _newCheckout.CheckoutBySp(inputModel, OrderRegistrationMethod.Excel, bulkorder.Id, false, false);
                var order = _orderService.GetOrderById(ret.orderId);
                var err = new List<string>();
                if (ret.ErrorCode == 0)
                {
                    return new PlaceOrderResult()
                    {
                        PlacedOrder = order,
                        Errors = err
                    };
                }
                else
                {
                    err.Add(ret.ErrorMessage);
                    return new PlaceOrderResult()
                    {
                        PlacedOrder = null,
                        Errors = err
                    };
                }
            }
            catch (Exception ex)
            {
                _newCheckout.LogException(ex);
                var err = new List<string>();
                err.Add("خطا در زمان ثبت سفارش");
                return new PlaceOrderResult()
                {
                    PlacedOrder = null,
                    Errors = err
                };
            }

            //*****************************
            long total = 0;
            var watch = StartStopwatch();
            //if (orderList.Select(p => p.ServiceType).Distinct().Count() > 1)
            //{
            //    return new PlaceOrderResult()
            //    {
            //        Errors = new List<string>() { "در هر سفارش از چند سرویس به صورت همزمان نمی توانید استفاده کنید" },
            //        PlacedOrder = null
            //    };
            //}

            var billingAddressId = orderList.First().SenderAddressId.Value;


            var customer = _customerService.GetCustomerById(bulkorder.CustomerId);
            customer.BillingAddress = _addressService.GetAddressById(billingAddressId);
            _customerService.UpdateCustomer(customer);

            if (customer.ShoppingCartItems.Any())
                CleanShopingCartItem(customer);
            //RestartStopwatch(watch, "از شروع تا خالی کردن سبد خرید", ref total);
            var lstExShipments = new List<ExnShippmentModel>();
            foreach (var orderItem in orderList)
            {
                if (orderItem.AgentSaleAmount > 0 &&
                    !(customer.CustomerRoles.Any(p => p.Active && new int[] { 1, 7 }.Contains(p.Id))))
                {
                    orderItem.AgentSaleAmount = 0;
                }
                var shippingAddressId = orderItem.ReciverAddressId.Value;

                if (shippingAddressId == 0)
                {
                    string address = (orderItem.Reciver_FristName ?? "") + "-" +
                          (orderItem.Reciver_LastName ?? "") + "-" +
                          (orderItem.Reciver_mobile ?? "") + "-" +
                          (orderItem.Reciver_Email ?? "") + "-" +
                          (orderItem.Reciver_Country ?? "") + "-" +
                          (orderItem.Reciver_State ?? "") + "-" +
                          (orderItem.Reciver_City ?? "") + "-" +
                          (orderItem.Reciver_Address ?? "");
                    return new PlaceOrderResult()
                    {
                        Errors = new List<string>() { $"آدرس گیرنده {address} قابل شناسایی نمی باشد" },
                        PlacedOrder = null
                    };
                }

                var product = DetectProduct(orderItem.ServiceId.Value);
                //RestartStopwatch(watch, "تشخبص محصول", ref total);
                var checkoutAttributeXml =
                    getCheckoutAttributeXml(orderItem, bulkorder, product.Id);
                //RestartStopwatch(watch, "برای سبد خرید xml ایجاد ", ref total);
                var warnings = _shoppingCartService.AddToCart(customer, product, ShoppingCartType.ShoppingCart,
                      _storeContext.CurrentStore.Id, checkoutAttributeXml,
                      0, automaticallyAddRequiredProductsIfEnabled: false);
                if (warnings.Any())
                {
                    return new PlaceOrderResult()
                    {
                        Errors = warnings,
                        PlacedOrder = null
                    };
                }
                var shopingCartIem = _shoppingCartService.FindShoppingCartItemInTheCart(customer.ShoppingCartItems.ToList(),
                    ShoppingCartType.ShoppingCart, product, checkoutAttributeXml);
                if (shopingCartIem == null)
                {
                    return new PlaceOrderResult()
                    {
                        Errors = new List<string>() { "خطا در زمان ثبت اطلاعات سبد خرید. با مسئول فنی تماس بگیرید" },
                        PlacedOrder = null
                    };
                }
                var shopingCartIemId = shopingCartIem.Id;
                //RestartStopwatch(watch, "اضافه کردن به سبد خرید", ref total);

                var newShipment = new Shipment
                {
                    DeliveryDateUtc = null
                };

                var newItem = new ShipmentItem
                {
                    OrderItemId = shopingCartIemId,
                    Quantity = 1
                };
                newShipment.ShipmentItems.Add(newItem);

                lstExShipments.Add(new ExnShippmentModel
                {
                    shipment = newShipment,
                    ShippmentAddressId = shippingAddressId,
                    ShippmentMethod = _shipingMethodRepository.Table.OrderBy(p => p.DisplayOrder).First().Name,
                });
                //RestartStopwatch(watch, "پایان حلقه", ref total);
            }

            if (lstExShipments.Any(p => p.ShippmentAddressId == 0))
            {
                return new PlaceOrderResult()
                {
                    Errors = new List<string>() { "آدرس های وارد شده دارای نقص می باشند" },
                    PlacedOrder = null
                };
            }
            if (_workContext.CurrentCustomer.Id == 4144899 || _workContext.CurrentCustomer.AffiliateId == 1149)
            {
                int TotalWeight = orderList.Sum(p => p.Wehight_g);

                int Weight_V = 0;
                foreach (var item in orderList)
                {
                    if (!(item.width.HasValue && item.length.HasValue && item.height.HasValue))
                        continue;
                    Weight_V += ((item.length.Value * item.width.Value * item.height.Value) / 6000) * 1000;
                }
                if (Weight_V > TotalWeight)
                    TotalWeight = Weight_V;

                //int HagheMagharPrice = _newCheckout.CalcHagheMaghar(customer.BillingAddress, orderList[0].ServiceId.Value, TotalWeight, _workContext.CurrentCustomer.Id);
                //if (HagheMagharPrice == 25000)
                //    bulkorder.discountCouponCode = "2500bdk";
                //else if (HagheMagharPrice == 30000)
                //    bulkorder.discountCouponCode = "3000bdk";
            }
            if (!string.IsNullOrEmpty(bulkorder.discountCouponCode))
            {
                List<string> Lst_Error = new List<string>();
                ApplyDiscountCoupon(customer, bulkorder.discountCouponCode, out Lst_Error);
                if (Lst_Error.Any())
                {
                    return new PlaceOrderResult()
                    {
                        Errors = Lst_Error,
                        PlacedOrder = null
                    };
                }
            }
            //RestartStopwatch(watch, "از پایان حلقه تا مشخص کردن نوع پرداخت", ref total);
            var paymentMethodModel = _checkoutModelFactory.PreparePaymentMethodModel(customer.ShoppingCartItems.ToList(), 0);
            //string selectedPaymentMethodSystemName = bulkorder.IsCod ? "Payments.CashOnDelivery" : (string)null;
            string selectedPaymentMethodSystemName = bulkorder.IsCod ? "Payments.CashOnDelivery" : "NopFarsi.Payments.SepShaparak";
            var ppr = new ProcessPaymentRequest
            {
                CustomerId = bulkorder.CustomerId,
                StoreId = _storeContext.CurrentStore.Id,
                PaymentMethodSystemName = selectedPaymentMethodSystemName
            };
            //RestartStopwatch(watch, "شروع ثبت سفارش در ناپ", ref total);
            var result = _extnOrderProcessingService.PlaceOrderApi_Postkhone(ppr, lstExShipments, 2, bulkorder.Id);//OrderRegistrationMethod.Excel);
            //RestartStopwatch(watch, "پایان ثبت سفارش در ناپ", ref total);
            return result;
        }

        public PlaceOrderResult RegisterPishtazForForginRequest(Order order, List<CheckoutItemApi> orderList, out bool needCheck)
        {
            if (!order.OrderItems.Any(p => p.Product.ProductCategories.Any(n => new int[] { 707, 719 }.Contains(n.CategoryId))))
            {
                needCheck = false;
                return null;
            }
            needCheck = true;
            var categoryId = order.OrderItems.Select(p => p.Product.ProductCategories.First().CategoryId).First();
            int reciverStateId = categoryId == 719 ? 581 : 582;
            string Addresss = (categoryId == 719 ? "خیابان آزادی، انتهای خیابان اسکندری شمالی-شماره 144- شرکت پستی راه آسمان آبی" : "بزرگراه حقانی غرب بین دیدار شمالی و 4 راه جهان کودک، پلاک -35 -شرکت پستی پی دی ای");
            string phoneNumber = (categoryId == 719 ? "09331473290" : "09050587273");
            string FirstName = (categoryId == 719 ? "شرکت" : "شرکت");
            string LastName = (categoryId == 719 ? "راه آسمان آبی" : "پست پی دی ای");
            foreach (var item in orderList)
            {
                item.ServiceId = 655;
                item.receiver_ForeginCityName = null;
                item.receiver_ForeginCountry = 0;
                item.receiver_ForeginCountryName = null;
                item.Reciver_Address = Addresss;
                item.Reciver_FristName = FirstName;
                item.Reciver_LastName = LastName;
                item.Reciver_StateId = 1;
                item.Reciver_townId = reciverStateId;
                item.Reciver_mobile = phoneNumber;
            }
            var pishtazResult = ProccessApiOrder_postkhone(orderList, order.CustomerId, "");
            return pishtazResult;
        }
        private ListOfOrderList ReadexcelContent_Postkhone(BulkOrderModel bulkOrder)
        {
            try
            {
                #region ExcelProccess

                var uploads = @"Plugins\Orders.BulkOrder\BulkOrderFile\";

                var filePath = Path.Combine(uploads, bulkOrder.FileName);
                filePath = Path.Combine(_hostingEnvironment.ContentRootPath, filePath);

                var fileStream = new FileStream(filePath, FileMode.Open);


                var excelReader = ExcelReaderFactory.CreateReader(fileStream);
                var result = excelReader.AsDataSet();
                fileStream.Close();
                var tb1 = result.Tables[1];
                var tb2 = result.Tables[2];
                result.Tables.Remove(tb1);
                result.Tables.Remove(tb2);
                result.DataSetName = "ArrayOfOrderList";
                result.Tables[0].TableName = "ListOfOrderList";
                if (bulkOrder.IsCod)
                {
                    result.Tables[0].Columns[0].ColumnName = "BoxSize";
                    result.Tables[0].Columns[1].ColumnName = "Wehight_g";
                    result.Tables[0].Columns[2].ColumnName = "NeedCarton";
                    result.Tables[0].Columns[3].ColumnName = "GoodsType";
                    result.Tables[0].Columns[4].ColumnName = "ApproximateValue";
                    result.Tables[0].Columns[5].ColumnName = "GetCodGoodsPrice";
                    result.Tables[0].Columns[6].ColumnName = "Insurance";

                    result.Tables[0].Columns[7].ColumnName = "Reciver_FristName";
                    result.Tables[0].Columns[8].ColumnName = "Reciver_LastName";
                    result.Tables[0].Columns[9].ColumnName = "Reciver_mobile";
                    result.Tables[0].Columns[10].ColumnName = "Reciver_Country";
                    result.Tables[0].Columns[11].ColumnName = "Reciver_State";
                    result.Tables[0].Columns[12].ColumnName = "Reciver_City";
                    result.Tables[0].Columns[13].ColumnName = "Reciver_vilage";
                    result.Tables[0].Columns[14].ColumnName = "Reciver_PostCode";
                    result.Tables[0].Columns[15].ColumnName = "Reciver_Address";
                    result.Tables[0].Columns[16].ColumnName = "Reciver_Email";
                    result.Tables[0].Columns[17].ColumnName = "AgentSaleAmount";

                    //result.Tables[0].Columns[17].ColumnName = "Sender_FristName";
                    //result.Tables[0].Columns[18].ColumnName = "Sender_LastName";
                    //result.Tables[0].Columns[19].ColumnName = "Sender_mobile";
                    //result.Tables[0].Columns[20].ColumnName = "Sender_Country";
                    //result.Tables[0].Columns[21].ColumnName = "Sender_State";
                    //result.Tables[0].Columns[22].ColumnName = "Sender_City";
                    //result.Tables[0].Columns[23].ColumnName = "Sender_PostCode";
                    //result.Tables[0].Columns[24].ColumnName = "Sender_Address";
                    //result.Tables[0].Columns[25].ColumnName = "Sender_Email";
                    //result.Tables[0].Columns[27].ColumnName = "HasAccessToPrinter";
                }
                else
                {
                    result.Tables[0].Columns[0].ColumnName = "BoxSize";
                    result.Tables[0].Columns[1].ColumnName = "Wehight_g";
                    result.Tables[0].Columns[2].ColumnName = "NeedCarton";
                    result.Tables[0].Columns[3].ColumnName = "GoodsType";
                    result.Tables[0].Columns[4].ColumnName = "ApproximateValue";
                    result.Tables[0].Columns[5].ColumnName = "Insurance";

                    result.Tables[0].Columns[6].ColumnName = "Reciver_FristName";
                    result.Tables[0].Columns[7].ColumnName = "Reciver_LastName";
                    result.Tables[0].Columns[8].ColumnName = "Reciver_mobile";
                    result.Tables[0].Columns[9].ColumnName = "Reciver_Country";
                    result.Tables[0].Columns[10].ColumnName = "Reciver_State";
                    result.Tables[0].Columns[11].ColumnName = "Reciver_City";
                    result.Tables[0].Columns[12].ColumnName = "Reciver_vilage";
                    result.Tables[0].Columns[13].ColumnName = "Reciver_PostCode";
                    result.Tables[0].Columns[14].ColumnName = "Reciver_Address";
                    result.Tables[0].Columns[15].ColumnName = "Reciver_Email";
                    result.Tables[0].Columns[16].ColumnName = "AgentSaleAmount";

                    //result.Tables[0].Columns[16].ColumnName = "Sender_FristName";
                    //result.Tables[0].Columns[17].ColumnName = "Sender_LastName";
                    //result.Tables[0].Columns[18].ColumnName = "Sender_mobile";
                    //result.Tables[0].Columns[19].ColumnName = "Sender_Country";
                    //result.Tables[0].Columns[20].ColumnName = "Sender_State";
                    //result.Tables[0].Columns[21].ColumnName = "Sender_City";
                    //result.Tables[0].Columns[22].ColumnName = "Sender_PostCode";
                    //result.Tables[0].Columns[23].ColumnName = "Sender_Address";
                    //result.Tables[0].Columns[24].ColumnName = "Sender_Email";
                    //result.Tables[0].Columns[26].ColumnName = "HasAccessToPrinter";

                }

                var row = result.Tables[0].Rows[0];
                result.Tables[0].Rows.Remove(row);


                result.Tables[0].Columns.Add("Sender_FristName");
                result.Tables[0].Columns.Add("Sender_LastName");
                result.Tables[0].Columns.Add("Sender_mobile");
                result.Tables[0].Columns.Add("Sender_Country");
                result.Tables[0].Columns.Add("Sender_State");
                result.Tables[0].Columns.Add("Sender_City");
                result.Tables[0].Columns.Add("Sender_PostCode");
                result.Tables[0].Columns.Add("Sender_Address");
                result.Tables[0].Columns.Add("Sender_Email");
                result.Tables[0].Columns.Add("Sender_Lat");
                result.Tables[0].Columns.Add("Sender_Lon");


                foreach (DataRow item in result.Tables[0].Rows)
                {
                    item["Sender_FristName"] = bulkOrder.Sender_FristName;
                    item["Sender_LastName"] = bulkOrder.Sender_LastName;
                    item["Sender_mobile"] = bulkOrder.Sender_mobile;
                    item["Sender_Country"] = bulkOrder.Sender_Country;
                    item["Sender_State"] = bulkOrder.Sender_State;
                    item["Sender_City"] = bulkOrder.Sender_City;
                    item["Sender_PostCode"] = bulkOrder.Sender_PostCode;
                    item["Sender_Address"] = bulkOrder.Sender_Address;
                    item["Sender_Email"] = bulkOrder.Sender_Email;
                    item["Sender_Lat"] = bulkOrder.Sender_Lat;
                    item["Sender_Lon"] = bulkOrder.Sender_Lon;
                }

                string xmlString;
                using (TextWriter writer = new StringWriter())
                {
                    result.Tables[0].WriteXml(writer);
                    xmlString = writer.ToString();
                }

                #endregion

                var serializer = new XmlSerializer(typeof(ListOfOrderList));
                var listOfOrderList = new ListOfOrderList();
                using (TextReader reader = new StringReader(xmlString))
                {
                    listOfOrderList = (ListOfOrderList)serializer.Deserialize(reader);
                }
                if (listOfOrderList.list.Any(p => string.IsNullOrEmpty(p.BoxSize)))
                {
                    return null;
                }
                foreach (var item in listOfOrderList.list)
                {
                    if (item.NeedCarton == "نـــــــــدارم")
                        item.Carton = item.BoxSize;
                    else
                        item.Carton = "کارتن نیاز ندارم.";
                    if (!item.BoxSize.Contains("سایر"))
                    {
                        var Dimentions = _newCheckout.getDimentionByName(item.BoxSize.Trim());
                        if (Dimentions != null)
                        {
                            item.width = Dimentions.Width;
                            item.length = Dimentions.Length;
                            item.height = Dimentions.Height;
                        }
                    }
                    if (item.BoxSize.Contains("سایر"))
                        continue;
                    var dimantions = item.BoxSize.Split('(')[1].Replace(")", "").Replace("\"", "").Split('*');
                    item.width = (int)float.Parse(dimantions[0]);
                    item.length = (int)float.Parse(dimantions[1]);
                    if (dimantions.Length > 2)
                        item.height = (int)float.Parse(dimantions[2]);
                    else
                        item.height = 2;
                    if (item.GetCodGoodsPrice == "بلی")
                    {
                        item.CodGoodsPrice = item.ApproximateValue;
                    }
                    item.BoxType = item.BoxSize.Contains("A") ? "پاکت" : "بسته";
                }
                return listOfOrderList;
            }
            catch (Exception ex)
            {
                Log("خطا در زمان خواندن  اکسل سفارش انبوه",
                    ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                return null;
            }
        }

        #endregion

        public bool ReadExcelFile(MemoryStream stream, string fileName, int customerId, bool IsCod, string discountCouponCode)
        {
            try
            {
                #region ExcelProccess

                var uploads = @"Plugins\Orders.BulkOrder\BulkOrderFile\";

                var filePath = Path.Combine(uploads, fileName);
                filePath = Path.Combine(_hostingEnvironment.ContentRootPath, filePath);

                using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
                {
                    stream.CopyTo(fileStream);
                }

                var excelReader = ExcelReaderFactory.CreateReader(stream);
                var result = excelReader.AsDataSet();
                stream.Close();
                var tb1 = result.Tables[1];
                var tb2 = result.Tables[2];
                result.Tables.Remove(tb1);
                result.Tables.Remove(tb2);
                result.DataSetName = "ArrayOfOrderList";
                result.Tables[0].TableName = "ListOfOrderList";
                if (IsCod)
                {
                    result.Tables[0].Columns[0].ColumnName = "ServiceType";
                    result.Tables[0].Columns[1].ColumnName = "GoodsType";
                    result.Tables[0].Columns[2].ColumnName = "ApproximateValue";
                    result.Tables[0].Columns[3].ColumnName = "CODGoodsPRice";
                    result.Tables[0].Columns[4].ColumnName = "Wehight_g";
                    result.Tables[0].Columns[5].ColumnName = "Insurance";
                    result.Tables[0].Columns[6].ColumnName = "Carton";

                    result.Tables[0].Columns[7].ColumnName = "Reciver_FristName";
                    result.Tables[0].Columns[8].ColumnName = "Reciver_LastName";
                    result.Tables[0].Columns[9].ColumnName = "Reciver_mobile";
                    result.Tables[0].Columns[10].ColumnName = "Reciver_Country";
                    result.Tables[0].Columns[11].ColumnName = "Reciver_State";
                    result.Tables[0].Columns[12].ColumnName = "Reciver_City";
                    result.Tables[0].Columns[13].ColumnName = "Reciver_PostCode";
                    result.Tables[0].Columns[14].ColumnName = "Reciver_Address";
                    result.Tables[0].Columns[15].ColumnName = "Reciver_Email";

                    result.Tables[0].Columns[16].ColumnName = "Sender_FristName";
                    result.Tables[0].Columns[17].ColumnName = "Sender_LastName";
                    result.Tables[0].Columns[18].ColumnName = "Sender_mobile";
                    result.Tables[0].Columns[19].ColumnName = "Sender_Country";
                    result.Tables[0].Columns[20].ColumnName = "Sender_State";
                    result.Tables[0].Columns[21].ColumnName = "Sender_City";
                    result.Tables[0].Columns[22].ColumnName = "Sender_PostCode";
                    result.Tables[0].Columns[23].ColumnName = "Sender_Address";
                    result.Tables[0].Columns[24].ColumnName = "Sender_Email";
                    result.Tables[0].Columns[25].ColumnName = "AgentSaleAmount";
                    result.Tables[0].Columns[26].ColumnName = "HasAccessToPrinter";

                }
                else
                {
                    result.Tables[0].Columns[0].ColumnName = "ServiceType";
                    result.Tables[0].Columns[1].ColumnName = "GoodsType";
                    result.Tables[0].Columns[2].ColumnName = "ApproximateValue";
                    result.Tables[0].Columns[3].ColumnName = "Wehight_g";
                    result.Tables[0].Columns[4].ColumnName = "Insurance";
                    result.Tables[0].Columns[5].ColumnName = "Carton";

                    result.Tables[0].Columns[6].ColumnName = "Reciver_FristName";
                    result.Tables[0].Columns[7].ColumnName = "Reciver_LastName";
                    result.Tables[0].Columns[8].ColumnName = "Reciver_mobile";
                    result.Tables[0].Columns[9].ColumnName = "Reciver_Country";
                    result.Tables[0].Columns[10].ColumnName = "Reciver_State";
                    result.Tables[0].Columns[11].ColumnName = "Reciver_City";
                    result.Tables[0].Columns[12].ColumnName = "Reciver_PostCode";
                    result.Tables[0].Columns[13].ColumnName = "Reciver_Address";
                    result.Tables[0].Columns[14].ColumnName = "Reciver_Email";

                    result.Tables[0].Columns[15].ColumnName = "Sender_FristName";
                    result.Tables[0].Columns[16].ColumnName = "Sender_LastName";
                    result.Tables[0].Columns[17].ColumnName = "Sender_mobile";
                    result.Tables[0].Columns[18].ColumnName = "Sender_Country";
                    result.Tables[0].Columns[19].ColumnName = "Sender_State";
                    result.Tables[0].Columns[20].ColumnName = "Sender_City";
                    result.Tables[0].Columns[21].ColumnName = "Sender_PostCode";
                    result.Tables[0].Columns[22].ColumnName = "Sender_Address";
                    result.Tables[0].Columns[23].ColumnName = "Sender_Email";
                    result.Tables[0].Columns[24].ColumnName = "AgentSaleAmount";
                    result.Tables[0].Columns[25].ColumnName = "HasAccessToPrinter";
                }

                var row = result.Tables[0].Rows[0];
                result.Tables[0].Rows.Remove(row);

                string xmlString;
                using (TextWriter writer = new StringWriter())
                {
                    result.Tables[0].WriteXml(writer);
                    xmlString = writer.ToString();
                }

                //stream.Close();

                #endregion

                var serializer = new XmlSerializer(typeof(ListOfOrderList));
                var listOfOrderList = new ListOfOrderList();
                using (TextReader reader = new StringReader(xmlString))
                {
                    listOfOrderList = (ListOfOrderList)serializer.Deserialize(reader);
                }

                var recordCount = listOfOrderList.list.Count;


                var boModel = new BulkOrderModel
                {
                    CreateDate = DateTime.Now,
                    CustomerId = customerId,
                    FileName = fileName,
                    OrderCount = recordCount,
                    OrderStatusId = (int)OrderStatus.Pending,
                    PaymentStatusId = (int)PaymentStatus.Pending,
                    IsCod = IsCod,
                    discountCouponCode = discountCouponCode
                };
                InsertBulkOrder(boModel);
                return true;
            }
            catch (Exception ex)
            {
                Log("خطا در زمان بارگذاری  اکسل سفارش انبوه",
                    ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                return false;
            }
        }
        public PlaceOrderResult ProcessOrderList(BulkOrderModel bulkOrder = null, List<CheckoutItemApi> apiModel = null,
            int customerId = 0, string discountCouponCode = null)
        {
            if (bulkOrder != null)
            {
                var listOfOrderList = ReadexcelContent(bulkOrder.FileName, bulkOrder.IsCod);
                if (listOfOrderList == null)
                    return null;
                var orderList = listOfOrderList.list;
                return ProccessExcelOrder(orderList, bulkOrder);
            }
            else if (apiModel != null)
            {
                return ProccessApiOrder(apiModel, customerId, discountCouponCode);
            }

            return new PlaceOrderResult()
            {
                Errors = new List<string>() { "اطلاعات جهت ثبت سفاسرش معتبر نمی باشد" }
            };
        }
        private PlaceOrderResult ProccessApiOrder(List<CheckoutItemApi> orderList, int CustomerId, string discountCouponCode)
        {
            if (orderList.Select(p => p.ServiceId).Distinct().Count() > 1)
            {
                return new PlaceOrderResult()
                {
                    Errors = new List<string>() { "در هر سفارش از چند سرویس به صورت همزمان نمی توانید استفاده کنید" },
                    PlacedOrder = null
                };
            }
            var BillingAddressRecord = orderList[0];
            var billingAddressId = ProcessAddress(BillingAddressRecord.Sender_FristName,
                BillingAddressRecord.Sender_LastName, BillingAddressRecord.Sender_mobile, BillingAddressRecord.Sender_Email, null, null,
                BillingAddressRecord.Sender_Address, null, BillingAddressRecord.Sender_City,
                BillingAddressRecord.Sender_townId, BillingAddressRecord.Sender_StateId, BillingAddressRecord.Sender_PostCode,
                CustomerId);

            var customer = _customerService.GetCustomerById(CustomerId);
            customer.BillingAddress = _addressService.GetAddressById(billingAddressId);
            _customerService.UpdateCustomer(customer);

            if (customer.ShoppingCartItems.Any())
                CleanShopingCartItem(customer);

            var lstExShipments = new List<ExnShippmentModel>();
            Product FirstProduct = null;
            foreach (var orderItem in orderList)
            {
                var shippingAddressId = ProcessAddress(orderItem.Reciver_FristName,
                    orderItem.Reciver_LastName, orderItem.Reciver_mobile, orderItem.Reciver_Email, null, null,
                    orderItem.Reciver_Address, null, orderItem.Reciver_City,
                    orderItem.Reciver_townId, orderItem.Reciver_StateId, orderItem.Reciver_PostCode,
                    CustomerId);

                var product = DetectProduct(orderItem.ServiceId);
                if (FirstProduct == null)
                    FirstProduct = product;
                if (orderItem.AgentSaleAmount > 0 && !(customer.CustomerRoles.Any(p => p.Active && new int[] { 1, 7 }.Contains(p.Id)) && !string.IsNullOrEmpty(discountCouponCode)))
                {
                    orderItem.AgentSaleAmount = 0;
                }
                var checkoutAttributeXml =
                    getCheckoutAttributeXmlApi(orderItem, product.Id);

                var warnings = _shoppingCartService.AddToCart(customer, product, ShoppingCartType.ShoppingCart,
                      _storeContext.CurrentStore.Id, checkoutAttributeXml,
                      0, automaticallyAddRequiredProductsIfEnabled: false);
                if (warnings.Any())
                {
                    return new PlaceOrderResult()
                    {
                        Errors = warnings,
                        PlacedOrder = null
                    };
                }

                var shoppingCartItem = _shoppingCartService.FindShoppingCartItemInTheCart(customer.ShoppingCartItems.ToList(),
                    ShoppingCartType.ShoppingCart, product, checkoutAttributeXml);
                if (shoppingCartItem == null)
                {
                    return new PlaceOrderResult()
                    {
                        Errors = new List<string>() { "خطا در زمان فرایند سفارش-Error Code:920" },
                        PlacedOrder = null
                    };
                }

                var shoppingCartIemId = shoppingCartItem.Id;

                var newShipment = new Shipment
                {
                    DeliveryDateUtc = null
                };

                var newItem = new ShipmentItem
                {
                    OrderItemId = shoppingCartIemId,
                    Quantity = 1
                };
                newShipment.ShipmentItems.Add(newItem);

                lstExShipments.Add(new ExnShippmentModel
                {
                    shipment = newShipment,
                    ShippmentAddressId = shippingAddressId,
                    ShippmentMethod = _shipingMethodRepository.Table.OrderBy(p => p.DisplayOrder).First().Name,
                });
            }

            if (lstExShipments.Any(p => p.ShippmentAddressId == 0))
                return null;
            var paymentMethodModel = _checkoutModelFactory.PreparePaymentMethodModel(customer.ShoppingCartItems.ToList(), 0);
            var categoryInfo = GetCategoryInfo(FirstProduct);
            string selectedPaymentMethodSystemName = categoryInfo.IsCod ? "Payments.CashOnDelivery" : (string)null;
            //string selectedPaymentMethodSystemName = orderList[0].IsCOD ? "Payments.CashOnDelivery" : selectedPaymentMethodSystemName = paymentMethodModel.PaymentMethods.Where(p =>
            //       p.PaymentMethodSystemName != "Payments.CashOnDelivery"
            //       && !string.IsNullOrEmpty(p.PaymentMethodSystemName)
            //    ).ToList()[0].PaymentMethodSystemName;
            var ppr = new ProcessPaymentRequest
            {
                CustomerId = CustomerId,
                StoreId = _storeContext.CurrentStore.Id,
                PaymentMethodSystemName = selectedPaymentMethodSystemName
            };
            if (!string.IsNullOrEmpty(discountCouponCode))
            {
                List<string> Lst_Error = new List<string>();
                ApplyDiscountCoupon(customer, discountCouponCode, out Lst_Error);
                if (Lst_Error.Any())
                {
                    return new PlaceOrderResult()
                    {
                        Errors = Lst_Error,
                        PlacedOrder = null
                    };
                }
            }
            return _extnOrderProcessingService.PlaceOrderApi(ppr, lstExShipments, 3);// OrderRegistrationMethod.Api);
        }

        private PlaceOrderResult ProccessExcelOrder(List<OrderList> orderList, BulkOrderModel bulkorder)
        {
            return null;
            //if (orderList.Select(p => p.ServiceType).Distinct().Count() > 1)
            //{
            //    return new PlaceOrderResult()
            //    {
            //        Errors = new List<string>() { "در هر سفارش از چند سرویس به صورت همزمان نمی توانید استفاده کنید" },
            //        PlacedOrder = null
            //    };
            //}
            //var senderAddress = orderList[0];
            //var billingAddressId = ProcessAddress(senderAddress.Sender_FristName,
            //    senderAddress.Sender_LastName, senderAddress.Sender_mobile, senderAddress.Sender_Email, null, null,
            //    senderAddress.Sender_Address, null, senderAddress.Sender_City,
            //    senderAddress.Sender_State, senderAddress.Sender_Country, senderAddress.Sender_PostCode,
            //    bulkorder.CustomerId);

            //var customer = _customerService.GetCustomerById(bulkorder.CustomerId);
            //customer.BillingAddress = _addressService.GetAddressById(billingAddressId);
            //_customerService.UpdateCustomer(customer);

            //if (customer.ShoppingCartItems.Any())
            //    CleanShopingCartItem(customer);
            //Product FirstProduct = null;
            //var lstExShipments = new List<ExnShippmentModel>();
            //foreach (var orderItem in orderList)
            //{
            //    if (orderItem.AgentSaleAmount > 0 &&
            //        !(customer.CustomerRoles.Any(p => p.Active && new int[] { 1, 7 }.Contains(p.Id))))
            //    {
            //        orderItem.AgentSaleAmount = 0;
            //    }
            //    var shippingAddressId = ProcessAddress(orderItem.Reciver_FristName,
            //        orderItem.Reciver_LastName, orderItem.Reciver_mobile, orderItem.Reciver_Email, null, null,
            //        orderItem.Reciver_Address, null, orderItem.Reciver_City,
            //        orderItem.Reciver_State, orderItem.Reciver_Country, orderItem.Reciver_PostCode,
            //        bulkorder.CustomerId);
            //    if (shippingAddressId == 0)
            //    {
            //        string address = (orderItem.Reciver_FristName ?? "") + "-" +
            //              (orderItem.Reciver_LastName ?? "") + "-" +
            //              (orderItem.Reciver_mobile ?? "") + "-" +
            //              (orderItem.Reciver_Email ?? "") + "-" +
            //              (orderItem.Reciver_Country ?? "") + "-" +
            //              (orderItem.Reciver_State ?? "") + "-" +
            //              (orderItem.Reciver_City ?? "") + "-" +
            //              (orderItem.Reciver_Address ?? "");
            //        return new PlaceOrderResult()
            //        {
            //            Errors = new List<string>() { $"آدرس گیرنده {address} قابل شناسایی نمی باشد" },
            //            PlacedOrder = null
            //        };
            //    }

            //    var product = DetectProduct(orderItem.Wehight_g, orderItem.ServiceType);
            //    if (FirstProduct == null)
            //        FirstProduct = product;
            //    var checkoutAttributeXml =
            //        getCheckoutAttributeXml(orderItem, bulkorder, product.Id);

            //    var warnings = _shoppingCartService.AddToCart(customer, product, ShoppingCartType.ShoppingCart,
            //          _storeContext.CurrentStore.Id, checkoutAttributeXml,
            //          0, automaticallyAddRequiredProductsIfEnabled: false);
            //    if (warnings.Any())
            //    {
            //        return new PlaceOrderResult()
            //        {
            //            Errors = warnings,
            //            PlacedOrder = null
            //        };
            //    }
            //    var shopingCartIem = _shoppingCartService.FindShoppingCartItemInTheCart(customer.ShoppingCartItems.ToList(),
            //        ShoppingCartType.ShoppingCart, product, checkoutAttributeXml);
            //    if (shopingCartIem == null)
            //    {
            //        return new PlaceOrderResult()
            //        {
            //            Errors = new List<string>() { "خطا در زمان ثبت اطلاعات سبد خرید. با مسئول فنی تماس بگیرید" },
            //            PlacedOrder = null
            //        };
            //    }
            //    var shopingCartIemId = shopingCartIem.Id;


            //    var newShipment = new Shipment
            //    {
            //        DeliveryDateUtc = null
            //    };

            //    var newItem = new ShipmentItem
            //    {
            //        OrderItemId = shopingCartIemId,
            //        Quantity = 1
            //    };
            //    newShipment.ShipmentItems.Add(newItem);

            //    lstExShipments.Add(new ExnShippmentModel
            //    {
            //        shipment = newShipment,
            //        ShippmentAddressId = shippingAddressId,
            //        ShippmentMethod = _shipingMethodRepository.Table.OrderBy(p => p.DisplayOrder).First().Name,
            //    });
            //}

            //if (lstExShipments.Any(p => p.ShippmentAddressId == 0))
            //{
            //    return new PlaceOrderResult()
            //    {
            //        Errors = new List<string>() { "آدرس های وارد شده دارای نقص می باشند" },
            //        PlacedOrder = null
            //    };
            //}
            //if (!string.IsNullOrEmpty(bulkorder.discountCouponCode))
            //{
            //    List<string> Lst_Error = new List<string>();
            //    ApplyDiscountCoupon(customer, bulkorder.discountCouponCode, out Lst_Error);
            //    if (Lst_Error.Any())
            //    {
            //        return new PlaceOrderResult()
            //        {
            //            Errors = Lst_Error,
            //            PlacedOrder = null
            //        };
            //    }
            //}
            ////var paymentMethodModel = _checkoutModelFactory.PreparePaymentMethodModel(customer.ShoppingCartItems.ToList(), 0);
            //var categoryInfo = GetCategoryInfo(FirstProduct);
            //string selectedPaymentMethodSystemName = categoryInfo.IsCod ? "Payments.CashOnDelivery" : (string)null;
            ////if (bulkorder.IsCod)
            ////{
            ////    selectedPaymentMethodSystemName = "Payments.CashOnDelivery";
            ////}
            ////else
            ////    selectedPaymentMethodSystemName = paymentMethodModel.PaymentMethods.Where(p =>
            ////        p.PaymentMethodSystemName != "Payments.CashOnDelivery"
            ////        && !string.IsNullOrEmpty(p.PaymentMethodSystemName)
            ////    ).ToList()[0].PaymentMethodSystemName;
            //var ppr = new ProcessPaymentRequest
            //{
            //    CustomerId = bulkorder.CustomerId,
            //    StoreId = _storeContext.CurrentStore.Id,
            //    PaymentMethodSystemName = selectedPaymentMethodSystemName
            //};
            //return _extnOrderProcessingService.PlaceOrderApi(ppr, lstExShipments, 2);//OrderRegistrationMethod.Excel);
        }
        public bool ApplyDiscountCoupon(Customer currenCustomer, string discountcouponcode, out List<string> errorList)
        {
            var cart = currenCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();


            errorList = new List<string>();

            if (!String.IsNullOrWhiteSpace(discountcouponcode))
            {
                var discounts = _discountService.GetAllDiscountsForCaching(couponCode: discountcouponcode, showHidden: true)
                    .Where(d => d.RequiresCouponCode)
                    .ToList();
                if (discounts.Any())
                {
                    var userErrors = new List<string>();
                    var anyValidDiscount = discounts.Any(discount =>
                    {
                        var validationResult = _discountService.ValidateDiscount(discount, currenCustomer, new[] { discountcouponcode });
                        userErrors.AddRange(validationResult.Errors);

                        return validationResult.IsValid;
                    });
                    if (anyValidDiscount)
                    {
                        //valid
                        currenCustomer.ApplyDiscountCouponCode(discountcouponcode);
                        //model.DiscountBox.Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.Applied");
                        return true;
                    }
                    else
                    {
                        if (userErrors.Any())
                        {
                            //some user error
                            errorList = userErrors;
                            return false;
                        }
                        else
                        {
                            //general error text
                            errorList.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount"));
                            return false;
                        }
                    }
                }
                else
                {
                    //discount cannot be found
                    errorList.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount"));
                    return false;
                }
            }
            else
            {
                errorList.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount"));
                return false;
            }
        }
        private void CleanShopingCartItem(Customer customer)
        {
            while (customer.ShoppingCartItems.Any())
            {
                customer.ShoppingCartItems.ToList().ForEach(sci => _shoppingCartService.DeleteShoppingCartItem(sci, false));
            }
        }

        public string getZipCode(int StateId)
        {
            if (StateId == 0)
                return "";
            string Query = @"SELECT
	                            *
                            FROM
	                            dbo.StateCode AS SC
                            WHERE
	                            SC.stateId = " + StateId;
            var CityCode = _dbContext.SqlQuery<StateCodemodel>(Query, new object[0]).FirstOrDefault();
            if (CityCode == null)
                return "";
            if (string.IsNullOrEmpty(CityCode.StateCode))
                return "";
            int[] tehranManategh = new int[] { 585, 4, 579, 580, 582, 583, 584, 581 };
            string StateCode = CityCode.StateCode;
            if (tehranManategh.Contains(StateId))
            {
                StateCode = StateCode.Replace("10", "");
            }
            int count = 10 - StateCode.Length;
            return StateCode + new string('1', count);
        }
        public string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                   + "_"
                   + Guid.NewGuid().ToString().Substring(0, 9)
                   + Path.GetExtension(fileName);
        }
        private ListOfOrderList ReadexcelContent(string FileName, bool IsCod)
        {
            try
            {
                #region ExcelProccess

                var uploads = @"Plugins\Orders.BulkOrder\BulkOrderFile\";

                var filePath = Path.Combine(uploads, FileName);
                filePath = Path.Combine(_hostingEnvironment.ContentRootPath, filePath);

                var fileStream = new FileStream(filePath, FileMode.Open);


                var excelReader = ExcelReaderFactory.CreateReader(fileStream);
                var result = excelReader.AsDataSet();
                fileStream.Close();
                var tb1 = result.Tables[1];
                var tb2 = result.Tables[2];
                result.Tables.Remove(tb1);
                result.Tables.Remove(tb2);
                result.DataSetName = "ArrayOfOrderList";
                result.Tables[0].TableName = "ListOfOrderList";
                if (IsCod)
                {
                    result.Tables[0].Columns[0].ColumnName = "ServiceType";
                    result.Tables[0].Columns[1].ColumnName = "GoodsType";
                    result.Tables[0].Columns[2].ColumnName = "ApproximateValue";
                    result.Tables[0].Columns[3].ColumnName = "CodGoodsPrice";
                    result.Tables[0].Columns[4].ColumnName = "Wehight_g";
                    result.Tables[0].Columns[5].ColumnName = "Insurance";
                    result.Tables[0].Columns[6].ColumnName = "Carton";

                    result.Tables[0].Columns[7].ColumnName = "Reciver_FristName";
                    result.Tables[0].Columns[8].ColumnName = "Reciver_LastName";
                    result.Tables[0].Columns[9].ColumnName = "Reciver_mobile";
                    result.Tables[0].Columns[10].ColumnName = "Reciver_Country";
                    result.Tables[0].Columns[11].ColumnName = "Reciver_State";
                    result.Tables[0].Columns[12].ColumnName = "Reciver_City";
                    result.Tables[0].Columns[13].ColumnName = "Reciver_PostCode";
                    result.Tables[0].Columns[14].ColumnName = "Reciver_Address";
                    result.Tables[0].Columns[15].ColumnName = "Reciver_Email";

                    result.Tables[0].Columns[16].ColumnName = "Sender_FristName";
                    result.Tables[0].Columns[17].ColumnName = "Sender_LastName";
                    result.Tables[0].Columns[18].ColumnName = "Sender_mobile";
                    result.Tables[0].Columns[19].ColumnName = "Sender_Country";
                    result.Tables[0].Columns[20].ColumnName = "Sender_State";
                    result.Tables[0].Columns[21].ColumnName = "Sender_City";
                    result.Tables[0].Columns[22].ColumnName = "Sender_PostCode";
                    result.Tables[0].Columns[23].ColumnName = "Sender_Address";
                    result.Tables[0].Columns[24].ColumnName = "Sender_Email";
                    result.Tables[0].Columns[25].ColumnName = "AgentSaleAmount";
                    result.Tables[0].Columns[26].ColumnName = "HasAccessToPrinter";
                }
                else
                {
                    result.Tables[0].Columns[0].ColumnName = "ServiceType";
                    result.Tables[0].Columns[1].ColumnName = "GoodsType";
                    result.Tables[0].Columns[2].ColumnName = "ApproximateValue";
                    result.Tables[0].Columns[3].ColumnName = "Wehight_g";
                    result.Tables[0].Columns[4].ColumnName = "Insurance";
                    result.Tables[0].Columns[5].ColumnName = "Carton";

                    result.Tables[0].Columns[6].ColumnName = "Reciver_FristName";
                    result.Tables[0].Columns[7].ColumnName = "Reciver_LastName";
                    result.Tables[0].Columns[8].ColumnName = "Reciver_mobile";
                    result.Tables[0].Columns[9].ColumnName = "Reciver_Country";
                    result.Tables[0].Columns[10].ColumnName = "Reciver_State";
                    result.Tables[0].Columns[11].ColumnName = "Reciver_City";
                    result.Tables[0].Columns[12].ColumnName = "Reciver_PostCode";
                    result.Tables[0].Columns[13].ColumnName = "Reciver_Address";
                    result.Tables[0].Columns[14].ColumnName = "Reciver_Email";

                    result.Tables[0].Columns[15].ColumnName = "Sender_FristName";
                    result.Tables[0].Columns[16].ColumnName = "Sender_LastName";
                    result.Tables[0].Columns[17].ColumnName = "Sender_mobile";
                    result.Tables[0].Columns[18].ColumnName = "Sender_Country";
                    result.Tables[0].Columns[19].ColumnName = "Sender_State";
                    result.Tables[0].Columns[20].ColumnName = "Sender_City";
                    result.Tables[0].Columns[21].ColumnName = "Sender_PostCode";
                    result.Tables[0].Columns[22].ColumnName = "Sender_Address";
                    result.Tables[0].Columns[23].ColumnName = "Sender_Email";
                    result.Tables[0].Columns[24].ColumnName = "AgentSaleAmount";
                    result.Tables[0].Columns[25].ColumnName = "HasAccessToPrinter";

                }

                var row = result.Tables[0].Rows[0];
                result.Tables[0].Rows.Remove(row);

                string xmlString;
                using (TextWriter writer = new StringWriter())
                {
                    result.Tables[0].WriteXml(writer);
                    xmlString = writer.ToString();
                }

                #endregion

                var serializer = new XmlSerializer(typeof(ListOfOrderList));
                var listOfOrderList = new ListOfOrderList();
                using (TextReader reader = new StringReader(xmlString))
                {
                    listOfOrderList = (ListOfOrderList)serializer.Deserialize(reader);
                }

                return listOfOrderList;
            }
            catch (Exception ex)
            {
                Log("خطا در زمان خواندن  اکسل سفارش انبوه",
                    ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                return null;
            }
        }
        private string getCheckoutAttributeXml(OrderList item, BulkOrderModel BulkOrderModel, int productId)
        {
            int hagheSabt = 0;
            if (_storeContext.CurrentStore.Id == 5)
            {
                int serviceId = (_productService.GetProductById(productId).ProductCategories.FirstOrDefault()?.CategoryId).GetValueOrDefault(0);
                if (serviceId != 0)
                    hagheSabt = _newCheckout.CalcHagheSabet(_workContext.CurrentCustomer.Id, serviceId, 0);
            }
            var attributeByValueTofetch = $"N'{item.Carton}',N'{item.Insurance}'";
            if (BulkOrderModel.IsCod)
            {
                attributeByValueTofetch += ",N'حق مقر',N'حق ثبت'";
            }

            attributeByValueTofetch += ",N'خودم انجام می شود.'";

            #region Weight
            string WeightQuery = @"SELECT TOP(1)
	                            pav.Name
                            FROM
	                            dbo.Product AS P
	                            INNER JOIN dbo.Product_ProductAttribute_Mapping AS PPAM ON PPAM.ProductId = P.Id
	                            INNER JOIN dbo.ProductAttribute AS PA ON PA.Id = PPAM.ProductAttributeId
	                            INNER JOIN dbo.ProductAttributeValue AS PAV ON PAV.ProductAttributeMappingId = PPAM.Id
                            WHERE
	                            p.id = " + productId + @"
	                            AND PAV.WeightAdjustment <> 0
	                            AND (PAV.WeightAdjustment *1000) - " + item.Wehight_g + @" >= 0
                            ORDER BY ((PAV.WeightAdjustment * 1000) - " + item.Wehight_g + @") ";
            string Weight = _dbContext.SqlQuery<string>(WeightQuery, new object[0]).FirstOrDefault();
            attributeByValueTofetch += ",N'" + Weight + "'";
            #endregion
            string propertyFetcher = " OR (pa.Name LIKE N'%نوع و وزن%') ";
            propertyFetcher += " OR (pa.Name LIKE N'%دسترسی به پرینتر%' AND PAV.Name LIKE N'%" + ((BulkOrderModel.HasAccessToPrinter.HasValue && BulkOrderModel.HasAccessToPrinter.Value) ? "بلی" : "خیر") + "%') ";
            if (BulkOrderModel.PrintLogo.HasValue && BulkOrderModel.PrintLogo.Value)
                propertyFetcher += " OR (pa.Name LIKE N'%چاپ نشان تجاری من بر روی فاکتور%' AND PAV.Name LIKE N'%بلی%') ";
            else
                propertyFetcher += " OR (pa.Name LIKE N'%چاپ نشان تجاری من بر روی فاکتور%' AND PAV.Name LIKE N'%خیر%') ";


            if (BulkOrderModel.SendSms.HasValue && BulkOrderModel.SendSms.Value)
                propertyFetcher += " OR (pa.Name LIKE N'%اطلاع رسانی پیامکی%' AND PAV.Name LIKE N'%بلی%') ";
            else
                propertyFetcher += " OR (pa.Name LIKE N'%اطلاع رسانی پیامکی%' AND PAV.Name LIKE N'%خیر%') ";


            propertyFetcher += " OR (pa.Name LIKE N'%تضمین غرامت پست%' AND PAV.Name LIKE N'%هزینه%') ";

            string caseWhen = " WHEN pa.Name LIKE N'%نوع و وزن%' THEN ISNULL(CAST(pav.Id AS VARCHAR(10)),'GoodsType') ";
            if (item.AgentSaleAmount > 0)
            {
                caseWhen += " WHEN pa.Name LIKE N'%ارزش افزوده%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'AgentSaleAmount') ";
                propertyFetcher += " OR pa.Name LIKE N'%ارزش افزوده%'";

            }
            if (BulkOrderModel.IsCod && item.CodGoodsPrice > 0)
            {
                propertyFetcher += "OR pa.Name LIKE N'%وجه%'";
                caseWhen += "WHEN pa.Name LIKE N'%وجه%' THEN ISNULL(CAST(pav.Id AS VARCHAR(10)),'CODValue')";
            }
            caseWhen += " WHEN pa.Name LIKE N'%وزن دقیق%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'ExactWeight') ";
            propertyFetcher += "OR (pa.Name LIKE N'%وزن دقیق%')";

            caseWhen += " WHEN pa.Name LIKE N'%ارزش کالا%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'ApproximateValue') ";
            propertyFetcher += "OR (pa.Name LIKE N'%ارزش کالا%')";

            if (hagheSabt > 0)
            {
                caseWhen += " WHEN pa.Name LIKE N'%ثبت مرسوله%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'HagheSabt') ";
                propertyFetcher += "OR (pa.Name LIKE N'%ثبت مرسوله%')";
            }
            if (item.height.HasValue && item.height.Value > 0)
            {
                caseWhen += " WHEN pa.Name LIKE N'%ارتفاع مرسوله%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'_height') ";
                propertyFetcher += "OR (pa.Name LIKE N'%ارتفاع مرسوله%')";
            }
            if (item.length.HasValue && item.length.Value > 0)
            {
                caseWhen += " WHEN pa.Name LIKE N'%طول مرسوله%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'_length') ";
                propertyFetcher += "OR (pa.Name LIKE N'%طول مرسوله%')";
            }
            if (item.width.HasValue && item.width.Value > 0)
            {
                caseWhen += " WHEN pa.Name LIKE N'%عرض مرسوله%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'_width') ";
                propertyFetcher += "OR (pa.Name LIKE N'%عرض مرسوله%')";
            }

            var query = @"SELECT
	                        PPAM.Id AS '@ID'
	                        ,CASE " + caseWhen + @"  ELSE ISNULL(CAST(pav.Id AS VARCHAR(10)),'') END AS 'ProductAttributeValue/Value'
                        FROM
	                        dbo.Product AS P
	                        INNER JOIN dbo.Product_ProductAttribute_Mapping AS PPAM ON PPAM.ProductId = P.Id
	                        INNER JOIN dbo.ProductAttribute AS PA ON PA.Id = PPAM.ProductAttributeId
	                        LEFT JOIN dbo.ProductAttributeValue AS PAV ON PAV.ProductAttributeMappingId = PPAM.Id
                        WHERE
	                        p.id = " + productId + @"
	                        AND (PAV.Name IN(" + attributeByValueTofetch + @") OR PAV.Name LIKE N'%آشپز%' 
                                " + propertyFetcher + @")
                        FOR XML PATH('ProductAttribute'),ROOT('Attributes'), ELEMENTS";
            string xml = _dbContext.SqlQuery<string>(query, new object[0]).FirstOrDefault();
            xml = xml.Replace("CODValue", item.CodGoodsPrice.ToString());
            xml = xml.Replace("AgentSaleAmount", item.AgentSaleAmount.ToString());
            xml = xml.Replace("ExactWeight", item.Wehight_g.ToString());
            xml = xml.Replace("HagheSabt", hagheSabt.ToString());
            if (item.height.HasValue && item.height.Value > 0)
                xml = xml.Replace("_height", item.height.ToString());
            if (item.length.HasValue && item.length.Value > 0)
                xml = xml.Replace("_length", item.length.ToString());
            if (item.width.HasValue && item.width.Value > 0)
                xml = xml.Replace("_width", item.width.ToString());
            xml = xml.Replace("ApproximateValue", item.ApproximateValue.ToString());
            xml = xml.Replace("GoodsType", string.IsNullOrEmpty(item.GoodsType) ? "-" : item.GoodsType);// + " به ارزش" + item.ApproximateValue.ToString() + " ريال ");
            return xml;

        }
        private int getCheckoutAttributePrice(OrderList item, int ServiceId)
        {
            var attributeByValueTofetch = $"N'{item.Carton}',N'{item.Insurance}'";

            string propertyFetcher = "";
            propertyFetcher += " OR (pa.Name LIKE N'%دسترسی به پرینتر%' AND PAV.Name LIKE N'%" + (string.IsNullOrEmpty(item.HasAccessToPrinter) ? "بلی" : item.HasAccessToPrinter) + "%') ";

            var query = @"SELECT
	                        CAST(SUM(PAV.PriceAdjustment) AS int)as Price
                        FROM
	                        dbo.Product AS P
	                        INNER JOIN dbo.Product_Category_Mapping AS PCM ON PCM.ProductId = P.Id
	                        INNER JOIN dbo.Category AS C ON C.Id = PCM.CategoryId
	                        INNER JOIN dbo.Product_ProductAttribute_Mapping AS PPAM ON PPAM.ProductId = P.Id
	                        INNER JOIN dbo.ProductAttribute AS PA ON PA.Id = PPAM.ProductAttributeId
	                        LEFT JOIN dbo.ProductAttributeValue AS PAV ON PAV.ProductAttributeMappingId = PPAM.Id
                        WHERE
	                        C.id = " + ServiceId + @"
                            AND p.Deleted = 0
	                        AND p.Published = 1
	                        AND (PAV.Name IN(" + attributeByValueTofetch + @") OR PAV.Name LIKE N'%آشپز%' 
                                " + propertyFetcher + @")";
            int price = _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
            price += (price * 9) / 100;
            return price;

        }
        private string getCheckoutAttributeXmlApi(CheckoutItemApi item, int productId, int hagheSabt = 0)
        {
            var attributeByValueTofetch = $"N'{(item.NeedCarton ? item.CartonSizeName : "کارتن نیاز ندارم.")}',N'{item.InsuranceName}'";
            if (item.IsCOD)
            {
                attributeByValueTofetch += ",N'حق مقر',N'حق ثبت'";
            }

            attributeByValueTofetch += ",N'خودم انجام می شود.'";

            #region Weight
            string WeightQuery = @"SELECT TOP(1)
	                            pav.Name
                            FROM
	                            dbo.Product AS P
	                            INNER JOIN dbo.Product_ProductAttribute_Mapping AS PPAM ON PPAM.ProductId = P.Id
	                            INNER JOIN dbo.ProductAttribute AS PA ON PA.Id = PPAM.ProductAttributeId
	                            INNER JOIN dbo.ProductAttributeValue AS PAV ON PAV.ProductAttributeMappingId = PPAM.Id
                            WHERE
	                            p.id = " + productId + @"
	                            AND PAV.WeightAdjustment <> 0
	                            AND (PAV.WeightAdjustment *1000) - " + item.Weight + @" >= 0
                            ORDER BY ((PAV.WeightAdjustment * 1000) - " + item.Weight + @") ";
            //Log("query1", WeightQuery);
            string pishtazWeight = _dbContext.SqlQuery<string>(WeightQuery, new object[0]).FirstOrDefault();
            attributeByValueTofetch += ",N'" + pishtazWeight + "'";
            if (!string.IsNullOrEmpty(item.boxType))
            {
                attributeByValueTofetch += ",N'" + item.boxType + "'";
            }
            #endregion

            string propertyFetcher = " OR (pa.Name LIKE N'%نوع و وزن%') ";
            if (item.HasAccessToPrinter)
                propertyFetcher += " OR (pa.Name LIKE N'%دسترسی به پرینتر%' AND PAV.Name LIKE N'%بلی%') ";
            else
                propertyFetcher += " OR (pa.Name LIKE N'%دسترسی به پرینتر%' AND PAV.Name LIKE N'%خیر%') ";

            if (item.printLogo)
                propertyFetcher += " OR (pa.Name LIKE N'%چاپ نشان تجاری من بر روی فاکتور%' AND PAV.Name LIKE N'%بلی%') ";
            else
                propertyFetcher += " OR (pa.Name LIKE N'%چاپ نشان تجاری من بر روی فاکتور%' AND PAV.Name LIKE N'%خیر%') ";

            if (item.notifBySms)
                propertyFetcher += " OR (pa.Name LIKE N'%اطلاع رسانی پیامکی%' AND PAV.Name LIKE N'%بلی%') ";
            else
                propertyFetcher += " OR (pa.Name LIKE N'%اطلاع رسانی پیامکی%' AND PAV.Name LIKE N'%خیر%') ";

            if (new int[] { 655, 662, 654, 725, 726, 727 }.Contains(item.ServiceId))
                propertyFetcher += " OR (pa.Name LIKE N'%تضمین غرامت پست%' AND PAV.Name LIKE N'%هزینه%') ";
            if (!string.IsNullOrEmpty(item.boxType))
                propertyFetcher += " OR (pa.Name LIKE N'%نوع مرسوله%' AND PAV.Name LIKE N'%" + item.boxType + "%') ";
            string caseWhen = " WHEN pa.Name LIKE N'%نوع و وزن%' THEN ISNULL(CAST(pav.Id AS VARCHAR(10)),'GoodsType') ";
            if (item.AgentSaleAmount > 0)
            {
                caseWhen += " WHEN pa.Name LIKE N'%ارزش افزوده%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'AgentSaleAmount') ";
                propertyFetcher += " OR (pa.Name LIKE N'%ارزش افزوده%')";
            }

            if (item.IsCOD && item.CodGoodsPrice > 0)
            {
                propertyFetcher += " OR pa.Name LIKE N'%وجه%'";
                caseWhen += " WHEN pa.Name LIKE N'%وجه%' THEN ISNULL(CAST(pav.Id AS VARCHAR(10)),'CODValue')";
            }
            caseWhen += " WHEN pa.Name LIKE N'%وزن دقیق%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'ExactWeight') ";
            propertyFetcher += " OR (pa.Name LIKE N'%وزن دقیق%')";

            caseWhen += " WHEN pa.Name LIKE N'%ارزش کالا%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'ApproximateValue') ";
            propertyFetcher += "OR (pa.Name LIKE N'%ارزش کالا%')";

            if (hagheSabt > 0)
            {
                caseWhen += " WHEN pa.Name LIKE N'%ثبت مرسوله%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'HagheSabt') ";
                propertyFetcher += "OR (pa.Name LIKE N'%ثبت مرسوله%')";
            }
            if (!string.IsNullOrEmpty(item.CartonSizeName) && item.CartonSizeName != "کارتن نیاز ندارم."
                && (!(item.height.HasValue && item.height.Value > 0) || !(item.length.HasValue && item.length.Value > 0)
                    || (item.width.HasValue && item.width.Value > 0))
                )
            {
                var DimentionResult = _newCheckout.getDimentionByName(item.CartonSizeName);
                if (DimentionResult != null)
                {
                    item.length = DimentionResult.Length;
                    item.width = DimentionResult.Width;
                    item.height = DimentionResult.Height;
                }
            }
            if (item.height.HasValue && item.height.Value > 0)
            {
                caseWhen += " WHEN pa.Name LIKE N'%ارتفاع مرسوله%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'_height') ";
                propertyFetcher += "OR (pa.Name LIKE N'%ارتفاع مرسوله%')";
            }
            if (item.length.HasValue && item.length.Value > 0)
            {
                caseWhen += " WHEN pa.Name LIKE N'%طول مرسوله%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'_length') ";
                propertyFetcher += "OR (pa.Name LIKE N'%طول مرسوله%')";
            }
            if (item.width.HasValue && item.width.Value > 0)
            {
                caseWhen += " WHEN pa.Name LIKE N'%عرض مرسوله%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'_width') ";
                propertyFetcher += "OR (pa.Name LIKE N'%عرض مرسوله%')";
            }
            if (item.ServiceId == 701)
            {
                if (!string.IsNullOrEmpty(item.dispatch_date))
                {
                    caseWhen += " WHEN pa.Name LIKE N'%تاریخ و ساعت بارگیری%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'_dispatch_date') ";
                    propertyFetcher += "OR (pa.Name LIKE N'%تاریخ و ساعت بارگیری%')";
                }
                if (!string.IsNullOrEmpty(item.UbbraTruckType))
                {
                    caseWhen += " WHEN pa.Name LIKE N'%نوع خودرو%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'_UbbraTruckType') ";
                    propertyFetcher += "OR (pa.Name LIKE N'%نوع خودرو%')";
                }
                if (!string.IsNullOrEmpty(item.VechileOptions))
                {
                    caseWhen += " WHEN pa.Name LIKE N'%ویژگی خودرو%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'_VechileOptions') ";
                    propertyFetcher += "OR (pa.Name LIKE N'%ویژگی خودرو%')";
                }
                if (!string.IsNullOrEmpty(item.UbbarPackingLoad))
                {
                    caseWhen += " WHEN pa.Name LIKE N'%نوع بسته بندی%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'_UbbarPackingLoad') ";
                    propertyFetcher += "OR (pa.Name LIKE N'%نوع بسته بندی%')";
                }
                {
                    caseWhen += " WHEN pa.Name LIKE N'%منطقه فرستنده اوبار%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'_SenderStateId') ";
                    propertyFetcher += "OR (pa.Name LIKE N'%منطقه فرستنده اوبار%')";
                }
                {
                    caseWhen += " WHEN pa.Name LIKE N'%منطقه گیرنده اوبار%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'_ReciverStateId') ";
                    propertyFetcher += "OR (pa.Name LIKE N'%منطقه گیرنده اوبار%')";
                }
            }
            var query = @"SELECT
	                        PPAM.Id AS '@ID'
	                        ,CASE " + caseWhen + @"  ELSE ISNULL(CAST(pav.Id AS VARCHAR(10)),'') END AS 'ProductAttributeValue/Value'
                        FROM
	                        dbo.Product AS P
	                        INNER JOIN dbo.Product_ProductAttribute_Mapping AS PPAM ON PPAM.ProductId = P.Id
	                        INNER JOIN dbo.ProductAttribute AS PA ON PA.Id = PPAM.ProductAttributeId
	                        LEFT JOIN dbo.ProductAttributeValue AS PAV ON PAV.ProductAttributeMappingId = PPAM.Id
                        WHERE
	                        p.id = " + productId + @"
	                        AND (PAV.Name IN(" + attributeByValueTofetch + @") OR PAV.Name LIKE N'%آشپز%' 
                                " + propertyFetcher + @")
                        FOR XML PATH('ProductAttribute'),ROOT('Attributes'), ELEMENTS";
            //Log("query2", query);
            string xml = _dbContext.SqlQuery<string>(query, new object[0]).FirstOrDefault();

            xml = xml.Replace("CODValue", item.CodGoodsPrice.ToString());
            xml = xml.Replace("AgentSaleAmount", item.AgentSaleAmount.ToString());
            xml = xml.Replace("ExactWeight", item.Weight.ToString());
            xml = xml.Replace("HagheSabt", hagheSabt.ToString());
            if (item.height.HasValue && item.height.Value > 0)
                xml = xml.Replace("_height", item.height.ToString());
            if (item.length.HasValue && item.length.Value > 0)
                xml = xml.Replace("_length", item.length.ToString());
            if (item.width.HasValue && item.width.Value > 0)
                xml = xml.Replace("_width", item.width.ToString());
            if (item.ServiceId == 701)
            {
                xml = xml.Replace("_dispatch_date", Convert.ToDateTime(item.dispatch_date).ToString());
                xml = xml.Replace("_UbbraTruckType", item.UbbraTruckType);
                xml = xml.Replace("_VechileOptions", item.VechileOptions);
                xml = xml.Replace("_UbbarPackingLoad", item.UbbarPackingLoad);
                xml = xml.Replace("_UbbarPackingLoad", item.UbbarPackingLoad);
                xml = xml.Replace("_SenderStateId", item.Sender_townId.ToString());
                xml = xml.Replace("_ReciverStateId", item.Reciver_townId.ToString());
            }
            xml = xml.Replace("ApproximateValue", item.ApproximateValue.ToString());
            xml = xml.Replace("GoodsType", string.IsNullOrEmpty(item.GoodsType) ? "-" : item.GoodsType);// + " به ارزش" + item.ApproximateValue.ToString() + " ريال ");
            return xml;
        }

        //private string getCheckoutAttributeXmlApi(CheckoutItemApi item, int productId)
        //{
        //    int hagheSabt = 0;
        //    if (_storeContext.CurrentStore.Id == 5)
        //    {
        //        hagheSabt = _extendedShipmentService.CalcHagheSabet(_workContext.CurrentCustomer.Id, item.ServiceId, 0);
        //    }
        //    var attributeByValueTofetch = $"N'{item.CartonSizeName}',N'{item.InsuranceName}'";
        //    if (item.IsCOD)
        //    {
        //        attributeByValueTofetch += ",N'حق مقر',N'حق ثبت'";
        //    }

        //    attributeByValueTofetch += ",N'خودم انجام می شود.'";

        //    #region Weight
        //    string WeightQuery = @"SELECT TOP(1)
        //                     pav.Name
        //                    FROM
        //                     dbo.Product AS P
        //                     INNER JOIN dbo.Product_ProductAttribute_Mapping AS PPAM ON PPAM.ProductId = P.Id
        //                     INNER JOIN dbo.ProductAttribute AS PA ON PA.Id = PPAM.ProductAttributeId
        //                     INNER JOIN dbo.ProductAttributeValue AS PAV ON PAV.ProductAttributeMappingId = PPAM.Id
        //                    WHERE
        //                     p.id = " + productId + @"
        //                     AND PAV.WeightAdjustment <> 0
        //                     AND (PAV.WeightAdjustment *1000) - " + item.Weight + @" >= 0
        //                    ORDER BY ((PAV.WeightAdjustment * 1000) - " + item.Weight + @") ";
        //    string pishtazWeight = _dbContext.SqlQuery<string>(WeightQuery, new object[0]).FirstOrDefault();
        //    attributeByValueTofetch += ",N'" + pishtazWeight + "'";
        //    #endregion

        //    string propertyFetcher = " OR (pa.Name LIKE N'%نوع و وزن%') ";
        //    if (item.HasAccessToPrinter)
        //        propertyFetcher += " OR (pa.Name LIKE N'%دسترسی به پرینتر%' AND PAV.Name LIKE N'%بلی%') ";
        //    else
        //        propertyFetcher += " OR (pa.Name LIKE N'%دسترسی به پرینتر%' AND PAV.Name LIKE N'%خیر%') ";
        //    propertyFetcher += " OR (pa.Id = 126 AND PAV.Name LIKE N'%هزینه%') ";
        //    string caseWhen = " WHEN pa.Name LIKE N'%نوع و وزن%' THEN ISNULL(CAST(pav.Id AS VARCHAR(10)),'GoodsType') ";
        //    if (item.AgentSaleAmount > 0)
        //    {
        //        caseWhen += " WHEN pa.Name LIKE N'%ارزش افزوده%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'AgentSaleAmount') ";
        //        propertyFetcher += " OR pa.Name LIKE N'%ارزش افزوده%'";
        //    }

        //    if (item.IsCOD && item.CodGoodsPrice > 0)
        //    {
        //        propertyFetcher += "OR pa.Name LIKE N'%وجه%'";
        //        caseWhen += "WHEN pa.Name LIKE N'%وجه%' THEN ISNULL(CAST(pav.Id AS VARCHAR(10)),'CODValue')";
        //    }
        //    caseWhen += " WHEN pa.Name LIKE N'%وزن دقیق%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'ExactWeight') ";
        //    propertyFetcher += "OR (pa.Name LIKE N'%وزن دقیق%')";

        //    caseWhen += " WHEN pa.Name LIKE N'%ارزش کالا%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'ApproximateValue') ";
        //    propertyFetcher += "OR (pa.Name LIKE N'%ارزش کالا%')";
        //    if (hagheSabt > 0)
        //    {
        //        caseWhen += " WHEN pa.Name LIKE N'%ثبت مرسوله%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'HagheSabt') ";
        //        propertyFetcher += "OR (pa.Name LIKE N'%ثبت مرسوله%')";
        //    }
        //    var query = @"SELECT
        //                 PPAM.Id AS '@ID'
        //                 ,CASE " + caseWhen + @"  ELSE ISNULL(CAST(pav.Id AS VARCHAR(10)),'') END AS 'ProductAttributeValue/Value'
        //                FROM
        //                 dbo.Product AS P
        //                 INNER JOIN dbo.Product_ProductAttribute_Mapping AS PPAM ON PPAM.ProductId = P.Id
        //                 INNER JOIN dbo.ProductAttribute AS PA ON PA.Id = PPAM.ProductAttributeId
        //                 LEFT JOIN dbo.ProductAttributeValue AS PAV ON PAV.ProductAttributeMappingId = PPAM.Id
        //                WHERE
        //                 p.id = " + productId + @"
        //                 AND (PAV.Name IN(" + attributeByValueTofetch + @") OR PAV.Name LIKE N'%آشپز%' 
        //                        " + propertyFetcher + @")
        //                FOR XML PATH('ProductAttribute'),ROOT('Attributes'), ELEMENTS";

        //    string xml = _dbContext.SqlQuery<string>(query, new object[0]).FirstOrDefault();

        //    xml = xml.Replace("CODValue", item.CodGoodsPrice.ToString());
        //    xml = xml.Replace("AgentSaleAmount", item.AgentSaleAmount.ToString());
        //    xml = xml.Replace("ExactWeight", item.Weight.ToString());
        //    xml = xml.Replace("HagheSabt", hagheSabt.ToString());
        //    xml = xml.Replace("ApproximateValue", item.ApproximateValue.ToString());
        //    xml = xml.Replace("GoodsType", string.IsNullOrEmpty(item.GoodsType) ? "-" : item.GoodsType);
        //    return xml;
        //}
        public int? getCheckoutAttributePrice(int Wehight_g, string Insurance, string Carton, bool isCod, int productId, bool AccessPrintBill)
        {
            int hagheSabt = 0;
            if (_storeContext.CurrentStore.Id == 5)
            {
                int serviceId = (_productService.GetProductById(productId).ProductCategories.FirstOrDefault()?.CategoryId).GetValueOrDefault(0);
                if (serviceId != 0)
                    hagheSabt = _newCheckout.CalcHagheSabet(_workContext.CurrentCustomer.Id, serviceId, 0);
            }
            string attributeToFetch = "";
            if (!string.IsNullOrEmpty(Carton) && !string.IsNullOrWhiteSpace(Carton))
                attributeToFetch = $"N'{Carton}'";
            if (!string.IsNullOrEmpty(Insurance) && !string.IsNullOrWhiteSpace(Insurance))
                attributeToFetch += (attributeToFetch != "" ? "," : "") + $"N'{Insurance}'";
            if (isCod)
            {
                attributeToFetch += (attributeToFetch != "" ? "," : "") + "N'حق مقر',N'حق ثبت'";
            }

            attributeToFetch += (attributeToFetch != "" ? "," : "") + "N'خودم انجام می شود.'";


            string pishtazWeightQuery = @"SELECT TOP(1)
	                            pav.Name
                            FROM
	                            dbo.Product AS P
	                            INNER JOIN dbo.Product_ProductAttribute_Mapping AS PPAM ON PPAM.ProductId = P.Id
	                            INNER JOIN dbo.ProductAttribute AS PA ON PA.Id = PPAM.ProductAttributeId
	                            INNER JOIN dbo.ProductAttributeValue AS PAV ON PAV.ProductAttributeMappingId = PPAM.Id
                            WHERE
	                            p.id = " + productId + @"
	                            AND PAV.WeightAdjustment <> 0
	                            AND (PAV.WeightAdjustment *1000) - " + Wehight_g + @" >= 0
                            ORDER BY ((PAV.WeightAdjustment * 1000) - " + Wehight_g + @") ";
            string productWeight = _dbContext.SqlQuery<string>(pishtazWeightQuery, new object[0]).FirstOrDefault();

            attributeToFetch += (attributeToFetch != "" ? "," : "") + "N'" + productWeight + "'";
            string porpWhere = "";
            if (!AccessPrintBill)
                porpWhere = @" AND (PAV.Name IN(" + attributeToFetch + @") OR (PAV.Name LIKE N'خیر' and pa.name LIKE N'%دسترسی به پرینتر%')
                                       OR(pa.Name LIKE N'%تضمین غرامت پست%' AND PAV.Name LIKE N'%هزینه%'))";
            else
                porpWhere = " AND (PAV.Name IN(" + attributeToFetch + @") OR(pa.Name LIKE N'%تضمین غرامت پست%' AND PAV.Name LIKE N'%هزینه%'))";
            var query = @"SELECT
	                        CAST(SUM(pav.PriceAdjustment) AS INT) AttrPrice
                        FROM
	                        dbo.Product AS P
	                        INNER JOIN dbo.Product_ProductAttribute_Mapping AS PPAM ON PPAM.ProductId = P.Id
	                        INNER JOIN dbo.ProductAttribute AS PA ON PA.Id = PPAM.ProductAttributeId
	                        INNER JOIN dbo.ProductAttributeValue AS PAV ON PAV.ProductAttributeMappingId = PPAM.Id
                        WHERE
	                        p.id = " + productId + @"
	                       " + porpWhere;

            return (_dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault()).GetValueOrDefault(0) + hagheSabt;

        }


        public OrderPriceModel GetCheckoutAttributePriceSeparately(int Weight,
            string Insurance,
            decimal length,
            decimal width,
            decimal height,
            bool isCod,
            bool printBill,
            bool hasSms,
            bool hasLogo,
            bool needCarton,
            int approximateValue,
            int senderCityId,
            int receiverCityId,
            int? serviceId,
            int receiverCountryId = 0,
            string Address = "")
        {

            int hagheSabt = 0;
            //if (productId.HasValue && productId.Value != 0)
            //{
            //    serviceId = (_productService.GetProductById(productId.Value).ProductCategories.FirstOrDefault()?.CategoryId).GetValueOrDefault(0);
            if (serviceId.HasValue && serviceId != 0 && _storeContext.CurrentStore.Id == 5)
            {
                hagheSabt = _newCheckout.CalcHagheSabet(_workContext.CurrentCustomer.Id, serviceId.Value, 0);
            }
            //}


            var finalResult = new OrderPriceModel();
            int totalPrice = 0;
            string Carton = "";

            Carton = _cartonService.GetRequiredCartonBySize(length, width, height);
            if (string.IsNullOrWhiteSpace(Carton))
            {
                finalResult.CartonName = Carton = "سایر(بزرگتر از سایز 9)";
                finalResult.CartonPrice = _cartonService.CalculateSize9Price(length, width, height);
                totalPrice += finalResult.CartonPrice;
            }


            var receiverCity = _stateProvinceRepository.Table.FirstOrDefault(p => p.Id == receiverCityId);
            var senderCity = _stateProvinceRepository.Table.FirstOrDefault(p => p.Id == senderCityId);


            var services = getServiceInfo(new _getServiceInfoModel()
            {
                AproximateValue = approximateValue,
                boxType = Carton.Contains("A") ? (byte)0 : (byte)1,//********
                weightItem = Weight,
                Content = "",
                height = (int)height,
                length = (int)length,
                width = (int)width,
                IsCod = isCod,
                ListType = 0,
                receiverStateId = receiverCity == null ? 0 : receiverCity.CountryId,
                receiverTownId = receiverCityId,
                senderStateId = senderCity.CountryId,
                senderTownId = senderCityId,
                customerId = _workContext.CurrentCustomer.IsRegistered() ? _workContext.CurrentCustomer.Id : 0,
                serviceId = serviceId ?? 0,
                receiver_ForeginCountry = receiverCountryId
            }).GetAwaiter().GetResult();
            if (services == null || !services.Any())
            {
                return null;
            }
            var categoryIds = services.Select(p => p.ServiceId).ToArray();

            var query = @"SELECT
	                        DISTINCT CAST(pav.PriceAdjustment AS INT) AttrPrice,
                            PA.Name ProductAttrName,
                            PAV.Name ProductAttrValue
                        FROM
	                        dbo.Product AS P
                            INNER JOIN Product_Category_Mapping pcm on p.Id = pcm.ProductId
	                        INNER JOIN dbo.Product_ProductAttribute_Mapping AS PPAM ON PPAM.ProductId = P.Id
	                        INNER JOIN dbo.ProductAttribute AS PA ON PA.Id = PPAM.ProductAttributeId
	                        INNER JOIN dbo.ProductAttributeValue AS PAV ON PAV.ProductAttributeMappingId = PPAM.Id
                        WHERE
	                        pcm.CategoryId IN (" + string.Join(",", categoryIds) + ")";
            //" + porpWhere;

            var sqlResult = _dbContext.SqlQuery<OrderPriceSqlModel>(query, new object[0]).ToList();

            foreach (var item in sqlResult)
            {
                if (hasSms && item.ProductAttrName == "اطلاع رسانی پیامکی" && item.ProductAttrValue == "بلی")
                {
                    finalResult.SmsPrice = item.AttrPrice;
                    totalPrice += item.AttrPrice;
                }
                else if (item.ProductAttrName == "بیمه" && item.ProductAttrValue == Insurance)
                {
                    finalResult.InsurancePrice = item.AttrPrice;
                    totalPrice += item.AttrPrice;
                }
                else if (hasLogo && item.ProductAttrName == "چاپ نشان تجاری من بر روی فاکتور" && item.ProductAttrValue == "بلی")
                {
                    finalResult.LogoPrice = item.AttrPrice;
                    totalPrice += item.AttrPrice;
                }
                else if (printBill && item.ProductAttrName == "دسترسی به پرینتر" && item.ProductAttrValue == "خیر")
                {
                    finalResult.PrintPrice = item.AttrPrice;
                    totalPrice += item.AttrPrice;
                }
                else if (needCarton && item.ProductAttrName == "کارتن و لفاف بندی" && item.ProductAttrValue == Carton && string.IsNullOrEmpty(finalResult.CartonName))
                {
                    finalResult.CartonName = Carton;
                    finalResult.CartonPrice = item.AttrPrice;
                    totalPrice += item.AttrPrice;
                }
            }

            finalResult.ServicePrices = services.Where(p => (serviceId == null || serviceId == 0 || p.ServiceId == serviceId) && p.Price != 0)
                .Select(p => new ServicePrice()
                {
                    ServiceId = p.ServiceId,
                    Price = p.Price,
                    ServiceName = p.ServiceName,
                    TotalPrice = p.Price + totalPrice,
                    SLA = p.SLA
                }).ToList();

            query = $@"EXEC dbo.Sp_CheckBillingAddressForHagheMaghar @Int_CountryId = {senderCity.CountryId},      
                                              @Int_StateId = {senderCityId},        
                                              @Nvc_Address = N'{Address}',      
                                              @Nvc_FirstName = N'',    
                                              @Nvc_LastName = N'',     
                                              @vc_PhoneNumber = N'',   
                                              @Int_CustomerId = {_workContext.CurrentCustomer.Id},     
                                              @ServiceId = {serviceId},          
                                              @InComeTotalWeight = {Weight},  
                                              @isInTarheTraffic = 0,
                                              @isInBazar = 0";

            //query = "SELECT tci.CategoryId FROM dbo.Tb_CategoryInfo AS tci WHERE tci.HasHagheMaghar = 1 AND tci.CategoryId IN (" + string.Join(",", categoryIds) + ")";

            var hagheMagharResult = _dbContext.SqlQuery<int>(query, new object[0]).ToList();

            foreach (var item in finalResult.ServicePrices)
            {
                item.TotalPrice += hagheMagharResult?.FirstOrDefault() ?? 0;
                item.CollectionPrice = hagheMagharResult?.FirstOrDefault() ?? 0;
            }
            //foreach (var item in finalResult.ServicePrices)
            //{
            //    if (hagheMagharResult.Contains(item.ServiceId))
            //    {
            //        if (receiverCountryId != 0)
            //        {
            //            item.TotalPrice += item.CollectionPrice = 0;
            //        }
            //        else if (senderCity.CountryId == 1)
            //        {
            //            item.TotalPrice += item.CollectionPrice = 170000;
            //        }
            //        else if (senderCity.CountryId == 252)
            //        {
            //            if (senderCity.Id == 259)
            //            {
            //                item.TotalPrice += item.CollectionPrice = 165000;
            //            }
            //            else
            //            {
            //                item.TotalPrice += item.CollectionPrice = 14500;
            //            }
            //        }
            //        else if ((new int[] { 80, 207, 343, 74 }).Contains(senderCity.Id))
            //        {
            //            item.TotalPrice += item.CollectionPrice = 155000;
            //        }
            //        else if ((new int[] { 409, 446 }).Contains(senderCity.Id))
            //        {
            //            item.TotalPrice += item.CollectionPrice = 150000;
            //        }
            //        else if (senderCity.Id == 526)
            //        {
            //            item.TotalPrice += item.CollectionPrice = 140000;
            //        }
            //        else
            //        {
            //            item.TotalPrice += item.CollectionPrice = 135000;
            //        }
            //    }
            //}


            return finalResult;
        }

        public int CalcCodPrice(Product product
            , int weight
            , string userName
            , int countryId
            , int steteId
            , int postType
            , string cartonSizeName
            , string insuranceName
            , int goodsPrice
            , bool AccessPrintBill
            , out string error)
        {
            var orderList = new OrderList()
            {
                Wehight_g = weight,
                Carton = cartonSizeName,
                Insurance = insuranceName,
                CodGoodsPrice = goodsPrice,
                HasAccessToPrinter = (AccessPrintBill ? "بلی" : "خیر")
            };
            string xml = getCheckoutAttributeXml(orderList, new BulkOrderModel() { IsCod = true }, product.Id);
            error = "";
            return _newCheckout.CalcCODPriceApi(product, weight, xml, userName, countryId, steteId, postType, out error);
        }
        public Product DetectProduct(int wehightG, string serviceType)
        {
            SqlParameter P_CategoryName = new SqlParameter()
            {
                ParameterName = "CategoryName",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)serviceType
            };
            SqlParameter[] prms = new SqlParameter[]
           {
                P_CategoryName
           };
            string query = $@"SELECT
	                            TOP(1) P.Id 
                            FROM
	                            dbo.Category AS C
	                            INNER JOIN dbo.Product_Category_Mapping AS PCM ON PCM.CategoryId = C.Id
	                            INNER JOIN dbo.Product AS P ON P.Id = PCM.ProductId
	                            INNER JOIN dbo.StoreMapping AS SM ON sm.EntityId = C.Id AND sm.EntityName ='Category'
	                            INNER JOIN dbo.CateguryPostType AS CPT ON CPT.CateguryId = C.Id
                            WHERE	
	                            C.ParentCategoryId <> 0
                                AND C.Deleted = 0
	                            AND C.Published = 1
	                            AND SM.StoreId = 3
	                            AND p.Deleted= 0
	                            AND p.Published = 1
	                            AND C.Name LIKE N'%'+@CategoryName+'%'
                            ORDER BY C.Name";
            var producId = _dbContext.SqlQuery<int?>(query, prms).FirstOrDefault().GetValueOrDefault(0);
            if (producId == 0)
                return null;
            return _productService.GetProductById(producId);
            //var category = _categoryService.GetAllCategories(serviceType.Replace("_", " ")).FirstOrDefault(p => p.ParentCategoryId != 0 && p.Published ==true);
            //if (category == null)
            //    return null;

            //var Cp1 = _productService.SearchProducts(categoryIds: new List<int>() { category.Id }).ToList();
            //var product = Cp1.Where(p =>
            //          p.Deleted == false
            //          && p.Published)
            //     .OrderByDescending(p => p.Id);
            //return product.Count() > 1 ? null : product.First();
        }
        public Product DetectProduct(int serviceType)
        {
            SqlParameter P_CategoryId = new SqlParameter()
            {
                ParameterName = "CategoryId",
                SqlDbType = SqlDbType.Int,
                Value = (object)serviceType
            };
            SqlParameter[] prms = new SqlParameter[]
           {
                P_CategoryId
           };
            string query = $@"SELECT
	                            TOP(1) P.Id 
                            FROM
	                            dbo.Category AS C
	                            INNER JOIN dbo.Product_Category_Mapping AS PCM ON PCM.CategoryId = C.Id
	                            INNER JOIN dbo.Product AS P ON P.Id = PCM.ProductId
	                            --INNER JOIN dbo.StoreMapping AS SM ON sm.EntityId = C.Id AND sm.EntityName ='Category'
	                            --INNER JOIN dbo.CateguryPostType AS CPT ON CPT.CateguryId = C.Id
                            WHERE	
	                            C.ParentCategoryId <> 0
                                AND C.Deleted = 0
	                            AND C.Published = 1
	                            --AND SM.StoreId = 3
	                            AND p.Deleted= 0
	                            AND p.Published = 1
	                            AND C.Id = @CategoryId";
            var producId = _dbContext.SqlQuery<int?>(query, prms).FirstOrDefault().GetValueOrDefault(0);
            if (producId == 0)
                return null;
            return _productService.GetProductById(producId);

        }
        private int ProcessAddress(
            string firstName,
            string lastName,
            string phoneNumber,
            string email,
            string faxNumber,
            string company,
            string address1,
            string address2,
            string city,
            string state,
            string country,
            string zipCode,
            int customerId,
            float? lat = null,
            float? lon = null
        )
        {
            if (address2 == "")
                address2 = null;
            var countryId = 0;
            string Str_Country = country.Replace("_", " ").Trim();
            var repCountry = _countryRepository.Table.FirstOrDefault(p => p.Published == true && p.Name == Str_Country);
            if (repCountry == null)
                return 0;
            countryId = repCountry.Id;
            string Str_State = state.Replace("_", " ").Trim();
            var repStateProvince = _stateProvinceRepository.Table.FirstOrDefault(p => p.Published == true && p.Name == Str_State && p.CountryId == repCountry.Id);

            var stateProvId = 0;
            if (repStateProvince == null)
                return 0;
            stateProvId = repStateProvince.Id;

            #region Check Customer Address Exist
            SqlParameter P_CustomerId = new SqlParameter()
            {
                ParameterName = "CustomerId",
                SqlDbType = SqlDbType.Int,
                Value = (object)customerId
            };
            SqlParameter P_CountryId = new SqlParameter()
            {
                ParameterName = "CountryId",
                SqlDbType = SqlDbType.Int,
                Value = (object)countryId
            };
            SqlParameter P_StateId = new SqlParameter()
            {
                ParameterName = "StateId",
                SqlDbType = SqlDbType.Int,
                Value = (object)stateProvId
            };
            SqlParameter P_FirstName = new SqlParameter()
            {
                ParameterName = "FirstName",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)firstName ?? ""
            };
            SqlParameter P_LastName = new SqlParameter()
            {
                ParameterName = "LastName",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)lastName ?? ""
            };
            SqlParameter P_PhoneNumber = new SqlParameter()
            {
                ParameterName = "PhoneNumber",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)phoneNumber ?? ""
            };
            SqlParameter P_Address = new SqlParameter()
            {
                ParameterName = "Address",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)address1 ?? ""
            };
            SqlParameter P_Company = new SqlParameter()
            {
                ParameterName = "Company",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)company ?? (object)DBNull.Value

            };
            SqlParameter P_Address2 = new SqlParameter()
            {
                ParameterName = "Address2",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)address2 ?? (object)DBNull.Value
            };
            SqlParameter[] prms = new SqlParameter[]
            {
                P_CustomerId,
                P_CountryId,
                P_StateId,
                P_FirstName,
                P_LastName,
                P_PhoneNumber,
                P_Address,
                P_Company,
                P_Address2
            };
            string Query = @"EXEC dbo.Sp_CheckExistCustomerAddress @CustomerId, @CountryId, @StateId, @FirstName, @LastName, @PhoneNumber, @Address, @Company, @Address2";
            int AddressId = _dbContext.SqlQuery<int?>(Query, prms).FirstOrDefault().GetValueOrDefault(0);
            var customer = _customerService.GetCustomerById(customerId);
            if (AddressId == 0)
            {
                if (string.IsNullOrEmpty(zipCode) || zipCode.Length != 10)
                {
                    zipCode = getZipCode(stateProvId);
                }

                var newAddress = new AddressModel()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phoneNumber,
                    Email = email ?? "",
                    FaxNumber = faxNumber ?? "",
                    Company = company ?? "",
                    Address1 = address1 ?? "",
                    Address2 = address2 ?? "",
                    City = city ?? "",
                    StateProvinceId = stateProvId,
                    CountryId = countryId,
                    ZipPostalCode = zipCode,
                };
                if (newAddress.CountryId == 0)
                    newAddress.CountryId = null;
                if (newAddress.StateProvinceId == 0)
                    newAddress.StateProvinceId = null;
                //var MyAddress = newAddress.ToEntity();
                //MyAddress.CreatedOnUtc = DateTime.Now;
                //MyAddress.CustomAttributes = "";
                var addressId = InsertCustomerAddress(newAddress, customer.Id);
                if (lat.HasValue && lon.HasValue)
                {
                    _extendedShipmentService.InsertAddressLocation(addressId, lat.Value, lon.Value);
                }
                return addressId;
                //customer.Addresses.Add(MyAddress);
                //_customerService.UpdateCustomer(customer);
                //return MyAddress.Id;
            }
            else
            {
                var address = _addressService.GetAddressById(AddressId);
                if (string.IsNullOrEmpty(zipCode) || zipCode.Length != 10)
                {
                    address.ZipPostalCode = getZipCode(address.StateProvinceId.Value);
                    _addressService.UpdateAddress(address);
                }
                return address.Id;
            }
            #endregion           

        }
        public int InsertCustomerAddress(AddressModel model, int CustomerId)
        {


            SqlParameter P_FirstName = new SqlParameter()
            {
                ParameterName = "FirstName",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)model.FirstName ?? ""
            };

            SqlParameter P_LastName = new SqlParameter()
            {
                ParameterName = "LastName",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)model.LastName ?? ""
            };

            SqlParameter P_Email = new SqlParameter()
            {
                ParameterName = "Email",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)model.Email ?? ""
            };

            SqlParameter P_Company = new SqlParameter()
            {
                ParameterName = "Company",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)model.Company ?? (object)DBNull.Value

            };

            SqlParameter P_CountryId = new SqlParameter()
            {
                ParameterName = "CountryId",
                SqlDbType = SqlDbType.Int,
                Value = (object)model.CountryId
            };

            SqlParameter P_StateId = new SqlParameter()
            {
                ParameterName = "StateProvinceId",
                SqlDbType = SqlDbType.Int,
                Value = (object)model.StateProvinceId
            };

            SqlParameter P_City = new SqlParameter()
            {
                ParameterName = "City",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)model.City ?? ""
            };

            SqlParameter P_Address = new SqlParameter()
            {
                ParameterName = "Address1",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)model.Address1 ?? ""
            };

            SqlParameter P_Address1 = new SqlParameter()
            {
                ParameterName = "Address2",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)model.Address2 ?? ""
            };
            SqlParameter P_ZipPostalCode = new SqlParameter()
            {
                ParameterName = "ZipPostalCode",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)model.ZipPostalCode ?? ""
            };

            SqlParameter P_PhoneNumber = new SqlParameter()
            {
                ParameterName = "PhoneNumber",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)(model.PhoneNumber ?? "")
            };

            SqlParameter P_FaxNumber = new SqlParameter()
            {
                ParameterName = "FaxNumber",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)model.FaxNumber ?? ""
            };


            SqlParameter P_CustomerId = new SqlParameter()
            {
                ParameterName = "CustomerId",
                SqlDbType = SqlDbType.Int,
                Value = (object)CustomerId
            };
            SqlParameter[] prms = new SqlParameter[]
            {
                P_FirstName,
                P_LastName,
                P_Email,
                P_Company,
                P_CountryId,
                P_StateId,
                P_City,
                P_Address,
                P_Address1,
                P_ZipPostalCode,
                P_PhoneNumber,
                P_FaxNumber,
                P_CustomerId,
            };
            string query = "EXEC dbo.Sp_InsertCustomerAddress @FirstName, @LastName, @Email, @Company, @CountryId, @StateProvinceId, @City, @Address1, @Address2, @ZipPostalCode, @PhoneNumber, @FaxNumber , @CustomerId ";
            return _dbContext.SqlQuery<int>(query, prms).SingleOrDefault();

        }
        private int ProcessAddress(
           string firstName,
           string lastName,
           string phoneNumber,
           string email,
           string faxNumber,
           string company,
           string address1,
           string address2,
           string city,
           int stateProvId,
           int countryId,
           string zipCode,
           int customerId
       )
        {
            #region Check Customer Address Exist
            SqlParameter P_CustomerId = new SqlParameter()
            {
                ParameterName = "CustomerId",
                SqlDbType = SqlDbType.Int,
                Value = (object)customerId
            };
            SqlParameter P_CountryId = new SqlParameter()
            {
                ParameterName = "CountryId",
                SqlDbType = SqlDbType.Int,
                Value = (object)countryId
            };
            SqlParameter P_StateId = new SqlParameter()
            {
                ParameterName = "StateId",
                SqlDbType = SqlDbType.Int,
                Value = (object)stateProvId
            };
            SqlParameter P_FirstName = new SqlParameter()
            {
                ParameterName = "FirstName",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)firstName ?? ""
            };
            SqlParameter P_LastName = new SqlParameter()
            {
                ParameterName = "LastName",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)lastName ?? ""
            };
            SqlParameter P_PhoneNumber = new SqlParameter()
            {
                ParameterName = "PhoneNumber",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)phoneNumber ?? ""
            };
            SqlParameter P_Address = new SqlParameter()
            {
                ParameterName = "Address",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)address1 ?? ""
            };
            SqlParameter P_Company = new SqlParameter()
            {
                ParameterName = "Company",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)company ?? (object)DBNull.Value

            };
            SqlParameter P_Address2 = new SqlParameter()
            {
                ParameterName = "Address2",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)address2 ?? (object)DBNull.Value
            };
            SqlParameter[] prms = new SqlParameter[]
            {
                P_CustomerId,
                P_CountryId,
                P_StateId,
                P_FirstName,
                P_LastName,
                P_PhoneNumber,
                P_Address,
                P_Company,
                P_Address2
            };
            string Query = @"EXEC dbo.Sp_CheckExistCustomerAddress @CustomerId, @CountryId, @StateId, @FirstName, @LastName, @PhoneNumber, @Address, @Company, @Address2";
            int AddressId = _dbContext.SqlQuery<int?>(Query, prms).FirstOrDefault().GetValueOrDefault(0);
            var customer = _customerService.GetCustomerById(customerId);
            if (AddressId == 0)
            {
                if (string.IsNullOrEmpty(zipCode) || zipCode.Length != 10)
                {
                    zipCode = getZipCode(stateProvId);
                }

                var newAddress = new AddressModel()
                {
                    FirstName = firstName.Trim(),
                    LastName = lastName.Trim(),
                    PhoneNumber = phoneNumber.Trim(),
                    Email = (email ?? "").Trim(),
                    FaxNumber = (faxNumber ?? "").Trim(),
                    Company = (company ?? "").Trim(),
                    Address1 = (address1 ?? "").Trim(),
                    Address2 = (address2 ?? "").Trim(),
                    City = (city ?? "").Trim(),
                    StateProvinceId = stateProvId,
                    CountryId = countryId,
                    ZipPostalCode = zipCode.Trim(),
                };
                if (newAddress.CountryId == 0)
                    newAddress.CountryId = null;
                if (newAddress.StateProvinceId == 0)
                    newAddress.StateProvinceId = null;
                var MyAddress = newAddress.ToEntity();
                MyAddress.CreatedOnUtc = DateTime.Now;
                MyAddress.CustomAttributes = "";
                _addressService.InsertAddress(MyAddress);
                customer.Addresses.Add(MyAddress);
                _customerService.UpdateCustomer(customer);
                return MyAddress.Id;
            }
            else
            {
                var address = _addressService.GetAddressById(AddressId);
                if (string.IsNullOrEmpty(zipCode) || zipCode.Length != 10)
                {
                    address.ZipPostalCode = getZipCode(address.StateProvinceId.Value);
                    _addressService.UpdateAddress(address);
                }
                return address.Id;
            }
            #endregion           
        }
        public void Log(string header, string message)
        {
            var logger =
                (DefaultLogger)EngineContext.Current
                    .Resolve<ILogger>();
            logger.InsertLog(LogLevel.Information, header, message, null);
        }

        #region BulkOrder Table

        public void InsertBulkOrder(BulkOrderModel model)
        {
            if (model == null)
                return;
            _bulkOrderRepository.Insert(model);
        }
        public void UpdateBulkOrder(BulkOrderModel model)
        {
            if (model == null)
                return;
            _bulkOrderRepository.Update(model);
        }
        public BulkOrderModel GetBulkOrder(int bulkOrderId)
        {
            if (bulkOrderId == 0)
                return null;
            return _bulkOrderRepository.Table.SingleOrDefault(p => p.Id == bulkOrderId);
        }

        public void ChangeBulkOrderStatus(int bulkOrderId, OrderStatus orderStatus)
        {
            var bOrder = GetBulkOrder(bulkOrderId);
            if (bOrder == null)
                return;
            bOrder.OrderStatusId = (int)orderStatus;
            UpdateBulkOrder(bOrder);
        }

        public List<BulkOrderModel> getBulkOrderList(out int count, int pageIndex = 0, int pageSize = 999999,
            int customerId = 0,
            string CustomerName = null, DateTime? createDateFrom = null, DateTime? createDateTo = null,
            int OrderStatusId = 0, int PaymentStatusId = 0)
        {
            var query = @"SELECT TOP(100)
	                            BO.Id,
                                BO.FileName,
                                ISNULL(O.OrderStatusId,10) OrderStatusId,
                                ISNULL(O.PaymentStatusId,10) as PaymentStatusId,
                                BO.CreateDate,
                                IsCod,
                                BO.CustomerId,
	                            CASE  WHEN o.Id IS NULL THEN 0 WHEN o.OrderTotal = 0 AND o.PaymentMethodSystemName IS NULL THEN RPH.Points * -1 ELSE ISNULL(O.OrderTotal,0) END OrderTotal
	                            ,BO.OrderId
	                            ,BO.OrderCount
	                            ,CASE WHEN A.Id IS NULL THEN ISNULL(C.Email,'')+'__'+C.Username  else ISNULL(A.FirstName,'')+' '+ISNULL(A.LastName,'') END AS CustomerName 
                                ,Bo.discountCouponCode
                                ,Bo.FileType
                                ,Bo.SendSms
                                ,Bo.PrintLogo
                                ,Bo.ServiceSort
                                ,Bo.HasAccessToPrinter
                                ,Bo.OrderIds
                                ,ISNULL(Ct.Name,'') CategoryName
                            FROM
	                            dbo.BulkOrder AS BO
	                            INNER JOIN dbo.Customer AS C ON Bo.CustomerId = C.Id
                                LEFT JOIN dbo.[order] O on BO.OrderId = O.Id
	                            LEFT JOIN dbo.Address AS A ON A.Id = ISNULL(C.BillingAddress_Id,O.BillingAddressId)
	                            LEFT JOIN dbo.RewardPointsHistory AS RPH ON RPH.UsedWithOrder_Id = O.Id
                                LEFT JOIN dbo.Category AS Ct ON BO.ServiceId = Ct.Id
                            WHERE
                                C.Deleted = 0 AND BO.Deleted = 0";
            count = 0;
            var bulkOrder = _bulkOrderRepository.Table;
            var strWhere = "";
            if (customerId != 0) strWhere += "AND C.Id =" + customerId;
            if (createDateFrom != null) strWhere += "AND CreateDate >='" + createDateFrom + "'";
            if (createDateTo != null) strWhere += "AND BO.CreateDate <= '" + createDateTo + "'";
            if (OrderStatusId != 0) strWhere += "AND O.OrderStatusId = " + OrderStatusId;
            if (PaymentStatusId != 0) strWhere += "AND O.PaymentStatusId =" + PaymentStatusId;
            if (!string.IsNullOrEmpty(CustomerName))
                strWhere += "AND ISNULL(A.FirstName,'')+' '+ISNULL(A.LastName,'') LIKE N'%" + CustomerName + "%'";

            query += (strWhere ?? "") + " Order BY BO.Id Desc";
            var data = _dbContext.SqlQuery<BulkOrderModel>(query).AsQueryable();
            count = data.Count();
            return new PagedList<BulkOrderModel>(data, pageIndex, pageSize).ToList();
        }

        public bool DeleteBulkOrder(int Id, int customerId, out string msg)
        {
            msg = "حذف با موفقیت انجام شد";
            var bulkOrder = this.GetBulkOrder(Id);
            if (bulkOrder == null)
                return true;

            if (bulkOrder.OrderId != 0)
            {

                var order = _orderService.GetOrderById(bulkOrder.OrderId);
                if (customerId != 0)
                {
                    if (order.CustomerId != customerId)
                    {
                        msg = "امکان حذف این سفارش برای شما وجود ندارد";
                        return false;
                    }
                }

                if (order.Shipments.Any(p => p.ShippedDateUtc.HasValue || p.DeliveryDateUtc.HasValue))
                {
                    msg = "امکان حذف درخواست وجود ندارد بسته/ها تحویل مرکز پست شده است";
                    return false;
                }
                //if (_shipmentTrackingService.HasTrackingRecourd(bulkOrder.OrderId))
                //{
                //    msg = "امکان حذف درخواست وجود ندارد بسته تحویل مرکز پست شده است";
                //    return false;
                //}
                order.OrderStatus = OrderStatus.Cancelled;
                order.Deleted = true;
                _orderService.UpdateOrder(order);
            }

            bulkOrder.Deleted = true;
            _bulkOrderRepository.Update(bulkOrder);
            return true;
        }

        #endregion
        /// <summary>
        /// ارسال اطلاعات به پست برای کاربرانی که سفارشات دو مرحله ای دارند
        /// </summary>
        /// <param name="order"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public bool _SendDataToPost(Order order, out string strError)
        {
            return _newCheckout._SendDataToPost(order, out strError);
        }
        public List<SelectListItem> getForinCountry()
        {
            return _newCheckout.getForginCountry();
        }
        public System.Diagnostics.Stopwatch StartStopwatch()
        {
            var watch = new System.Diagnostics.Stopwatch();
            long Total = 0;
            watch.Start();
            return watch;
        }
        public void RestartStopwatch(System.Diagnostics.Stopwatch watch, string logNote, ref long Total)
        {
            #region زمان سنجی
            watch.Stop();
            common.Log(logNote + ":" + watch.ElapsedMilliseconds, "");
            Total += watch.ElapsedMilliseconds;
            watch.Reset();
            watch.Start();
            #endregion
        }
        public List<SelectListItem> getcategoryFileType(int FileType,bool ispeyk)
        {
            string query = "";
            if (!ispeyk)
            {
                query = $@"
                            SELECT
                            	CAST(C.Id AS VARCHAR(10)) Value,
                            	C.Name Text
                            FROM
                            	dbo.Category AS C
                            	INNER JOIN dbo.Tb_CategoryInfo AS TCI ON TCI.CategoryId = C.Id
                            WHERE
                            	c.Published = 1
                            	AND c.Deleted = 0
                            	AND (@FileType = 4 OR (@FileType = 1 AND Tci.IsCod = 0 AND TCI.IsHeavyTransport = 0 AND Tci.IsForeign= 0)
                            	OR (@FileType = 2 AND Tci.IsCod = 1)
                            	OR (@FileType = 3 AND Tci.IsForeign = 1))";
            }
            else
            {
                query = $@"SELECT
                            	CAST(C.Id AS VARCHAR(10)) Value,
                            	C.Name Text
                            FROM
                            	dbo.Category AS C
                            	INNER JOIN dbo.Tb_CategoryInfo AS TCI ON TCI.CategoryId = C.Id
                            WHERE
                            	c.Published = 1
                            	AND c.Deleted = 0
                            	AND C.id = 718";
            }
            SqlParameter P_FileType = new SqlParameter()
            {
                ParameterName = "FileType",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)FileType
            };
            SqlParameter[] prms = new SqlParameter[]
           {
                P_FileType
           };
            return _dbContext.SqlQuery<SelectListItem>(query, prms).ToList();
        }
        public CategoryInfoModel GetCategoryInfo(int categoryId)
        {
            string query = $@"Select
                                TCI.*,
	                            C.Name AS CategoryName
                            FROM
	                            dbo.Tb_CategoryInfo AS TCI
	                            INNER JOIN dbo.Category AS C ON C.Id = TCI.CategoryId
                            WHERE
	                            TCI.CategoryId = @CategoryId";
            SqlParameter P_CategoryId = new SqlParameter()
            {
                ParameterName = "CategoryId",
                SqlDbType = SqlDbType.Int,
                Value = (object)categoryId
            };
            SqlParameter[] prms = new SqlParameter[]{
                P_CategoryId
            };
            return _dbContext.SqlQuery<CategoryInfoModel>(query, prms).FirstOrDefault();
        }
        public CategoryInfoModel GetCategoryInfo(Nop.Core.Domain.Catalog.Product product)
        {
            int catId = ((int?)product.ProductCategories.FirstOrDefault(p => p.Category.Published == true && p.Category.Deleted == false)?.CategoryId).GetValueOrDefault(0);
            return GetCategoryInfo(catId);
        }

    }
    public class StateCodemodel : BaseEntity
    {
        public int stateId { get; set; }
        public string StateCode { get; set; }
        public string SenderStateCode { get; set; }
    }
    public class common
    {
        public static void LogException(Exception ex)
        {
            DataSettingsManager p = new DataSettingsManager();
            var dataSettings = p.LoadSettings();
            string connectionString = dataSettings.DataConnectionString;
            var fullMessage = ex?.ToString() ?? string.Empty;

            // define INSERT query with parameters
            string query = $@"INSERT INTO dbo.Log
		                        (
			                        LogLevelId
			                        , ShortMessage
			                        , FullMessage
			                        , IpAddress
			                        , CustomerId
			                        , PageUrl
			                        , ReferrerUrl
			                        , CreatedOnUtc
		                        )
		                        VALUES
		                        (	40 -- LogLevelId - int
			                        , N'{ex.Message.Replace("'", "''")}' -- ShortMessage - nvarchar(max)
			                        , N'{fullMessage.Replace("'", "''")}' -- FullMessage - nvarchar(max)
			                        , N'127.0.0.1' -- IpAddress - nvarchar(200)
			                        , NULL -- CustomerId - int
			                        , N'خطا های ثبت شده در زمان ارتباط با سرویس های همکار' -- PageUrl - nvarchar(max)
			                        , N'' -- ReferrerUrl - nvarchar(max)
			                        , GETDATE() -- CreatedOnUtc - datetime
			                        )";

            // create connection and command
            using (SqlConnection cn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }
        public static void Log(string shortMessage, string fullMessage)
        {
            DataSettingsManager p = new DataSettingsManager();
            var dataSettings = p.LoadSettings();
            string connectionString = dataSettings.DataConnectionString;


            // define INSERT query with parameters
            string query = $@"INSERT INTO dbo.Log
		                        (
			                        LogLevelId
			                        , ShortMessage
			                        , FullMessage
			                        , IpAddress
			                        , CustomerId
			                        , PageUrl
			                        , ReferrerUrl
			                        , CreatedOnUtc
		                        )
		                        VALUES
		                        (	40 -- LogLevelId - int
			                        , N'{shortMessage.Replace("'", "''")}' -- ShortMessage - nvarchar(max)
			                        , N'{fullMessage.Replace("'", "''")}' -- FullMessage - nvarchar(max)
			                        , N'127.0.0.1' -- IpAddress - nvarchar(200)
			                        , NULL -- CustomerId - int
			                        , N'خطا های ثبت شده در زمان ارتباط با سرویس های همکار' -- PageUrl - nvarchar(max)
			                        , N'' -- ReferrerUrl - nvarchar(max)
			                        , GETDATE() -- CreatedOnUtc - datetime
			                        )";

            // create connection and command
            using (SqlConnection cn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }
        public static void InsertOrderNote(string Note, int orderId)
        {
            DataSettingsManager p = new DataSettingsManager();
            var dataSettings = p.LoadSettings();
            string connectionString = dataSettings.DataConnectionString;
            string query = $@"INSERT INTO  dbo.OrderNote
                    (
	                    OrderId
	                    , Note
	                    , DownloadId
	                    , DisplayToCustomer
	                    , CreatedOnUtc
                    )
                    VALUES
                    (	{orderId} -- OrderId - int
	                    , N'{Note}' -- Note - nvarchar(max)
	                    , 0 -- DownloadId - int
	                    , 0 -- DisplayToCustomer - bit
	                    , GETDATE() -- CreatedOnUtc - datetime
                    )";
            using (SqlConnection cn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }
    }
    public class CategoryInfoModel : BaseEntity
    {
        public int CategoryId { get; set; }
        public bool IsCod { get; set; }
        public bool IsPishtaz { get; set; }
        public bool IsSefareshi { get; set; }
        public bool IsVIje { get; set; }
        public bool IsNromal { get; set; }
        public bool IsDroonOstani { get; set; }
        public bool IsAdjoining { get; set; }
        public bool IsNotAdjacent { get; set; }
        public bool IsHeavyTransport { get; set; }
        public bool IsForeign { get; set; }
        public bool IsInCity { get; set; }
        public bool IsAmanat { get; set; }
        public bool IsTwoStep { get; set; }
        public bool HasHagheMaghar { get; set; }
        public string CategoryName { get; set; }
    }
}