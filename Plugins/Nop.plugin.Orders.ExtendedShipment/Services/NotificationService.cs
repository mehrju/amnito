using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Models.NotificationModel;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using kavenegarDLL;
using kavenegarDLL.Models;
using Nop.Services.Logging;
using Nop.Core.Infrastructure;
using Nop.Core.Domain.Logging;
using System.Data.SqlClient;
using System.Data;
using Nop.Web.Models.Common;
using Nop.Core.Domain.Common;
using Nop.Services.Stores;
using Nop.Services.Common;
using Nop.plugin.Orders.ExtendedShipment.Domain;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IShipmentService _shipmentService;
        private readonly IRepository<ShipmentAppointmentModel> _ShipmentAppointmentRepository;
        private readonly IOrderService _orderService;
        private readonly IDbContext _dbContext;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerService _customerService;
        private readonly IRepository<NotifItemsModel> _NotifItemsRepository;
        private readonly IRepository<NotifTitleModel> _NotifTitleRepository;
        private readonly string ApiKey;
        private readonly string LineNumber;
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly IRepository<OrderItemAttributeValueModel> _OrderItemAttributeValueRepository;
        private readonly IAddressService _addressService;
        private readonly IRepository<Tbl_PopupNotification> _repository_PopupNotification;

        public NotificationService(IShipmentService shipmentService
        , IRepository<ShipmentAppointmentModel> ShipmentAppointmentRepository
        , IOrderService orderService
        , IDbContext dbContext
        , ISettingService settingService
        , IStoreContext storeContext
        , ICustomerService customerService
        , IRepository<NotifItemsModel> NotifItemsRepository
        , IRepository<NotifTitleModel> NotifTitleRepository
        , IWorkContext workContext
         , IStoreService storeService
            , IRepository<OrderItemAttributeValueModel> OrderItemAttributeValueRepository
            , IAddressService addressService,
            IRepository<Tbl_PopupNotification> repository_PopupNotification
            )
        {
            _addressService = addressService;
            _repository_PopupNotification = repository_PopupNotification;
            _storeService = storeService;
            _workContext = workContext;
            _NotifItemsRepository = NotifItemsRepository;
            _dbContext = dbContext;
            _shipmentService = shipmentService;
            _ShipmentAppointmentRepository = ShipmentAppointmentRepository;
            _orderService = orderService;
            _settingService = settingService;
            _storeContext = storeContext;
            _customerService = customerService;
            _NotifTitleRepository = NotifTitleRepository;
            ApiKey = _settingService.GetSettingByKey(key: "phoneloginsettings.apikey", defaultValue: "72754D326847724F48345263306A45573649333957773D3D", storeId: 3);
            LineNumber = _settingService.GetSettingByKey(key: "phoneloginsettings.linenumber", defaultValue: "10004440044440", storeId: 3);
            _OrderItemAttributeValueRepository = OrderItemAttributeValueRepository;
        }
        /// <summary>
        /// اطلاع رسانی جمع آوری محوله
        /// </summary>
        public void NotifyCollectShipment(int shipmentId)
        {
            var shipment = _shipmentService.GetShipmentById(shipmentId);
            SendSmsByTemplate(null, shipment, 8, false);
            SendSmsByTemplate(null, shipment, 9, true);
        }
        /// <summary>
        /// ارسال پیام برای فرستنده و گیرنده
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="_extendedShipmentService"></param>
        public void SendSmsToSender_Reciver(int orderId, IExtendedShipmentService _extendedShipmentService)
        {
            //var order = _orderService.GetOrderById(orderId);
            //if (order.BillingAddress == null)
            //    return;
            //if (!order.Shipments.Any())
            //    return;
            //string trakingNUmbers = string.Join("\r\n",
            //      order.Shipments.Where(p => !string.IsNullOrEmpty(p.TrackingNumber)).Select(p => p.TrackingNumber));
            //string Msg = "";
            //bool sended = false;
            //if (!order.Customer.IsInCustomerRole("mini-Administrators"))
            //{
            //    Msg = $"مشتری گرامی سفارش پستی شماره {order.Id} برای شما ثبت شد. کد/های رهگیری:" + trakingNUmbers ?? "";
            //    sended = _sendSms(order.BillingAddress.PhoneNumber, Msg);
            //}
            //bool isMultiShpment = _extendedShipmentService.IsMultiShippment(order);
            //if (!isMultiShpment && order.ShippingAddress != null)
            //{
            //    Msg = "با سلام" + "\r\n" + " مرسوله پستی برای شما در سامانه " + "Postbar.ir" + " ثبت گردید. " + " کد/های رهگیری:" + "\r\n" + trakingNUmbers ?? "";
            //    _sendSms(order.ShippingAddress.PhoneNumber, Msg);
            //}
            //else
            //{
            //    string query = @"SELECT
            //     A.PhoneNumber
            //     ,dbo.FnStr_Agg(s.TrackingNumber) TrackingNumbers 
            //    FROM
            //     dbo.Shipment AS S
            //     INNER JOIN dbo.XtnShippment AS XS ON xs.ShipmentId = S.Id
            //     INNER JOIN dbo.Address AS A ON xs.ShippmentAddressId = A.Id
            //    WHERE
            //     S.OrderId = " + order.Id + @"
            //     AND S.TrackingNumber IS NOT NULL
            //     AND S.TrackingNumber <> ''
            //     AND A.PhoneNumber IS NOT NULL
            //        AND a.PhoneNumber <> ''
            //    GROUP BY
            //     A.PhoneNumber";
            //    var Data = _dbContext.SqlQuery<MultiShippingTrackingData>(query, new object[0]).ToList();
            //    if (Data == null)
            //        return;
            //    foreach (var item in Data)
            //    {
            //        try
            //        {
            //            Msg = "با سلام" + "\r\n" + " مرسوله پستی برای شما در سامانه " + "Postbar.ir" + " ثبت گردید. " + " کد/های رهگیری:" + "\r\n" + item.TrackingNumbers ?? "";
            //            sended = _sendSms(item.PhoneNumber, Msg);
            //        }
            //        catch
            //        {
            //        }
            //    }
            //}
        }
        /// <summary>
        /// ارسال پیام برای ناظر پست
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="_extendedShipmentService"></param>
        public void NotifyPostSupervisor(Order order, int serviceId, IExtendedShipmentService _extendedShipmentService)
        {
            var extendedShipmentSetting = _settingService.LoadSetting<ExtendedShipmentSetting>(_storeContext.CurrentStore.Id);
            if (extendedShipmentSetting == null)
                return;
            if (order == null)
                return;
            string postAdminPhoneNumber = "";
            if (new int[] { 662, 654, 655,725,726,727 }.Contains(serviceId))
            {
                int senderStateId = 0;
                if (!_extendedShipmentService.getDefulteSenderState(order.CustomerId, out senderStateId))
                {
                    senderStateId = order.BillingAddress.StateProvinceId.Value;
                }
                var errorMsg = order.BillingAddress == null
                    ? "آدرس فرستنده خالی است"
                    : (!order.BillingAddress.CountryId.HasValue ? "استان فرستنده مشخص نیست" : "");

                if (!string.IsNullOrEmpty(errorMsg))
                {
                    _extendedShipmentService.Log("خطا در زمان ارسال پیام سفارش جدید" + extendedShipmentSetting.PostAdminRoleId, errorMsg);
                    return;
                }

                if (!CheckSmsSendedByBillAddress(order))
                {
                    var customers = _customerService.GetAllCustomers(
                        customerRoleIds: new[] { extendedShipmentSetting.PostAdminRoleId },
                        pageIndex: 0,
                        pageSize: 5000).Where(p => p.Addresses.Any() && p.Deleted == false && p.Active);
                    if (!customers.Any())
                    {
                        _extendedShipmentService.Log("خطا در زمان ارسال پیام سفارش جدید", "ناظر مورد نظر یافت نشد");
                        return;
                    }

                    Customer customer = null;
                    if (order.BillingAddress.Country.Name == "تهران")
                        try
                        {
                            customer = customers.Where(p => p.Addresses.First().CountryId == order.BillingAddress.CountryId
                                                            && p.Addresses.First().StateProvinceId == senderStateId
                                                            )
                                .OrderByDescending(p => p.Id)
                                .FirstOrDefault();
                        }
                        catch
                        {
                        }

                    if (customer == null)
                        customer = customers.Where(p => p.Addresses.First().CountryId == order.BillingAddress.CountryId)
                            .OrderByDescending(p => p.Id)
                            .FirstOrDefault();

                    if (customer == null)
                    {
                        _extendedShipmentService.Log("خطا در زمان ارسال پیام سفارش جدید", "ناظر مورد نظر یافت نشد");
                        return;
                    }

                    postAdminPhoneNumber = _extendedShipmentService.getPhoneNumber(customer.Id);

                }
                else if (new int[] { 703, 699, 705, 706 }.Contains(serviceId))//DTS
                {
                    postAdminPhoneNumber = "09371100191";
                }
                else if (new int[] { 712, 713, 714, 715 }.Contains(serviceId))//chapar
                {
                    postAdminPhoneNumber = "09024202282";
                }
                if (string.IsNullOrWhiteSpace(postAdminPhoneNumber))
                {
                    _extendedShipmentService.Log("خطا در زمان ارسال پیام سفارش جدید", "شماره همراه ناظر مورد نظر نامعتبر است");
                    return;
                }
                int TotalWeight = _extendedShipmentService.getTotalWeight(order);
                var StrPostAdminSms = string.Format(extendedShipmentSetting.PostAdminMessageTemplate
                    , order.BillingAddress.PhoneNumber
                    , ((order.BillingAddress.FirstName ?? "") + " " + (order.BillingAddress.LastName ?? ""))
                      + "-" + order.BillingAddress.Country.Name
                      + "-" + order.BillingAddress.StateProvince.Name
                      + "-" + order.BillingAddress.Address1);

                StrPostAdminSms += $" وزن این سفارش حدود {TotalWeight} گرم می باشد ";

                var sended1 = _sendSms(postAdminPhoneNumber, StrPostAdminSms);
                if (!sended1)
                {
                    _extendedShipmentService.Log("عدم ارسال پیام سفارش جدید" + order.Id,
                        postAdminPhoneNumber + "" + extendedShipmentSetting.PostAdminMessageTemplate);
                }
            }
        }
        /// <summary>
        /// بررسی اینکه آیا برای این آدرس برای ناظر پیام ارسال شده یا نه
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private bool CheckSmsSendedByBillAddress(Order order)
        {
            string query = $@"DECLARE @CountryId INT ,@stateId INT, @Address NVARCHAR(2000),@phoneNumber VARCHAR(14)
                            SELECT
	                            @CountryId = A.CountryId,
	                            @stateId = A.StateProvinceId,
	                            @Address = A.Address1,
	                            @phoneNumber = A.PhoneNumber
                            FROM
	                            dbo.Address AS A
                            WHERE
	                            A.Id = {order.BillingAddressId}

                            SELECT
	                            TOP(1) O.Id
                            FROM
	                            dbo.[Order] AS O
	                            INNER JOIN dbo.PostCoordination AS PC ON PC.orderId = O.Id
	                            INNER JOIN dbo.Address AS A ON A.Id = O.BillingAddressId
                            WHERE
	                            O.Id <> {order.Id}
	                            AND CAST(Pc.CoordinationDate AS DATE) = CAST(GETDATE() AS DATE)
	                            AND O.OrderStatusId = 30
	                            AND O.CustomerId = {order.CustomerId}
	                            AND a.CountryId = @CountryId
	                            AND A.StateProvinceId  =@stateId
	                            AND a.Address1 = @Address
	                            AND A.PhoneNumber = @phoneNumber";
            int? orderId = _dbContext.SqlQuery<int>(query, new object[0]).FirstOrDefault();
            return orderId.HasValue && orderId > 0;
        }
        /// <summary>
        /// ارسال پیام سفارش برای مدیر سایت
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="_extendedShipmentService"></param>
        public void sendSmsPostAdminForNewOrder(string orderIds, IExtendedShipmentService _extendedShipmentService)
        {
            //var extendedShipmentSetting = _settingService.LoadSetting<ExtendedShipmentSetting>(_storeContext.CurrentStore.Id);
            //if (extendedShipmentSetting == null)
            //    return;
            //var storeAdmin = _customerService.GetAllCustomers(
            //    customerRoleIds: new[] { extendedShipmentSetting.StoreAdminRoleId },
            //    pageIndex: 0,
            //    pageSize: 5000).OrderByDescending(p => p.Id).FirstOrDefault();

            //if (storeAdmin == null)
            //{
            //    _extendedShipmentService.Log("خطا در زمان ارسال پیام سفارش جدید برای مدیر", "مدیر فروشگاه مورد نظر یافت نشد");
            //    return;
            //}

            //var StoreAdminMobileNo = _extendedShipmentService.getPhoneNumber(storeAdmin.Id);
            //if (string.IsNullOrEmpty(StoreAdminMobileNo))
            //    _extendedShipmentService.Log("خطا در زمان ارسال پیام سفارش جدید برای مدیر", "شماره همراه مدیر فروشگاه نامعتبر است");
            //var AdminMsg = string.Format(extendedShipmentSetting.StoreAdminMessageTemplate, orderIds);
            //var sended = _sendSms(StoreAdminMobileNo, AdminMsg);
            //if (sended)
            //    _extendedShipmentService.Log("ارسال پیام ارجا سفارش به کارشناس برای مدیر " + orderIds, "");
            //else
            //    _extendedShipmentService.Log("عدم ارسال پیام ارجا سفارش به کارشناس برای مدیر " + orderIds, "");

        }
        /// <summary>
        /// ارسال پیام کنسل شدن سفارش برای ناظر پست و مشتری
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="_extendedShipmentService"></param>
        public void NotifyForCancel(int orderId, IExtendedShipmentService _extendedShipmentService)
        {

            var extendedShipmentSetting = _settingService.LoadSetting<ExtendedShipmentSetting>(_storeContext.CurrentStore.Id);
            if (extendedShipmentSetting == null)
                return;
            var order = _orderService.GetOrderById(orderId);

            if (order.OrderStatus == OrderStatus.Pending)
                return;
            int serviceId = order.OrderItems.First().Product.ProductCategories.First().CategoryId;

            if (order == null)
                return;
            if (order.Customer.IsInCustomerRole("mini-Administrators"))
            {
                return;
            }
            _extendedShipmentService.InsertOrderNote("SendSmsForCancel", orderId);
            if (order.Shipments.Any() && order.Shipments.Any(p => !string.IsNullOrEmpty(p.TrackingNumber))
                && _extendedShipmentService.IsPostService(serviceId))
            {
                var customers = _customerService.GetAllCustomers(
                    customerRoleIds: new[] { extendedShipmentSetting.PostAdminRoleId },
                    pageIndex: 0,
                    pageSize: 5000).Where(p => p.Addresses.Any() && p.Deleted == false && p.Active);
                if (!customers.Any())
                {
                    _extendedShipmentService.InsertOrderNote(
                        "خطا در زمان ارسال پیام کنسل شدن سفارش-ناظر مورد نظر یافت نشد", order.Id);
                    return;
                }

                Customer customer = null;
                if (order.BillingAddress.Country.Name == "تهران")
                    try
                    {
                        customer = customers.Where(p => p.Addresses.First().CountryId == order.BillingAddress.CountryId
                                                        && p.Addresses.First().StateProvinceId ==
                                                        order.BillingAddress.StateProvinceId)
                            .OrderByDescending(p => p.Id)
                            .FirstOrDefault();
                    }
                    catch
                    {
                    }

                if (customer == null)
                    customer = customers.Where(p => p.Addresses.First().CountryId == order.BillingAddress.CountryId)
                        .OrderByDescending(p => p.Id)
                        .FirstOrDefault();

                if (customer == null)
                {
                    _extendedShipmentService.InsertOrderNote(
                        "خطا در زمان ارسال پیام کنسل شدن سفارش-ناظر مورد نظر یافت نشد", order.Id);
                    return;
                }

                var postAdminPhoneNumber = _extendedShipmentService.getPhoneNumber(customer.Id);
                if (string.IsNullOrWhiteSpace(postAdminPhoneNumber))
                {
                    _extendedShipmentService.InsertOrderNote(
                        "خطا در زمان ارسال پیام  کنسل شدن سفارش-شماره همراه ناظر پست مورد نظر نامعتبر است", order.Id);
                    return;
                }

                var StrPostAdminSms =
                    $"ناظر محترم محموبه پستی با شماره سفارش {order.Id} کنسل شد. لطفا برای جمع آوری اقدام نشود";
                var sended1 = _sendSms(postAdminPhoneNumber, StrPostAdminSms);
                if (!sended1)
                {
                    _extendedShipmentService.Log("عدم ارسال  پیام  کنسل شدن سفارش  برای ناظر پست" + order.Id,
                        postAdminPhoneNumber + "" + extendedShipmentSetting.PostAdminMessageTemplate);
                }
            }
            if (order.BillingAddress == null)
                return;


            SendSmsByTemplate(order, null, 14, false);
            foreach (var item in order.Shipments)
            {
                SendSmsByTemplate(null, item, 15, true);
            }

        }
        public NotifTemplate getNotifTamplate(int NotifTypeId, Order order, Shipment shipment)
        {
            if (order == null && shipment == null)
                return null;
            string query = $@"SELECT
	                            TOP(1)TNI.NotifTitleId,
	                            TNI.NotifTamplate
                            FROM
	                            dbo.Tb_NotifItems AS TNI
	                            INNER JOIN dbo.Tb_NotifTitle AS TNT  ON TNT.Id = TNI.NotifTitleId
                            WHERE
	                            TNI.NotifTitleId = {NotifTypeId}
	                            AND TNI.NotifTypeId = 1
	                            AND tnt.IsActive = 1";
            var result = _dbContext.SqlQuery<NotifTemplate>(query, new object[0]).FirstOrDefault();
            if (result == null)
                return null;
            int orderId = order != null ? order.Id : shipment.OrderId;
            int shipmentId = shipment != null ? shipment.Id : 0;
            string TrakcingNumber = shipment != null ? (shipment.TrackingNumber != null ? shipment.TrackingNumber : "") : "";
            if(NotifTypeId== 3 && string.IsNullOrEmpty(TrakcingNumber))
            {
                return null;
            }
            string SenderPhone = "";
            string SenderName = "";
            string SenderAddress = "";
            string StoreUrl = "";
            string AllTrackingNumbers = "";
            if (order != null)
            {
                StoreUrl = _storeService.GetStoreById(order.StoreId).Url;
                if (order.BillingAddress != null)
                {
                    SenderPhone = order.BillingAddress.PhoneNumber ?? "";
                    SenderName = (order.BillingAddress.FirstName + " " + order.BillingAddress.LastName);
                    SenderAddress = order.BillingAddress.Country?.Name + "-" + order.BillingAddress?.StateProvince.Name;
                }
                if (order.Shipments.Any(p => p.TrackingNumber != null))
                {
                    AllTrackingNumbers = string.Join(",", order.Shipments.Select(p => p.TrackingNumber).ToList());
                }
            }
            string ReciverPhoneNumber = "";
            string ReceiverName = "";
            string ReciverAddress = "";
            if (shipment != null)
            {
                var shippingAddress = getShippingAddress(shipment.Id);
                if (shippingAddress != null)
                {
                    ReciverPhoneNumber = shippingAddress.PhoneNumber;
                    ReceiverName = shippingAddress.FirstName + " " + shippingAddress.LastName;
                    ReciverAddress = shippingAddress.Country?.Name + "-" + shippingAddress?.StateProvince.Name;
                }
                if (StoreUrl == "")
                {
                    StoreUrl = _storeService.GetStoreById(shipment.Order.StoreId).Url;
                }
            }
            else if (order.ShippingAddress != null)
            {
                ReciverPhoneNumber = order.ShippingAddress.PhoneNumber ?? "";
                ReceiverName = order.ShippingAddress.FirstName + " " + order.ShippingAddress.LastName;
                ReciverAddress = order.ShippingAddress.Country?.Name + "-" + order.ShippingAddress?.StateProvince.Name;
            }
            result.NotifTamplate = string.Format(result.NotifTamplate, orderId, shipmentId, TrakcingNumber, SenderPhone, ReciverPhoneNumber, StoreUrl, "", AllTrackingNumbers, "", "", SenderName, ReceiverName,
                SenderAddress, ReciverAddress);
            result.ReciverPhoneNumber = ReciverPhoneNumber;
            result.SenderPhoneNumber = SenderPhone;
            return result;
        }
        public Address getShippingAddress(int shipmentId)
        {
            string query = $@"SELECT
		                            Xs.ShippmentAddressId
	                            FROM
		                            dbo.Shipment AS S
		                            INNER JOIN dbo.XtnShippment AS XS ON XS.ShipmentId = S.Id
	                            WHERE
		                            S.Id = {shipmentId}";
            int AddressId = _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
            if (AddressId == 0)
                return null;
            return _addressService.GetAddressById(AddressId);
        }
        public List<NotifItemsModel> getNotifItem(int NotifTypeId)
        {
            string Query = @"SELECT
	                            TNI.Id,
	                            TNT.NotifTypeName,
	                            TNOT.NotifTitleName +CASE WHEN TNOT.IsActive = 0 THEN N'غیر فعال' ELSE N'' END NotifTitle,
	                            C.Name NotifCategoryName,
	                            TNI.NotifTamplate,
	                            TNI.IsActive,
                                CASE WHEN TNI.IsActive = 1 THEN N'بلی' ELSE N'خیر' END Str_IsActive,
	                            TNI.NotifTitleId,
	                            TNI.NotifCategoryId,
	                            TNI.NotifTypeId
                            FROM
	                            dbo.Tb_NotifItems AS TNI
	                            INNER JOIN dbo.Tb_NotifType AS TNT ON TNT.Id = TNI.NotifTypeId
	                            INNER JOIN dbo.Tb_NotifTitle AS TNOT ON TNOT.Id = TNI.NotifTitleId
	                            INNER JOIN dbo.Category AS C ON C.Id = TNI.NotifCategoryId"
                    + (NotifTypeId > 0 ? " WHERE  TNI.NotifTypeId = " + NotifTypeId : "");
            return _dbContext.SqlQuery<NotifItemsModel>(Query, new object[0]).ToList();
        }
        /// <summary>
        /// ذخیره تملیت پیام ها
        /// </summary>
        /// <param name="model"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public bool SaveNotifItems(NotifItemsModel model, out string strError)
        {
            if (model.NotifCategoryId == 0)
            {
                strError = "توع سرویس پستی را وارد کنید";
                return false;
            }
            if (string.IsNullOrEmpty(model.NotifTamplate))
            {
                strError = "متن پیام را وارد کنید";
                return false;
            }
            if (model.NotifTitleId == 0)
            {
                strError = "عنوان متن پیام را وارد کنید";
                return false;
            }
            if (model.NotifTypeId == 0)
            {
                strError = "روش ارسال پیام را انتخاب کنید";
                return false;
            }
            if (model.Id != 0)
            {
                var item = _NotifItemsRepository.Table.SingleOrDefault(p => p.Id == model.Id);
                if (item == null)
                {
                    strError = "رکود مورد نظر جهت ویرایش یافت نشد";
                    return false;
                }
                item.IsActive = model.IsActive;
                item.NotifCategoryId = model.NotifCategoryId;
                item.NotifTamplate = model.NotifTamplate;
                item.NotifTitleId = model.NotifTitleId;
                item.NotifTypeId = model.NotifTypeId;
                _NotifItemsRepository.Update(item);
                strError = "ویرایش با موفقیت انجام شد";
                return true;
            }
            else
            {
                _NotifItemsRepository.Insert(model);
                strError = "درج با موفقیت انجام شد";
                return true;
            }

        }

        public NotifItemsModel IntiNotifModel(int NotifTypeId)
        {
            var listOfService = new List<SelectListItem>() { new SelectListItem() { Text = "انتخاب کنید...", Value = "0" } };
            var ListOfNotifTitle = new List<SelectListItem>() { new SelectListItem() { Text = "انتخاب کنید...", Value = "0" } };
            listOfService.AddRange(ListOfService());
            ListOfNotifTitle.AddRange(getListOfNotifTitle());
            var notifItemsModel = new NotifItemsModel()
            {
                AvailableCategory = listOfService,
                AvailableNotifTitle = ListOfNotifTitle,
                NotifTypeId = NotifTypeId
            };
            return notifItemsModel;
        }
        private List<SelectListItem> ListOfService()
        {
            string query = @"SELECT
	                            DISTINCT C.Name AS Name
	                            ,C.Id AS Id
                            FROM
	                            dbo.Category AS C
	                            INNER JOIN dbo.Tb_CategoryInfo AS TCI ON TCI.CategoryId = C.Id
                            WHERE	
	                            C.ParentCategoryId <> 0
                                AND C.Deleted = 0
	                            AND C.Published = 1
                            ORDER BY C.Name";
            return _dbContext.SqlQuery<_SelectListItem>(query, new object[0]).AsEnumerable().Select(p => new SelectListItem()
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).ToList();
        }
        private List<SelectListItem> getListOfNotifTitle()
        {
            string query = @"SELECT
	                            TNT.Id
	                            , TNT.NotifTitleName + CASE WHEN TNT.IsActive = 0 THEN N'-غیر فعال' ELSE N'' END Name
                            FROM
	                            dbo.Tb_NotifTitle AS TNT";
            return _dbContext.SqlQuery<_SelectListItem>(query, new object[0]).AsEnumerable().Select(p => new SelectListItem()
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).ToList();
        }
        /// <summary>
        /// لیست عنوانین اطلاع رسانی
        /// لیست عنوانین اطلاع رسانی
        /// </summary>
        /// <returns></returns>
        public List<NotifTitleModel> getNofitTitleList()
        {
            return _NotifTitleRepository.Table.ToList();
        }
        public void updateNotifTitle(List<int> notifitemIds)
        {
            var datas = _NotifTitleRepository.Table.ToList();
            datas.ForEach((p) => { p.IsActive = false; _NotifTitleRepository.Update(p); });
            var itemUpdate = _NotifTitleRepository.Table.Where(p => notifitemIds.Contains(p.Id)).ToList();
            itemUpdate.ForEach((p) => { p.IsActive = true; _NotifTitleRepository.Update(p); });
        }
        public bool HasOrderSendingSmsRequest(Order order)
        {
            return false;
        }
        public bool IsSmsSendedForThis(NotifTitle notifTitle, int orderId, int shipmentId)
        {
            return true;
        }
        public void SendSms(NotifTitle notifTitle, int orderId, int shipmentId)
        {
            Shipment shipment = null;
            Order order = null;
            if (shipmentId > 0)
            {
                shipment = _shipmentService.GetShipmentById(shipmentId);
                order = shipment?.Order;
            }
            else
            {
                order = _orderService.GetOrderById(orderId);
            }
            if (shipment == null && order == null)
            {
                return;
            }

        }
        public bool getNotifItem(Shipment shipment, NotifTitle notifTitle, out string NotifItem, int notifType = 1)
        {
            string CategoryQuery = @"SELECT
	                            TOP(1) PCM.CategoryId
                            FROM
	                            dbo.Shipment AS S
	                            INNER JOIN dbo.ShipmentItem AS SI ON SI.ShipmentId = S.Id
	                            INNER JOIN dbo.OrderItem AS OI ON OI.Id = S.Id
	                            INNER JOIN dbo.Product AS P ON P.Id = OI.ProductId
	                            INNER JOIN dbo.Product_Category_Mapping AS PCM ON PCM.ProductId = P.Id
                            WHERE
	                            S.Id =" + shipment.Id;
            int categoryId = _dbContext.SqlQuery<int?>(CategoryQuery, new object[0]).SingleOrDefault().GetValueOrDefault(0);
            if (categoryId == 0)
            {
                InsertNotifNote(notifTitle, shipment.Id, "متن پیام برای محموله شماره " + shipment.Id + "  به علت شماره دسته بندی محمصول نامعتبر جهت پیام " + nameof(notifTitle) + " یافت نشد");
                NotifItem = "";
                return false;
            }
            NotifItem = _NotifItemsRepository.Table.Where(P => P.IsActive == true
                 && P.NotifCategoryId == categoryId
                 && P.NotifTitleId == (int)notifTitle && P.NotifTypeId == notifType).FirstOrDefault()?.NotifTamplate;
            if (string.IsNullOrEmpty(NotifItem))
            {
                InsertNotifNote(notifTitle, shipment.Id, "متن پیام برای محموله شماره " + shipment.Id + " جهت پیام " + nameof(notifTitle) + " یافت نشد");
                NotifItem = "";
                return false;
            }

            string.Format(NotifItem, shipment.OrderId, shipment.Id, shipment.TrackingNumber ?? "", shipment.Order.BillingAddress.PhoneNumber);
            return true;
        }
        private void InsertNotifNote(NotifTitle notifTitle, int shipmentId, string Note)
        {
            int OrderId = 0;
            InsertOrderNote(Note, OrderId);
        }
        private void InsertOrderNote(string Note, int OrderId)
        {

        }

        public void sendSms(string msg, string receiver)
        {
            var SmsService = Call.GetInstance(this.ApiKey, this.LineNumber);

            ParamsSendSMS paramss = new ParamsSendSMS();
            paramss.ListPhoneNumber = new List<string>();
            paramss.ListPhoneNumber.Add(receiver);
            paramss.Listlocalid = new List<string>();
            string id = Guid.NewGuid().ToString("N");
            paramss.Listlocalid.Add(id);
            paramss.Message = msg;
            paramss.Date = DateTime.Now;
            paramss.type = kavenegarDLL.Models.Enums.MessageType.AppMemory;

            var resultsendsms = SmsService.Send(paramss);
            if (resultsendsms.Status == true)
            {
                //insert data to table
                ResultSend resulttemp = resultsendsms.ResultSend;
                long ds = resulttemp.Messageid;
                // سایر خروجی ها مدل برگشتی چک شود
            }
            else
            {
                Log($"{resultsendsms.Status.ToString()} {resultsendsms.Message}", "");
            }
        }
        public bool _sendSms(string receiver, string msg)
        {
            var SmsService = Call.GetInstance(this.ApiKey, this.LineNumber);

            ParamsSendSMS paramss = new ParamsSendSMS();
            paramss.ListPhoneNumber = new List<string>();
            paramss.ListPhoneNumber.Add(receiver);
            paramss.Listlocalid = new List<string>();
            string id = Guid.NewGuid().ToString("N");
            paramss.Listlocalid.Add(id);
            paramss.Message = msg;
            paramss.Date = DateTime.Now;
            paramss.type = kavenegarDLL.Models.Enums.MessageType.AppMemory;

            var resultsendsms = SmsService.Send(paramss);
            if (resultsendsms.Status == true)
            {
                //insert data to table
                ResultSend resulttemp = resultsendsms.ResultSend;
                long ds = resulttemp.Messageid;
                return true;
                // سایر خروجی ها مدل برگشتی چک شود
            }
            else
            {
                Log($"{resultsendsms.Status.ToString()} {resultsendsms.Message}", "");
                return false;
            }
        }
        public void Log(string header, string Message)
        {
            var logger =
                (DefaultLogger)EngineContext.Current
                    .Resolve<ILogger>();
            logger.InsertLog(LogLevel.Information, header, Message, null);
        }
        public void RemoveNotifyPostMan(int shipmentId)
        {

        }
        public void InsertNotifyPostMan(int shipmentId, string message, string mobile)
        {
            SqlParameter[] prms = new SqlParameter[]{
                new SqlParameter(){ParameterName="shipmentId",SqlDbType = SqlDbType.Int,Value= (object)shipmentId },
                new SqlParameter(){ParameterName="SenderCustomer",SqlDbType = SqlDbType.Int,Value= (object)_workContext.CurrentCustomer.Id },
                new SqlParameter(){ParameterName="RevicerMobileNo",SqlDbType = SqlDbType.VarChar,Value= (object)mobile },
                new SqlParameter(){ParameterName="message",SqlDbType = SqlDbType.NVarChar,Value= (object)message },
            };
            string query = "EXEC dbo.Sp_InsertNotifyPostmanShipment @shipmentId, @SenderCustomer,@RevicerMobileNo, @message";
            _dbContext.SqlQuery<bool>(query, prms).FirstOrDefault();
        }
        public bool IsSendedSmsToPostman(int shipmentId)
        {
            string query = $@"SELECT
	                            TOP(1)CAST(1 AS BIT)
                            FROM
	                            dbo.Tb_NotifyPostmanShipment AS TNPS
                            WHERE
	                            TNPS.ShipmentId = {shipmentId}";
            return _dbContext.SqlQuery<bool?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(false);
        }

        public void SendSmsForPlaceOrder(Order order)
        {
            SendSmsByTemplate(order, null, 2, false);
            foreach (var item in order.Shipments)
            {
                SendSmsByTemplate(null, item, 3, true);
            }
        }
        public bool HasRequestSendSmsNotif(Order order)
        {
            foreach (var item in order.OrderItems)
            {
                if (_OrderItemAttributeValueRepository.Table.Any(p => p.OrderItemId == item.Id && p.PropertyAttrName.Contains("اطلاع رسانی پیامکی")
             && p.PropertyAttrValueName == "بلی"))
                    return true;
            }
            return false;
        }
        public bool HasRequestSendSmsNotif(Shipment shipment)
        {
            foreach (var item in shipment.Order.OrderItems)
            {
                if (_OrderItemAttributeValueRepository.Table.Any(p => p.OrderItemId == item.Id && p.PropertyAttrName.Contains("اطلاع رسانی پیامکی")
             && p.PropertyAttrValueName == "بلی"))
                    return true;
            }
            return false;
        }
        public void InsertSmsLog(int? orderId, int? shipmentId, int SmsTitleId, bool SendResult)
        {
            string query = "EXEC dbo.Sp_InserSmsLog @OrderId, @ShipmentId, @SmsTitleId, @SendResult, @ShipmentEventId, @MasterShipmentEventId";

            SqlParameter P_OrderId = new SqlParameter()
            {
                ParameterName = "OrderId",
                SqlDbType = SqlDbType.Int,
                Value = (orderId.HasValue ? (object)orderId.Value : DBNull.Value)
            };
            SqlParameter P_ShipmentId = new SqlParameter()
            {
                ParameterName = "ShipmentId",
                SqlDbType = SqlDbType.Int,
                Value = (shipmentId.HasValue ? (object)shipmentId.Value : DBNull.Value)
            };
            SqlParameter P_SmsTitleId = new SqlParameter()
            {
                ParameterName = "SmsTitleId",
                SqlDbType = SqlDbType.Int,
                Value = (object)SmsTitleId
            };
            SqlParameter P_SendResult = new SqlParameter()
            {
                ParameterName = "SendResult",
                SqlDbType = SqlDbType.Bit,
                Value = (object)SendResult
            };
            SqlParameter P_ShipmentEventId = new SqlParameter()
            {
                ParameterName = "ShipmentEventId",
                SqlDbType = SqlDbType.Int,
                Value = DBNull.Value
            };
            SqlParameter P_MasterShipmentEventId = new SqlParameter()
            {
                ParameterName = "MasterShipmentEventId",
                SqlDbType = SqlDbType.VarChar,
                Value = DBNull.Value
            };

            SqlParameter[] prms = new SqlParameter[]{
                P_OrderId,
                P_ShipmentId,
                P_SmsTitleId,
                P_SendResult,
                P_ShipmentEventId,
                P_MasterShipmentEventId
            };
            int Id = _dbContext.SqlQuery<int?>(query, prms).FirstOrDefault().GetValueOrDefault(0);
        }
        public bool HasSmsLog(int? orderId, int? shipmentId, int SmsTitleId)
        {
            string query = "EXEC dbo.Sp_HasSmsLog @OrderId, @ShipmentId, @SmsTitleId";

            SqlParameter P_OrderId = new SqlParameter()
            {
                ParameterName = "OrderId",
                SqlDbType = SqlDbType.Int,
                Value = (orderId.HasValue ? (object)orderId.Value : DBNull.Value)
            };
            SqlParameter P_ShipmentId = new SqlParameter()
            {
                ParameterName = "ShipmentId",
                SqlDbType = SqlDbType.Int,
                Value = (shipmentId.HasValue ? (object)shipmentId.Value : DBNull.Value)
            };
            SqlParameter P_SmsTitleId = new SqlParameter()
            {
                ParameterName = "SmsTitleId",
                SqlDbType = SqlDbType.Int,
                Value = (object)SmsTitleId
            };
            SqlParameter[] prms = new SqlParameter[]{
                P_OrderId,
                P_ShipmentId,
                P_SmsTitleId
            };
            return _dbContext.SqlQuery<bool?>(query, prms).FirstOrDefault().GetValueOrDefault(false);
        }
        public void SendSmsByTemplate(Order _order, Shipment shipment, int notifTitle, bool isReciver)
        {
            var tempOrder = _order ?? shipment.Order;
            if (tempOrder.OrderStatus == OrderStatus.Cancelled)
                return;
            if (!HasRequestSendSmsNotif(tempOrder))
                return;
            //if (string.IsNullOrEmpty(tempOrder.PaymentMethodSystemName) || tempOrder.PaymentMethodSystemName != "Payments.CashOnDelivery")
            //{
            //    if ((_order ?? shipment.Order).PaymentStatus != Core.Domain.Payments.PaymentStatus.Paid)
            //        return;
            //}
            if (!HasSmsLog((_order != null ? _order.Id : (int?)null), (shipment != null ? shipment.Id : (int?)null), notifTitle))
            {
                var Msg = getNotifTamplate(notifTitle, _order, shipment);

                if (Msg != null)
                {
                    if (!string.IsNullOrEmpty(Msg.NotifTamplate))
                    {
                        var sended = _sendSms((isReciver ? Msg.ReciverPhoneNumber : Msg.SenderPhoneNumber), Msg.NotifTamplate);
                        InsertSmsLog((_order != null ? _order.Id : (int?)null), (shipment != null ? shipment.Id : (int?)null), notifTitle, sended);
                    }
                }
            }
        }

        public PagedList<Tbl_PopupNotification> GetPopupNotifications(int page, int pageSize)
        {
            return new PagedList<Tbl_PopupNotification>(_repository_PopupNotification.Table.OrderByDescending(p => p.Id), page, pageSize);
        }


        public void SavePopupNotification(PopupNotificationModel model)
        {
            if(model.Id == 0)
            {
                _repository_PopupNotification.Insert(new Tbl_PopupNotification()
                {
                    Content = model.Content,
                    FromDate = model.FromDate,
                    IsActive = model.IsActive,
                    Title = model.Title,
                    ToDate = model.ToDate,
                    StoreIds = string.Join(",", model.StoreIds)
                });
            }
            else
            {
                var entity = _repository_PopupNotification.GetById(model.Id);
                if(entity != null)
                {
                    entity.IsActive = model.IsActive;
                    entity.Title = model.Title;
                    entity.ToDate = model.ToDate;
                    entity.Content = model.Content;
                    entity.FromDate = model.FromDate;
                    entity.StoreIds = string.Join(",", model.StoreIds);
                    _repository_PopupNotification.Update(entity);
                }
            }
        }

        public PopupNotificationModel GetPopupNotificationById(int id)
        {
            var entity = _repository_PopupNotification.GetById(id);
            return new PopupNotificationModel()
            {
                Content = entity.Content,
                FromDate = entity.FromDate,
                Id = entity.Id,
                IsActive = entity.IsActive,
                Title = entity.Title,
                ToDate = entity.ToDate,
                StoreIds = entity.StoreIds?.Split(',')
            };
        }


        public PopupNotificationModel GetLatestNotification(int[] oldNotifs)
        {
            var now = DateTime.Now;
            var currentStore = _storeContext.CurrentStore.Id.ToString();
            var query = _repository_PopupNotification.Table.OrderByDescending(p => p.Id).Where(p => p.IsActive).Where(p=> p.FromDate.HasValue ? p.FromDate <= now : true && p.ToDate.HasValue ? p.ToDate >= now : true &&
            (string.IsNullOrEmpty(p.StoreIds) || p.StoreIds.Contains(p.StoreIds)));
            if(oldNotifs != null)
            {
                query = query.Where(p => !oldNotifs.Contains(p.Id));
            }
            var entity = query.FirstOrDefault();
            if(entity == null)
            {
                return null;
            }
            return new PopupNotificationModel()
            {
                Content = entity.Content,
                Id = entity.Id,
                Title = entity.Title
            };
        }

    }
    public class NotCollectedForSms
    {
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
    }
    public class MultiShippingTrackingData
    {
        public string PhoneNumber { get; set; }
        public string TrackingNumbers { get; set; }
    }
    public class _SelectListItem
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
    public class NotifTemplate
    {
        public int NotifTitleId { get; set; }
        public string NotifTamplate { get; set; }
        public string SenderPhoneNumber { get; set; }
        public string ReciverPhoneNumber { get; set; }
    }

}

