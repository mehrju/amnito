using AutoMapper;
using BarcodeLib;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Enums;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.SendToPostDataService;
using Nop.plugin.Orders.ExtendedShipment.webBarCode;
using Nop.Plugin.Misc.ShippingSolutions.Domain;
using Nop.Plugin.Misc.ShippingSolutions.Models.kalaResan;
using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Mahex;
using Nop.Plugin.Misc.ShippingSolutions.Services;
using Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Stimulsoft.Report;
using Stimulsoft.Report.Export;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{


    #region enum

    public enum OrderRegistrationMethod
    {
        none = 0,
        Normal = 1,
        Excel = 2,
        Api = 3,
        NewUi = 4,
        MultiShipment = 5,
        Ap = 6,
        bidok = 7,
        hyperJet = 8,
        androidApp = 9,
        Sep = 10,
        Bulk = 11,
        PhoneOrder = 12
    }
    public enum OrderStatusToPost
    {
        BarcodeIsGenerating,
        IsSendingToPost,
        BarcodeGenerateError,
        BarcodeGenerateDown
    }
    #endregion
    public class ExtendedShipmentService : IExtendedShipmentService
    {
        #region Fields

        private readonly IkalaResan_Service _kalaResan_Service;
        private readonly ICartonService _cartonService;
        private readonly IApiOrderRefrenceCodeService _apiOrderRefrenceCodeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICollectorService _collectorService;
        private readonly ILocationService _locationService;
        private readonly IYarBox_Service _yarBox_Service;
        private readonly ISafiran_Service _safiran_Service;
        private readonly IPDE_Service _pDE_Service;
        private readonly IUbaar_Service _ubaar_Service;
        private readonly ITPG_Service _tPG_Service;
        private readonly IChapar_Service _chapar_Service;
        private readonly IMahex_Service _mahex_Service;
        private readonly ITaroff_Service _taroff_Service;
        private readonly ISnappbox_Service _snappbox_Service;
        private readonly IPostBarCodeService _postBarCodeService;
        private readonly ICodService _codService;
        private readonly ExtendedShipmentSetting _setting;
        private readonly IRepository<Tb_ShippingServiceLog> _repository_Tb_ShippingServiceLog;
        private readonly ISkyBlue_Service _SkyBlue_Service;
        private readonly ICheckRegionDesVijePost _CheckRegionDesVijePost;
        private readonly IAgentAmountRuleService _agentAmountRuleService;


        private readonly IRewardPointService _rewardPointService;
        private readonly IRepository<RewardPointsHistory> _rewardPointsHistoryRepository;

        private readonly INotificationService _notificationService;
        private readonly IAddressService _addressService;
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;
        private readonly IShippingService _shippingService;
        private readonly ILocalizationService _localizationService;
        private readonly IDbContext _dbContext;
        private readonly IRepository<OrderItemAttributeValueModel> _OrderItemAttributeValueRepository;
        private readonly IRepository<Shipment> _shipmentRepository;
        private readonly IRepository<ShipmentItem> _siRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly IRepository<PostCoordinationModel> _PostCoordinationRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICustomerService _customerService;
        private readonly IRepository<ShipmentAppointmentModel> _ShipmentAppointmentRepository;
        private readonly IShipmentService _shipmentService;
        //private readonly ISettingService _settingService;
        private readonly IRepository<StateCodemodel> _repositoryStateCode;
        private readonly IRepository<CountryCodeModel> _repositoryCountryCode;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IRepository<BarcodeRepositoryModel> _repositoryBarcodeRepository;
        private readonly IRepository<CateguryPostTypeModel> _repositoryCateguryPostType;
        private readonly IStoreContext _storeContext;
        private readonly IShipmentTrackingService _shipmentTrackingService;
        private readonly IRepository<UserStetesModel> _repositoryUserStetes;

        #endregion

        #region ctor
        public ExtendedShipmentService(
            ICartonService cartonService,
            IApiOrderRefrenceCodeService apiOrderRefrenceCodeService,
            IDateTimeHelper dateTimeHelper,
            ICollectorService collectorService,
            ICheckRegionDesVijePost CheckRegionDesVijePost,
              ISkyBlue_Service SkyBlue_Service,
            IYarBox_Service yarBox_Service
          , ITPG_Service tPG_Service
          , IPDE_Service pDE_Service
          , IUbaar_Service ubaar_Service
          , IChapar_Service chapar_Service
          , ITaroff_Service taroff_Service
          , IRepository<OrderItemAttributeValueModel> OrderItemAttributeValueRepository
          , IRepository<Shipment> shipmentRepository
          , IAddressService addressService
          , IRepository<ShipmentItem> siRepository
          , IRepository<OrderItem> orderItemRepository
          , IEventPublisher eventPublisher
          , ICustomerService customerService
          , IRepository<ShipmentAppointmentModel> ShipmentAppointmentRepository
          , IShipmentService shipmentService
          , IRepository<StateCodemodel> repositoryStateCode
          , IRepository<CountryCodeModel> repositoryCountryCode
          //, ISettingService settingService
          , IStoreService storeService
          , IWorkContext workContext
          , IOrderService orderService
          , IGenericAttributeService genericAttributeService
          , ICustomerAttributeParser customerAttributeParser
          , IRepository<Order> orderRepository
          , IProductAttributeParser productAttributeParser
          , IRepository<BarcodeRepositoryModel> repositoryBarcodeRepository
          , IRepository<CateguryPostTypeModel> repositoryCateguryPostType
          , ITaxService taxService
          , IMeasureService measureService
          , MeasureSettings measureSettings
          , IShippingService shippingService
          , ILocalizationService localizationService
          , IRepository<OrderNote> orderNoteRepository
          , IStoreContext storeContext
          , IDbContext dbContext
          , IRepository<PostCoordinationModel> PostCoordinationRepository
          , INotificationService notificationService
          , IShipmentTrackingService shipmentTrackingService
          , IRepository<UserStetesModel> repositoryUserStetes
          , Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces.ISafiran_Service safiran_Service
          , IRewardPointService rewardPointService
          , IRepository<RewardPointsHistory> rewardPointsHistoryRepository
          , ILocationService locationService
          , ISnappbox_Service snappbox_Service,
            IPostBarCodeService postBarCodeService,
            ExtendedShipmentSetting setting,
            IRepository<Tb_ShippingServiceLog> repository_Tb_ShippingServiceLog,
            IMahex_Service mahex_Service,
            IAgentAmountRuleService agentAmountRuleService,
            ICodService codService,
            IkalaResan_Service kalaResan_Service

      )
        {
            _kalaResan_Service = kalaResan_Service;
            _codService = codService;
            _cartonService = cartonService;
            _agentAmountRuleService = agentAmountRuleService;
            _apiOrderRefrenceCodeService = apiOrderRefrenceCodeService;
            _collectorService = collectorService;
            _CheckRegionDesVijePost = CheckRegionDesVijePost;
            _SkyBlue_Service = SkyBlue_Service;
            _snappbox_Service = snappbox_Service;
            _postBarCodeService = postBarCodeService;
            _setting = setting;
            _repository_Tb_ShippingServiceLog = repository_Tb_ShippingServiceLog;
            _taroff_Service = taroff_Service;
            _chapar_Service = chapar_Service;
            _tPG_Service = tPG_Service;
            _ubaar_Service = ubaar_Service;
            _pDE_Service = pDE_Service;
            _rewardPointService = rewardPointService;
            _yarBox_Service = yarBox_Service;
            _safiran_Service = safiran_Service;
            _rewardPointsHistoryRepository = rewardPointsHistoryRepository;
            _OrderItemAttributeValueRepository = OrderItemAttributeValueRepository;
            _repositoryUserStetes = repositoryUserStetes;
            _shipmentTrackingService = shipmentTrackingService;
            _notificationService = notificationService;
            _PostCoordinationRepository = PostCoordinationRepository;
            _storeContext = storeContext;
            _dbContext = dbContext;
            _addressService = addressService;
            _orderNoteRepository = orderNoteRepository;
            _measureService = measureService;
            _measureSettings = measureSettings;
            _shippingService = shippingService;
            _localizationService = localizationService;
            _ShipmentAppointmentRepository = ShipmentAppointmentRepository;
            _shipmentRepository = shipmentRepository;
            _siRepository = siRepository;
            _orderItemRepository = orderItemRepository;
            _eventPublisher = eventPublisher;
            _customerService = customerService;
            _shipmentService = shipmentService;
            //_settingService = settingService;
            _repositoryStateCode = repositoryStateCode;
            _repositoryCountryCode = repositoryCountryCode;
            _storeService = storeService;
            _workContext = workContext;
            _orderService = orderService;
            _genericAttributeService = genericAttributeService;
            _customerAttributeParser = customerAttributeParser;
            _orderRepository = orderRepository;
            _productAttributeParser = productAttributeParser;
            _repositoryBarcodeRepository = repositoryBarcodeRepository;
            _repositoryCateguryPostType = repositoryCateguryPostType;
            _locationService = locationService;
            _dateTimeHelper = dateTimeHelper;
            _mahex_Service = mahex_Service;
        }
        #endregion

        public List<SelectListItem> getCustomerByRoleId(int roleId)
        {
            string query = $@"SELECT
	                            CAST(C.Id AS NVARCHAR(20)) Value,
	                            ISNULL(TCI.FullName+'-N'+C.Username,C.Username) Text
                            FROM
	                            dbo.Customer AS C
	                            INNER JOIN dbo.Customer_CustomerRole_Mapping AS CCRM ON CCRM.Customer_Id = C.Id
	                            LEFT JOIN dbo.Tb_CustomerInfo AS TCI ON TCI.CustomerId = C.Id
                            WHERE
	                            C.Active = 1
	                            AND CCRM.CustomerRole_Id ={roleId}";
            return _dbContext.SqlQuery<SelectListItem>(query, new object[0]).ToList();
        }
        public bool isInvalidSender(int SenderCountry, int senderState)
        {
            string Query = $@"SELECT
	                            TOP(1)cast(1 as bit)
                            FROM
	                            dbo.Tb_InvalidSenders AS TIS
                            WHERE
	                            tis.CountryId = {SenderCountry}
	                            AND TIS.StateId = {senderState}
                                AND ServiceId IS NULL";

            return _dbContext.SqlQuery<bool?>(Query, new object[0]).FirstOrDefault().GetValueOrDefault(false);
        }
        public bool isInvalidSender(int SenderCountry, int senderState, int serviceId)
        {
            string Query = $@"SELECT
	                            TOP(1)cast(1 as bit)
                            FROM
	                            dbo.Tb_InvalidSenders AS TIS
                            WHERE
	                            tis.CountryId = {SenderCountry}
	                            AND TIS.StateId = {senderState}
                                AND ServiceId ={ serviceId}";

            return _dbContext.SqlQuery<bool?>(Query, new object[0]).FirstOrDefault().GetValueOrDefault(false);
        }
        public string MakeMahexVoid(string waybill_number)
        {
            var res = _mahex_Service.VoidShipment(waybill_number);
            if (res.status.code == 200) return "OK";
            return res.status.message;
        }
        public int getHagheMaghar(int orderItemId, int SenderCountryId, int ServiceId, out int postHagheMaghar)
        {
            string Query = @"SELECT
	                            TOP(1)THM.HagheMagharPrice+ISNULL(ShipmentHagheMaghr,0) HagheMagharPrice
                            FROM
	                            dbo.Tb_HagheMaghar AS THM
                            WHERE
	                            THM.OrderItemId = " + orderItemId;
            int hagheMaghar = _dbContext.SqlQuery<int?>(Query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
            postHagheMaghar = 0;

            if (hagheMaghar > 0 && GetCategoryInfo(ServiceId).HasHagheMaghar)
            {
                string _query = $@"SELECT
				                    TCHM.HagheMagharPrice
			                    FROM
				                    dbo.Tb_CountryHagheMaghar AS TCHM
			                    WHERE
				                    TCHM.CountryId = {SenderCountryId}";
                postHagheMaghar = _dbContext.SqlQuery<int?>(_query, new object[0]).FirstOrDefault().GetValueOrDefault();
            }
            return hagheMaghar;
        }
        public bool IsPostService(int ServiceId)
        {
            string query = $@"EXEC dbo.Sp_IsPostService @serviceId";

            SqlParameter P_serviceId = new SqlParameter()
            {
                ParameterName = "serviceId",
                SqlDbType = SqlDbType.Int,
                Value = (object)ServiceId
            };

            SqlParameter[] prms = new SqlParameter[]{
                P_serviceId
            };
            return _dbContext.SqlQuery<bool?>(query, prms).FirstOrDefault().GetValueOrDefault(false);
        }
        public bool IsInValidDiscountPeriod(Order order)
        {
            DateTime StartDate = new DateTime(2020, 04, 01);

            var firstOrder = _orderRepository.Table.Where(p => p.CustomerId == order.CustomerId && p.OrderStatusId != 40).OrderBy(n => n.Id).FirstOrDefault();
            if (firstOrder == null)
                return true;
            if (StartDate.ToUniversalTime().CompareTo(firstOrder.CreatedOnUtc) > 0)
                return false;
            int day = ((int)(order.CreatedOnUtc - firstOrder.CreatedOnUtc).TotalDays);
            return day <= 31;
        }
        public bool IsInValidDiscountPeriod()
        {
            DateTime StartDate = new DateTime(2020, 04, 01);
            var utcNow = DateTime.UtcNow;
            var firstOrder = _orderRepository.Table.Where(p => p.CustomerId == _workContext.CurrentCustomer.Id && p.OrderStatusId != 40).OrderBy(n => n.Id).FirstOrDefault();
            if (firstOrder == null)
                return true;
            if (StartDate.ToUniversalTime().CompareTo(firstOrder.CreatedOnUtc) > 0)
                return false;

            int day = ((int)(utcNow - firstOrder.CreatedOnUtc).TotalDays);
            return day <= 31;
        }
        public bool CanUseFirstOrderDiscount(Order order)
        {
            var mark = GetOrderRegistrationMethod(order);
            if (order.Customer.IsInCustomerRole("mini-Administrators") || mark == OrderRegistrationMethod.Ap || mark == OrderRegistrationMethod.bidok)
                return false;
            return true;
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
        public int getTotalWeight(Order order)
        {
            int TotalWeight = 0;
            foreach (var item in order.OrderItems)
            {
                TotalWeight += (GetItemWeightFromAttr(item) * item.Quantity);
            }
            return TotalWeight;
        }
        public bool CheckHasValidPrice(Order order)
        {
            var mark = GetOrderRegistrationMethod(order);
            int DealerId = (mark == OrderRegistrationMethod.Ap) ? 1 : (mark == OrderRegistrationMethod.bidok ? 4 : 0);
            string ToatalPrice = _genericAttributeService.GetAttributesForEntity(order.Id, "Order").FirstOrDefault(p => p.Key == "PriceUpdateFromApi" && p.StoreId == order.StoreId)?.Value;
            if (string.IsNullOrEmpty(ToatalPrice))
            {

                UpdateOrderTotalByApiPrice(order, DealerId);
                ToatalPrice = _genericAttributeService.GetAttributesForEntity(order.Id, "Order").FirstOrDefault(p => p.Key == "PriceUpdateFromApi" && p.StoreId == order.StoreId)?.Value;
                if (string.IsNullOrEmpty(ToatalPrice) && mark != OrderRegistrationMethod.PhoneOrder)
                {
                    InsertOrderNote($" به دلیل اینکه اطلاعات قیمت نهایی این سفارش به درستی محاسبه نشده، ثبت درخواست در سمت پذیرنده انجام نشد", order.Id);
                    return false;
                }
            }
            if (ToatalPrice.ToEnDigit() <= 0)
            {
                UpdateOrderTotalByApiPrice(order, DealerId);
                ToatalPrice = _genericAttributeService.GetAttributesForEntity(order.Id, "Order").FirstOrDefault(p => p.Key == "PriceUpdateFromApi" && p.StoreId == order.StoreId)?.Value;
                if (string.IsNullOrEmpty(ToatalPrice))
                {
                    InsertOrderNote($" به دلیل اینکه اطلاعات قیمت نهایی این سفارش به درستی محاسبه نشده، ثبت درخواست در سمت پذیرنده انجام نشد", order.Id);
                    return false;
                }
            }
            return true;
        }

        #region Post
        public List<_ServiceInfo> GetBasPriceAndSlA(
            int senderCountry
            , int senderState
            , int receiverCountry
            , int receiverState
            , int weightItem
            , int storeId
            , int customerId
            , int receiver_ForeginCountry
            , bool? IsCod
            , int serviceId
            , bool ShowPrivatePost = true
            , bool ShowDistributer = true
            )
        {
            try
            {
                #region SpParams
                SqlParameter P_SenderCountrId = new SqlParameter()
                {
                    ParameterName = "SenderCountrId",
                    SqlDbType = SqlDbType.Int,
                    Value = (object)senderCountry
                };
                SqlParameter P_SenderStateId = new SqlParameter()
                {
                    ParameterName = "SenderStateId",
                    SqlDbType = SqlDbType.Int,
                    Value = (object)senderState
                };
                SqlParameter P_ReciverCountryId = new SqlParameter()
                {
                    ParameterName = "ReciverCountryId",
                    SqlDbType = SqlDbType.Int,
                    Value = (object)receiverCountry
                };
                SqlParameter P_ReciverStateId = new SqlParameter()
                {
                    ParameterName = "ReciverStateId",
                    SqlDbType = SqlDbType.Int,
                    Value = (object)receiverState
                };
                SqlParameter P_Weight_g = new SqlParameter()
                {
                    ParameterName = "Weight_g",
                    SqlDbType = SqlDbType.Int,
                    Value = (object)weightItem
                };
                SqlParameter P_StoreId = new SqlParameter()
                {
                    ParameterName = "StoreId",
                    SqlDbType = SqlDbType.Int,
                    Value = (object)storeId
                };
                SqlParameter P_CustomerId = new SqlParameter()
                {
                    ParameterName = "CustomerId",
                    SqlDbType = SqlDbType.Int,
                    Value = (object)customerId
                };
                SqlParameter P_receiver_ForeginCountry = new SqlParameter()
                {
                    ParameterName = "receiver_ForeginCountry",
                    SqlDbType = SqlDbType.Int,
                    Value = (object)receiver_ForeginCountry
                };
                bool IgnoreHagheMaghar = true;// FromAPi;
                SqlParameter P_IgnoreHagheMaghar = new SqlParameter()
                {
                    ParameterName = "IgnoreHagheMaghar",
                    SqlDbType = SqlDbType.Bit,
                    Value = (object)IgnoreHagheMaghar
                };
                SqlParameter P_IsCod = new SqlParameter()
                {
                    ParameterName = "IsCod",
                    SqlDbType = SqlDbType.Bit,
                    Value = IsCod.HasValue ? (object)IsCod.Value : (object)DBNull.Value
                };
                SqlParameter P_ServiceId = new SqlParameter()
                {
                    ParameterName = "ServiceId",
                    SqlDbType = SqlDbType.Int,
                    Value = serviceId
                };
                SqlParameter P_ShowPrivatePost = new SqlParameter()
                {
                    ParameterName = "ShowPrivatePost",
                    SqlDbType = SqlDbType.Int,
                    Value = ShowPrivatePost
                };
                SqlParameter P_ShowDistributer = new SqlParameter()
                {
                    ParameterName = "ShowDistributer",
                    SqlDbType = SqlDbType.Bit,
                    Value = ShowDistributer
                };
                SqlParameter[] prms = new SqlParameter[]
                {
                    P_SenderCountrId,
                    P_SenderStateId,
                    P_ReciverCountryId,
                    P_ReciverStateId,
                    P_Weight_g,
                    P_StoreId,
                    P_CustomerId,
                    P_receiver_ForeginCountry,
                    P_IgnoreHagheMaghar,
                    P_IsCod,
                    P_ServiceId,
                    P_ShowPrivatePost,
                    P_ShowDistributer
                };

                #endregion

                string Query = $@"EXEC dbo.Sp_CalcPriceAndSLA @SenderCountrId, @SenderStateId, @ReciverCountryId, @ReciverStateId, @Weight_g, @StoreId, @CustomerId
                            ,@receiver_ForeginCountry,@IgnoreHagheMaghar,@IsCod,@ServiceId,@ShowPrivatePost,@ShowDistributer";

                var data = _dbContext.SqlQuery<_ServiceInfo>(Query, prms).ToList();
                data.ForEach(p => p.CanSelect = true);
                return data;
            }
            catch (Exception ex)
            {
                LogException(ex);
                Log("خطا در زمان لود کرد سرویس ها", "");
                return null;
            }
        }

        #endregion

        #region BlueSky
        public async Task<GetPriceResult> getBlueSkyPrice(getPriceInputModel model)
        {
            GetPriceResult getPriceResult = new GetPriceResult();
            getPriceResult.ServiceId = model.ServiceId;
            //int ReciverCountry = int.Parse(model.ReciverAddress.Address2.Split('|')[0]);
            //string ReciverCountryCode = getBlueSkyCountryCode(ReciverCountry);
            int _parcelType = 1;
            if (model.ConsType == 0)
            {
                if (model.Weight > 2000)
                    _parcelType = 2;
            }
            else
            {
                _parcelType = 2;
            }
            var Result_SkyBlue_ParcelPrice = await _SkyBlue_Service.GetParcelPrice(new Plugin.Misc.ShippingSolutions.Models.Params.SkyBlue.Params_SkyBlue_ParcelPrice()
            {
                Weight = model.Weight,
                Length = model.Length,
                Width = model.Width,
                Height = model.Height,
                Countrycode = model.ReciverCountryCode,
                ParcelType = _parcelType//(model.Weight < 2000 ? 1 : 2)//(model.ConsType == 0 ? 1 : 2)
            });
            if (!Result_SkyBlue_ParcelPrice.Status)
            {
                common.Log("بروز مشکل در زمان دریافت قیمت ابی آسمان", Result_SkyBlue_ParcelPrice.Message);
                getPriceResult.errorMessage = Result_SkyBlue_ParcelPrice.Message;
                getPriceResult.canSelect = false;
                getPriceResult.price = 0;
                return getPriceResult;
            }
            getPriceResult.errorMessage = "";
            getPriceResult.price = Convert.ToInt32(Result_SkyBlue_ParcelPrice.DetailParcelPrice.Price);
            getPriceResult.canSelect = getPriceResult.price > 0;
            return getPriceResult;
        }
        public async Task<string> RegisterBlueSky_Order(Order order)
        {
            if (order.OrderNotes.Any(p => p.Note.Contains("SendDataToPost")))
            {
                return "ممکن است اطلاعات مرسوله قبلا ارسال شده باشد";
            }
            if (!CheckHasValidPrice(order))
                return "قیمت سرویس مورد نظر شما به درستی محاسبه نشده با واحد پشتیبانی تماس بگیرید";
            InsertOrderNote("SendDataToPost", order.Id);
            bool IsMultishipment = IsMultiShippment(order);
            bool isOk = false;
            foreach (var item in order.OrderItems)
            {
                int serviceId = 0;
                var cat = item.Product.ProductCategories.Where(p => p.Category.ParentCategoryId != 0).FirstOrDefault();

                //int kalaPrice = getGoodsPrice(item);
                if (cat == null)
                {
                    InsertOrderNote($"سرویس مربوط به آیتم {item.Id} از این سفارش یافت نشد", order.Id);
                    continue;
                }
                if (!new int[] { 719 }.Contains(cat.CategoryId))
                {
                    InsertOrderNote($"آیتم شماره {0} مربوط به سرویس راه آبی آسمان نمی باشد", order.Id);
                    continue;
                }
                serviceId = cat.CategoryId;
                bool IsCod = false;//new int[] { 713, 715 }.Contains(serviceId);
                int ApproximateValue = getApproximateValue(item.Id);
                int weight = GetItemWeightFromAttr(item);
                var Dimensions = getDimensions(item);
                var ConsType = getPDEConsType(item);
                for (int i = 0; i < item.Quantity; i++)
                {
                    Shipment shipment = null;
                    int ShipmentAddressId = 0;

                    if (IsMultishipment)
                    {
                        var multishipment = getShipmentFromMultiShipment(item, i);
                        if (multishipment == null)
                        {
                            InsertOrderNote($"اطلاعات محموله آتیم شماره {item.Id}  ردیف {i} یافت نشد", order.Id);
                            continue;
                        }
                        if (!string.IsNullOrEmpty(multishipment.shipment.TrackingNumber))
                            continue;
                        shipment = multishipment.shipment;
                        ShipmentAddressId = multishipment.ShipmentAddressId;
                    }
                    else
                    {
                        shipment = GetShipment(item, i);
                        if (!string.IsNullOrEmpty(shipment.TrackingNumber))
                            continue;
                        ShipmentAddressId = order.ShippingAddressId.Value;
                    }
                    var address = _addressService.GetAddressById(ShipmentAddressId);
                    int ReciverCountry = int.Parse(address.Address2.Split('|')[0]);
                    string ReciverCountryCode = getBlueSkyCountryCode(ReciverCountry);
                    //string ReciverCountryCode = getBlueSkyCountryCode(ReciverCountry);

                    int _parcelType = 1;
                    if (ConsType == 0)
                    {
                        if (weight > 2000)
                            _parcelType = 2;
                    }
                    else
                        _parcelType = 2;

                    var _result = await _SkyBlue_Service.RegisterOrder(new Plugin.Misc.ShippingSolutions.Models.Params.SkyBlue.Params_SkyBlue_RegisterOrder()
                    {
                        SenderName = order.BillingAddress.FirstName ?? "",
                        SenderAddress = order.BillingAddress.Address1,
                        SenderPostCode = order.BillingAddress.ZipPostalCode ?? "",
                        SenderPhone = order.BillingAddress.PhoneNumber ?? "",
                        SenderEmail = order.BillingAddress.Email ?? "",
                        ReceiverName = (address.FirstName ?? "") + (" " + address.LastName ?? ""),
                        ReceiverAddress = address.Address1,
                        ReceiverPostCode = address.ZipPostalCode,
                        ReceiverPhone = address.PhoneNumber,
                        ReceiverEmail = address.Email,
                        Content = getOrderItemContent(item),
                        ContentValue = ApproximateValue.ToString(),
                        Weight = weight,
                        Width = Dimensions.Item2,
                        Height = Dimensions.Item3,
                        Length = Dimensions.Item1,
                        Countrycode = ReciverCountryCode,// از آدرس2 به عنوان کشور مقصد استفاده میشود,
                        ParcelType = _parcelType,// (weight < 2000 ? 1 : 2),// (ConsType == 0 ? 1 : 2)
                        ServiceType = 1//صادرات = 1 و واردات = 2

                    });
                    if (_result == null)
                    {
                        ChangeOrderState(order, OrderStatus.Processing, $" دلیل انجام نشد این درخواست در آتیم {item.Id} ردیف {i} " + " : " + _result.Message);
                        return "در زمان ثبت سفارش در سرویس مورد نظر مشکلی پیش آمده با واحد پشتیبانی تماس بگیرید";
                    }
                    if (_result.DetailRegisterOrder == null)
                    {
                        ChangeOrderState(order, OrderStatus.Processing, $" دلیل انجام نشد این درخواست در آتیم {item.Id} ردیف {i} " + " : " + _result.Message);
                        return "در زمان ثبت سفارش در سرویس مورد نظر مشکلی پیش آمده با واحد پشتیبانی تماس بگیرید";
                    }
                    if (_result.Status)
                    {
                        shipment.TrackingNumber = _result.DetailRegisterOrder.OrderNumber;
                        _shipmentService.UpdateShipment(shipment);
                        isOk = true;
                    }
                    else
                    {
                        ChangeOrderState(order, OrderStatus.Processing, $" دلیل انجام نشد این درخواست در آتیم {item.Id} ردیف {i} " + " : " + _result.Message);
                        return "در زمان ثبت سفارش در سرویس مورد نظر مشکلی پیش آمده با واحد پشتیبانی تماس بگیرید";
                    }
                }
            }
            //if (isOk)// تکمیل سفارش پست داخلی متناظر پست خارجی
            //{
            //    int orderId = getRelatedOrder(order.Id);
            //    var child_order = _orderService.GetOrderById(orderId);
            //    child_order.OrderStatus = order.OrderStatus;
            //    child_order.PaymentStatus = order.PaymentStatus;
            //    child_order.PaymentMethodSystemName = order.PaymentMethodSystemName;
            //    _orderService.UpdateOrder(child_order);
            //}
            return "";
        }
        public int getRelatedOrder(int ParentOrderId)
        {
            string query = $@"SELECT
	                           ChildOrderId
                            FROM
	                            dbo.Tb_RelatedOrders AS TRO
                            WHERE
	                            TRO.ParentOrderId = " + ParentOrderId;
            return _dbContext.SqlQuery<int?>(query, new object[0]).SingleOrDefault().GetValueOrDefault(0);
        }
        public string getBlueSkyCountryCode(int CountryNameEn)
        {
            string query = $@"SELECT
	                            TCI.Alpha2Code
                            FROM
	                            dbo.Tbl_CountryISO3166 AS TCI
                            WHERE
	                            TCI.Id ={CountryNameEn}";
            //SqlParameter[] prms = new SqlParameter[]{
            //    new SqlParameter(){ParameterName="CountryName",SqlDbType = SqlDbType.NVarChar,Value= (object)CountryNameEn},
            //};
            Log("کوئری کشور خارجی", query);
            return (_dbContext.SqlQuery<string>(query, new object[0]).FirstOrDefault() ?? "");
        }
        #endregion

        #region Chapar
        public async Task<GetPriceResult> getChaparPrice(getPriceInputModel model, int SenderCode, int TotalPrice = 0)
        {
            GetPriceResult getPriceResult = new GetPriceResult();
            getPriceResult.ServiceId = model.ServiceId;
            try
            {
                bool IsCod = new int[] { 713, 715 }.Contains(model.ServiceId);
                //var _w = ((model.Length * model.Width * model.Height) / 6000) * 1000;
                //model.Weight = (model.Weight > _w ? model.Weight : _w);
                // string _method = IsCod ? "COD" : new int[] { 712, 713 }.Contains(model.ServiceId) ? "6" : "1";
                string _method = new int[] { 712, 713 }.Contains(model.ServiceId) ? "6" : "1";
                var chaparPricingResult = await _chapar_Service.GetQuote(new Plugin.Misc.ShippingSolutions.Models.Params.Chapar.Params_Chapar_GetQuote()
                {
                    order = new Plugin.Misc.ShippingSolutions.Models.Params.Chapar.Order()
                    {
                        payment_terms = (IsCod ? 1 : 0),
                        inv_value = (IsCod ? (TotalPrice > 0 ? TotalPrice : model.AproximateValue) : 0),
                        weight = Convert.ToDecimal((decimal)model.Weight / (decimal)1000),
                        value = model.AproximateValue,
                        receiver_code = "",
                        sender_code = SenderCode,
                        method = _method,
                        origin = model.SenderStateId.ToString(),
                        destination = model.ReaciverStateId.ToString(),
                        length = model.Length,
                        width = model.Width,
                        height = model.Height
                    }
                });
                if (!chaparPricingResult.Status)
                {
                    common.Log("بروز مشکل در زمان دریافت قیمت چاپار", chaparPricingResult.Message);

                    getPriceResult.errorMessage = chaparPricingResult.Message;
                    getPriceResult.canSelect = false;
                    getPriceResult.price = 0;
                    return getPriceResult;
                }
                getPriceResult.errorMessage = "";
                getPriceResult.price = Convert.ToInt32(chaparPricingResult.objects.order.quote);
                if (IsCod && chaparPricingResult.objects.order.price != null)
                {
                    getPriceResult.CodTranPrice = chaparPricingResult.objects.order.price.fld_Lab_Cost;
                }
                getPriceResult.canSelect = true;
                return getPriceResult;
            }
            catch (Exception ex)
            {
                LogException(ex);
                getPriceResult.errorMessage = "خطا در زمان استعلام قیمت چاپار";
                getPriceResult.canSelect = false;
                getPriceResult.price = 0;
                return getPriceResult;
            }
        }
        public async Task<string> RegisterChapar_Order(Order order)
        {
            if (order.OrderNotes.Any(p => p.Note.Contains("SendDataToPost")))
            {
                return "ممکن است اطلاعات مرسوله قبلا ارسال شده باشد";
            }
            if (!CheckHasValidPrice(order))
                return "قیمت سرویس مورد نظر شما به درستی محاسبه نشده با واحد پشتیبانی تماس بگیرید";
            InsertOrderNote("SendDataToPost", order.Id);
            bool IsMultishipment = IsMultiShippment(order);
            Nop.Plugin.Misc.ShippingSolutions.Models.Params.Chapar.SenderBulkImport sender = new Nop.Plugin.Misc.ShippingSolutions.Models.Params.Chapar.SenderBulkImport()
            {
                address = order.BillingAddress.Address1,
                city_no = order.BillingAddress.getDtsStateId().ToString(),
                company = order.BillingAddress.Company,
                email = order.BillingAddress.Email,
                mobile = order.BillingAddress.PhoneNumber,
                person = (order.BillingAddress.FirstName ?? "") + (" " + order.BillingAddress.LastName ?? ""),
                postcode = order.BillingAddress.ZipPostalCode,
                telephone = "",
            };
            foreach (var item in order.OrderItems)
            {
                int serviceId = 0;
                var cat = item.Product.ProductCategories.Where(p => p.Category.ParentCategoryId != 0).FirstOrDefault();
                decimal weight = (decimal)GetItemWeightFromAttr(item) / (decimal)1000;
                int kalaPrice = getGoodsPrice(item);
                int CodValue = 0;
                if (cat == null)
                {
                    InsertOrderNote($"سرویس مربوط به آیتم {item.Id} از این سفارش یافت نشد", order.Id);
                    continue;
                }
                if (!new int[] { 712, 713, 714, 715 }.Contains(cat.CategoryId))
                {
                    InsertOrderNote($"آیتم شماره {0} مربوط به سرویس چاپار نمی باشد", order.Id);
                    continue;
                }
                serviceId = cat.CategoryId;
                bool IsCod = new int[] { 713, 715 }.Contains(serviceId);
                var dimension = getDimensions(item);
                for (int i = 0; i < item.Quantity; i++)
                {
                    Shipment shipment = null;
                    int ShipmentAddressId = 0;
                    Nop.Plugin.Misc.ShippingSolutions.Models.Params.Chapar.ReceiverBulkImport reciver = null;
                    if (IsMultishipment)
                    {
                        var multishipment = getShipmentFromMultiShipment(item, i);
                        if (multishipment == null)
                        {
                            InsertOrderNote($"اطلاعات محموله آتیم شماره {item.Id}  ردیف {i} یافت نشد", order.Id);
                            continue;
                        }
                        if (!string.IsNullOrEmpty(multishipment.shipment.TrackingNumber))
                            continue;
                        shipment = multishipment.shipment;
                        ShipmentAddressId = multishipment.ShipmentAddressId;
                    }
                    else
                    {
                        shipment = GetShipment(item, i);
                        if (!string.IsNullOrEmpty(shipment.TrackingNumber))
                            continue;
                        ShipmentAddressId = order.ShippingAddressId.Value;
                    }
                    if (IsCod)
                    {
                        int engPrice = GetEngPriceForCod(shipment.Id);
                        //int IncomePrice = getIncomePrice(item.Id);
                        CodValue = engPrice + kalaPrice;//+ IncomePrice;
                                                        //CodValue += (CodValue * 4 / 100);
                    }
                    var address = _addressService.GetAddressById(ShipmentAddressId);
                    reciver = new Nop.Plugin.Misc.ShippingSolutions.Models.Params.Chapar.ReceiverBulkImport()
                    {
                        address = address.Address1,
                        city_no = address.getDtsStateId().ToString(),
                        company = address.Company,
                        email = address.Email,
                        mobile = address.PhoneNumber,
                        person = (address.FirstName ?? "") + (" " + address.LastName ?? ""),
                        postcode = address.ZipPostalCode,
                        telephone = ""
                    };
                    var _result = await _chapar_Service.Bulkimport(new Plugin.Misc.ShippingSolutions.Models.Params.Chapar.Params_Chapar_BulkImport()
                    {
                        bulk = new List<Plugin.Misc.ShippingSolutions.Models.Params.Chapar.Bulk>() {
                            new Plugin.Misc.ShippingSolutions.Models.Params.Chapar.Bulk(){
                                cn= new Plugin.Misc.ShippingSolutions.Models.Params.Chapar.CnBulkImport(){
                                    assinged_pieces = "1",
                                    content = getOrderItemContent(item),
                                    payment_term = new int[] { 713,715 }.Contains(serviceId) ? 1 : 0,
                                    payment_terms = new int[] { 713,715 }.Contains(serviceId) ? 1 : 0,
                                    weight = weight.ToString(),
                                    date = DateTime.Now.Year+"-"+DateTime.Now.Month+"-"+DateTime.Now.Day,
                                    reference=(shipment.Id*2)+62,
                                    length = dimension.Item1,
                                    width = dimension.Item2,
                                    height = dimension.Item3,
                                    service =  (new int[] { 713,712 }.Contains(serviceId) ? "6" : "1"),
                                    value = (IsCod?CodValue.ToString():"0"),
                                    inv_value = (IsCod?CodValue:0)
                                },
                                receiver = reciver,
                                sender = sender
                            }
                        }
                    });
                    if (_result == null)
                    {
                        ChangeOrderState(order, OrderStatus.Processing, $" دلیل انجام نشدن این درخواست در آتیم {item.Id} ردیف {i} " + " : " + _result.Message);
                        return "در زمان ثبت سفارش در سرویس مورد نظر مشکلی پیش آمده با واحد پشتیبانی تماس بگیرید";
                    }
                    if (_result.objects == null)
                    {
                        ChangeOrderState(order, OrderStatus.Processing, $" دلیل انجام نشد این درخواست در آتیم {item.Id} ردیف {i} " + " : " + _result.Message);
                        return "در زمان ثبت سفارش در سرویس مورد نظر مشکلی پیش آمده با واحد پشتیبانی تماس بگیرید";
                    }
                    var result = _result.objects.result.First();
                    if (_result.Status)
                    {
                        shipment.TrackingNumber = result.package[0];
                        shipment.AdminComment = result.tracking;
                        _shipmentService.UpdateShipment(shipment);
                    }
                    else
                    {
                        ChangeOrderState(order, OrderStatus.Processing, $" دلیل انجام نشد این درخواست در آتیم {item.Id} ردیف {i} " + " : " + _result.Message);
                        return "در زمان ثبت سفارش در سرویس مورد نظر مشکلی پیش آمده با واحد پشتیبانی تماس بگیرید";
                    }
                }
            }
            if (order.Shipments.All(p => !string.IsNullOrEmpty(p.TrackingNumber)))
            {
                order.OrderStatus = OrderStatus.Complete;
                _orderService.UpdateOrder(order);
                SavePostCoordination(order.Id, "ارجاع به پست گیت وی");
            }
            return "";
        }

        private int GetEngPriceForCod(int id)
        {

            string query = $@"SELECT
	                            (dbo.CalcShipmentPrice(oi.Id,S.Id,TCPOI.Id,O.PaymentMethodSystemName)  - TCPOI.IncomePrice) AS EngPrice
                            FROM
	                            dbo.[Order] AS O
	                            INNER JOIN dbo.Shipment AS S ON S.OrderId = O.Id
	                            INNER JOIN dbo.ShipmentItem AS SI ON SI.ShipmentId = S.Id
	                            INNER JOIN dbo.OrderItem AS OI ON OI.Id = SI.OrderItemId
	                            INNER JOIN dbo.Tb_CalcPriceOrderItem AS TCPOI ON TCPOI.OrderItemId = OI.Id
                            WHERE
	                            S.Id = {id}";
            return _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
        }
        #endregion

        #region Mahex
        public async Task<GetPriceResult> getmahexPrice(getPriceInputModel model)
        {
            GetPriceResult getPriceResult = new GetPriceResult();
            getPriceResult.ServiceId = model.ServiceId;
            try
            {
                var SnederMahexCode = _mahex_Service.GetCityCode(model.ReaciverStateId == 0 ? model.ReciverAddress.StateProvinceId.Value : model.ReaciverStateId);
                if (SnederMahexCode == null || SnederMahexCode.Code == null)
                {
                    common.Log("بروز مشکل در زمان دریافت قیمت ماهکس", "شهر فرستنده در فهرست شهر های ماهکس موجود نمی باشد");

                    getPriceResult.errorMessage = "شهر فرستنده در فهرست شهر های ماهکس موجود نمی باشد";
                    getPriceResult.canSelect = false;
                    getPriceResult.price = 0;
                    return getPriceResult;
                }
                var ReceiverMahexCode = _mahex_Service.GetCityCode(model.SenderStateId == 0 ? model.SenderAddress.StateProvinceId.Value : model.SenderStateId);
                if (ReceiverMahexCode == null || ReceiverMahexCode.Code == null)
                {
                    common.Log("بروز مشکل در زمان دریافت قیمت ماهکس", "شهر گیرنده در فهرست شهر های ماهکس موجود نمی باشد");

                    getPriceResult.errorMessage = "شهر گیرنده در فهرست شهر های ماهکس موجود نمی باشد";
                    getPriceResult.canSelect = false;
                    getPriceResult.price = 0;
                    return getPriceResult;
                }
                var obj = new Plugin.Misc.ShippingSolutions.Models.Params.Mahex.Params_Mahex_GetPrices()
                {
                    to_address = new Plugin.Misc.ShippingSolutions.Models.Params.Mahex.to_address
                    {
                        city_code = SnederMahexCode.Code,
                        Street = model.ReciverAddress.Address1,
                        postal_code = model.ReciverAddress.ZipPostalCode
                    },
                    from_address = new Plugin.Misc.ShippingSolutions.Models.Params.Mahex.to_address
                    {
                        city_code = ReceiverMahexCode.Code,
                        Street = model.SenderAddress.Address1,
                        postal_code = model.SenderAddress.ZipPostalCode
                    }
                };
                obj.parcels = new List<Plugin.Misc.ShippingSolutions.Models.Params.Mahex.parcels>();
                obj.parcels.Add(new Plugin.Misc.ShippingSolutions.Models.Params.Mahex.parcels()
                {
                    weight = Convert.ToDecimal((decimal)model.Weight / (decimal)1000),
                    height = model.Height,
                    length = model.Length,
                    width = model.Width,
                    content = model.Content,
                    declared_value = model.AproximateValue
                });
                var mahexPricingResult = await _mahex_Service.GetQuote(obj);
                ;
                if (mahexPricingResult.status.code != 200)
                {
                    common.Log("بروز مشکل در زمان دریافت قیمت ماهکس", mahexPricingResult.status.message);

                    getPriceResult.errorMessage = mahexPricingResult.status.message;
                    getPriceResult.canSelect = false;
                    getPriceResult.price = 0;
                    return getPriceResult;
                }
                getPriceResult.errorMessage = "";
                getPriceResult.price = Convert.ToInt32(mahexPricingResult.data.rate.amount);
                getPriceResult.canSelect = true;
                getPriceResult.SLA = mahexPricingResult.data.delivery_window;
                return getPriceResult;
            }
            catch (Exception ex)
            {
                LogException(ex);
                getPriceResult.errorMessage = "خطا در زمان استعلام قیمت ماهکس";
                getPriceResult.canSelect = false;
                getPriceResult.price = 0;
                return getPriceResult;
            }
        }
        public async Task<string> RegisterMahex_Order(Order order)
        {
            if (order.OrderNotes.Any(p => p.Note.Contains("SendDataToPost")))
            {
                return "ممکن است اطلاعات مرسوله قبلا ارسال شده باشد";
            }
            if (!CheckHasValidPrice(order))
                return "قیمت سرویس مورد نظر شما به درستی محاسبه نشده با واحد پشتیبانی تماس بگیرید";


            InsertOrderNote("SendDataToPost", order.Id);
            bool IsMultishipment = IsMultiShippment(order);
            var sender = new Nop.Plugin.Misc.ShippingSolutions.Models.Params.Mahex.address()
            {
                street = order.BillingAddress.Address1,
                city_code = _mahex_Service.GetCityCode(order.BillingAddress.StateProvinceId.Value).Code,
                organization = order.BillingAddress.Company,
                // = order.BillingAddress.Email,
                mobile = order.BillingAddress.PhoneNumber,
                first_name = order.BillingAddress.FirstName,
                last_name = order.BillingAddress.LastName,
                postal_code = order.BillingAddress.ZipPostalCode,
                phone = "",
                type = "LEGAL"
            };

            foreach (var item in order.OrderItems)
            {
                int serviceId = 0;
                var cat = item.Product.ProductCategories.Where(p => p.Category.ParentCategoryId != 0).FirstOrDefault();
                decimal weight = (decimal)GetItemWeightFromAttr(item) / (decimal)1000;
                int kalaPrice = getGoodsPrice(item);

                int CodValue = 0;
                if (cat == null)
                {
                    InsertOrderNote($"سرویس مربوط به آیتم {item.Id} از این سفارش یافت نشد", order.Id);
                    continue;
                }
                if (!new int[] { 730, 731 }.Contains(cat.CategoryId))
                {
                    InsertOrderNote($"آیتم شماره {0} مربوط به سرویس ماهکس نمی باشد", order.Id);
                    continue;
                }
                serviceId = cat.CategoryId;
                var dimension = getDimensions(item);


                for (int i = 0; i < item.Quantity; i++)
                {
                    Shipment shipment = null;
                    int ShipmentAddressId = 0;
                    Nop.Plugin.Misc.ShippingSolutions.Models.Params.Mahex.address reciver = null;
                    if (IsMultishipment)
                    {
                        var multishipment = getShipmentFromMultiShipment(item, i);
                        if (multishipment == null)
                        {
                            InsertOrderNote($"اطلاعات محموله آتیم شماره {item.Id}  ردیف {i} یافت نشد", order.Id);
                            continue;
                        }
                        if (!string.IsNullOrEmpty(multishipment.shipment.TrackingNumber))
                            continue;
                        shipment = multishipment.shipment;
                        ShipmentAddressId = multishipment.ShipmentAddressId;
                    }
                    else
                    {
                        shipment = GetShipment(item, i);
                        if (!string.IsNullOrEmpty(shipment.TrackingNumber))
                            continue;
                        ShipmentAddressId = order.ShippingAddressId.Value;
                    }
                    int EngAndCOdPrice = 0;
                    List<price_items> PriceItems = new List<price_items>();
                    int _cod_amount = 0;
                    if (serviceId == 731)
                    {
                        int CodGoodsValue = getGoodsPrice(item);
                        EngAndCOdPrice = GetEngPriceForCod(shipment.Id);
                        //_cod_amount = EngAndCOdPrice + CodGoodsValue;
                        PriceItems.Add(new price_items()
                        {
                            amount = EngAndCOdPrice + CodGoodsValue,
                            code = "PACKAGING"
                        });
                    }

                    string Uuid = "";
                    if (string.IsNullOrEmpty(shipment.TrackingNumber))
                    {
                        // Uuid = HandelIfMahexRegistredBefore(shipment.Id);
                        if (string.IsNullOrEmpty(Uuid))
                        {
                            var address = _addressService.GetAddressById(ShipmentAddressId);
                            reciver = new Nop.Plugin.Misc.ShippingSolutions.Models.Params.Mahex.address()
                            {
                                street = address.Address1,
                                city_code = _mahex_Service.GetCityCode(address.StateProvinceId.Value).Code,
                                organization = address.Company,
                                // = address.Email,
                                mobile = address.PhoneNumber,
                                first_name = address.FirstName,
                                last_name = address.LastName,
                                postal_code = address.ZipPostalCode,
                                phone = "",
                                type = "LEGAL"
                            };
                            var obj = new Plugin.Misc.ShippingSolutions.Models.Params.Mahex.Params_Mahex_createShipment()
                            {
                                from_address = sender,
                                to_address = reciver,
                                //delivery_date = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day,
                                reference = "999919" + ((shipment.Id * 2) + 62).ToString(),
                                cod_amount = _cod_amount,
                                charge_party = (serviceId == 731 ? "third_party" : "SHIPPER"),
                                price_items = PriceItems

                            };
                            obj.parcels = new List<Plugin.Misc.ShippingSolutions.Models.Params.Mahex.parcels>();
                            obj.parcels.Add(new Plugin.Misc.ShippingSolutions.Models.Params.Mahex.parcels()
                            {
                                id = "999919" + shipment.Id.ToString(),
                                content = getOrderItemContent(item),
                                length = dimension.Item1,
                                width = dimension.Item2,
                                height = dimension.Item3,
                                weight = weight,

                            });

                            var _result = await _mahex_Service.Bulkimport(obj);
                            if (_result == null)
                            {
                                ChangeOrderState(order, OrderStatus.Processing, $" دلیل انجام نشدن این درخواست در آتیم {item.Id} ردیف {i} " + " : " + _result.status.message);
                                return "در زمان ثبت سفارش در سرویس مورد نظر مشکلی پیش آمده با واحد پشتیبانی تماس بگیرید";
                            }
                            if (_result.data == null)
                            {
                                ChangeOrderState(order, OrderStatus.Processing, $" دلیل انجام نشد این درخواست در آتیم {item.Id} ردیف {i} " + " : " + _result.status.message);
                                return "در زمان ثبت سفارش در سرویس مورد نظر مشکلی پیش آمده با واحد پشتیبانی تماس بگیرید";
                            }

                            int h = _dbContext.ExecuteSqlCommand($@"INSERT INTO dbo.Tb_Mahex_Shipment
                                                            (
                                                                ShipmentId,
                                                                guid
                                                            )

                                                            VALUES
                                                            ({shipment.Id},
                                                            '{_result.data.shipment_uuid}'
                                                            )");

                            var result = _result.data.shipment_uuid;
                            if (_result.status.code == 201)
                            {
                                var trackResult = _mahex_Service.UpdateShipmentTracking(_result.data.shipment_uuid);
                                if (trackResult.status.code == 200)
                                {
                                    shipment.TrackingNumber = trackResult.data.waybill_number;//.tracking;
                                    _shipmentService.UpdateShipment(shipment);
                                }
                                else
                                {
                                    Log("Mahex UpdateShipmentTracking error", trackResult.status.message);
                                    ChangeOrderState(order, OrderStatus.Processing, $" دلیل انجام نشد این درخواست در آتیم {item.Id} ردیف {i} " + " : " + trackResult.status.message);
                                    return "در زمان ثبت سفارش در سرویس مورد نظر مشکلی پیش آمده با واحد پشتیبانی تماس بگیرید";
                                }
                            }
                            else
                            {
                                Log("Mahex register error", _result.status.message);
                                ChangeOrderState(order, OrderStatus.Processing, $" دلیل انجام نشد این درخواست در آتیم {item.Id} ردیف {i} " + " : " + _result.status.message);
                                return "در زمان ثبت سفارش در سرویس مورد نظر مشکلی پیش آمده با واحد پشتیبانی تماس بگیرید";
                            }
                        }
                        else
                        {
                            var trackResult = _mahex_Service.UpdateShipmentTracking(Uuid);
                            if (trackResult.status.code == 200)
                            {
                                shipment.TrackingNumber = trackResult.data.waybill_number;//.tracking;
                                _shipmentService.UpdateShipment(shipment);
                            }
                            else
                            {
                                Log("Mahex UpdateShipmentTracking error", trackResult.status.message);
                                ChangeOrderState(order, OrderStatus.Processing, $" دلیل انجام نشد این درخواست در آتیم {item.Id} ردیف {i} " + " : " + trackResult.status.message);
                                return "در زمان ثبت سفارش در سرویس مورد نظر مشکلی پیش آمده با واحد پشتیبانی تماس بگیرید";
                            }
                        }
                    }
                }
            }
            if (order.Shipments.All(p => !string.IsNullOrEmpty(p.TrackingNumber)))
            {
                order.OrderStatus = OrderStatus.Complete;
                _orderService.UpdateOrder(order);
                SavePostCoordination(order.Id, "ارجاع به نماینده پس از ثبت درخواست");
            }
            return "";
        }
        private string HandelIfMahexRegistredBefore(int shipmentId)
        {
            try
            {
                string query = $@"SELECT
	                                TOP(1) TMS.guid
                                FROM
	                                dbo.Tb_Mahex_Shipment AS TMS
	                                INNER JOIN dbo.Shipment AS S ON S.Id = TMS.ShipmentId
                                WHERE
	                                s.Id = {shipmentId}
	                                AND S.TrackingNumber IS NULL 
                                ORDER BY TMS.Id";
                return _dbContext.SqlQuery<string>(query, new object[0]).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogException(ex);
                return "";
            }
        }
        #endregion

        #region DTS
        public int getDtsStateId(int StateId)
        {
            if (new int[] { 4, 579, 580, 581, 582, 583, 584, 585 }.Contains(StateId))//برای مناطق تهران باید کد شهر تهران ارسال شود در دی تی اس
            {
                return 10866;
            }
            string query = $@"SELECT top(1)
	                            TDSP.City_Id
                            FROM
	                            dbo.Tb_Dts_StateProvince AS TDSP
                            WHERE
	                            TDSP.StateId = {StateId} ORDER BY City_Id";
            return _dbContext.SqlQuery<int?>(query).FirstOrDefault().GetValueOrDefault(0);
        }
        /// <summary>
        /// ثبت سفارش دی تی اس
        /// </summary>
        /// <param name="order"></param>
        public async Task<string> RegisterDTS_Order(Order order)
        {
            if (order.OrderNotes.Any(p => p.Note.Contains("SendDataToPost")))
            {
                return "ممکن است اطلاعات مرسوله قبلا ارسال شده باشد";
            }
            if (!CheckHasValidPrice(order))
                return "قیمت سرویس مورد نظر شما به درستی محاسبه نشده با واحد پشتیبانی تماس بگیرید";
            InsertOrderNote("SendDataToPost", order.Id);
            bool IsMultishipment = IsMultiShippment(order);
            Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran.Sender sender = new Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran.Sender()
            {
                address = order.BillingAddress.Address1,
                city_no = order.BillingAddress.getDtsStateId().ToString(),
                code = "10825",
                company = order.BillingAddress.Company,
                email = order.BillingAddress.Email,
                mobile = order.BillingAddress.PhoneNumber,
                person = (order.BillingAddress.FirstName ?? "") + (" " + order.BillingAddress.LastName ?? ""),
                postcode = order.BillingAddress.ZipPostalCode,
                telephone = "",
                GEO = new Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran.GEO() { lat = "0", lng = "0" }
            };
            foreach (var item in order.OrderItems)
            {
                int serviceId = 0;
                var cat = item.Product.ProductCategories.Where(p => p.Category.ParentCategoryId != 0).FirstOrDefault();
                decimal weight = (decimal)GetItemWeightFromAttr(item) / (decimal)1000;
                int kalaPrice = getGoodsPrice(item);

                if (cat == null)
                {
                    InsertOrderNote($"سرویس مربوط به آیتم {item.Id} از این سفارش یافت نشد", order.Id);
                    continue;
                }
                if (!new int[] { 703, 699, 705, 706 }.Contains(cat.CategoryId))
                {
                    InsertOrderNote($"آیتم شماره {0} مربوط به سرویس دی تی اس نمی باشد", order.Id);
                    continue;
                }
                serviceId = cat.CategoryId;
                var dimension = getDimensions(item);
                for (int i = 0; i < item.Quantity; i++)
                {
                    Shipment shipment = null;
                    int ShipmentAddressId = 0;
                    Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran.Receiver reciver = null;
                    if (IsMultishipment)
                    {
                        var multishipment = getShipmentFromMultiShipment(item, i);
                        if (multishipment == null)
                        {
                            InsertOrderNote($"اطلاعات محموله آتیم شماره {item.Id}  ردیف {i} یافت نشد", order.Id);
                            continue;
                        }
                        if (!string.IsNullOrEmpty(multishipment.shipment.TrackingNumber))
                            continue;
                        shipment = multishipment.shipment;
                        ShipmentAddressId = multishipment.ShipmentAddressId;
                    }
                    else
                    {
                        shipment = GetShipment(item, i);
                        if (!string.IsNullOrEmpty(shipment.TrackingNumber))
                            continue;
                        ShipmentAddressId = order.ShippingAddressId.Value;
                    }
                    if (new int[] { 705, 706 }.Contains(cat.CategoryId))
                    {
                        int engPrice = GetEngPriceForCod(shipment.Id);
                        kalaPrice = engPrice + kalaPrice;
                    }
                    var address = _addressService.GetAddressById(ShipmentAddressId);
                    reciver = new Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran.Receiver()
                    {
                        address = address.Address1,
                        city_no = address.getDtsStateId().ToString(),
                        code = "",
                        company = address.Company,
                        email = address.Email,
                        mobile = address.PhoneNumber,
                        person = (address.FirstName ?? "") + (" " + address.LastName ?? ""),
                        postcode = address.ZipPostalCode,
                        telephone = ""
                    };
                    var result = await _safiran_Service.Pickup(new Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran.Params_Safiran_PickupRequest()
                    {
                        pickup_man = "8000",
                        cn = new Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran.Cn()
                        {
                            assiged_pieces = "1",
                            content = getOrderItemContent(item),
                            payment_term = new int[] { 705, 706 }.Contains(serviceId) ? "1" : "0",
                            weight = weight.ToString(),
                            height = dimension.Item3.ToString(),
                            length = dimension.Item1.ToString(),
                            width = dimension.Item2.ToString(),
                            service = new int[] { 699, 706 }.Contains(serviceId) ? "6" : "1",
                            value = kalaPrice.ToString(),
                        },
                        awb = new Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran.Awb()
                        {
                            package = new List<string>()
                        },
                        cod = (kalaPrice > 0 && new int[] { 705, 706 }.Contains(serviceId)),
                        receiver = reciver,
                        sender = sender
                    });
                    if (result.Status)
                    {
                        shipment.TrackingNumber = result.objects.cons;
                        _shipmentService.UpdateShipment(shipment);
                    }
                    else
                    {
                        string error = $" دلیل انجام نشد این درخواست در آتیم {item.Id} ردیف {i} " + " : " + result.Message;
                        ChangeOrderState(order, OrderStatus.Processing, error);
                        return error;
                    }
                }
            }
            return "";
        }
        /// <summary>
        /// گرفتن قیمت از سرویس دی تی اس
        /// </summary>
        /// <param name="ServiceId"></param>
        /// <param name="AproximateValue"></param>
        /// <param name="weightItem"></param>
        /// <param name="SenderCityId"></param>
        /// <param name="ReciverCityId"></param>
        /// <param name="SenderCode"></param>
        /// <param name="price"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public async Task<GetPriceResult> getDtsPrice(getPriceInputModel model, int SenderCode)
        {
            GetPriceResult priceResult = new GetPriceResult();
            priceResult.ServiceId = model.ServiceId;
            int _method = new int[] { 699, 706 }.Contains(model.ServiceId) ? 6 : 1;
            bool IsCod = new int[] { 705, 706 }.Contains(model.ServiceId);
            var dtsPricei = await _safiran_Service.GetQuote(new Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran.Params_Safiran_GetQuote
            {
                order = new Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran.Order
                {
                    cod = (IsCod ? 1 : 0),
                    weight = Convert.ToDecimal((decimal)model.Weight / (decimal)1000),
                    value = model.AproximateValue,
                    receiver_code = "",
                    sender_code = SenderCode,
                    method = _method,
                    origin = model.SenderStateId.ToString(),
                    destination = model.ReaciverStateId.ToString(),
                    length = model.Length,
                    Height = model.Height,
                    width = model.Width
                }
            });
            if (!dtsPricei.Status)
            {
                common.Log("بروز مشکل در زمان دریافت قیمت از دی تی اس", dtsPricei.Message);
                priceResult.errorMessage = dtsPricei.Message;
                priceResult.canSelect = false;
                priceResult.price = 0;
                return priceResult;
            }
            priceResult.errorMessage = "";
            priceResult.canSelect = true;
            priceResult.price = Convert.ToInt32(dtsPricei.objects.order.quote);
            return priceResult;
        }

        /// <summary>
        /// مبلغ سفارش در هر آیتم
        /// </summary>
        /// <param name="IncomePrice"></param>
        /// <param name="orderitemId"></param>
        /// <returns></returns>
        public int getOrderTotalbyIncomeApiPrice(int IncomePrice, int orderitemId, int serviceId, int Weight = 0, int? DealerId = null)
        {
            DealerId = DealerId.HasValue ? DealerId : 0;
            SqlParameter P_TotalPrice = new SqlParameter()
            {
                ParameterName = "TotalPrice",
                SqlDbType = SqlDbType.Int,
                Value = (object)0,
                Direction = ParameterDirection.Output
            };
            SqlParameter[] prms = new SqlParameter[]{
                new SqlParameter(){ParameterName="IncomePrice",SqlDbType = SqlDbType.Int,Value= (object)IncomePrice },
                new SqlParameter(){ParameterName="CustomerId",SqlDbType = SqlDbType.Int,Value= (object)_workContext.CurrentCustomer.Id },
                new SqlParameter(){ParameterName="StoreId",SqlDbType = SqlDbType.Int,Value= (object)_storeContext.CurrentStore.Id },
                new SqlParameter(){ParameterName="OrderItemId",SqlDbType = SqlDbType.Int,Value= (object)orderitemId },
                new SqlParameter(){ParameterName="Weight",SqlDbType = SqlDbType.Int,Value= (object)Weight },
                new SqlParameter(){ParameterName="ServiceId",SqlDbType = SqlDbType.Int,Value= (object)serviceId },
                new SqlParameter(){ParameterName="DealerId",SqlDbType = SqlDbType.Int,Value= (object)DealerId },
                P_TotalPrice
            };
            string query = "";
            if (DealerId.Value == 0)
                query = "EXEC dbo.Bil_Sp_CalcPrtnerPrice @IncomePrice, @CustomerId, @StoreId, @OrderItemId, @Weight, @ServiceId, @TotalPrice OUTPUT";
            else
                query = "EXEC dbo.Bil_Sp_CalcPrtnerPriceFor_Ap @IncomePrice, @CustomerId, @DealerId, @StoreId, @OrderItemId, @Weight, @ServiceId, @TotalPrice OUTPUT";
            _dbContext.SqlQuery<int>(query, prms).FirstOrDefault();
            return (int)P_TotalPrice.Value;
        }
        public int getIncomePrice(int orderItemId)
        {
            string query = $@"SELECT
	                         TCPOI.IncomePrice
                        FROM
	                        dbo.Tb_CalcPriceOrderItem AS TCPOI
                        WHERE
	                        TCPOI.OrderItemId = " + orderItemId;
            return _dbContext.SqlQuery<int?>(query, new object[0]).SingleOrDefault().GetValueOrDefault(0);
        }
        #endregion

        #region PDE
        public PDEStateModel getPDEState(int StateId)
        {
            string query = $@"SELECT
	                            TPSP.PDEStateId,
                                TPSP.IsForcedTehran,
                                TPSP.PDEStateName
                            FROM
	                            dbo.Tb_PDE_StateProvince AS TPSP
                            WHERE
	                            TPSP.StateId = " + StateId;
            return _dbContext.SqlQuery<PDEStateModel>(query).FirstOrDefault();
        }
        public async Task<GetPriceResult> getPDE_Price(getPriceInputModel model)
        {
            try
            {
                GetPriceResult getPriceResult = new GetPriceResult();
                getPriceResult.ServiceId = model.ServiceId;
                var PDEresult = await _pDE_Service.DomesticCalculator(new Plugin.Misc.ShippingSolutions.Models.Params.PDE.Params_PDE_DomesticCalculator()
                {
                    Ccode = 101,
                    Chw = Convert.ToDecimal((decimal)model.Weight / (decimal)1000),
                    DestCity = model.ReaciverStateId,
                    OriginCity = model.SenderStateId,
                    Height = model.Height,
                    Length = model.Length,
                    Width = model.Width,
                    Password = "2fKaR0CX"
                });
                if (!PDEresult.Status)
                {
                    getPriceResult.errorMessage = PDEresult.Message;
                    common.Log("بروز مشکل در زمان دریافت قیمت از پی دی ای-داخلی", PDEresult.Message);
                    getPriceResult.canSelect = false;
                    return getPriceResult;
                }
                getPriceResult.price = Convert.ToInt32(PDEresult.PriceCalculator.Price);
                // getPriceResult.price = getPriceResult.price - ((price * 20) / 100);
                getPriceResult.canSelect = true;
                return getPriceResult;
            }
            catch (Exception ex)
            {
                var getPriceResult = new GetPriceResult();
                LogException(ex);
                getPriceResult.errorMessage = "در حال حاضر سرویس ارائه نمی شود";
                getPriceResult.canSelect = false;
                return getPriceResult;
            }
        }
        public async Task<GetPriceResult> getPDEInternational_Price(getPriceInputModel model)
        {
            GetPriceResult getPriceResult = new GetPriceResult();
            getPriceResult.ServiceId = model.ServiceId;
            if (model.Height == 0 || model.Length == 0 || model.Width == 0)
            {
                getPriceResult.errorMessage = "وارد کردن ابعاد برای این سرویس الزامی می باشد";
                getPriceResult.canSelect = false;
                return getPriceResult;
            }

            var PDEresult = await _pDE_Service.IntenationalCalculator(new Plugin.Misc.ShippingSolutions.Models.Params.Params_PDE_IntenationalCalculator
            {
                Ccode = 101,
                Chw = Convert.ToDecimal((decimal)model.Weight / (decimal)1000),
                ConsType = model.ConsType,
                OriginCountry = 106,
                DestCountry = model.ReciverCountry,
                Height = model.Height,
                Length = model.Length,
                Width = model.Width,
                Password = "2fKaR0CX"
            });
            if (!PDEresult.Status)
            {
                getPriceResult.errorMessage = PDEresult.Message;
                common.Log("بروز مشکل در زمان دریافت قیمت از پی دی ای-خارجی", PDEresult.Message);
                getPriceResult.canSelect = false;
                return getPriceResult;
            }
            else if (PDEresult.PriceCalculator.Price == 0)
            {
                getPriceResult.errorMessage = "قیمت 0 از سمت سرویس PDE";
                getPriceResult.canSelect = false;
                return getPriceResult;
            }
            getPriceResult.price = Convert.ToInt32(PDEresult.PriceCalculator.Price);
            getPriceResult.canSelect = getPriceResult.price > 0;
            return getPriceResult;
        }

        public int getForinCountryForPDE(int IsoCountryId)
        {
            string query = $@"SELECT
	                            ISNULL(TCI.PDEId,0) PDEId
                            FROM
	                            dbo.Tbl_CountryISO3166 AS TCI
                            WHERE
	                            TCI.Id ={IsoCountryId}";
            return (_dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0));
        }
        public List<SelectListItem> getForinCountry()
        {
            //var result = _pDE_Service.Countries();
            string query = $@"SELECT
	            TCI.Name_F [Text]
	            ,CAST(TCI.Id AS NVARCHAR(10))+'|'+TCI.Name_E [Value]
	
            FROM
	            dbo.Tbl_CountryISO3166 AS TCI
            WHERE
	            TCI.IsActive = 1";
            return _dbContext.SqlQuery<SelectListItem>(query, new object[0]).OrderBy(p => p.Text).ToList();
            //if (result.Status)
            //{
            //    var data = result.ItemsCountries.Select(p => new SelectListItem()
            //    {
            //        Text = p.FaTitle,
            //        Value = p.Id.ToString() + "|" + p.Title
            //    }).ToList();
            //    data.Add(new SelectListItem()
            //    {
            //        Text = "قطر",
            //        Value = "634|Qatar"
            //    });
            //    return data.OrderBy(p => p.Text).ToList();
            //}
            //return null;
        }
        public List<ForgeinCountry> getForinCountryForApi()
        {
            //var result = _pDE_Service.Countries();
            string query = $@"SELECT
	                            TCI.Id,
	                            TCI.Name_F+'/'+TCI.Name_E CountryName,
	                            TCI.Alpha2Code ISO3166Code
                            FROM
	                            dbo.Tbl_CountryISO3166 AS TCI";
            return _dbContext.SqlQuery<ForgeinCountry>(query, new object[0]).OrderBy(p => p.ISO3166Code).ToList();
        }
        public class ForgeinCountry
        {
            public int Id { get; set; }
            public string CountryName { get; set; }
            public string ISO3166Code { get; set; }
        }
        public async Task<string> RegisterDomesticPDEOrder(Order order)
        {

            if (order.OrderNotes.Any(p => p.Note.Contains("SendDataToPost")))
            {
                return "ممکن است اطلاعات مرسوله قبلا ارسال شده باشد";
            }
            if (!CheckHasValidPrice(order))
                return "قیمت سرویس مورد نظر شما به درستی محاسبه نشده با واحد پشتیبانی تماس بگیرید";
            InsertOrderNote("SendDataToPost", order.Id);

            bool IsMultishipment = IsMultiShippment(order);
            foreach (var item in order.OrderItems)
            {
                int ServiceId = item.Product.ProductCategories.First().CategoryId;
                if (ServiceId != 708)
                {
                    InsertOrderNote($"آیتم شماره {0} مربوط به سرویس PDE نمی باشد", order.Id);
                    continue;
                }
                int ApproximateValue = getApproximateValue(item.Id);
                int weight = GetItemWeightFromAttr(item);
                var Dimensions = getDimensions(item);

                for (int i = 0; i < item.Quantity; i++)
                {
                    Shipment shipment = null;
                    int ShipmentAddressId = 0;
                    if (IsMultishipment)
                    {
                        var multishipment = getShipmentFromMultiShipment(item, i);
                        if (multishipment == null)
                        {
                            InsertOrderNote($"اطلاعات محموله آتیم شماره {item.Id}  ردیف {i} یافت نشد", order.Id);
                            continue;
                        }
                        if (!string.IsNullOrEmpty(multishipment.shipment.TrackingNumber))
                            continue;
                        shipment = multishipment.shipment;
                        ShipmentAddressId = multishipment.ShipmentAddressId;
                    }
                    else
                    {
                        shipment = GetShipment(item, i);
                        if (!string.IsNullOrEmpty(shipment.TrackingNumber))
                            continue;
                        ShipmentAddressId = order.ShippingAddressId.Value;
                    }
                    var shipmentAddress = _addressService.GetAddressById(ShipmentAddressId);
                    int calcPrice = 0;
                    var SenderPdeStateId = getPDEState(order.BillingAddress.StateProvinceId.Value);
                    var ReciverPdeStateId = getPDEState(shipmentAddress.StateProvinceId.Value);
                    string _error = "";
                    bool canselect = true;

                    var pdePriceresult = await getPDE_Price(PriceInpuModelFactory(senderCountry: 0, receiverCountry: 0, error: out _error, canSelect: out canselect, width: Dimensions.Item2, AproximateValue: ApproximateValue, height: Dimensions.Item3,
                        length: Dimensions.Item1, receiverState: shipmentAddress.StateProvinceId.Value,
                        senderState: order.BillingAddress.StateProvinceId.Value,
                        ServiceId: ServiceId,
                        weightItem: weight));
                    if (!pdePriceresult.canSelect)
                    {
                        InsertOrderNote($"قیمت محاسبه شده برای محموله شماره {shipment.Id} نامعتبر بوده و درخواست مربوط به این محموله ثبت نشده" + _error, order.Id);
                        continue;
                    }
                    var result = await _pDE_Service.RegisterDomesticOrder(new Plugin.Misc.ShippingSolutions.Models.Params.PDE.Params_PDE_RegisterDomesticOrder()
                    {
                        //APIRegisterOrderViewModel = "?Chw=1",
                        ClientId = 23553,
                        Commodity1 = item.Product.Name + "-" + getOrderItemContent(item),
                        Quantity = 1,
                        HsCode1 = "",
                        Consignment = getPDEConsType(item),
                        ConsValue = ApproximateValue,
                        CustCode = 101,
                        Length = Dimensions.Item1,
                        Width = Dimensions.Item2,
                        Height = Dimensions.Item3,
                        Nw = Convert.ToDecimal((decimal)weight / (decimal)1000),
                        Chw = Convert.ToDecimal((decimal)weight / (decimal)1000),
                        Password = "2fKaR0CX",
                        DesId = ReciverPdeStateId.PDEStateId,
                        Dest = ReciverPdeStateId.PDEStateName,//shipmentAddress.StateProvince.Name,
                        OrigId = SenderPdeStateId.PDEStateId,
                        Origin = SenderPdeStateId.PDEStateName,// order.BillingAddress.StateProvince.Name,
                        RecieverAddress = shipmentAddress.Address1,
                        RecieverCompanyName = shipmentAddress.Company ?? "",
                        RecieverEmail = shipmentAddress.Email ?? "",
                        RecieverLastName = shipmentAddress.LastName ?? "",
                        RecieverMobile = shipmentAddress.PhoneNumber ?? "",
                        RecieverName = shipmentAddress.FirstName ?? "",
                        RecieverPostalCode = shipmentAddress.ZipPostalCode ?? "",
                        RecieverTel = shipmentAddress.PhoneNumber ?? "",
                        RefNo = shipment.Id.ToString(),
                        Remarks = "",
                        SenderAddress = order.BillingAddress.Address1,
                        SenderCompanyName = order.BillingAddress.Company ?? "",
                        SenderEmail = order.BillingAddress.Email ?? "",
                        SenderLastName = order.BillingAddress.LastName ?? "",
                        SenderMobile = order.BillingAddress.PhoneNumber ?? "",
                        SenderName = order.BillingAddress.FirstName ?? "",
                        SenderPostalCode = order.BillingAddress.ZipPostalCode ?? "",
                        SenderTel = order.BillingAddress.PhoneNumber ?? "",
                        ServiceTypeId = 1,//??
                        SystemType = 2,
                        TransportationFee = pdePriceresult.price,
                        UnitValue1 = ApproximateValue,
                    });
                    if (result.Status)
                    {
                        shipment.TrackingNumber = result.KEY.ToString();
                        _shipmentService.UpdateShipment(shipment);
                    }
                    else
                    {
                        string error = $" دلیل انجام نشد این درخواست در آتیم {item.Id} ردیف {i} " + " : " + result.Message;
                        ChangeOrderState(order, OrderStatus.Processing, error);
                        return error;
                    }
                }
            }
            return "";
        }
        public async Task<string> RegisterInternationalPDEOrder(Order order)
        {
            if (order.OrderNotes.Any(p => p.Note.Contains("SendDataToPost")))
            {
                return "ممکن است اطلاعات مرسوله قبلا ارسال شده باشد";
            }
            if (!CheckHasValidPrice(order))
                return "قیمت سرویس مورد نظر شما به درستی محاسبه نشده با واحد پشتیبانی تماس بگیرید";
            InsertOrderNote("SendDataToPost", order.Id);

            bool IsMultishipment = IsMultiShippment(order);
            bool isOk = false;
            foreach (var item in order.OrderItems)
            {
                int ServiceId = item.Product.ProductCategories.First().CategoryId;
                if (ServiceId != 707)
                {
                    InsertOrderNote($"آیتم شماره {0} مربوط به سرویس PDE نمی باشد", order.Id);
                    continue;
                }
                int ApproximateValue = getApproximateValue(item.Id);
                int weight = GetItemWeightFromAttr(item);
                var Dimensions = getDimensions(item);
                var consType = getPDEConsType(item);
                for (int i = 0; i < item.Quantity; i++)
                {
                    Shipment shipment = null;
                    int ShipmentAddressId = 0;
                    if (IsMultishipment)
                    {
                        var multishipment = getShipmentFromMultiShipment(item, i);
                        if (multishipment == null)
                        {
                            InsertOrderNote($"اطلاعات محموله آتیم شماره {item.Id}  ردیف {i} یافت نشد", order.Id);
                            continue;
                        }
                        if (!string.IsNullOrEmpty(multishipment.shipment.TrackingNumber))
                            continue;
                        shipment = multishipment.shipment;
                        ShipmentAddressId = multishipment.ShipmentAddressId;
                    }
                    else
                    {
                        shipment = GetShipment(item, i);
                        if (!string.IsNullOrEmpty(shipment.TrackingNumber))
                            continue;
                        ShipmentAddressId = order.ShippingAddressId.Value;
                    }
                    var shipmentAddress = _addressService.GetAddressById(ShipmentAddressId);

                    var SenderPdeStateId = getPDEState(order.BillingAddress.StateProvinceId.Value);
                    string _error = "";
                    bool canselect = true;

                    #region Address
                    CustomAddressModel CustomReciverAddressModel = new CustomAddressModel()
                    {
                        Address1 = shipmentAddress.Address1,
                        Address2 = shipmentAddress.Address2,
                        City = shipmentAddress.City,
                        Company = shipmentAddress.Company,
                        CountryId = shipmentAddress.CountryId,
                        CreatedOnUtc = shipmentAddress.CreatedOnUtc,
                        CustomAttributes = shipmentAddress.CustomAttributes,
                        Email = shipmentAddress.Email,
                        FaxNumber = shipmentAddress.FaxNumber,
                        FirstName = shipmentAddress.FirstName,
                        LastName = shipmentAddress.LastName,
                        PhoneNumber = shipmentAddress.PhoneNumber,
                        StateProvinceId = shipmentAddress.StateProvinceId,
                        ZipPostalCode = shipmentAddress.ZipPostalCode
                    };
                    var senderLocation = GetAddressLocation(order.BillingAddress.Id);
                    CustomAddressModel CustomSenderAddressModel = new CustomAddressModel()
                    {
                        Address1 = order.BillingAddress.Address1,
                        Address2 = order.BillingAddress.Address2,
                        City = order.BillingAddress.City,
                        Company = order.BillingAddress.Company,
                        CountryId = order.BillingAddress.CountryId,
                        CreatedOnUtc = order.BillingAddress.CreatedOnUtc,
                        CustomAttributes = order.BillingAddress.CustomAttributes,
                        Email = order.BillingAddress.Email,
                        FaxNumber = order.BillingAddress.FaxNumber,
                        FirstName = order.BillingAddress.FirstName,
                        LastName = order.BillingAddress.LastName,
                        PhoneNumber = order.BillingAddress.PhoneNumber,
                        StateProvinceId = order.BillingAddress.StateProvinceId,
                        Lat = senderLocation == null ? 0 : senderLocation.Lat,
                        Lon = senderLocation == null ? 0 : senderLocation.Long,
                        ZipPostalCode = order.BillingAddress.ZipPostalCode
                    };
                    #endregion

                    var pdeDomesticPrice = await getPDEInternational_Price(PriceInpuModelFactory(senderCountry: 0, receiverCountry: 0, error: out _error, canSelect: out canselect, width: Dimensions.Item2, AproximateValue: ApproximateValue, height: Dimensions.Item3,
                        length: Dimensions.Item1,
                        receiver_ForeginCountry: shipmentAddress.Address2.Split('|')[0].ToEnDigit(),
                        senderState: order.BillingAddress.StateProvinceId.Value,
                        receiverState: 0,
                        consType: consType,
                        ServiceId: ServiceId,
                        weightItem: weight,
                        SenderAddress: CustomSenderAddressModel,
                        ReciverAddress: CustomReciverAddressModel
                        ));
                    if (!pdeDomesticPrice.canSelect)
                    {
                        string error = $"قیمت محاسبه شده برای محموله شماره {shipment.Id} نامعتبر بوده و درخواست مربوط به این محموله ثبت نشده" + pdeDomesticPrice.errorMessage;
                        InsertOrderNote(error + pdeDomesticPrice.errorMessage, order.Id);
                        ChangeOrderState(order, OrderStatus.Processing, error + pdeDomesticPrice.errorMessage);
                        return error;
                    }
                    int ReciverCountryId = int.Parse(shipmentAddress.Address2.Split('|')[0]);
                    var result = await _pDE_Service.RegisterInternationalOrder(new Plugin.Misc.ShippingSolutions.Models.Params.Params_PDE_RegisterInternationalOrder()
                    {
                        //APIRegisterOrderViewModel = "?Chw=1",
                        ClientId = 23553,
                        Commodity1 = item.Product.Name + "-" + getOrderItemContent(item),
                        Quantity = 1,
                        HsCode1 = "",
                        Consignment = consType,
                        ConsValue = ApproximateValue,
                        CustCode = 101,
                        Length = Dimensions.Item1,
                        Width = Dimensions.Item2,
                        Height = Dimensions.Item3,
                        Nw = Convert.ToDecimal((decimal)weight / (decimal)1000),
                        Chw = Convert.ToDecimal((decimal)weight / (decimal)1000),
                        Password = "2fKaR0CX",
                        DestCId = getForinCountryForPDE(ReciverCountryId),// از آدرس2 به عنوان کشور مقصد استفاده میشود
                        Dest = (shipmentAddress.Address2.Split('|').Count() > 2 ? shipmentAddress.Address2.Split('|')[2] : shipmentAddress.Address2.Split('|')[1]),
                        OrigCId = 106,
                        Origin = "IRAN",
                        RecieverAddress = shipmentAddress.Address1,
                        RecieverCompanyName = shipmentAddress.Company ?? "",
                        RecieverEmail = shipmentAddress.Email ?? "",
                        RecieverLastName = shipmentAddress.LastName ?? "",
                        RecieverMobile = shipmentAddress.PhoneNumber ?? "",
                        RecieverName = shipmentAddress.FirstName ?? "",
                        RecieverPostalCode = shipmentAddress.ZipPostalCode ?? "",
                        RecieverTel = shipmentAddress.PhoneNumber ?? "",
                        RefNo = shipment.Id.ToString(),
                        Remarks = "",
                        SenderAddress = order.BillingAddress.Address1,
                        SenderCompanyName = order.BillingAddress.Company ?? "",
                        SenderEmail = order.BillingAddress.Email ?? "",
                        SenderLastName = order.BillingAddress.LastName ?? "",
                        SenderMobile = order.BillingAddress.PhoneNumber ?? "",
                        SenderName = order.BillingAddress.FirstName ?? "",
                        SenderPostalCode = order.BillingAddress.ZipPostalCode ?? "",
                        SenderTel = order.BillingAddress.PhoneNumber ?? "",
                        ServiceTypeId = 1,//??
                        SystemType = 1,
                        TransportationFee = pdeDomesticPrice.price,//??
                        UnitValue1 = ApproximateValue,//?
                        Quantity1 = 1,
                    });
                    if (result.Status)
                    {
                        shipment.TrackingNumber = result.KEY.ToString();
                        _shipmentService.UpdateShipment(shipment);
                        isOk = true;
                    }
                    else
                    {
                        string error = $" دلیل انجام نشد این درخواست در آتیم {item.Id} ردیف {i} " + " : " + result.Message;
                        ChangeOrderState(order, OrderStatus.Processing, error);
                        return error;
                    }
                }
            }
            //if (isOk)// تکمیل سفارش پست داخلی متناظر پست خارجی
            //{
            //    int orderId = getRelatedOrder(order.Id);
            //    var child_order = _orderService.GetOrderById(orderId);
            //    child_order.OrderStatus = order.OrderStatus;
            //    child_order.PaymentStatus = order.PaymentStatus;
            //    child_order.PaymentMethodSystemName = order.PaymentMethodSystemName;
            //    _orderService.UpdateOrder(child_order);
            //}
            return "";
        }
        #endregion

        #region Ubar

        public UbbarRegionModel getUbarState(int ObaarRegionId)
        {
            string Query = $@"SELECT
								TUCS.RegionName,
								TUCS.RegionId
							FROM
								dbo.Tb_ubbar_CityState AS TUCS
							WHERE
								TUCS.RegionId = {ObaarRegionId}";
            return _dbContext.SqlQuery<UbbarRegionModel>(Query, new object[0]).FirstOrDefault();
        }
        public async Task<GetPriceResult> getUbarPrice(getPriceInputModel model)
        {
            GetPriceResult getPriceResult = new GetPriceResult();
            getPriceResult.ServiceId = model.ServiceId;
            var result = await _ubaar_Service.Priceenquiry(new Plugin.Misc.ShippingSolutions.Models.Params.Ubbar.Params_Ubaar_priceenquiry()
            {
                weight = (double)((double)model.Weight / (double)1000000),
                source_city = model.SenderStateName,
                source_region_id = model.SenderStateId,
                destination_city = model.ReaciverStateName,
                destination_region_id = model.ReaciverStateId,
                load_type = model.Content,
                load_value = model.AproximateValue,
                dispatch_date = model.dispatch_date.Value.Date.ToString("yyyy/MM/dd"),
                dispatch_hour = model.dispatch_date.Value.ToLocalTime().ToString("HH:mm"),
                baarnameh = "yes",
                baarnameh_options = "",
                package_options = model.UbbarPackingLoad,
                suspention_type = "fanari",
                vehicle_options = (model.UbbraTruckType + ":" + ((model.UbbraTruckType.ToLower() == "" ? "" : model.VechileOptions))),
                vehicle_type = model.UbbraTruckType,
                announce_type = "ubaar"
            });
            if (result.Status)
            {
                if (result.DetailUbaarPriceInquiry.success_flag == 1)
                {

                    getPriceResult.UbbarTracking_code = result.DetailUbaarPriceInquiry.order_data.tracking_code;
                    getPriceResult.price = Convert.ToInt32(result.DetailUbaarPriceInquiry.order_data.predicted_price.Replace(".0", "")) * 10;
                    getPriceResult.canSelect = true;
                    return getPriceResult;
                }
                else
                {
                    getPriceResult.errorMessage = string.Join(Environment.NewLine, result.DetailUbaarPriceInquiry.error_messages);
                    getPriceResult.canSelect = false;
                    return getPriceResult;
                }
            }
            getPriceResult.errorMessage = result.Message;

            common.Log("بروز مشکل در زمان دریافت قیمت از اوبار", result.Message);
            getPriceResult.canSelect = false;
            return getPriceResult;
        }
        public async Task<string> RegisterUbbarOrder(Order order)
        {
            if (order.OrderNotes.Any(p => p.Note.Contains("SendDataToPost")))
            {
                return "ممکن است اطلاعات مرسوله قبلا ارسال شده باشد";
            }
            InsertOrderNote("SendDataToPost", order.Id);
            bool IsMultishipment = IsMultiShippment(order);
            if (order.BillingAddress.StateProvinceId != null)
            {
                order.BillingAddress.StateProvinceId = null;
                _addressService.UpdateAddress(order.BillingAddress);
            }
            foreach (var item in order.OrderItems)
            {
                int serviceId = item.Product.ProductCategories.First().CategoryId;
                if (serviceId != 701)
                    continue;
                Shipment shipment = null;
                int ShipmentAddressId = 0;
                DateTime dispatch_Date = getDispatchDate(item);
                int _weight = GetItemWeightFromAttr(item);
                double weight = ((double)_weight) / ((double)1000000);
                string content = getOrderItemContent(item);
                int ApproximateVslue = getApproximateValue(item.Id);
                VecileItems truckitems = getVecileItem(item);

                for (int i = 0; i < item.Quantity; i++)
                {
                    if (IsMultishipment)
                    {
                        var multishipment = getShipmentFromMultiShipment(item, i);
                        if (multishipment == null)
                        {
                            InsertOrderNote($"اطلاعات محموله آتیم شماره {item.Id}  ردیف {i} یافت نشد", order.Id);
                            continue;
                        }
                        if (!string.IsNullOrEmpty(multishipment.shipment.TrackingNumber))
                            continue;
                        shipment = multishipment.shipment;
                        ShipmentAddressId = multishipment.ShipmentAddressId;
                    }
                    else
                    {
                        shipment = GetShipment(item, i);
                        if (!string.IsNullOrEmpty(shipment.TrackingNumber))
                            continue;
                        ShipmentAddressId = order.ShippingAddressId.Value;
                    }
                    var address = _addressService.GetAddressById(ShipmentAddressId);
                    address.StateProvinceId = null;
                    _addressService.UpdateAddress(address);
                    string ubaarPriceTrackingNumber = "";
                    var getUbbarPrice = await getUbarPrice(new getPriceInputModel()
                    {
                        Weight = _weight,
                        SenderStateId = truckitems.SenderRegionId,
                        ReaciverStateId = truckitems.ReciverRegionId,
                        Content = content,
                        AproximateValue = ApproximateVslue,
                        dispatch_date = dispatch_Date,
                        UbbarPackingLoad = truckitems.package_options,
                        UbbraTruckType = truckitems.vehicle_type,
                        VechileOptions = truckitems.vehicle_options
                    });
                    if (!getUbbarPrice.canSelect)
                    {
                        InsertOrderNote($"آیتم شماره {item.Id} ردیف {i}" + "->" + getUbbarPrice.errorMessage, order.Id);
                        ChangeOrderState(order, OrderStatus.Processing, getUbbarPrice.errorMessage);
                        return getUbbarPrice.errorMessage;
                    }
                    ubaarPriceTrackingNumber = getUbbarPrice.UbbarTracking_code;
                    var result = await _ubaar_Service.Modifyorder(new Plugin.Misc.ShippingSolutions.Models.Params.Ubbar.Params_Ubaar_modifyorder()
                    {
                        baarnameh_type = "baarnameh",
                        unload_option = "day",
                        destination_region_id = truckitems.ReciverRegionId,
                        dispatch_date = dispatch_Date.Date.ToString("yyyy-MM-dd"),
                        dispatch_hour = dispatch_Date.ToLocalTime().ToString("HH:mm"),
                        suspention_type = "fanari",
                        source_region_id = truckitems.SenderRegionId,
                        weight = weight,
                        load_type = content,
                        load_value = ApproximateVslue,
                        package_options = truckitems.package_options,
                        vehicle_options = truckitems.vehicle_type + ":" + truckitems.vehicle_options,
                        vehicle_type = truckitems.vehicle_type,
                        order_code = ubaarPriceTrackingNumber,
                        channel_submit_type = "web",
                        payment_type = "sender",
                        description = "",
                        destination_address = address.Address1,
                        sender_mobile_phone = order.BillingAddress.PhoneNumber,
                        sender_company = order.BillingAddress.Company,
                        sender_name = ((order.BillingAddress.FirstName ?? "") + " " + (order.BillingAddress.LastName ?? "")),
                        sender_phone = order.BillingAddress.PhoneNumber,
                        receiver_company = address.Company,
                        receiver_mobile_phone = address.PhoneNumber,
                        receiver_name = (address.FirstName ?? "" + " " + address.LastName ?? ""),
                        receiver_phone = address.PhoneNumber,
                        source_address = order.BillingAddress.Address1,
                        announce_type = "ubaar",
                        price = getUbbarPrice.price
                    });
                    if (result.Status)
                    {
                        if (result.DetailUbaarmodifyorder.success_flag == 0)
                        {
                            string error = string.Join(Environment.NewLine, result.DetailUbaarmodifyorder.error_messages);
                            ChangeOrderState(order, OrderStatus.Processing, error);
                            return error;
                        }
                        shipment.TrackingNumber = ubaarPriceTrackingNumber;
                        _shipmentService.UpdateShipment(shipment);
                    }
                    else
                    {
                        string error = $" دلیل انجام نشد این درخواست در آتیم {item.Id} ردیف {i} " + " : " + Environment.NewLine + result.Message;
                        ChangeOrderState(order, OrderStatus.Processing, error);
                        return error;
                    }
                }
            }
            return "";
        }

        private VecileItems getVecileItem(OrderItem item)
        {
            VecileItems vecileItems = new VecileItems();
            vecileItems.package_options = _OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == item.Id && p.PropertyAttrName.Contains("نوع بسته بندی")).FirstOrDefault()?.PropertyAttrValueText;
            vecileItems.SenderRegionId = int.Parse(_OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == item.Id && p.PropertyAttrName.Contains("منطقه فرستنده اوبار")).FirstOrDefault()?.PropertyAttrValueText ?? "");
            vecileItems.ReciverRegionId = int.Parse(_OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == item.Id && p.PropertyAttrName.Contains("منطقه گیرنده اوبار")).FirstOrDefault()?.PropertyAttrValueText ?? "");
            vecileItems.vehicle_type = _OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == item.Id && p.PropertyAttrName.Contains("نوع خودرو")).FirstOrDefault()?.PropertyAttrValueText;
            vecileItems.vehicle_options = _OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == item.Id && p.PropertyAttrName.Contains("ویژگی خودرو")).FirstOrDefault()?.PropertyAttrValueText;
            return vecileItems;
        }

        private DateTime getDispatchDate(OrderItem item)
        {
            return Convert.ToDateTime(_OrderItemAttributeValueRepository.Table
    .Where(p => p.OrderItemId == item.Id && p.PropertyAttrName.Contains("تاریخ و ساعت بارگیری")).FirstOrDefault()?.PropertyAttrValueText);

        }
        #endregion

        #region yarBox
        public YarboxStateModel getYarBoxState(int stateId)
        {
            string Query = $@"SELECT
	                                TYC.YarBoxCountryName,
	                                TYSP.YarboxCityName,
	                                TYSP.YarboxCityId,
	                                TYC.YarboxCountryId
                                FROM
	                                dbo.Tb_Yarbox_StateProvince AS TYSP 
	                                INNER JOIN dbo.Tb_Yarbox_Country AS TYC ON TYC.PostbarCountryId = TYSP.YarBoxStateId 
                                WHERE
	                                TYSP.PostbarStateId = {stateId}";
            return _dbContext.SqlQuery<YarboxStateModel>(Query, new object[0]).FirstOrDefault();
        }
        public int getApTypeName(int Weight)
        {
            string query = $@"SELECT
	                            TOP(1)TYAT.APTYpeNameId
                            FROM
	                            dbo.Tb_Yarbox_ApType AS TYAT
                            WHERE
	                            {Weight} BETWEEN TYAT.MinWeight AND TYAT.MaxWeight";
            return _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
        }
        public async Task<GetPriceResult> getYarboxPrice(getPriceInputModel model)
        {
            GetPriceResult getPriceResult = new GetPriceResult();
            getPriceResult.ServiceId = model.ServiceId;
            var result = await _yarBox_Service.Qute(new Plugin.Misc.ShippingSolutions.Models.Params.YarBox.Params_YarBox_Quote()
            {
                count = 1,
                apPackingTypeId = 11,
                apTypeId = model.ApTypeName,
                cityId = model.ReaciverStateId,
            });

            if (!result.Status)
            {
                if (result.Message.Contains("403"))
                    getPriceResult.errorMessage = "باتوجه به مبدا و مقصد این سرویس ارایه نمی شود";
                else
                    getPriceResult.errorMessage = "در حال حاضر امکان ارائه قیمت در این سرویس وجود ندارد";
                common.Log("بروز مشکل در زمان دریافت قیمت از یارکس", result.Message);
                getPriceResult.canSelect = false;
                return getPriceResult;
            }

            getPriceResult.price = Convert.ToInt32(result.Detail_Result_Yarbox_Qute.price);
            getPriceResult.canSelect = true;
            return getPriceResult;
        }
        public async Task<string> registerYarboxOrder(Order order)
        {

            if (order.OrderNotes.Any(p => p.Note.Contains("SendDataToPost")))
            {
                return "ممکن است اطلاعات مرسوله قبلا ارسال شده باشد";
            }
            if (!new int[] { 4, 579, 580, 581, 582, 583, 584, 585 }.Contains(order.BillingAddress.StateProvinceId.Value))
            {
                string error = "ارائه این سرویس فقط از مبدا تهران امکان پذیر است";
                InsertOrderNote(error, order.Id);
                return error;
            }
            if (!CheckHasValidPrice(order))
                return "قیمت سرویس مورد نظر شما به درستی محاسبه نشده با واحد پشتیبانی تماس بگیرید";
            InsertOrderNote("SendDataToPost", order.Id);
            var YarboxSernder = getYarBoxState(order.BillingAddress.StateProvinceId.Value);

            bool IsMultishipment = IsMultiShippment(order);
            foreach (var item in order.OrderItems)
            {
                if (order.OrderItems.Any(p => p.Product.ProductCategories.Any(n => n.CategoryId != 702)))
                {
                    InsertOrderNote($"آیتم شماره {0} مربوط به سرویس یارباکس نمی باشد", order.Id);
                    continue;
                }
                int ApproximateValue = getApproximateValue(item.Id);
                int weight = GetItemWeightFromAttr(item);
                var Dimensions = getDimensions(item);
                var apPackingTypeId = 11;//=====================================??? درخواست کارتون برای یارباکس ارسال نمی شود
                for (int i = 0; i < item.Quantity; i++)
                {
                    Shipment shipment = null;
                    int ShipmentAddressId = 0;
                    if (IsMultishipment)
                    {
                        var multishipment = getShipmentFromMultiShipment(item, i);
                        if (multishipment == null)
                        {
                            InsertOrderNote($"اطلاعات محموله آتیم شماره {item.Id}  ردیف {i} یافت نشد", order.Id);
                            continue;
                        }
                        if (!string.IsNullOrEmpty(multishipment.shipment.TrackingNumber))
                            continue;
                        shipment = multishipment.shipment;
                        ShipmentAddressId = multishipment.ShipmentAddressId;
                    }
                    else
                    {
                        shipment = GetShipment(item, i);
                        if (!string.IsNullOrEmpty(shipment.TrackingNumber))
                            continue;
                        ShipmentAddressId = order.ShippingAddressId.Value;
                    }
                    var shipmentAddress = _addressService.GetAddressById(ShipmentAddressId);
                    var YarboxReciver = getYarBoxState(shipmentAddress.StateProvinceId.Value);
                    var result = await _yarBox_Service.AddPostPacks(new Plugin.Misc.ShippingSolutions.Models.Params.YarBox.Params_YarBox_AddPostPacks()
                    {
                        apPackingTypeId = apPackingTypeId,
                        apTypeId = getApTypeName(weight),
                        count = 1,
                        destinationAddress = shipmentAddress.Address1,
                        destination_City = YarboxReciver.YarboxCityName,
                        destination_State = YarboxReciver.YarBoxCountryName,
                        receiverPhone = shipmentAddress.PhoneNumber,
                        latitude = "0",
                        longitude = "0",
                        originAddress = order.BillingAddress.Address1,
                        origin_City = YarboxSernder.YarboxCityName,
                        origin_State = YarboxSernder.YarBoxCountryName,
                        senderPhone = order.BillingAddress.PhoneNumber,
                        receiveType = 0,
                        insurance = 0
                    });
                    if (result.Status)
                    {
                        string key = result.KEY.Key.ToString();
                        var factorResult = await _yarBox_Service.Factor(new Plugin.Misc.ShippingSolutions.Models.Params.YarBox.Params_YarBox_Factor()
                        {
                            Key = key
                        });
                        if (factorResult.Status)
                        {
                            shipment.TrackingNumber = factorResult.detailFactor.id.ToString();
                            _shipmentService.UpdateShipment(shipment);

                            var accesptResult = await _yarBox_Service.accept(new Plugin.Misc.ShippingSolutions.Models.Params.YarBox.Params_YarBox_accept
                            {
                                Id = factorResult.detailFactor.id
                            });
                            if (!accesptResult.Status)
                            {
                                string error = $" دلیل انجام نشد این درخواست در آتیم {item.Id} ردیف {i} " + " : " + accesptResult.Message;
                                ChangeOrderState(order, OrderStatus.Processing, error);
                                return error;
                            }
                        }
                    }
                    else
                    {
                        string error = $" دلیل انجام نشد این درخواست در آتیم {item.Id} ردیف {i} " + " : " + result.Message;
                        ChangeOrderState(order, OrderStatus.Processing, error);
                        return error;
                    }
                }
            }
            if (order.Shipments.All(p => !string.IsNullOrEmpty(p.TrackingNumber)))
            {
                order.OrderStatus = OrderStatus.Complete;
                _orderService.UpdateOrder(order);
                SavePostCoordination(order.Id, "ارجاع به نماینده پس از ثبت درخواست");
            }
            return "";
        }
        #endregion

        #region TPG
        public TPGStateModel getTpgState(int stateId)
        {
            string query = $@"SELECT
	                             TTS.TpgCityId TPGStateId
	                            , TTS.TpgCityName TPGStateName
                            FROM
	                            dbo.Tb_Tpg_States AS TTS
                            WHERE
	                            TTS.PostbarStateId = {stateId}";
            return _dbContext.SqlQuery<TPGStateModel>(query, new object[0]).FirstOrDefault();
        }
        public async Task<GetPriceResult> getTPGPrice(getPriceInputModel model, bool minPrice = false)
        {
            GetPriceResult getPriceResult = new GetPriceResult();
            getPriceResult.ServiceId = model.ServiceId;
            var TPGResult = await _tPG_Service.Compute(new Plugin.Misc.ShippingSolutions.Models.Params.TPG.Params_TPG_Compute()
            {
                weight = model.Weight.ToString(),
                Width = model.Width,
                Height = model.Height,
                Length = model.Length,
                hasInsurance = false,
                source = model.SenderStateId.ToString(),
                destination = model.ReaciverStateId.ToString(),
                name = "مرسوله امنیتو",
                service = (model.ServiceId == 711 ? 7 : 6),
                isPas = (model.ServiceId == 711 ? true : false)
            });
            if (!TPGResult.Status)
            {
                getPriceResult.errorMessage = TPGResult.Message;
                common.Log("بروز مشکل در زمان دریافت قیمت از تی پی جی", TPGResult.Message);
                getPriceResult.canSelect = false;
                return getPriceResult;
            }
            if (!TPGResult.Deatil_Result_TGP_Compute.Coverage)
            {
                getPriceResult.errorMessage = "با توجه به مبدا و قصد این سرویس ارایه نمی شود";
                common.Log("بروز مشکل در زمان دریافت قیمت از تی پی جی", getPriceResult.errorMessage);
                getPriceResult.canSelect = false;
                return getPriceResult;
            }
            if (minPrice)
                getPriceResult.price = Convert.ToInt32(TPGResult.Deatil_Result_TGP_Compute.Total);
            else
                getPriceResult.price = Convert.ToInt32(TPGResult.Deatil_Result_TGP_Compute.Price + TPGResult.Deatil_Result_TGP_Compute.ExtraPrice);
            getPriceResult.canSelect = true;
            return getPriceResult;
        }
        public async Task<string> registerTPGOrder(Order order)
        {
            if (order.OrderNotes.Any(p => p.Note.Contains("SendDataToPost")))
            {
                return "ممکن است اطلاعات مرسوله قبلا ارسال شده باشد";

            }
            if (!CheckHasValidPrice(order))
                return "قیمت سرویس مورد نظر شما به درستی محاسبه نشده با واحد پشتیبانی تماس بگیرید";
            InsertOrderNote("SendDataToPost", order.Id);
            var TPGSernder = getTpgState(order.BillingAddress.StateProvinceId.Value);
            if (TPGSernder == null)
            {
                return "این سرویس در شهر مبدا ارائه نمی شود";
            }
            bool IsMultishipment = IsMultiShippment(order);
            foreach (var item in order.OrderItems)
            {
                int serviceId = item.Product.ProductCategories.First().CategoryId;
                if (!new int[] { 709, 711 }.Contains(serviceId))
                {
                    InsertOrderNote($"آیتم شماره {0} مربوط به سرویس تی پی جی نمی باشد", order.Id);
                    continue;
                }
                int ApproximateValue = getApproximateValue(item.Id);
                int weight = GetItemWeightFromAttr(item);
                var Dimensions = getDimensions(item);
                for (int i = 0; i < item.Quantity; i++)
                {
                    Shipment shipment = null;
                    int ShipmentAddressId = 0;
                    if (IsMultishipment)
                    {
                        var multishipment = getShipmentFromMultiShipment(item, i);
                        if (multishipment == null)
                        {
                            InsertOrderNote($"اطلاعات محموله آتیم شماره {item.Id}  ردیف {i} یافت نشد", order.Id);
                            continue;
                        }
                        if (!string.IsNullOrEmpty(multishipment.shipment.TrackingNumber))
                            continue;
                        shipment = multishipment.shipment;
                        ShipmentAddressId = multishipment.ShipmentAddressId;
                    }
                    else
                    {
                        shipment = GetShipment(item, i);
                        if (!string.IsNullOrEmpty(shipment.TrackingNumber))
                            continue;
                        ShipmentAddressId = order.ShippingAddressId.Value;
                    }
                    var shipmentAddress = _addressService.GetAddressById(ShipmentAddressId);
                    var TPGReciver = getTpgState(shipmentAddress.StateProvinceId.Value);
                    var result = await _tPG_Service.Pickup(new Plugin.Misc.ShippingSolutions.Models.Params.TPG.Params_TPG_Pickup()
                    {
                        Desc = "",
                        Lat = 0,
                        Longt = 0,
                        Name = order.BillingAddress.FirstName,
                        Family = order.BillingAddress.LastName,
                        CompanyName = order.BillingAddress.Company,
                        PostalCode = shipmentAddress.ZipPostalCode,
                        //SubregionId = TPGReciver.TPGStateId,
                        Address = order.BillingAddress.Address1 + "-" + order.BillingAddress.PhoneNumber,
                        Weight = weight,
                        DstId = TPGReciver.TPGStateId,
                        SrcId = TPGSernder.TPGStateId,
                        DstAddress = shipmentAddress.Address1 + "-" + shipmentAddress.PhoneNumber,
                        SenderName = (shipmentAddress.FirstName ?? "" + " " + shipmentAddress.LastName ?? ""),
                        Width = Dimensions.Item2,
                        Height = Dimensions.Item3,
                        Length = Dimensions.Item1,
                        ServiceId = (serviceId == 711 ? 7 : 6),
                        CODCost = (serviceId == 711 ? calcCodPirce(item) : 0),
                        IsPostPaid = (serviceId == 711 ? true : false)
                    });
                    if (result.Status)
                    {
                        shipment.TrackingNumber = result.Deatil_TPG_Pickup.CN.ToString();
                        _shipmentService.UpdateShipment(shipment);
                        InsertOrderNote("کد درخواست:" + (result.RequestId ?? ""), order.Id);
                        return "";
                    }
                    else
                    {
                        string error = $" دلیل انجام نشد این درخواست در آتیم {item.Id} ردیف {i} " + " : " + result.Message +
                             Environment.NewLine + "کد درخواست:" + (result.RequestId ?? "");
                        ChangeOrderState(order, OrderStatus.Processing, error);
                        return error;
                    }
                }
            }
            return "";
        }
        #endregion

        #region peykhub
        public async Task<GetPriceResult> peykhubGetPrice(getPriceInputModel model)
        {
            GetPriceResult getPriceResult = new GetPriceResult();
            getPriceResult.ServiceId = model.ServiceId;
            try
            {
                int senderStateId = model.SenderStateId == 0 ? model.SenderAddress.StateProvinceId.Value : model.SenderStateId;
                if (model.Length == 0 || model.Width == 0 || model.Height == 0)
                {
                    common.Log("", "سایز بسته جهت استعلام قیمت مشخص نیست");
                    getPriceResult.errorMessage = "سایز بسته حهت استعلام قیمت مشخص نیست";
                    getPriceResult.canSelect = false;
                    getPriceResult.price = 0;
                    return getPriceResult;
                }
                int countryId = model.SenderAddress.CountryId.Value;
                if (!HasAgentInCity(senderStateId))
                {
                    getPriceResult.errorMessage = "برای سرویس امنیتو در مبدا مورد نظر شما نماینده فعالی یافت نشد";
                    getPriceResult.canSelect = false;
                    getPriceResult.price = 0;
                    return getPriceResult;
                }
                bool IsAgent = _workContext.CurrentCustomer.IsInCustomerRole("mini-Administrators");
                int Price = 0;
                int volume = model.Length * model.Width * model.Height;
                SqlParameter P_Price = new SqlParameter() { ParameterName = "Price", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
                SqlParameter[] prms = new SqlParameter[] {
                    new SqlParameter() { ParameterName = "Volume", SqlDbType = SqlDbType.Int, Value = volume },
                    new SqlParameter() { ParameterName = "ReciverStateId", SqlDbType = SqlDbType.Int, Value = senderStateId},
                    P_Price
                };
                string query = "EXEC dbo.Sp_GetDistribtionPrice @Volume,@ReciverStateId ,@Price OUTPUT";

                var data = _dbContext.SqlQuery<int?>(query, prms).FirstOrDefault().GetValueOrDefault(0);
                if ((int)P_Price.Value == 0)
                {
                    getPriceResult.errorMessage = "اطلاعاتی جهت استعلام قیمت یافت نشد";
                    getPriceResult.canSelect = false;
                    getPriceResult.price = 0;
                    return getPriceResult;
                }
                Price = (int)P_Price.Value;
                getPriceResult.errorMessage = "";
                getPriceResult.price = Price + (Price * 9 / 100);
                getPriceResult.canSelect = true;
                getPriceResult.SLA = "تا 24 ساعت";
                return getPriceResult;
            }
            catch (Exception ex)
            {
                LogException(ex);
                getPriceResult.errorMessage = "خطا در زمان استعلام قیمت امنیتو";
                getPriceResult.canSelect = false;
                getPriceResult.price = 0;
                return getPriceResult;
            }
        }
        public async Task<string> Registerpeykhub_Order(Order order)
        {
            if (order.OrderNotes.Any(p => p.Note.Contains("SendDataToPost")))
            {
                return "ممکن است اطلاعات مرسوله قبلا ارسال شده باشد";
            }
            if (!CheckHasValidPrice(order))
                return "قیمت سرویس مورد نظر شما به درستی محاسبه نشده با واحد پشتیبانی تماس بگیرید";
            InsertOrderNote("SendDataToPost", order.Id);
            bool IsMultishipment = IsMultiShippment(order);
            foreach (var item in order.OrderItems)
            {
                int serviceId = 0;
                var cat = item.Product.ProductCategories.Where(p => p.Category.ParentCategoryId != 0).FirstOrDefault();
                decimal weight = (decimal)GetItemWeightFromAttr(item) / (decimal)1000;
                int kalaPrice = getGoodsPrice(item);
                if (cat == null)
                {
                    InsertOrderNote($"سرویس مربوط به آیتم {item.Id} از این سفارش یافت نشد", order.Id);
                    continue;
                }
                if (!new int[] { 718 }.Contains(cat.CategoryId))
                {
                    InsertOrderNote($"آیتم شماره {0} مربوط به سرویس امنیتو نمی باشد", order.Id);
                    continue;
                }
                serviceId = cat.CategoryId;
                var dimension = getDimensions(item);
                for (int i = 0; i < item.Quantity; i++)
                {
                    Shipment shipment = null;
                    int ShipmentAddressId = 0;

                    if (IsMultishipment)
                    {
                        var multishipment = getShipmentFromMultiShipment(item, i);
                        if (multishipment == null)
                        {
                            InsertOrderNote($"اطلاعات محموله آتیم شماره {item.Id}  ردیف {i} یافت نشد", order.Id);
                            continue;
                        }
                        if (!string.IsNullOrEmpty(multishipment.shipment.TrackingNumber))
                            continue;
                        shipment = multishipment.shipment;
                        ShipmentAddressId = multishipment.ShipmentAddressId;
                    }
                    else
                    {
                        shipment = GetShipment(item, i);
                        if (!string.IsNullOrEmpty(shipment.TrackingNumber))
                            continue;
                        ShipmentAddressId = order.ShippingAddressId.Value;
                    }
                    var address = _addressService.GetAddressById(ShipmentAddressId);

                    shipment.TrackingNumber = shipment.Id.ToString() + "1401";
                    _shipmentService.UpdateShipment(shipment);
                }
            }
            if (order.Shipments.All(p => !string.IsNullOrEmpty(p.TrackingNumber)))
            {
                order.OrderStatus = OrderStatus.Complete;
                _orderService.UpdateOrder(order);
                SavePostCoordination(order.Id, "ارجاع به پست گیت وی");
            }
            return "";
        }

        #endregion

        #region Snapbox
        public async Task<GetPriceResult> getsnappboxPrice(getPriceInputModel model)
        {
            GetPriceResult getPriceResult = new GetPriceResult();
            getPriceResult.ServiceId = model.ServiceId;
            string snapCityName = "";
            string stateName = model.SenderAddress.StateProvinceName;
            if (stateName == "اصفهان")
                snapCityName = "isfahan";
            else if (stateName == "تهران")
                snapCityName = "tehran";
            else if (stateName == "شیراز")
                snapCityName = "shiraz";
            else if (stateName == "قم")
                snapCityName = "qom";
            else if (stateName == "مشهد")
                snapCityName = "mashhad";
            var snapPrice = await _snappbox_Service.GetPrice(new Plugin.Misc.ShippingSolutions.Models.Params.Snappbox.Params_Snappbox_Get_Price()
            {
                city = stateName,
                customerWalletType = null,
                deliveryCategory = "bike-without-box",
                deliveryFarePaymentType = "prepaid",//--------> این مورد دارد باید اصلاح شود,
                id = null,
                isReturn = false,
                pricingId = null,
                sequenceNumberDeliveryCollection = 1,
                waitingTime = 0,
                voucherCode = null,
                totalFare = null,
                //customerId= ===> در زمان درخواست مقدار دهی میشود
                terminals = new List<Plugin.Misc.ShippingSolutions.Models.Params.Snappbox.Terminal>() {
                    new Plugin.Misc.ShippingSolutions.Models.Params.Snappbox.Terminal(){
                        id=null,
                        contactName=(model.SenderAddress.FirstName??"")+" "+(model.SenderAddress.LastName??""),
                        address=model.SenderAddress.Address1,
                        contactPhoneNumber=model.SenderAddress.PhoneNumber,
                        plate="",
                        sequenceNumber=1,
                        unit="",
                        comment="",
                        latitude=model.SenderAddress.Lat.Value,
                        longitude=model.SenderAddress.Lon.Value,
                        type="pickup",
                        collectCash="no",
                        paymentType="prepaid",
                        cashOnPickup=0,
                        cashOnDelivery=0,
                        isHub=null,
                        vendorId=null
                    },
                    new Plugin.Misc.ShippingSolutions.Models.Params.Snappbox.Terminal(){
                        id=null,
                        contactName=(model.ReciverAddress.FirstName??"")+" "+(model.ReciverAddress.LastName??""),
                        address = model.ReciverAddress.Address1,
                        contactPhoneNumber = model.ReciverAddress.PhoneNumber,
                        plate="",
                        sequenceNumber=1,
                        unit="",
                        comment="",
                        latitude= model.ReciverAddress.Lat.Value,
                        longitude=model.ReciverAddress.Lon.Value,
                        type="drop",
                        collectCash="no",
                        paymentType="prepaid",
                        cashOnPickup=0,
                        cashOnDelivery=0,
                        isHub=null,
                        vendorId=null
                    }
                }
            });
            if (!snapPrice.Status)
            {
                getPriceResult.errorMessage = snapPrice.Message;
                common.Log("بروز مشکل در زمان دریافت قیمت از اسنپ باکس", snapPrice.Message);
                getPriceResult.canSelect = false;
                return getPriceResult;
            }
            getPriceResult.canSelect = true;
            getPriceResult.price = snapPrice.Detail_Result_Snappbox_GetPrice.totalFare;
            return getPriceResult;
        }
        public async Task<string> RegisterSnappbox_Order(Order order)
        {
            if (order.OrderNotes.Any(p => p.Note.Contains("SendDataToPost")))
            {
                return "ممکن است اطلاعات مرسوله قبلا ارسال شده باشد";
            }
            if (!CheckHasValidPrice(order))
                return "قیمت سرویس مورد نظر شما به درستی محاسبه نشده با واحد پشتیبانی تماس بگیرید";
            InsertOrderNote("SendDataToPost", order.Id);

            bool IsMultishipment = IsMultiShippment(order);
            foreach (var item in order.OrderItems)
            {
                int ServiceId = item.Product.ProductCategories.First().CategoryId;
                if (ServiceId != 717)
                {
                    InsertOrderNote($"آیتم شماره {0} مربوط به سرویس اسنپ باکس نمی باشد", order.Id);
                    continue;
                }
                int ApproximateValue = getApproximateValue(item.Id);
                int weight = GetItemWeightFromAttr(item);
                var Dimensions = getDimensions(item);

                for (int i = 0; i < item.Quantity; i++)
                {
                    Shipment shipment = null;
                    int ShipmentAddressId = 0;
                    if (IsMultishipment)
                    {
                        var multishipment = getShipmentFromMultiShipment(item, i);
                        if (multishipment == null)
                        {
                            InsertOrderNote($"اطلاعات محموله آتیم شماره {item.Id}  ردیف {i} یافت نشد", order.Id);
                            continue;
                        }
                        if (!string.IsNullOrEmpty(multishipment.shipment.TrackingNumber))
                            continue;
                        shipment = multishipment.shipment;
                        ShipmentAddressId = multishipment.ShipmentAddressId;
                    }
                    else
                    {
                        shipment = GetShipment(item, i);
                        if (!string.IsNullOrEmpty(shipment.TrackingNumber))
                            continue;
                        ShipmentAddressId = order.ShippingAddressId.Value;
                    }
                    var shipmentAddress = _addressService.GetAddressById(ShipmentAddressId);
                    var address = _addressService.GetAddressById(ShipmentAddressId);
                    //var SenderPdeStateId = getPDEState(order.BillingAddress.StateProvinceId.Value);
                    //var ReciverPdeStateId = getPDEState(shipmentAddress.StateProvinceId.Value);

                    #region ckwck wight
                    if (weight > 25000)
                    {
                        string error = $"  درخواست به دلیل معتبرنبودن وزن انجام نشد";
                        ChangeOrderState(order, OrderStatus.Processing, error);
                        return error;
                    }
                    #endregion

                    #region ثبت سفارش انپ باکس
                    Plugin.Misc.ShippingSolutions.Models.Params.Snappbox.Params_Snappbox_create_order param = new Plugin.Misc.ShippingSolutions.Models.Params.Snappbox.Params_Snappbox_create_order();
                    string snapCityName = "";
                    if (shipmentAddress.Country.Name == "اصفهان")
                        snapCityName = "isfahan";
                    else if (shipmentAddress.Country.Name == "تهران")
                        snapCityName = "tehran";
                    else if (shipmentAddress.Country.Name == "شیراز")
                        snapCityName = "shiraz";
                    else if (shipmentAddress.Country.Name == "قم")
                        snapCityName = "qom";
                    else if (shipmentAddress.Country.Name == "مشهد")
                        snapCityName = "mashhad";
                    param.data.orderDetails.packageSize = ((double)(weight / 1000));
                    param.data.orderDetails.city = snapCityName;//shipmentAddress.Country.Name;
                    param.data.orderDetails.deliveryCategory = "bike-without-box";
                    param.data.orderDetails.deliveryFarePaymentType = "prepaid";
                    param.data.orderDetails.isReturn = false;
                    param.data.orderDetails.pricingId = "";
                    param.data.orderDetails.sequenceNumberDeliveryCollection = 1;
                    param.data.orderDetails.totalFare = 0;
                    param.data.orderDetails.customerRefId = order.Id.ToString();
                    param.data.orderDetails.voucherCode = null;
                    param.data.orderDetails.waitingTime = 0;



                    Plugin.Misc.ShippingSolutions.Models.Params.Snappbox.ItemDetail id = new Plugin.Misc.ShippingSolutions.Models.Params.Snappbox.ItemDetail();
                    id.pickedUpSequenceNumber = 1;
                    id.dropOffSequenceNumber = 2;
                    id.name = getOrderItemContent(item);
                    id.quantity = 1;
                    id.quantityMeasuringUnit = "unit";
                    id.packageValue = ApproximateValue;
                    id.createdAt = "";
                    id.updatedAt = "";
                    param.data.itemDetails.Add(id);



                    Plugin.Misc.ShippingSolutions.Models.Params.Snappbox.PickUpDetail pd = new Plugin.Misc.ShippingSolutions.Models.Params.Snappbox.PickUpDetail();
                    var lallongsender = GetAddressLocation(order.BillingAddress.Id);
                    #region sender
                    pd.id = null;
                    pd.contactName = shipment.Order.BillingAddress.FirstName ?? "" + " " + shipment.Order.BillingAddress.LastName ?? "";
                    pd.address = shipment.Order.BillingAddress.Address1;
                    pd.contactPhoneNumber = shipment.Order.BillingAddress.PhoneNumber;
                    pd.plate = "";
                    pd.sequenceNumber = 1;
                    pd.unit = "";
                    pd.comment = "";
                    pd.latitude = lallongsender != null ? lallongsender.Lat : 0;
                    pd.longitude = lallongsender != null ? lallongsender.Long : 0;
                    pd.type = "pickup";
                    pd.collectCash = "no";
                    pd.paymentType = "prepaid";
                    pd.cashOnPickup = 0;
                    pd.cashOnDelivery = 0;
                    pd.isHub = null;
                    pd.vendorId = null;

                    param.data.pickUpDetails.Add(pd);
                    #endregion

                    Plugin.Misc.ShippingSolutions.Models.Params.Snappbox.DropOffDetail dd = new Plugin.Misc.ShippingSolutions.Models.Params.Snappbox.DropOffDetail();
                    var lallongreciver = GetAddressLocation(address.Id);
                    #region reciver
                    dd.id = null;
                    dd.contactName = (address.FirstName ?? "") + (" " + address.LastName ?? "");
                    dd.address = address.Address1;
                    dd.contactPhoneNumber = address.PhoneNumber;
                    dd.plate = "";
                    dd.sequenceNumber = 2;
                    dd.unit = "";
                    dd.comment = "";
                    dd.latitude = lallongreciver != null ? lallongreciver.Lat : 0;
                    dd.longitude = lallongreciver != null ? lallongreciver.Long : 0;
                    dd.type = "drop";
                    dd.collectCash = "no";
                    dd.paymentType = "prepaid";
                    dd.cashOnPickup = 0;
                    dd.cashOnDelivery = 0;
                    dd.isHub = null;
                    dd.vendorId = null;
                    param.data.dropOffDetails.Add(dd);
                    #endregion
                    #endregion
                    var snapPrice = await _snappbox_Service.CreateOrder(param);
                    if (snapPrice.Status)
                    {
                        shipment.TrackingNumber = snapPrice.DetailResult_Snappbox_CreateOrder.data.orderId.ToString();
                        _shipmentService.UpdateShipment(shipment);
                    }
                    else
                    {
                        string error = $" دلیل انجام نشد این درخواست در آتیم {item.Id} ردیف {i} " + " : " + snapPrice.Message;
                        ChangeOrderState(order, OrderStatus.Processing, error);
                        return error;
                    }
                }
            }
            return "";
        }
        public bool CancelSappbox_Order(Order order, out string strError)
        {
            var SnappOrderId = _genericAttributeService.GetAttributesForEntity(order.Id, "Order").FirstOrDefault(p => p.Key == "SnappboxOrderId" && p.StoreId == order.StoreId)?.Value;
            if (string.IsNullOrEmpty(SnappOrderId))
            {
                strError = "شماره سفارش اسنپ جهت کنسل کردن یافت نشد";
                return false;
            }
            var reuslt = _snappbox_Service.CancelOrder(new Plugin.Misc.ShippingSolutions.Models.Params.Snappbox.Params_Snappbox_Cancel_Order()
            {
                orderId = SnappOrderId
            });
            strError = reuslt.DetailResult_Snappbox_Cancel_Order.message;
            return reuslt.Status;
        }
        #endregion

        #region InCityCollecting

        public async Task<GetPriceResult> getCollectingPrice(getPriceInputModel model)
        {
            return null;
            GetPriceResult getPriceResult = new GetPriceResult();
            getPriceResult.ServiceId = model.ServiceId;
            try
            {


            }
            catch (Exception ex)
            {
            }
        }
        public async Task<string> RegisterCollecting(Order order)
        {
            return "";
        }
        #endregion

        #region Kalaresan
        public async Task<GetPriceResult> getKalaresanPrice(getPriceInputModel model)
        {
            GetPriceResult getPriceResult = new GetPriceResult();
            try
            {
                int senderStateId = model.SenderStateId == 0 ? model.SenderAddress.StateProvinceId.Value : model.SenderStateId;
                int receiverStateId = model.ReaciverStateId == 0 ? model.ReciverAddress.StateProvinceId.Value : model.ReaciverStateId;
                //if (!HasAgentInCity(senderStateId))
                //{
                //    getPriceResult.errorMessage = "برای سرویس کالارسان در مبدا مورد نظر شما نماینده فعالی یافت نشد";
                //    getPriceResult.canSelect = false;
                //    getPriceResult.price = 0;
                //    return getPriceResult;
                //}
                //if (!HasAgentInCity(receiverStateId))
                //{
                //    getPriceResult.errorMessage = "برای سرویس کالارسان در مقصد مورد نظر شما نماینده فعالی یافت نشد";
                //    getPriceResult.canSelect = false;
                //    getPriceResult.price = 0;
                //    return getPriceResult;
                //}
                if (model.Length == 0 || model.Width == 0 || model.Height == 0)
                {
                    getPriceResult.canSelect = false;
                    getPriceResult.errorMessage = "ابعاد وارد شده نامعتبر می باشد";
                    getPriceResult.price = 0;
                    getPriceResult.ServiceId = model.ServiceId;
                    return getPriceResult;
                }
                GetPriceInputModel _kalaresanInputmodel = new GetPriceInputModel();
                int size = model.Length * model.Width * model.Height;
                _kalaresanInputmodel.originCity = senderStateId;
                _kalaresanInputmodel.destinationCity = receiverStateId;
                _kalaresanInputmodel.packetsDetail = new List<GetPricepacketsDetail>() {
                new GetPricepacketsDetail(){count = 1,size= size}
            };
                var _getPriceresult = await _kalaResan_Service.GetPrice(_kalaresanInputmodel);
                if (_getPriceresult.shipment_cost == 0)
                {
                    common.Log("بروز مشکل در زمان دریافت قیمت کالارسان", _getPriceresult.message);
                    getPriceResult.canSelect = false;
                    getPriceResult.errorMessage = "خطا در زمان استعلام قیمت کالارسان" + " ==> " + _getPriceresult.message;
                    getPriceResult.price = 0;
                    getPriceResult.ServiceId = model.ServiceId;
                    return getPriceResult;
                }
                getPriceResult.ServiceId = model.ServiceId;
                getPriceResult.price = _getPriceresult.shipment_cost * 10;
                //if(!model.OrderItemId.HasValue)
                //{
                //    GetdistributionValue
                //}
                getPriceResult.canSelect = true;
                return getPriceResult;
            }
            catch (Exception ex)
            {
                LogException(ex);
                getPriceResult.errorMessage = "خطا در زمان استعلام قیمت کالا رسان";
                getPriceResult.canSelect = false;
                getPriceResult.price = 0;
                return getPriceResult;
            }

        }
        public async Task<string> RegisterKalaresan(Order order)
        {
            if (order.OrderNotes.Any(p => p.Note.Contains("SendDataToPost")))
            {
                return "ممکن است اطلاعات مرسوله قبلا ارسال شده باشد";
            }
            //if (!HasAgentInCity(order.BillingAddress.StateProvinceId.Value))
            //{
            //    return "برای سرویس کالارسان در مبدا مورد نظر شما نماینده فعالی یافت نشد";
            //}

            if (!CheckHasValidPrice(order))
                return "قیمت سرویس مورد نظر شما به درستی محاسبه نشده با واحد پشتیبانی تماس بگیرید";

            InsertOrderNote("SendDataToPost", order.Id);
            foreach (var item in order.OrderItems)
            {
                int serviceId = 0;
                var cat = item.Product.ProductCategories.Where(p => p.Category.ParentCategoryId != 0).FirstOrDefault();
                decimal weight = (decimal)GetItemWeightFromAttr(item) / (decimal)1000;
                int kalaPrice = getGoodsPrice(item);
                int CodValue = 0;
                if (cat == null)
                {
                    InsertOrderNote($"سرویس مربوط به آیتم {item.Id} از این سفارش یافت نشد", order.Id);
                    continue;
                }
                if (!new int[] { 733 }.Contains(cat.CategoryId))
                {
                    InsertOrderNote($"آیتم شماره {0} مربوط به سرویس کالارسان نمی باشد", order.Id);
                    continue;
                }
                serviceId = cat.CategoryId;

                var dimension = getDimensions(item);
                int _size = dimension.Item1 * dimension.Item2 * dimension.Item3;
                string _Content = getOrderItemContent(item);
                for (int i = 0; i < item.Quantity; i++)
                {
                    Shipment shipment = null;
                    int ShipmentAddressId = 0;
                    Nop.Plugin.Misc.ShippingSolutions.Models.Params.Chapar.ReceiverBulkImport reciver = null;

                    var multishipment = getShipmentFromMultiShipment(item, i);
                    if (multishipment == null)
                    {
                        InsertOrderNote($"اطلاعات محموله آتیم شماره {item.Id}  ردیف {i} یافت نشد", order.Id);
                        continue;
                    }
                    if (!string.IsNullOrEmpty(multishipment.shipment.TrackingNumber))
                        continue;
                    shipment = multishipment.shipment;
                    ShipmentAddressId = multishipment.ShipmentAddressId;
                    var address = _addressService.GetAddressById(ShipmentAddressId);
                    //if (!HasAgentInCity(address.StateProvinceId.Value))
                    //{
                    //    return "برای سرویس کالارسان در مبدا مورد نظر شما نماینده فعالی یافت نشد";
                    //}
                    var InputModel = new RegisterParcelInputModel()
                    {
                        destinationCity = address.StateProvinceId.Value,
                        originCity = order.BillingAddress.StateProvinceId.Value,
                        postexShipmentCode = shipment.Id.ToString(),
                        receiverAddr = address.Address1,
                        receiverName = "امنیتو" + "--" + address.FirstName + " " + address.LastName,
                        receiverPhone = address.PhoneNumber,
                        senderAddr = order.BillingAddress.Address1,
                        senderName = order.BillingAddress.FirstName + " " + order.BillingAddress.LastName,
                        senderPhone = order.BillingAddress.PhoneNumber,
                        packetsDetail = new List<PacketsDetail>() {
                            new PacketsDetail(){count = 1, desc=_Content,size = _size }
                        }
                    };

                    var _result = await _kalaResan_Service.RegisterShipment(InputModel);

                    if (_result == null)
                    {
                        ChangeOrderState(order, OrderStatus.Processing, $" دلیل انجام نشدن این درخواست در آتیم {item.Id} ردیف {i} ");
                        return "در زمان ثبت سفارش در سرویس مورد نظر مشکلی پیش آمده با واحد پشتیبانی تماس بگیرید";
                    }
                    if (string.IsNullOrEmpty(_result.shipment_code) || _result.error_code > 0)
                    {
                        ChangeOrderState(order, OrderStatus.Processing, $" دلیل انجام نشدن این درخواست در آتیم {item.Id} ردیف {i} " + " : " + _result.message);
                        return "در زمان ثبت سفارش در سرویس مورد نظر مشکلی پیش آمده با واحد پشتیبانی تماس بگیرید";
                    }
                    if (!string.IsNullOrEmpty(_result.shipment_code))
                    {
                        shipment.TrackingNumber = _result.shipment_code;
                        _shipmentService.UpdateShipment(shipment);

                    }
                }
            }
            if (order.Shipments.All(p => !string.IsNullOrEmpty(p.TrackingNumber)))
            {
                order.OrderStatus = OrderStatus.Complete;
                _orderService.UpdateOrder(order);
                SavePostCoordination(order.Id, "ارجاع به پست کالا رسان");
            }
            return "";
        }
        #endregion

        public bool HasAgentInCity(int StateId)
        {
            string query = $@"SELECT distinct
	                                    TOP(1) c.Id
                                    FROM
	                                    dbo.Customer AS C
	                                    INNER JOIN dbo.Customer_CustomerRole_Mapping AS CCRM ON CCRM.Customer_Id = C.Id
	                                    INNER JOIN dbo.Tb_UserStates AS TUS ON TUS.CustomerId = C.Id
	                                    INNER JOIN dbo.StateProvince AS SP ON SP.Id = TUS.StateId
                                    WHERE
	                                    C.Active =1
	                                    AND CCRM.CustomerRole_Id = 7
	                                    AND sp.Id in({string.Join(",", StateId)})";
            int CustomerId = _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
            return CustomerId > 0;
        }
        public getPriceInputModel PriceInpuModelFactory(int senderCountry
            , int senderState
            , int receiverCountry
            , int receiverState
            , int weightItem
            , int AproximateValue
            , out string error
            , out bool canSelect
            , int height = 0
            , int length = 0
            , int width = 0
            , byte consType = 0
            , string content = ""
            , DateTime? dispach_date = null
            , string PackingOption = ""
            , string vechileType = ""
            , string VechileOption = ""
            , int receiver_ForeginCountry = 0
            , int ServiceId = 0
            , CustomAddressModel SenderAddress = null
            , CustomAddressModel ReciverAddress = null
            , string receiver_ForeginCountryNameEn = null
            , int quantity = 0
            )
        {
            canSelect = true;
            error = "";
            getPriceInputModel item = new getPriceInputModel();
            if (ServiceId != 0)
                item.ServiceId = ServiceId;
            item.AproximateValue = AproximateValue;
            item.Weight = weightItem;
            item.Height = height;
            item.Length = length;
            item.Width = width;
            item.ConsType = consType;
            item.Content = content;
            item.SenderAddress = SenderAddress;
            item.ReciverAddress = ReciverAddress;
            item.count = quantity;
            if (string.IsNullOrEmpty(item.CartonSizeName) && item.Width > 0)
            {
                item.CartonSizeName = _cartonService.GetRequiredCartonBySize(length, width, height);
            }

            if (item.SenderAddress == null)
            {
                item.SenderAddress = new CustomAddressModel()
                {
                    CountryId = senderCountry,
                    StateProvinceId = senderState
                };
            }
            if (item.ReciverAddress == null)
            {
                item.ReciverAddress = new CustomAddressModel()
                {
                    CountryId = receiverCountry,
                    StateProvinceId = receiverState
                };
            }
            #region CheckIsland
            if (IsIsland(senderState) && IsPostService(item.ServiceId))
            {
                if (item.Weight > 500)
                {
                    canSelect = false;
                    error = "امکان ارسال از جزایر و بنادر آزاد با وزن بیشتر از 500 گرم وجود ندارد";
                    return item;
                }
                //if (!IsPostService(item.ServiceId))
                //{
                //    canSelect = false;
                //    error = "امکان ارسال از جزایر و بنادر آزاد فقط با سرویس پست بار و با وزن حداکثر 500 گرم وجود دارد";
                //    return item;
                //}
            }
            if (IsIsland(receiverState))
            {
                if (item.Weight > 30000)
                {
                    canSelect = false;
                    error = "امکان ارسال  به جزایر و بنادر آزاد با وزن بیشتر از 3 کیلو گرم وجود ندارد";
                    return item;
                }
                if (item.ServiceId != 723)//,655
                {
                    canSelect = false;
                    error = "امکان ارسال به جزایر و بنادر آزاد فقط با سرویس پیشتاز و با وزن حداکثر از 3 کیلو گرم وجود دارد";
                    return item;
                }
                //if (!IsPostService(item.ServiceId))
                //{
                //    canSelect = false;
                //    error = "امکان ارسال و در یافت به جزایر فقط با سرویس پست بار و با وزن حداکثر 10000 گرم وجود دارد";
                //    return item;
                //}
            }
            #endregion

            if (new int[] { 703, 699, 705, 706 }.Contains(item.ServiceId))//DTS
            {
                #region DTS
                if (item.Width > 100 || item.Height > 100 || item.Length > 100)
                {
                    canSelect = false;
                    error = "سقف ابعاد 100*100*100 سانتیمتر";
                    return item;
                }
                if (weightItem > 35000)
                {
                    error = "سقف وزن بسته 35 کیلو گرم";
                    canSelect = false;
                    return item;
                }
                item.SenderStateId = getDtsStateId(senderState);
                item.ReaciverStateId = getDtsStateId(receiverState);
                #endregion
            }
            else if (item.ServiceId == 730 || item.ServiceId == 731)// Mahex
            {
                //if (senderCountry != 1 && receiverCountry != 1)
                //{
                //    canSelect = false;
                //    error = "ارائه این سرویس فقط از مبدا تهران یا به مقصد تهران مکان پذیر است";
                //    return item;
                //}
                if (width > 150 || height > 150 || length > 150)
                {
                    canSelect = false;
                    error = "با توجه به ابعاد مرسوله شما سرویس پست بار ارائه نمی شود(محدودیت 150 سانتیمتر در هر ضلع)";
                    return item;
                }
                if (weightItem >= 200001)
                {
                    canSelect = false;
                    error = "سقف وزن بسته 200 کیلو گرم";
                    return item;
                }
                var sender1 = _mahex_Service.GetCityCode(senderState);
                if (sender1?.Code == null)
                {
                    error = "شهر فرستنده پشتبانی نمی شود ";
                    canSelect = false;
                    return item;
                }
                var reciver11 = _mahex_Service.GetCityCode(receiverState);
                if (reciver11?.Code == null)
                {
                    error = "شهر گیرنده پشتبانی نمی شود ";
                    canSelect = false;
                    return item;
                }
                item.SenderStateId = senderState;
                item.ReaciverStateId = receiverState;
            }
            else if (item.ServiceId == 708)// PDE Domestic
            {
                #region PDE Domestic
                if (item.Width > 100 || item.Height > 100 || item.Length > 100)
                {
                    canSelect = false;
                    error = "سقف ابعاد 100*100*100 سانتیمتر";
                    return item;
                }
                if (weightItem > 50000)
                {
                    canSelect = false;
                    error = "سقف وزن بسته 50 کیلو گرم";
                    return item;
                }
                var PDESenderStateId = getPDEState(senderState);
                if (PDESenderStateId == null)
                {
                    error = " این سرویس در شهر مبدا ارایه نمی شود ";
                    canSelect = false;
                    return item;
                }
                var PDEReciverStateId = getPDEState(receiverState);
                if (PDEReciverStateId == null)
                {
                    error = " این سرویس در شهر مقصد ارایه نمی شود ";
                    canSelect = false;
                    return item;
                }
                if (PDESenderStateId.IsForcedTehran && PDEReciverStateId.PDEStateId != 103)
                {
                    error = "با توجه به مبدا و مقصد این سرویس قابل ارایه نیست";
                    canSelect = false;
                    return item;
                }
                if (PDEReciverStateId.IsForcedTehran && PDESenderStateId.PDEStateId != 103)
                {
                    error = "با توجه به مبدا و مقصد این سرویس قابل ارایه نیست";
                    canSelect = false;
                    return item;
                }
                item.SenderStateId = PDESenderStateId.PDEStateId;
                item.ReaciverStateId = PDEReciverStateId.PDEStateId;
                #endregion
            }
            else if (item.ServiceId == 719) //bluesky International
            {
                #region bluesky International
                if (item.Width > 70 || item.Height > 70 || item.Length > 70)
                {
                    canSelect = false;
                    error = "سقف ابعاد 70*70*70 سانتیمتر";
                    return item;
                }
                if (weightItem > 30000)
                {
                    canSelect = false;
                    error = "سقف وزن بسته 30 کیلو گرم";
                    return item;
                }
                if (!_CheckRegionDesVijePost.CheckValidSourceForInternationalPost(item.SenderAddress.CountryId.Value, item.SenderAddress.StateProvinceId.Value))
                {
                    canSelect = false;
                    error = $"امکان ارسال پست خارجی از شهر {item.SenderStateName} در حال حاضر وجود ندارد";
                    return item;
                }
                item.ReciverCountryCode = getBlueSkyCountryCode(receiver_ForeginCountry);
                #endregion
            }
            else if (item.ServiceId == 733) //Snapbox 
            {
                #region Snapbox 
                if (item.Width * item.Height * item.Length > 2475000)
                {
                    canSelect = false;
                    error = "سقف ابعاد 110*150*150 سانتیمتر";
                    return item;
                }
                if (weightItem > 135000)
                {
                    canSelect = false;
                    error = "سقف وزن بسته 135 کیلو گرم";
                    return item;
                }
                #endregion
            }
            else if (item.ServiceId == 717) //Snapbox 
            {
                #region Snapbox 
                if (item.Width > 70 || item.Height > 70 || item.Length > 70)
                {
                    canSelect = false;
                    error = "سقف ابعاد 70*70*70 سانتیمتر";
                    return item;
                }
                if (weightItem > 30000)
                {
                    canSelect = false;
                    error = "سقف وزن بسته 25 کیلو گرم";
                    return item;
                }
                #endregion
            }
            else if (item.ServiceId == 718) //postex+ 
            {
                #region postex+ 

                if (item.Width * item.Height * item.Length > 210000)
                {
                    canSelect = false;
                    error = "سقف ابعاد 50*60*70 سانتیمتر";
                    return item;
                }
                if (weightItem > 21000)
                {
                    canSelect = false;
                    error = "سقف وزن بسته 21 کیلو گرم";
                    return item;
                }
                //if (item.SenderAddress.Lat == null || item.SenderAddress.Lon == null)
                //{
                //    canSelect = false;
                //    error = "موقعیت جغرافیایی فرستنده مشخص نیست";
                //    return item;
                //}
                //if (item.ReciverAddress.Lat == null || item.ReciverAddress.Lon == null)
                //{
                //    canSelect = false;
                //    error = "موقعیت جغرافیایی گیرنده مشخص نیست";
                //    return item;
                //}

                //item.ReciverCountry = receiver_ForeginCountry;
                #endregion
            }
            else if (item.ServiceId == 707) //PDE International
            {
                #region PDE International
                if (item.Width > 70 || item.Height > 70 || item.Length > 70)
                {
                    canSelect = false;
                    error = "سقف ابعاد 70*70*70 سانتیمتر";
                    return item;
                }
                if (weightItem > 30000)
                {
                    canSelect = false;
                    error = "سقف وزن بسته 30 کیلو گرم";
                    return item;
                }
                if (!_CheckRegionDesVijePost.CheckValidSourceForInternationalPost(item.SenderAddress.CountryId.Value, item.SenderAddress.StateProvinceId.Value))
                {
                    canSelect = false;
                    error = $"امکان ارسال پست خارجی از شهر {item.SenderStateName} در حال حاضر وجود ندارد";
                    return item;
                }
                item.ReciverCountry = getForinCountryForPDE(receiver_ForeginCountry);
                #endregion
            }
            else if (new int[] { 709, 710, 711 }.Contains(item.ServiceId))//TPG
            {
                #region TPG
                if (item.Width > 300 || item.Height > 300 || item.Length > 300)
                {
                    canSelect = false;
                    error = "سقف ابعاد 300*300*300 سانتیمتر";
                    return item;
                }
                if (weightItem > 50000)
                {
                    canSelect = false;
                    error = "سقف وزن بسته 50 کیلو گرم";
                    return item;
                }
                var TpgSender = getTpgState(senderState);
                if (TpgSender == null)
                {
                    error = " این سرویس در شهر مبدا ارایه نمی شود ";
                    canSelect = false;
                    return item;
                }

                var TpgReciver = getTpgState(receiverState);
                if (TpgReciver == null)
                {
                    error = " این سرویس در شهر مقصد ارایه نمی شود ";
                    canSelect = false;
                    return item;
                }
                item.SenderStateId = TpgSender.TPGStateId;
                item.ReaciverStateId = TpgReciver.TPGStateId;
                #endregion
            }
            else if (item.ServiceId == 701)
            {
                #region ubbar
                if (!(weightItem >= 100000 && weightItem <= 20000000))
                {
                    canSelect = false;
                    error = "وزن مجاز حداقل 100 کیلو گرم و حداکثر 20 تن";
                    return item;
                }
                var Ubbar_senderState = getUbarState(senderState);
                var Ubbar_reciverState = getUbarState(receiverState);
                item.SenderStateId = Ubbar_senderState.RegionId;
                item.SenderStateName = Ubbar_senderState.RegionName;
                item.ReaciverStateId = Ubbar_reciverState.RegionId;
                item.ReaciverStateName = Ubbar_reciverState.RegionName;
                item.UbbarPackingLoad = PackingOption;
                item.UbbraTruckType = vechileType;
                item.dispatch_date = dispach_date;
                item.VechileOptions = VechileOption;
                #endregion
            }
            else if (item.ServiceId == 702)//yarbox
            {
                #region Yarbox
                if (item.Width > 100 || item.Height > 100 || item.Length > 100)
                {
                    canSelect = false;
                    error = "سقف ابعاد 100*100*100 سانتیمتر";
                    return item;
                }
                if (weightItem > 100001)
                {
                    error = "سقف وزن بسته 100 کیلو گرم";
                    canSelect = false;
                    return item;
                }
                if (!new int[] { 4, 579, 580, 581, 582, 583, 584, 585 }.Contains(senderState))
                {
                    error = "ارائه این سرویس فقط از مبدا تهران امکان پذیر است";
                    canSelect = false;
                    return item;
                }
                var Sender = getYarBoxState(senderState);
                var Reciver = getYarBoxState(receiverState);
                if (Sender == null)
                {
                    error = "این سرویس در مبدا ارائه نمی شود";
                    canSelect = false;
                    return item;
                }

                if (Reciver == null)
                {
                    error = "سرویس در مقصد ارائه نمی شود";
                    canSelect = false;
                    return item;
                }
                item.SenderStateId = Sender.YarboxCityId;
                item.ReaciverStateId = Reciver.YarboxCityId;
                item.ApTypeName = getApTypeName(weightItem);
                #endregion
            }

            else if (new int[] { 712, 713, 714, 715 }.Contains(item.ServiceId))
            {
                #region chapar

                if (item.Width > 160 || item.Height > 160 || item.Length > 160)
                {
                    canSelect = false;
                    error = "سقف ابعاد 160*160*160 سانتیمتر";
                    return item;
                }
                if (weightItem > 100001)
                {
                    error = "سقف وزن بسته 100 کیلو گرم";
                    canSelect = false;
                    return item;
                }


                int chaparSenderId = getDtsStateId(senderState);
                if (chaparSenderId == 0)
                {
                    error = "این سرویس در شهر مبدا ارائه نمی شود";
                    canSelect = false;
                    return item;
                }
                int ChaparReciverId = getDtsStateId(receiverState);
                if (ChaparReciverId == 0)
                {
                    error = "این سرویس در شهر مقصد ارائه نمی شود";
                    canSelect = false;
                    return item;
                }
                item.SenderStateId = chaparSenderId;
                item.ReaciverStateId = ChaparReciverId;
                #endregion
            }
            else if (new int[] { 660, 661, 662 }.Contains(item.ServiceId))
            {
                #region ویژه
                if (item.Width > 100 || item.Height > 100 || item.Length > 100)
                {
                    canSelect = false;
                    error = "سقف ابعاد 100*100*100 سانتیمتر";
                    return item;
                }
                if (weightItem > 30000)
                {
                    error = "سقف وزن بسته 30 کیلو گرم";
                    canSelect = false;
                    return item;
                }

                bool s = _CheckRegionDesVijePost.CheckValidSourceDistination(item.SenderAddress.CountryId.Value
                    , item.SenderAddress.StateProvinceId.Value
                    , item.ReciverAddress.StateProvinceId.Value
                    , item.ReciverAddress.StateProvinceId.Value);
                if (s == false)
                {
                    error = " سرویس  ویژه در این شهر مبدا و مقصد پشتیبانی نمیشود ";
                    canSelect = false;
                    return item;
                }
                #endregion
            }
            else if (new int[] { 662, 654, 655, 690, 691, 693, 694, 695, 696, 697, 698, 725, 726, 727, 722, 723 }.Contains(item.ServiceId))
            {
                #region پست
                if (AproximateValue > 200000000)
                {
                    canSelect = false;
                    error = "امکان ارسال کالا با ارزش بیش از 200،000،000 میلیون ریال در شرکت ملی پست وجود ندارد";
                    return item;
                }
                if (width > 100 || height > 100 || length > 100)
                {
                    canSelect = false;
                    error = "با توجه به ابعاد مرسوله شما سرویس پست بار ارائه نمی شود(محدودیت 100 سانتیمتر در هر ضلع)";
                    return item;
                }
                if (weightItem >= 30001)
                {
                    canSelect = false;
                    error = "سقف وزن بسته 30 کیلو گرم";
                    return item;
                }
                //if (_workContext.CurrentCustomer.Id != 13653617)//اسفندی نماینده تبریز
                //{
                //    if (item.ServiceId == 654 && weightItem > 5000)
                //    {
                //        canSelect = false;
                //        error = "سقف وزن بسته برای سرویس سفارشی 5 کیلو گرم می باشد";
                //        return item;
                //    }
                //}
                #endregion
            }
            item.OrderItemId = 0;
            return item;
        }

        /// <summary>
        /// که باید از گیرنده دریافت شود COD محاسبه مبلغ 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int calcCodPirce(OrderItem item)
        {
            var result = getEngAndPostPrice(item);

            int goodsPrice = getGoodsPrice(item);
            int engprice = ((int?)(result?.EngPrice)).GetValueOrDefault(0) + ((int?)(result?.AttrPrice)).GetValueOrDefault(0);
            engprice += ((engprice * 9) / 100);
            return goodsPrice + engprice;

        }

        #region UpdatePriceFromApi
        public void UpdateOrderTotalByApiPrice(Order order, int? DealerId = null)
        {
            try
            {
                var mark = GetOrderRegistrationMethod(order);
                DealerId = DealerId.HasValue ? DealerId : 0;
                int serviceId = order.OrderItems.First().Product.ProductCategories.First().Category.Id;
                var catInfo = GetCategoryInfo(serviceId);
                //if (mark != OrderRegistrationMethod.bidok && DealerId == 0 && new int[] { 654, 655, 660, 661, 662, 698, 697, 696, 695, 694, 693, 691, 690 }.Contains(serviceId))
                //    return;
                bool isMultiShippment = IsMultiShippment(order);
                int OrderTotalPrice = 0;
                foreach (var item in order.OrderItems)
                {
                    int weight = GetItemWeightFromAttr(item);
                    int ApproximateValue = getApproximateValue(item.Id);
                    var dimensions = getDimensions(item);

                    //int hagheMagharPrice = 0;
                    //if (catInfo.HasHagheMaghar)
                    //{
                    //    hagheMagharPrice = getInsertedHagheMaghar(item.Id);
                    //    hagheMagharPrice += ((hagheMagharPrice * 9) / 100);
                    //}
                    bool IsExtraForpeykhub = false;
                    for (int i = 0; i < item.Quantity; i++)
                    {

                        Address shipmentAddress = null;
                        //if (isMultiShippment)
                        //{
                        var Mulshipment = getShipmentFromMultiShipment(item, i);
                        shipmentAddress = _addressService.GetAddressById(Mulshipment.ShipmentAddressId);
                        //}
                        //else
                        //{
                        //    shipmentAddress = order.ShippingAddress;
                        //}
                        var consType = getPDEConsType(item);
                        int countryId = 0;
                        if (serviceId == 707)
                            countryId = int.Parse(shipmentAddress.Address2.Split('|')[0]);
                        string reciverForgeinCountryEnName = "";
                        if (serviceId == 719)
                            reciverForgeinCountryEnName = shipmentAddress.Address2.Split('|')[3];
                        DateTime? dispath_date = null;
                        VecileItems ubbarItem = new VecileItems();
                        string content = getOrderItemContent(item);
                        if (serviceId == 701)
                        {
                            ubbarItem = getVecileItem(item);
                            dispath_date = getDispatchDate(item);
                        }
                        string _error = "";
                        bool canselect = true;
                        var Dimensions = getDimensions(item);
                        var reciverLocation = GetAddressLocation(shipmentAddress.Id);
                        CustomAddressModel CustomReciverAddressModel = new CustomAddressModel()
                        {
                            Address1 = shipmentAddress.Address1,
                            Address2 = shipmentAddress.Address2,
                            City = shipmentAddress.City,
                            Company = shipmentAddress.Company,
                            CountryId = shipmentAddress.CountryId,
                            CreatedOnUtc = shipmentAddress.CreatedOnUtc,
                            CustomAttributes = shipmentAddress.CustomAttributes,
                            Email = shipmentAddress.Email,
                            FaxNumber = shipmentAddress.FaxNumber,
                            FirstName = shipmentAddress.FirstName,
                            LastName = shipmentAddress.LastName,
                            PhoneNumber = shipmentAddress.PhoneNumber,
                            StateProvinceId = shipmentAddress.StateProvinceId,
                            Lat = reciverLocation == null ? 0 : reciverLocation.Lat,
                            Lon = reciverLocation == null ? 0 : reciverLocation.Long,
                            ZipPostalCode = shipmentAddress.ZipPostalCode
                        };
                        var senderLocation = GetAddressLocation(order.BillingAddress.Id);
                        CustomAddressModel CustomSenderAddressModel = new CustomAddressModel()
                        {
                            Address1 = order.BillingAddress.Address1,
                            Address2 = order.BillingAddress.Address2,
                            StateProvinceName = order.BillingAddress.StateProvince == null ? "" : order.BillingAddress.StateProvince.Name,
                            City = order.BillingAddress.City,
                            Company = order.BillingAddress.Company,
                            CountryId = order.BillingAddress.CountryId,
                            CreatedOnUtc = order.BillingAddress.CreatedOnUtc,
                            CustomAttributes = order.BillingAddress.CustomAttributes,
                            Email = order.BillingAddress.Email,
                            FaxNumber = order.BillingAddress.FaxNumber,
                            FirstName = order.BillingAddress.FirstName,
                            LastName = order.BillingAddress.LastName,
                            PhoneNumber = order.BillingAddress.PhoneNumber,
                            StateProvinceId = order.BillingAddress.StateProvinceId,
                            Lat = senderLocation == null ? 0 : senderLocation.Lat,
                            Lon = senderLocation == null ? 0 : senderLocation.Long,
                            ZipPostalCode = order.BillingAddress.ZipPostalCode
                        };
                        var getPriceInputModel = PriceInpuModelFactory(senderCountry: order.BillingAddress.CountryId.Value,
                                    senderState: order.BillingAddress.StateProvinceId.Value,
                                    receiverCountry: shipmentAddress.CountryId.HasValue ? shipmentAddress.CountryId.Value : 0,
                                    receiverState: shipmentAddress.StateProvinceId.HasValue ? shipmentAddress.StateProvinceId.Value : 0,
                                    weightItem: weight,
                                    AproximateValue: ApproximateValue,
                                    error: out _error,
                                    canSelect: out canselect,
                                    height: Dimensions.Item3,
                                    length: Dimensions.Item1,
                                    width: Dimensions.Item2,
                                    consType: consType,
                                    content: content,
                                    dispach_date: dispath_date,
                                    PackingOption: ubbarItem.package_options,
                                    vechileType: ubbarItem.vehicle_type,
                                    VechileOption: ubbarItem.vehicle_options,
                                    receiver_ForeginCountry: new int[] { 710, 707, 719 }.Contains(serviceId) ? shipmentAddress.Address2.Split('|')[0].ToEnDigit() : 0,
                                    receiver_ForeginCountryNameEn: reciverForgeinCountryEnName,
                           ServiceId: serviceId, ReciverAddress: CustomReciverAddressModel, SenderAddress: CustomSenderAddressModel
                           , quantity: item.Quantity
                           );

                        if ((new int[] { 654, 655, 660, 661, 662, 667, 670, 698, 697, 696, 695, 694, 693, 691, 690, 718, 725, 726, 727, 722, 723 }.Contains(serviceId)))
                        {
                            string error = "";
                            int PostbasePrice = 0;
                            int price = 0;
                            if (new int[] { 667, 670, 722, 723 }.Contains(serviceId))
                            {
                                //hagheMagharPrice = 0;
                                bool IsCOD = new int[] { 667, 670 }.Contains(serviceId);
                                PostbasePrice = getCodbasePrice(order.CustomerId, serviceId, weight, shipmentAddress.CountryId.Value
                                    , shipmentAddress.StateProvinceId.Value, order.BillingAddress.StateProvinceId.Value
                                    , out error
                                    , (DealerId > 0)
                                    , item
                                    , IsCOD);
                                //int goodsPrice = getGoodsPrice(item);
                                price = PostbasePrice;// + goodsPrice;
                                if (PostbasePrice <= 0)
                                    return;
                            }
                            //else
                            ////if (!new int[] { 667, 670 }.Contains(serviceId))
                            //{
                            //    var data = GetBasPriceAndSlA(order.BillingAddress.CountryId.Value, order.BillingAddress.StateProvinceId.Value
                            //          , shipmentAddress.CountryId.Value, shipmentAddress.StateProvinceId.Value,
                            //          weight, order.StoreId, order.CustomerId, 0, false, serviceId);
                            //    if (data == null || !data.Any())
                            //        return;
                            //    PostbasePrice = data.First().CleanPrice;
                            //    price = getOrderTotalbyIncomeApiPrice(PostbasePrice, item.Id, serviceId, DealerId: DealerId);

                            //}
                            else if (serviceId == 718)
                            {
                                //getPriceInputModel.IsExtraForpeykhub = IsExtraForpeykhub;
                                var pustexPlusResult = peykhubGetPrice(getPriceInputModel).Result;
                                if (pustexPlusResult.canSelect)
                                {
                                    if (pustexPlusResult.price <= 0)
                                        return;
                                    price = getOrderTotalbyIncomeApiPrice(pustexPlusResult.price, item.Id, serviceId, DealerId: DealerId);
                                }
                                IsExtraForpeykhub = true;
                            }
                            if (!new int[] { 667, 670, 722, 723 }.Contains(serviceId))
                            {
                                int AgentSaleAmoun = GetAgentAddValue(item.Id);
                                OrderTotalPrice += price + AgentSaleAmoun;
                                //if (catInfo.HasHagheMaghar)
                                //{
                                //    OrderTotalPrice += hagheMagharPrice;
                                //    hagheMagharPrice = 0;
                                //}
                                //hagheMagharPrice = 0;
                            }
                            else
                            {
                                OrderTotalPrice += price;
                            }
                        }
                        else if ((new int[] { 703, 699, 705, 706 }).Contains(serviceId))
                        {
                            #region DTS
                            var DtsPriceResult = getDtsPrice(getPriceInputModel, 10825).Result;

                            if (DtsPriceResult.canSelect)
                            {
                                if (DtsPriceResult.price <= 0)
                                    return;
                                var price = getOrderTotalbyIncomeApiPrice(DtsPriceResult.price, item.Id, serviceId, DealerId: DealerId);
                                //int AgentSaleAmoun = GetAgentAddValue(item.Id);
                                OrderTotalPrice += price;//+ AgentSaleAmoun;
                                //if (catInfo.HasHagheMaghar)
                                //{
                                //    OrderTotalPrice += hagheMagharPrice;
                                //    hagheMagharPrice = 0;
                                //}
                                //hagheMagharPrice = 0;
                            }
                            else
                            {
                                InsertOrderNote($"آیتم شماره {item.Id} ردیف {i}" + "->" + DtsPriceResult.errorMessage, order.Id);
                                ChangeOrderState(order, OrderStatus.Pending, DtsPriceResult.errorMessage);
                                return;
                            }
                            #endregion
                        }
                        else if (serviceId == 708)// PDE Domestic)
                        {
                            #region PDE Dome
                            var PDEresult = getPDE_Price(getPriceInputModel).Result;
                            if (PDEresult.canSelect)
                            {
                                if (PDEresult.price <= 0)
                                    return;
                                var price = getOrderTotalbyIncomeApiPrice(PDEresult.price, item.Id, serviceId, DealerId: DealerId);
                                //int AgentSaleAmoun = GetAgentAddValue(item.Id);
                                //if (catInfo.HasHagheMaghar)
                                //{
                                //    OrderTotalPrice += hagheMagharPrice;
                                //    hagheMagharPrice = 0;
                                //}
                                //OrderTotalPrice += price;//+ AgentSaleAmoun;
                            }
                            else
                            {
                                InsertOrderNote($"آیتم شماره {item.Id} ردیف {i}" + "->" + PDEresult.errorMessage, order.Id);
                                ChangeOrderState(order, OrderStatus.Pending, PDEresult.errorMessage);
                                return;
                            }
                            #endregion
                        }
                        else if (serviceId == 707)// PDE INTERNATIONAL)
                        {
                            #region PDE inter

                            var pdeInternationalPrice = getPDEInternational_Price(getPriceInputModel).Result;
                            if (pdeInternationalPrice.canSelect)
                            {
                                if (pdeInternationalPrice.price <= 0)
                                    return;
                                var price = getOrderTotalbyIncomeApiPrice(pdeInternationalPrice.price, item.Id, serviceId, DealerId: DealerId);
                                //int AgentSaleAmoun = GetAgentAddValue(item.Id);
                                //if (catInfo.HasHagheMaghar)
                                //{
                                //    OrderTotalPrice += hagheMagharPrice;
                                //    hagheMagharPrice = 0;
                                //}
                                OrderTotalPrice += price;//+ AgentSaleAmoun;
                            }
                            else
                            {
                                InsertOrderNote($"آیتم شماره {item.Id} ردیف {i}" + "->" + pdeInternationalPrice.errorMessage, order.Id);
                                ChangeOrderState(order, OrderStatus.Pending, pdeInternationalPrice.errorMessage);
                                return;
                            }
                            #endregion
                        }
                        else if (new int[] { 709, 711 }.Contains(serviceId)) // TPG
                        {
                            #region TPG
                            var TpgPrice = getTPGPrice(getPriceInputModel).Result;
                            if (TpgPrice.canSelect)
                            {
                                if (TpgPrice.price <= 0)
                                    return;
                                var Totalprice = getOrderTotalbyIncomeApiPrice(TpgPrice.price, item.Id, serviceId, DealerId: DealerId);
                                //int AgentSaleAmoun = GetAgentAddValue(item.Id);
                                //if (catInfo.HasHagheMaghar)
                                //{
                                //    OrderTotalPrice += hagheMagharPrice;
                                //    hagheMagharPrice = 0;
                                //}
                                OrderTotalPrice += Totalprice;//+ AgentSaleAmoun;
                            }
                            else
                            {
                                InsertOrderNote($"آیتم شماره {item.Id} ردیف {i}" + "->" + TpgPrice.errorMessage, order.Id);
                                ChangeOrderState(order, OrderStatus.Pending, TpgPrice.errorMessage);
                                return;
                            }
                            #endregion
                            continue;

                        }
                        else if (serviceId == 701 && weight >= 100000) // اوبار
                        {
                            #region ubaar



                            var getubbarPrice = getUbarPrice(getPriceInputModel).Result;
                            if (getubbarPrice.canSelect)
                            {
                                if (getubbarPrice.price <= 0)
                                    return;
                                var Totalprice = getOrderTotalbyIncomeApiPrice(getubbarPrice.price, item.Id, serviceId, DealerId: DealerId);
                                //int AgentSaleAmoun = GetAgentAddValue(item.Id);
                                //if (catInfo.HasHagheMaghar)
                                //{
                                //    OrderTotalPrice += hagheMagharPrice;
                                //    hagheMagharPrice = 0;
                                //}
                                OrderTotalPrice += Totalprice;//+ AgentSaleAmoun;
                            }
                            else
                            {
                                InsertOrderNote($"آیتم شماره {item.Id} ردیف {i}" + "->" + getubbarPrice.errorMessage, order.Id);
                                ChangeOrderState(order, OrderStatus.Pending, getubbarPrice.errorMessage);
                                return;
                            }
                            #endregion
                            continue;
                        }
                        else if (serviceId == 702 && weight <= 35000) // یارباکس
                        {
                            #region Yarbox
                            var getYarBoxPrice = getYarboxPrice(getPriceInputModel).Result;
                            if (getYarBoxPrice.canSelect)
                            {
                                if (getYarBoxPrice.price <= 0)
                                    return;
                                var Totalprice = getOrderTotalbyIncomeApiPrice(getYarBoxPrice.price, item.Id, serviceId, weight, DealerId: DealerId);
                                // int AgentSaleAmoun = GetAgentAddValue(item.Id);
                                //if (catInfo.HasHagheMaghar)
                                //{
                                //    OrderTotalPrice += hagheMagharPrice;
                                //    hagheMagharPrice = 0;
                                //}
                                OrderTotalPrice += Totalprice;//+ AgentSaleAmoun;
                            }
                            else
                            {
                                InsertOrderNote($"آیتم شماره {item.Id} ردیف {i}" + "->" + getYarBoxPrice.errorMessage, order.Id);
                                ChangeOrderState(order, OrderStatus.Pending, getYarBoxPrice.errorMessage);
                                return;
                            }
                            #endregion
                            continue;
                        }
                        else if ((new int[] { 712, 713, 714, 715 }).Contains(serviceId))//چاپار
                        {
                            #region chapar
                            var chaparReslt = getChaparPrice(getPriceInputModel, 29863).Result;
                            if (chaparReslt.canSelect)
                            {
                                if (chaparReslt.price <= 0)
                                    return;
                                var price = getOrderTotalbyIncomeApiPrice(chaparReslt.price, item.Id, serviceId, DealerId: DealerId);
                                //int AgentSaleAmoun = GetAgentAddValue(item.Id);
                                OrderTotalPrice += price;//+ AgentSaleAmoun;
                                //if (catInfo.HasHagheMaghar)
                                //{
                                //    OrderTotalPrice += hagheMagharPrice;
                                //    hagheMagharPrice = 0;
                                //}
                                bool isCod = (new int[] { 713, 715 }).Contains(serviceId);
                                if (isCod)
                                {
                                    var _chaparresult1 = getChaparPrice(getPriceInputModel, 29863, OrderTotalPrice).Result;
                                    if (_chaparresult1.CodTranPrice > 0)
                                    {
                                        UpdateCalcorderitem(item.Id, _chaparresult1.CodTranPrice);
                                    }
                                }
                            }
                            else
                            {
                                InsertOrderNote($"آیتم شماره {item.Id} ردیف {i}" + "->" + chaparReslt.errorMessage, order.Id);
                                ChangeOrderState(order, OrderStatus.Pending, chaparReslt.errorMessage);
                                return;
                            }
                            #endregion
                            continue;
                        }
                        else if (serviceId == 730 || serviceId == 731)
                        {
                            #region mahex
                            var mahexReslt = getmahexPrice(getPriceInputModel).Result;
                            if (mahexReslt.canSelect)
                            {
                                if (mahexReslt.price <= 0)
                                    return;
                                var price = getOrderTotalbyIncomeApiPrice(mahexReslt.price, item.Id, serviceId, DealerId: DealerId);
                                //int AgentSaleAmoun = GetAgentAddValue(item.Id);
                                OrderTotalPrice += price;//+ AgentSaleAmoun;
                                //if (catInfo.HasHagheMaghar)
                                //{
                                //    OrderTotalPrice += hagheMagharPrice;
                                //    hagheMagharPrice = 0;
                                //}
                                if (serviceId == 731)
                                {
                                    int CodGoodPrice = getGoodsPrice(item);
                                    OrderTotalPrice += CodGoodPrice;

                                }
                            }
                            else
                            {
                                InsertOrderNote($"آیتم شماره {item.Id} ردیف {i}" + "->" + mahexReslt.errorMessage, order.Id);
                                ChangeOrderState(order, OrderStatus.Pending, mahexReslt.errorMessage);
                                return;
                            }
                            #endregion
                            continue;
                        }
                        else if (serviceId == 719)
                        {
                            #region chapar
                            var SkyBlueReslt = getBlueSkyPrice(getPriceInputModel).Result;
                            if (SkyBlueReslt.canSelect)
                            {
                                if (SkyBlueReslt.price <= 0)
                                    return;
                                var price = getOrderTotalbyIncomeApiPrice(SkyBlueReslt.price, item.Id, serviceId, DealerId: DealerId);
                                //int AgentSaleAmoun = GetAgentAddValue(item.Id);
                                OrderTotalPrice += price;//AgentSaleAmoun;
                                //if (catInfo.HasHagheMaghar)
                                //{
                                //    OrderTotalPrice += hagheMagharPrice;
                                //    hagheMagharPrice = 0;
                                //}
                            }
                            else
                            {
                                InsertOrderNote($"آیتم شماره {item.Id} ردیف {i}" + "->" + SkyBlueReslt.errorMessage, order.Id);
                                ChangeOrderState(order, OrderStatus.Pending, SkyBlueReslt.errorMessage);
                                return;
                            }
                            #endregion
                            continue;
                        }
                        else if (serviceId == 733)
                        {
                            #region Kalaresan
                            var KalaresanResult = getKalaresanPrice(getPriceInputModel).Result;
                            if (KalaresanResult.canSelect)
                            {
                                if (KalaresanResult.price <= 0)
                                    return;
                                var price = getOrderTotalbyIncomeApiPrice(KalaresanResult.price, item.Id, serviceId, DealerId: DealerId);
                                //int AgentSaleAmoun = GetAgentAddValue(item.Id);
                                OrderTotalPrice += price;//AgentSaleAmoun;
                                //if (catInfo.HasHagheMaghar)
                                //{
                                //    OrderTotalPrice += hagheMagharPrice;
                                //    hagheMagharPrice = 0;
                                //}
                            }
                            else
                            {
                                InsertOrderNote($"آیتم شماره {item.Id} ردیف {i}" + "->" + KalaresanResult.errorMessage, order.Id);
                                ChangeOrderState(order, OrderStatus.Pending, KalaresanResult.errorMessage);
                                return;
                            }
                            #endregion
                            continue;
                        }
                        else if (serviceId == 717)
                        {
                            #region snappbox
                            var snappboxReslt = getsnappboxPrice(getPriceInputModel).Result;
                            if (snappboxReslt.canSelect)
                            {
                                if (snappboxReslt.price <= 0)
                                    return;
                                var price = getOrderTotalbyIncomeApiPrice(snappboxReslt.price, item.Id, serviceId, DealerId: DealerId);
                                //int AgentSaleAmoun = GetAgentAddValue(item.Id);
                                OrderTotalPrice += price;//+ AgentSaleAmoun;
                                //if (catInfo.HasHagheMaghar)
                                //{
                                //    OrderTotalPrice += hagheMagharPrice;
                                //    hagheMagharPrice = 0;
                                //}
                            }
                            else
                            {
                                InsertOrderNote($"آیتم شماره {item.Id} ردیف {i}" + "->" + snappboxReslt.errorMessage, order.Id);
                                ChangeOrderState(order, OrderStatus.Pending, snappboxReslt.errorMessage);
                                return;
                            }
                            #endregion
                            continue;
                        }
                        else
                            return;

                    }
                }
                if (OrderTotalPrice > 0)
                {
                    OrderTotalPrice = OrderTotalPrice - (int)order.OrderDiscount;
                    int SafeBuyKalaPrice = 0;
                    if (IsSafeBuy(order.Id))
                    {
                        foreach (var item in order.OrderItems)
                        {

                            int value = getApproximateValue(item.Id);
                            SafeBuyKalaPrice += value * item.Quantity;
                        }
                    }
                    OrderTotalPrice += SafeBuyKalaPrice;
                    if (order.OrderTotal > 0)
                    {
                        order.OrderTotal = OrderTotalPrice;
                        var _order = _orderRepository.Table.Single(p => p.Id == order.Id);
                        _order.OrderTotal = OrderTotalPrice;
                        _orderRepository.Update(_order);
                    }
                    else
                    {

                        var rewardPoint = _rewardPointsHistoryRepository.Table.Where(p => p.UsedWithOrder != null && p.UsedWithOrder.Id == order.Id
                         && p.Points < 0 && p.CustomerId == order.CustomerId && p.StoreId == order.StoreId).FirstOrDefault();
                        if (rewardPoint == null)
                            return;
                        rewardPoint.Points = (int)(OrderTotalPrice) * -1;
                        rewardPoint.PointsBalance = (int?)null;
                        _rewardPointsHistoryRepository.Update(rewardPoint);
                        _rewardPointService.GetRewardPointsBalance(order.CustomerId, order.StoreId);
                    }
                    _genericAttributeService.SaveAttribute<string>(order, "PriceUpdateFromApi", OrderTotalPrice.ToString(), order.StoreId);
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }
        public void UpdateCalcorderitem(int orderitemId, int CodTranPrice)
        {
            string query = $@"UPDATE dbo.Tb_CalcPriceOrderItem SET EngPrice= EngPrice+{CodTranPrice} WHERE OrderItemId = {orderitemId}; select cast(1 as int) Id";
            _dbContext.SqlQuery<int>(query, new object[0]).FirstOrDefault();
        }
        public bool IsSafeBuy(int OrderId)
        {
            string query = $@"IF EXISTS( SELECT
	                        TOP(1)1
                        FROM
	                        dbo.Tb_OrderItemAttributeValue AS TOIAV
	                        INNER JOIN dbo.OrderItem AS OI ON OI.Id = TOIAV.OrderItemId
                        WHERE
	                        TOIAV.PropertyAttrName =N'خرید امن'
	                        AND TOIAV.PropertyAttrValueName =N'بله'
	                        AND OI.OrderId = {OrderId})
                        BEGIN
                            SELECT CAST(1 AS BIT) IsSafeBut
                        END
                        ELSE
                        BEGIN
                            SELECT CAST(0 AS BIT) IsSafeBut
                        END
	                        ";
            return _dbContext.SqlQuery<bool?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(false);
        }
        public int getInsertedHagheMaghar(Order order)
        {
            if (order == null)
                return 0;
            var orderItems = string.Join(",", order.OrderItems.Select(p => p.Id).ToList());
            string Query = $@"DECLARE @HagheMaghr int = 0
                            SELECT
	                            @HagheMaghr = SUM(ISNULL(TFLMP.CollectingPrice,0))  
                            FROM 
	                            dbo.[Order] AS O
	                            INNER JOIN dbo.Shipment AS S ON S.OrderId = O.Id
	                            INNER JOIN dbo.Tb_FirstLastMile_Price AS TFLMP ON TFLMP.ShipmentId = S.Id
                            WHERE
	                            O.Id = {order.Id}
                            IF ISNULL(@HagheMaghr,0) = 0
                            BEGIN
                                SELECT
	                                TOP(1) @HagheMaghr = THM.HagheMagharPrice+ISNULL(ShipmentHagheMaghr,0) 
                                FROM
	                                dbo.Tb_HagheMaghar AS THM
                                WHERE
	                                THM.OrderItemId IN ({orderItems})
                            END
                            SELECT @HagheMaghr HagheMagharPrice";
            return _dbContext.SqlQuery<int?>(Query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
        }
        //public int getInsertedHagheMaghar(int orderItemId)
        //{
        //    string Query = $@"SELECT
        //                     TOP(1) THM.HagheMagharPrice+ISNULL(ShipmentHagheMaghr,0) HagheMagharPrice
        //                    FROM
        //                     dbo.Tb_HagheMaghar AS THM
        //                    WHERE
        //                     THM.OrderItemId = {orderItemId}";
        //    return _dbContext.SqlQuery<int?>(Query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
        //}
        #endregion

        #region دریافت مقادیر ویژگی ها از سفارش
        //public List<Attr> getAttrList(int shipmentId)
        //{
        //    string query = $@"SELECT 
        //                     TOIAV.PropertyAttrName+ISNULL(N'('+TOIAV.PropertyAttrValueName+')','') AttrName,
        //                     CASE WHEN TOIAV.PropertyAttrName  LIKE N'%ثبت مرسوله%' 
        //                      OR TOIAV.PropertyAttrName LIKE N'%ارزش افزوده%' 
        //                      OR TOIAV.PropertyAttrName = N'هزینه جمع آوری هر مرسوله'
        //                       THEN CAST(ISNULL(TOIAV.PropertyAttrValueText,'0') AS INT) +(CAST(ISNULL(TOIAV.PropertyAttrValueText,'0') AS INT) *9/100)
        //                      WHEN TOIAV.PropertyAttrName LIKE N'%ارزش کالا%'  THEN CAST(ISNULL(TOIAV.PropertyAttrValueText,'0') AS INT) 
        //                      ELSE (TOIAV.PropertyAttrValuePrice+ CAST((TOIAV.PropertyAttrValuePrice *9/100) AS INT)) END AttrPrice
        //                    FROM
        //                     dbo.[Order] AS O
        //                     INNER JOIN dbo.Shipment AS S ON S.OrderId = O.Id
        //                     INNER JOIN dbo.ShipmentAppointment AS SA ON SA.ShipmentId = S.Id
        //                     INNER JOIN dbo.ShipmentItem AS SI ON SI.ShipmentId = S.Id
        //                     INNER JOIN dbo.OrderItem AS OI ON Si.OrderItemId = OI.Id
        //                     INNER JOIN dbo.Tb_OrderItemAttributeValue AS TOIAV ON TOIAV.OrderItemId = OI.Id
        //                     LEFT JOIN dbo.Tb_HagheMaghar AS THM ON OI.Id= THM.OrderItemId
        //                    WHERE
        //                     S.Id = {shipmentId}
        //                     AND ((TOIAV.PropertyAttrValuePrice IS NOT NULL AND  TOIAV.PropertyAttrValuePrice > 0) 
        //                      OR TOIAV.PropertyAttrName LIKE N'%ثبت مرسوله%' 
        //                      OR TOIAV.PropertyAttrName LIKE N'%ارزش افزوده%'
        //                      OR TOIAV.PropertyAttrName =N'هزینه جمع آوری هر مرسوله')
        //                     AND TOIAV.PropertyAttrName NOT LIKE N'%وزن بسته%'";
        //    return _dbContext.SqlQuery<Attr>(query, new object[0]).ToList();
        //}
        public class Attr
        {
            public string AttrName { get; set; }
            public int AttrPrice { get; set; }
        }
        public int getOrderItemsPrice(int orderItemId)
        {
            string query = $@"SELECT
			                     SUM(CASE WHEN TOIAV.PropertyAttrName  LIKE N'%ثبت مرسوله%' 
			                        OR TOIAV.PropertyAttrName LIKE N'%ارزش افزوده%' 
			                        OR TOIAV.PropertyAttrName = N'هزینه جمع آوری هر مرسوله'
			                        THEN CAST(ISNULL(TOIAV.PropertyAttrValueText,'0') AS INT) 
		                        ELSE TOIAV.PropertyAttrValuePrice END)
		                    FROM
			                    dbo.Tb_OrderItemAttributeValue AS TOIAV
		                    WHERE
			                   	TOIAV.OrderItemId = {orderItemId}
	                            AND ((TOIAV.PropertyAttrValuePrice IS NOT NULL AND  TOIAV.PropertyAttrValuePrice > 0) 
		                            OR TOIAV.PropertyAttrName LIKE N'%ثبت مرسوله%' 
		                            OR TOIAV.PropertyAttrName LIKE N'%ارزش افزوده%'
		                            OR TOIAV.PropertyAttrName =N'هزینه جمع آوری هر مرسوله')
	                            AND TOIAV.PropertyAttrName NOT LIKE N'%وزن بسته%'";
            return _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault();
        }
        public ApiOrderItemPrice getEngAndPostPrice(OrderItem orderitem)
        {
            string query = $@"SELECT TOP(1)
	                         TCPOI.IncomePrice
	                        , TCPOI.EngPrice
                            , TCPOI.AttrPrice
                        FROM
	                        dbo.Tb_CalcPriceOrderItem AS TCPOI
                        WHERE
	                        TCPOI.OrderItemId = {orderitem.Id}";
            return _dbContext.SqlQuery<ApiOrderItemPrice>(query, new object[0]).FirstOrDefault();
        }
        public byte getPDEConsType(OrderItem item)
        {
            string getConsType = "";

            getConsType = _OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == item.Id && p.PropertyAttrName.Contains("نوع مرسوله")).FirstOrDefault()?.PropertyAttrValueName;
            if (string.IsNullOrEmpty(getConsType))
                return Convert.ToByte(0);
            return getConsType == "پاکت" ? Convert.ToByte(0) : Convert.ToByte(1);
        }
        public int getyarBoxConsType(OrderItem item)
        {
            string getConsType = "";

            getConsType = _OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == item.Id && p.PropertyAttrName.Contains("نوع مرسوله")).FirstOrDefault()?.PropertyAttrValueText;
            if (string.IsNullOrEmpty(getConsType))
                return Convert.ToByte(0);
            // return getConsType == "پاکت" ? Convert.ToByte(1) : Convert.ToByte(1);
            return 0;
        }
        public bool HasRequestPrintAvatar(OrderItem item)
        {
            return _OrderItemAttributeValueRepository.Table.Any(p => p.OrderItemId == item.Id && p.PropertyAttrName.Contains("نشان تجاری")
            && p.PropertyAttrValueName == "بلی");
        }
        public int RequestPrintLogoPrice(OrderItem item)
        {
            return (_OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == item.Id && p.PropertyAttrName.Contains("نشان تجاری")
            && p.PropertyAttrValueName == "بلی").SingleOrDefault()?.PropertyAttrValuePrice).GetValueOrDefault(0);
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
        public int RequestSendSmsNotifPrice(OrderItem item)
        {
            return (_OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == item.Id && p.PropertyAttrName.Contains("اطلاع رسانی پیامکی")
            && p.PropertyAttrValueName == "بلی").SingleOrDefault()?.PropertyAttrValuePrice).GetValueOrDefault(0);
        }

        public bool? hasValidAvatar(int customerId)
        {
            string query = $@"SELECT
	                           TOP(1) cast(CASE WHEN TCAC.StateVerify=2 THEN 1 ELSE 0 END as bit)
                            FROM
	                            dbo.Tbl_CheckAvatarCustomer AS TCAC
                            WHERE
	                            TCAC.CustomerId = {customerId}
                            ORDER BY TCAC.Id DESC";
            return _dbContext.SqlQuery<bool?>(query, new object[0]).FirstOrDefault();
        }
        public (int, int, int) getDimensions(OrderItem item)
        {
            string _length = _OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == item.Id && p.PropertyAttrName.Contains("طول مرسوله")).FirstOrDefault()?.PropertyAttrValueText;
            int length = 0;
            if (!Int32.TryParse((string.IsNullOrEmpty(_length) ? "0" : _length), out length))
                length = (string.IsNullOrEmpty(_length) ? "0" : _length).ToEnDigit();
            int width = 0;
            string _width = _OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == item.Id && p.PropertyAttrName.Contains("عرض مرسوله")).FirstOrDefault()?.PropertyAttrValueText;
            if (!int.TryParse((string.IsNullOrEmpty(_width) ? "0" : _width), out width))
                width = (string.IsNullOrEmpty(_width) ? "0" : _width).ToEnDigit();
            int height = 0;
            string _height = _OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == item.Id && p.PropertyAttrName.Contains("ارتفاع مرسوله")).FirstOrDefault()?.PropertyAttrValueText;
            if (!int.TryParse((string.IsNullOrEmpty(_height) ? "0" : _height), out height))
                height = (string.IsNullOrEmpty(_height) ? "0" : _height).ToEnDigit();
            return (length, width, height);
        }
        public (int, int, int) getDimensions(int orderItemId)
        {
            string _length = _OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == orderItemId && p.PropertyAttrName.Contains("طول مرسوله")).FirstOrDefault()?.PropertyAttrValueText;
            int length = (string.IsNullOrEmpty(_length) ? "0" : _length).ToEnDigit();
            string _width = _OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == orderItemId && p.PropertyAttrName.Contains("عرض مرسوله")).FirstOrDefault()?.PropertyAttrValueText;
            int width = (string.IsNullOrEmpty(_width) ? "0" : _width).ToEnDigit();
            string _height = _OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == orderItemId && p.PropertyAttrName.Contains("ارتفاع مرسوله")).FirstOrDefault()?.PropertyAttrValueText;
            int height = (string.IsNullOrEmpty(_height) ? "0" : _height).ToEnDigit();
            return (length, width, height);
        }
        public int getHagheSabt(Order order)
        {
            string HagheSabt = "";
            foreach (var item in order.OrderItems)
            {
                HagheSabt = _OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == item.Id && p.PropertyAttrName.Contains("ثبت مرسوله")).FirstOrDefault()?.PropertyAttrValueText;
                if (!string.IsNullOrEmpty(HagheSabt))
                    break;
            }
            return (string.IsNullOrEmpty(HagheSabt) ? "0" : HagheSabt).ToEnDigit();
        }
        public int getHagheSabt(int orderItemId)
        {
            string HagheSabt = _OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == orderItemId && p.PropertyAttrName.Contains("ثبت مرسوله")).FirstOrDefault()?.PropertyAttrValueText;
            return (string.IsNullOrEmpty(HagheSabt) ? "0" : HagheSabt).ToEnDigit();
        }
        /// <summary>
        /// دریافت ارزش افزوده نمایندگی
        /// </summary>
        /// <param name="orderItemId"></param>
        /// <returns></returns>
        public int GetAgentAddValue(int orderItemId)
        {
            string AgneyAddValue = _OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == orderItemId && p.PropertyAttrName.Contains("ارزش افزوده")).FirstOrDefault()?.PropertyAttrValueText;
            return (string.IsNullOrEmpty(AgneyAddValue) ? "0" : AgneyAddValue).ToEnDigit();
        }
        /// <summary>
        /// دریافت ارزش کالا
        /// </summary>
        /// <param name="orderItemId"></param>
        /// <returns></returns>
        public int getApproximateValue(int orderItemId)
        {
            string _ApproximateValue = _OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == orderItemId && p.PropertyAttrName.Contains("ارزش کالا")).FirstOrDefault()?.PropertyAttrValueText;
            return (string.IsNullOrEmpty(_ApproximateValue) ? "0" : _ApproximateValue).ToEnDigit();
        }
        /// <summary>
        /// OrderItem دریافت کل هزینه بیمه کارآفرین برای هر 
        /// </summary>
        /// <param name="orderItem"></param>
        /// <returns></returns>
        public int GetBieme(OrderItem orderItem)
        {
            var InsuranceCost = (_OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == orderItem.Id && p.PropertyAttrName.Contains("بیمه")).SingleOrDefault()?.PropertyAttrValueCost)
                  .GetValueOrDefault(0);
            return Convert.ToInt32(InsuranceCost * orderItem.Quantity); // کل هزینه بیمه
        }
        /// <summary>
        /// دریافت هزینه بیمه پست
        /// </summary>
        /// <param name="orderItem"></param>
        /// <returns></returns>
        public int GetBiemeForOne(OrderItem orderItem)
        {
            var InsurancePostCost = (_OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == orderItem.Id && p.PropertyAttrName.Contains("غرامت")).SingleOrDefault()?.PropertyAttrValueCost)
                  .GetValueOrDefault(0);

            return Convert.ToInt32(InsurancePostCost); //  هزینه بیمه

        }
        public int GetBiemeDiff(OrderItem orderItem)
        {
            var InsurancePrice = (_OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == orderItem.Id && p.PropertyAttrName.Contains("بیمه")).SingleOrDefault()?.PropertyAttrValuePrice)
                  .GetValueOrDefault(0);
            return Convert.ToInt32(InsurancePrice *
                                   orderItem.Quantity); // کل هزینه بیمه
        }
        /// <summary>
        /// OrderItem دریافت هزینه کارتون و لفاف بندی به ازای 
        /// </summary>
        /// <param name="orderItem"></param>
        /// <returns></returns>
        public int GetkarotnPrice(OrderItem orderItem)
        {
            var KartonPrice = (_OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == orderItem.Id && p.PropertyAttrName.Contains("کارتن و لفاف بندی"))
                .SingleOrDefault()?.PropertyAttrValuePrice)
                .GetValueOrDefault(0);
            return Convert.ToInt32(KartonPrice * orderItem.Quantity);
        }
        /// <summary>
        /// Attribute گرفتن وزن بسته محصولات از 
        /// </summary>
        /// <param name="orderitem"></param>
        /// <returns></returns>
        public int GetItemWeightFromAttr(OrderItem orderitem)
        {
            decimal weight = 0;
            string _weight = (_OrderItemAttributeValueRepository.Table.SingleOrDefault(p => p.OrderItemId == orderitem.Id && p.PropertyAttrName.Contains("وزن دقیق"))?.PropertyAttrValueText);
            int int_weight = _weight.ToEnDigit();
            if (int_weight > 0)
            {
                return int_weight;
            }
            weight = (_OrderItemAttributeValueRepository.Table.SingleOrDefault(p => p.OrderItemId == orderitem.Id && p.PropertyAttrName.Contains("وزن بسته"))?.PropertyAttrValueWeight)
                .GetValueOrDefault(0);
            if (weight == 0)
                weight = Convert.ToInt32(orderitem.Product.Weight * 1000);
            return Convert.ToInt32(weight * 1000);
        }
        public int getItemWeight_V(OrderItem OI)
        {
            var dimantion = getDimensions(OI);
            var weight = GetItemWeightFromAttr(OI);
            int Area = dimantion.Item1 * dimantion.Item2 * dimantion.Item3;
            if (Area == 0)
                return weight;
            if (weight >= ((int)Area / 6000))
                return weight;
            return ((int)Area / 6000);
        }
        public int getItemWeight_V(int orderItemId)
        {
            var OI = _orderItemRepository.Table.Single(p => p.Id == orderItemId);
            var dimantion = getDimensions(OI);
            var weight = GetItemWeightFromAttr(OI);
            int Area = dimantion.Item1 * dimantion.Item2 * dimantion.Item3;
            if (Area == 0)
                return weight;
            if (weight >= ((int)Area / 6000))
                return weight;
            return ((int)Area / 6000);
        }
        public int GetItemWeightFromAttr(int orderitemId)
        {
            decimal weight = 0;
            string _weight = (_OrderItemAttributeValueRepository.Table.SingleOrDefault(p => p.OrderItemId == orderitemId && p.PropertyAttrName.Contains("وزن دقیق"))?.PropertyAttrValueText);
            int int_weight = _weight.ToEnDigit();
            if (int_weight > 0)
            {
                return int_weight;
            }
            weight = (_OrderItemAttributeValueRepository.Table.SingleOrDefault(p => p.OrderItemId == orderitemId && p.PropertyAttrName.Contains("وزن بسته"))?.PropertyAttrValueWeight)
                .GetValueOrDefault(0);
            if (weight == 0)
            {
                var orderitem = _orderItemRepository.Table.Single(p => p.Id == orderitemId);
                weight = Convert.ToInt32(orderitem.Product.Weight * 1000);
            }
            return Convert.ToInt32(weight * 1000);
        }
        /// <summary>
        /// بر اساس وزن بسته Attribute گرفتن هزینه محصولات از 
        /// </summary>
        /// <param name="orderitem"></param>
        /// <returns></returns>
        public int GetItemCostFromAttr(OrderItem orderitem)
        {
            var Cost = (_OrderItemAttributeValueRepository.Table.SingleOrDefault(p => p.OrderItemId == orderitem.Id && p.PropertyAttrName.Contains("وزن بسته"))?.PropertyAttrValueCost)
               .GetValueOrDefault(0);
            if (Cost == 0)
                Cost = Convert.ToInt32(orderitem.Product.ProductCost);
            return Cost;
        }
        /// <summary>
        /// بر اساس وزن بسته Attribute گرفتن قیمت محصولات از 
        /// </summary>
        /// <param name="orderitem"></param>
        /// <returns></returns>
        public int GetItemPriceFromAttr(OrderItem orderitem)
        {
            var price = _OrderItemAttributeValueRepository.Table.SingleOrDefault(p => p.OrderItemId == orderitem.Id && p.PropertyAttrName.Contains("وزن بسته"))
                ?.PropertyAttrValuePrice.GetValueOrDefault(0);
            if (price.GetValueOrDefault(0) == 0)
            {
                price = Convert.ToInt32(orderitem.Product.Price);
            }
            return price.Value;
        }
        public int GetValueAddedbyAgent(OrderItem orderitem)
        {
            var ValueAddedbyAgent = _OrderItemAttributeValueRepository.Table.SingleOrDefault(p => p.OrderItemId == orderitem.Id && p.PropertyAttrName.Contains("ارزش افزوده"))
                ?.PropertyAttrValueText;
            if (!string.IsNullOrEmpty(ValueAddedbyAgent))
            {
                return ValueAddedbyAgent.ToEnDigit();
            }
            return 0;
        }
        public int GetdistributionValue(OrderItem orderitem)
        {
            var ValueAddedbyAgent = _OrderItemAttributeValueRepository.Table.SingleOrDefault(p => p.OrderItemId == orderitem.Id && p.PropertyAttrName.Contains("هزینه توزیع"))
                ?.PropertyAttrValueText;
            if (!string.IsNullOrEmpty(ValueAddedbyAgent))
            {
                return ValueAddedbyAgent.ToEnDigit();
            }
            return 0;
        }
        #endregion

        #region مبالغ اضافه
        public int CalcHagheSabet(int customerId, int ServiceId, int OrderId = 0)
        {
            if (IsPostService(ServiceId))
                return 0;
            SqlParameter[] prms = new SqlParameter[] {
                new SqlParameter() { ParameterName = "Int_CustomerId", SqlDbType = SqlDbType.Int, Value = customerId },
                new SqlParameter() { ParameterName = "ServiceId", SqlDbType = SqlDbType.Int, Value = ServiceId },
                new SqlParameter() { ParameterName = "OrderId", SqlDbType = SqlDbType.Int, Value = OrderId }
            };
            int? price = _dbContext.SqlQuery<int>(@" EXECUTE [dbo].[Bil_Sp_CalcHagheSabt] @Int_CustomerId,@ServiceId,@OrderId ", prms).FirstOrDefault();
            return price.GetValueOrDefault(0);
        }
        #endregion

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
            var data = _dbContext.SqlQuery<CategoryInfoModel>(query, prms).FirstOrDefault();
            return data;
        }
        public CategoryInfoModel GetCategoryInfo(Nop.Core.Domain.Catalog.Product product)
        {
            int catId = ((int?)product.ProductCategories.OrderByDescending(p => p.Id).FirstOrDefault()?.CategoryId).GetValueOrDefault(0);
            return GetCategoryInfo(catId);
        }
        public void LogException(Exception exception)
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var logger = EngineContext.Current.Resolve<ILogger>();

            var customer = workContext.CurrentCustomer;
            logger.Error(exception.Message, exception, customer);
        }
        public bool setDataCollect(int shipmentId)
        {
            //try
            //{
            var shipment = _shipmentService.GetShipmentById(shipmentId);
            int SourceId = getSourceByOrder(shipment.OrderId);
            if (SourceId != 16)
            {
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //var basicHttpbinding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                //basicHttpbinding.Name = "CODContractsService";
                //basicHttpbinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                //basicHttpbinding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                //var endpointAddress =
                //          new EndpointAddress("http://gateway.post.ir/Deliver.asmx");
                //var COD = new WebService1SoapClient(basicHttpbinding, endpointAddress);
                //COD.ChangeStatus("postbar", "postbar12345", shipment.TrackingNumber, 0);
            }
            var SAR = _ShipmentAppointmentRepository.Table.OrderByDescending(p => p.Id)
                    .SingleOrDefault(p => p.ShipmentId == shipmentId);
            DateTime dt = DateTime.Now;
            if (SAR == null)
            {
                SAR = new ShipmentAppointmentModel()
                {
                    ShipmentId = shipmentId,
                    DataCollect = dt
                };
                _ShipmentAppointmentRepository.Insert(SAR);
                return true;
            }
            SAR.DataCollect = dt;
            _ShipmentAppointmentRepository.Update(SAR);
            _notificationService.NotifyCollectShipment(shipmentId);
            return true;
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
        }
        public bool IsShipmentCollected(int shipmentId)
        {
            return _ShipmentAppointmentRepository.Table.Any(p => p.ShipmentId == shipmentId && p.DataCollect.HasValue); ;
        }
        public bool IsMultiShippment(Order order)
        {
            var str_IsMultiShippment = order.Customer.GetAttribute<string>("IsOrderMultishipment_" + order.Id, _genericAttributeService
                , order.StoreId);
            if (string.IsNullOrEmpty(str_IsMultiShippment))
                return false;
            return true;
        }
        public List<SelectListItem> ListOfService()
        {
            string query = @"SELECT
	                            DISTINCT P.Name AS [Text]
	                            ,Cast(p.Id as Varchar(10)) AS [Value]
                            FROM
	                            dbo.Category AS C
	                            INNER JOIN dbo.Product_Category_Mapping AS PCM ON PCM.CategoryId = C.Id
	                            INNER JOIN dbo.StoreMapping AS SM ON sm.EntityId = C.Id AND sm.EntityName ='Category'
	                            INNER JOIN dbo.CateguryPostType AS CPT ON CPT.CateguryId = C.Id
	                            INNER JOIN dbo.Product AS P ON P.Id = PCM.ProductId
                            WHERE	
	                            C.ParentCategoryId <> 0
	                            AND C.Deleted = 0
	                            AND C.Published = 1
	                            AND p.Deleted = 0
	                            AND p.Published = 1
	                            AND SM.StoreId = 3
                            ORDER BY p.Name";
            return _dbContext.SqlQuery<SelectListItem>(query, new object[0]).AsEnumerable<SelectListItem>().ToList();
        }
        public void MarkOrder(OrderRegistrationMethod orderMethod, Order order)
        {
            int OrderDataSourceId = (int)orderMethod;
            int _OrderDataSourceId = (OrderDataSourceId == 4 || OrderDataSourceId == 5 || OrderDataSourceId == 1 ? 1 : OrderDataSourceId);
            InsertOrderSource(order.Id, _OrderDataSourceId);
            _genericAttributeService.SaveAttribute<string>(order, "OrderRegistrationMethod",
           ((int)orderMethod).ToString(), order.StoreId);
        }
        public OrderRegistrationMethod GetOrderRegistrationMethod(Order order)
        {
            var data = _genericAttributeService.GetAttributesForEntity(order.Id, "Order").Where(p => p.Key == "OrderRegistrationMethod").FirstOrDefault()?.Value;
            if (!string.IsNullOrEmpty(data))
            {
                return (OrderRegistrationMethod)int.Parse(data);
            }
            return OrderRegistrationMethod.none;
        }

        #region Order List

        public List<CustomOrder> SearchOrders(out CoardinationStatisticModel CoardinationStatistic, out int count, int storeId = 0,
           int vendorId = 0, int customerId = 0,
           int productId = 0, int affiliateId = 0, int warehouseId = 0,
           int billingCountryId = 0, string paymentMethodSystemName = null,
           DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
           List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
           string billingEmail = null, string billingLastName = "",
           string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue,
           int SenderStateProvinceId = 0, int ReciverCountryId = 0, int ReciverStateProvinceId = 0,
           string ReciverName = null, string SenderName = null, int OrderId = 0, bool IsOrderOutDate = false, int orderState = 0)
        {
            CoardinationStatistic = new CoardinationStatisticModel();
            var query = _orderRepository.Table;

            if (storeId > 0)
                query = query.Where(o => o.StoreId == storeId);
            if (vendorId > 0)
            {
                query = query
                    .Where(o => o.OrderItems
                    .Any(orderItem => orderItem.Product.VendorId == vendorId));
            }
            if (customerId > 0)
                query = query.Where(o => o.CustomerId == customerId);
            if (productId > 0)
            {
                query = query
                    .Where(o => o.OrderItems
                    .Any(orderItem => orderItem.Product.Id == productId));
            }
            if (warehouseId > 0)
            {
                var manageStockInventoryMethodId = (int)ManageInventoryMethod.ManageStock;
                query = query
                    .Where(o => o.OrderItems
                    .Any(orderItem =>
                        //"Use multiple warehouses" enabled
                        //we search in each warehouse
                        (orderItem.Product.ManageInventoryMethodId == manageStockInventoryMethodId &&
                        orderItem.Product.UseMultipleWarehouses &&
                        orderItem.Product.ProductWarehouseInventory.Any(pwi => pwi.WarehouseId == warehouseId))
                        ||
                        //"Use multiple warehouses" disabled
                        //we use standard "warehouse" property
                        ((orderItem.Product.ManageInventoryMethodId != manageStockInventoryMethodId ||
                        !orderItem.Product.UseMultipleWarehouses) &&
                        orderItem.Product.WarehouseId == warehouseId))
                        );
            }
            if (billingCountryId > 0)
                query = query.Where(o => o.BillingAddress != null && o.BillingAddress.CountryId.HasValue && o.BillingAddress.CountryId == billingCountryId);
            if (!string.IsNullOrEmpty(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);
            if (affiliateId > 0)
                query = query.Where(o => o.AffiliateId == affiliateId);
            if (createdFromUtc.HasValue)
                query = query.Where(o => createdFromUtc.Value <= o.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(o => createdToUtc.Value >= o.CreatedOnUtc);
            if (osIds != null && osIds.Any())
                query = query.Where(o => osIds.Contains(o.OrderStatusId));
            if (psIds != null && psIds.Any())
                query = query.Where(o => psIds.Contains(o.PaymentStatusId));
            if (ssIds != null && ssIds.Any())
                query = query.Where(o => ssIds.Contains(o.ShippingStatusId));
            if (!string.IsNullOrEmpty(billingEmail))
                query = query.Where(o => o.BillingAddress != null && !string.IsNullOrEmpty(o.BillingAddress.Email) && o.BillingAddress.Email.Contains(billingEmail));

            if (SenderStateProvinceId > 0)
            {
                query = query.Where(p => p.BillingAddress != null && p.BillingAddress.StateProvinceId.HasValue && p.BillingAddress.StateProvinceId == SenderStateProvinceId);
            }
            if (ReciverCountryId > 0)
            {
                query = query.Where(p => p.ShippingAddress != null && p.ShippingAddress.CountryId == ReciverCountryId);
            }
            if (ReciverStateProvinceId > 0)
            {
                query = query.Where(p => p.ShippingAddress != null && p.ShippingAddress.StateProvinceId == ReciverStateProvinceId);
            }
            if (!string.IsNullOrEmpty(ReciverName))
            {
                query = query.Where(p => p.ShippingAddress != null
                && (!string.IsNullOrEmpty(p.ShippingAddress.FirstName) || !string.IsNullOrEmpty(p.ShippingAddress.LastName))
                && ((p.ShippingAddress.FirstName ?? string.Empty) + " " + (p.ShippingAddress.LastName ?? string.Empty)).Contains(ReciverName));
            }
            if (!string.IsNullOrEmpty(SenderName))
            {
                query = query.Where(p => p.BillingAddress != null
                && (!string.IsNullOrEmpty(p.BillingAddress.FirstName) || !string.IsNullOrEmpty(p.BillingAddress.LastName))
                && ((p.BillingAddress.FirstName ?? string.Empty) + " " + (p.BillingAddress.LastName ?? string.Empty)).Contains(SenderName));
            }

            if (!string.IsNullOrEmpty(billingLastName))
                query = query.Where(o => o.BillingAddress != null && !string.IsNullOrEmpty(o.BillingAddress.LastName) && o.BillingAddress.LastName.Contains(billingLastName));
            if (!string.IsNullOrEmpty(orderNotes))
                query = query.Where(o => o.OrderNotes.Any(on => on.Note.Contains(orderNotes)));

            query = query.Where(o => !o.Deleted);
            if (IsOrderOutDate)
            {
                var mindate = DateTime.Now.AddDays(-5);
                query = query.Where(p => p.OrderItems.All(t => t.Product.IsShipEnabled)
                                 && (!p.Shipments.Any()
                                     || p.Shipments.Any(n => !n.DeliveryDateUtc.HasValue))
                                 && p.CreatedOnUtc <= mindate);
            }
            if (_workContext.CurrentCustomer.IsInCustomerRole("Ap-CallSenter"))
            {
                var ApOrder = getorderBySource(6);
                query = query.Where(p => ApOrder.Contains(p.Id));
            }
            if (orderState != 0)
            {
                var orderbyState = getOrderByState(orderState);
                if (!orderbyState.Any())
                {
                    count = 0;
                    return new List<CustomOrder>();
                }
                query = query.Where(p => orderbyState.Contains(p.Id));
            }

            var Ids = query.Select(p => p.Id);
            CoardinationStatistic = new CoardinationStatisticModel();
            //var strIds = string.Join(",", Ids.ToList());
            //CoardinationStatistic = GetOrderStateStatistic(strIds, customerId);
            if (!_workContext.CurrentCustomer.IsInCustomerRole("Administrators"))
            {
                var UserStates = _repositoryUserStetes.Table.Where(p => p.CustomerId == _workContext.CurrentCustomer.Id)
                    .Select(p => p.StateId).ToList();
                query = query.Where(p => UserStates.Contains(p.BillingAddress.StateProvinceId ?? 0));
            }
            query = query.OrderByDescending(o => o.CreatedOnUtc);
            count = query.Count();

            //int BoxCount = query.Where(p=> p.CreatedOnUtc >= DateTime.UtcNow.AddDays(-10)).Sum(s => s.OrderItems.Sum(n => n.Quantity));


            //int CustomerCount = query.Where(p => p.CreatedOnUtc >= DateTime.UtcNow.AddDays(-10)).DistinctBy(d=> d.CustomerId).Count();

            //int RouteCount = query.Where(p => p.CreatedOnUtc >= DateTime.UtcNow.AddDays(-10)).DistinctBy(d => d.BillingAddress.CountryId.ToString()+
            //                                                                                                  d.BillingAddress.StateProvinceId.ToString()+
            //                                                                                                  d.BillingAddress.Address1).Count();

            //database layer paging
            var orderPageedList = new PagedList<Order>(query, pageIndex, pageSize).ToList();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Order, CustomOrder>();
            });
            IMapper mapper = config.CreateMapper();
            var data = (List<CustomOrder>)mapper.Map(orderPageedList, typeof(List<Order>), typeof(List<CustomOrder>));
            var InternalPostForForeign = getInternalPostForForeign();
            if (InternalPostForForeign.Any())
            {
                data.Select(p => { p.IsInternalForForeign = (InternalPostForForeign.Contains(p.Id) ? true : false); return p; }).ToList();
            }
            #region PostCoardination
            var orderIds = data.Select(c => c.Id).ToList();

            var pc = _PostCoordinationRepository.Table.Where(p => orderIds.Contains(p.orderId)).ToList();
            foreach (var item in pc)
            {
                data.Single(p => p.Id == item.orderId).CoordinationDate = item.CoordinationDate;
            }
            var Fo = CheckFistOrder(orderIds);
            foreach (var item in Fo)
            {
                data.Single(p => p.Id == item).IsFirstORder = true;
            }
            var printNeeds = CheckNeedPrint(orderIds);
            foreach (var item in printNeeds)
            {
                data.Single(p => p.Id == item).NeedPrinter = true;
            }
            var needCartons = CheckNeedCarton(orderIds);
            foreach (var item in needCartons)
            {
                var currItem = data.Single(p => p.Id == item.OrderId);
                currItem.NeedCarton = true;
                currItem.CartonSizeName = item.CartonSizeName;
            }
            var _ubaar = CheckIsUbaar(orderIds);
            foreach (var item in _ubaar)
            {
                data.Single(p => p.Id == item).IsUbaar = true;
            }
            #endregion

            return data;
        }
        private List<int> getOrderByState(int State)
        {
            string query = $@"EXEC dbo.Sp_getOrderByOrderStatus @CustomerId,@OrderState";

            SqlParameter P_CustomerId = new SqlParameter()
            {
                ParameterName = "CustomerId",
                SqlDbType = SqlDbType.Int,
                Value = (object)_workContext.CurrentCustomer.Id
            };
            SqlParameter P_orderState = new SqlParameter()
            {
                ParameterName = "OrderState",
                SqlDbType = SqlDbType.Int,
                Value = (object)State
            };
            SqlParameter[] prms = new SqlParameter[]{
                P_CustomerId,
                P_orderState
            };
            return _dbContext.SqlQuery<int>(query, prms).ToList();
        }
        private List<int> getInternalPostForForeign()
        {
            string query = $@"SELECT
	                            DISTINCT TRO.ChildOrderId
                            FROM
	                            dbo.[Order] AS O
	                            INNER JOIN dbo.OrderItem AS OI ON OI.OrderId = O.Id
	                            INNER JOIN dbo.Product AS P ON P.Id = OI.ProductId
	                            INNER JOIN dbo.Product_Category_Mapping AS PCM ON PCM.ProductId = P.Id
	                            INNER JOIN dbo.Category AS C ON C.Id = PCM.CategoryId
	                            INNER JOIN dbo.Tb_CategoryInfo AS TCI ON C.id = TCI.CategoryId
	                            INNER JOIN dbo.Tb_RelatedOrders AS TRO ON TRO.ParentOrderId = O.Id
                            WHERE
	                            TCI.IsForeign = 1
	                            AND o.Deleted = 0
	                            AND o.CreatedOnUtc > GETUTCDATE() - 15
	                            AND O.OrderStatusId <> 40";
            return _dbContext.SqlQuery<int>(query, new object[0]).ToList();
        }

        public List<int> getorderBySource(int Source)
        {
            string query = $@"SELECT
	                            O.Id
                            FROM
	                            dbo.[Order] AS O
	                            INNER JOIN dbo.Tb_OrderSource AS TOS ON TOS.OrderId = O.Id
                            WHERE
	                            TOS.SourceId = {Source}
	                            AND O.CreatedOnUtc >= GETUTCDATE() -10";
            return _dbContext.SqlQuery<int>(query, new object[0]).ToList();
        }

        public int getSourceByOrder(int OrderId)
        {
            string query = $@"SELECT 
	                            SourceId
                            FROM dbo.Tb_OrderSource
                            WHERE OrderId = {OrderId}";
            return _dbContext.SqlQuery<int?>(query).FirstOrDefault().GetValueOrDefault(0);
        }
        public List<KeyValuePair<int, int>> getOrderByStateCount()
        {
            string query = $@"EXEC dbo.Sp_getOrderByOrderStatus_Count @CustomerId,@OrderState";
            List<KeyValuePair<int, int>> data = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < 4; i++)
            {

                SqlParameter P_CustomerId = new SqlParameter()
                {
                    ParameterName = "CustomerId",
                    SqlDbType = SqlDbType.Int,
                    Value = (object)_workContext.CurrentCustomer.Id
                };
                SqlParameter P_orderState = new SqlParameter()
                {
                    ParameterName = "OrderState",
                    SqlDbType = SqlDbType.Int,
                    Value = (object)i
                };
                SqlParameter[] prms = new SqlParameter[]{
                P_CustomerId,
                P_orderState
                };
                var result = _dbContext.SqlQuery<int?>(query, prms).FirstOrDefault().GetValueOrDefault(0);
                data.Add(new KeyValuePair<int, int>(i, result));
            }
            for (int i = 11; i < 23; i++)
            {

                SqlParameter P_CustomerId = new SqlParameter()
                {
                    ParameterName = "CustomerId",
                    SqlDbType = SqlDbType.Int,
                    Value = (object)_workContext.CurrentCustomer.Id
                };
                SqlParameter P_orderState = new SqlParameter()
                {
                    ParameterName = "OrderState",
                    SqlDbType = SqlDbType.Int,
                    Value = (object)i
                };
                SqlParameter[] prms = new SqlParameter[]{
                P_CustomerId,
                P_orderState
                };
                var result = _dbContext.SqlQuery<int?>(query, prms).FirstOrDefault().GetValueOrDefault(0);
                data.Add(new KeyValuePair<int, int>(i, result));
            }
            return data;
        }
        public List<node> OrdersOverView(int orderState)
        {
            string query = $@"EXEC dbo.Sp_OrderTreeView @CustomerId,@OrderState";

            SqlParameter P_CustomerId = new SqlParameter()
            {
                ParameterName = "CustomerId",
                SqlDbType = SqlDbType.Int,
                Value = (object)_workContext.CurrentCustomer.Id
            };
            SqlParameter P_orderState = new SqlParameter()
            {
                ParameterName = "OrderState",
                SqlDbType = SqlDbType.Int,
                Value = (object)orderState
            };
            SqlParameter[] prms = new SqlParameter[]{
                P_CustomerId,
                P_orderState
            };
            var result = _dbContext.SqlQuery<OrderTreeViewModel>(query, prms).ToList();
            List<node> ParentNode = new List<node>();
            if (result.Any())
            {
                foreach (var countryId in result.Select(p => p.CountryId).Distinct())
                {
                    var country = result.First(p => p.CountryId == countryId);
                    int CountryCount = result.Where(p => p.CountryId == countryId).Sum(p => p._Count);
                    var country_CatgoryGroup = result.Where(p => p.CountryId == countryId).GroupBy(p => p.CategoryName).Select(y =>
                     new CategoryItem()
                     {
                         CategoryName = y.Key,
                         CategoryCount = y.Sum(S => S._Count)
                     }).ToList();
                    node CountryNode = new node()
                    {
                        text = country.CountryName,
                        value = country.CountryId.ToString(),
                        tags = new List<string>() { CountryCount.ToString() },
                        nodes = new List<node>(),
                        CategoryItems = country_CatgoryGroup,
                        FilterLevel = "country"
                    };
                    foreach (var StateId in result.Where(p => p.CountryId == countryId).Select(p => p.StateId).Distinct())
                    {
                        var state = result.First(p => p.StateId == StateId);
                        int StateCount = result.Where(p => p.StateId == StateId).Sum(p => p._Count);
                        var state_CatgoryGroup = result.Where(p => p.StateId == StateId).GroupBy(p => p.CategoryName).Select(y =>
                            new CategoryItem()
                            {
                                CategoryName = y.Key,
                                CategoryCount = y.Sum(S => S._Count)
                            }).ToList();
                        node StateNode = new node()
                        {
                            text = state.StateName,
                            value = state.StateId.ToString(),
                            tags = new List<string>() { StateCount.ToString() },
                            nodes = new List<node>(),
                            CategoryItems = state_CatgoryGroup,
                            FilterLevel = "state"
                        };
                        foreach (var customerId in result.Where(p => p.StateId == StateId).Select(p => p.CustomerId).Distinct())
                        {
                            var customer = result.First(p => p.CustomerId == customerId && p.StateId == StateId);
                            int customerCount = result.Where(p => p.CustomerId == customerId && p.StateId == StateId).Sum(p => p._Count);
                            var customer_CatgoryGroup = result.Where(p => p.CustomerId == customerId && p.StateId == StateId).GroupBy(p => p.CategoryName).Select(y =>
                               new CategoryItem()
                               {
                                   CategoryName = y.Key,
                                   CategoryCount = y.Sum(S => S._Count)
                               }).ToList();
                            node CustomerNode = new node()
                            {
                                text = customer.Username,
                                value = customer.CustomerId.ToString(),
                                tags = new List<string>() { customerCount.ToString() },
                                CategoryItems = customer_CatgoryGroup,
                                FilterLevel = "customer"
                            };
                            StateNode.nodes.Add(CustomerNode);
                        }
                        CountryNode.nodes.Add(StateNode);
                    }
                    ParentNode.Add(CountryNode);
                }
            }
            return ParentNode;
            //if (!_workContext.CurrentCustomer.IsInCustomerRole("Administrators"))
            //{
            //    var UserStates = _repositoryUserStetes.Table.Where(p => p.CustomerId == _workContext.CurrentCustomer.Id)
            //        .Select(p => p.StateId).ToList();
            //    query = query.Where(p => UserStates.Contains(p.BillingAddress.StateProvinceId ?? 0));
            //}
        }
        public List<node> ShipmentTreeView(int orderState)
        {
            string query = $@"EXEC dbo.Sp_ShipmentTreeView @CustomerId,@CurrentStore,@OrderState";

            SqlParameter P_CustomerId = new SqlParameter()
            {
                ParameterName = "CustomerId",
                SqlDbType = SqlDbType.Int,
                Value = (object)_workContext.CurrentCustomer.Id
            };
            SqlParameter P_orderState = new SqlParameter()
            {
                ParameterName = "OrderState",
                SqlDbType = SqlDbType.Int,
                Value = (object)orderState
            };
            SqlParameter P_CurrentStore = new SqlParameter()
            {
                ParameterName = "CurrentStore",
                SqlDbType = SqlDbType.Int,
                Value = (object)_storeContext.CurrentStore.Id
            };
            SqlParameter[] prms = new SqlParameter[]{
                P_CustomerId,
                P_orderState,
                P_CurrentStore
            };
            var result = _dbContext.SqlQuery<OrderTreeViewModel>(query, prms).ToList();
            List<node> ParentNode = new List<node>();
            if (result.Any())
            {
                foreach (var countryId in result.Select(p => p.CountryId).Distinct())
                {
                    var country = result.First(p => p.CountryId == countryId);
                    int CountryCount = result.Where(p => p.CountryId == countryId).Sum(p => p._Count);
                    var country_CatgoryGroup = result.Where(p => p.CountryId == countryId).GroupBy(p => p.CategoryName).Select(y =>
                     new CategoryItem()
                     {
                         CategoryName = y.Key,
                         CategoryCount = y.Sum(S => S._Count)
                     }).ToList();
                    node CountryNode = new node()
                    {
                        text = country.CountryName,
                        value = country.CountryId.ToString(),
                        tags = new List<string>() { CountryCount.ToString() },
                        nodes = new List<node>(),
                        CategoryItems = country_CatgoryGroup,
                        FilterLevel = "country"
                    };
                    foreach (var StateId in result.Where(p => p.CountryId == countryId).Select(p => p.StateId).Distinct())
                    {
                        var state = result.First(p => p.StateId == StateId);
                        int StateCount = result.Where(p => p.StateId == StateId).Sum(p => p._Count);
                        var state_CatgoryGroup = result.Where(p => p.StateId == StateId).GroupBy(p => p.CategoryName).Select(y =>
                            new CategoryItem()
                            {
                                CategoryName = y.Key,
                                CategoryCount = y.Sum(S => S._Count)
                            }).ToList();
                        node StateNode = new node()
                        {
                            text = state.StateName,
                            value = state.StateId.ToString(),
                            tags = new List<string>() { StateCount.ToString() },
                            nodes = new List<node>(),
                            CategoryItems = state_CatgoryGroup,
                            FilterLevel = "state"
                        };
                        foreach (var customerAddress in result.Where(p => p.StateId == StateId).Select(p => new { p.CustomerId, p.Username }).Distinct())
                        {
                            var customer = result.First(p => (p.CustomerId == customerAddress.CustomerId && p.Username == customerAddress.Username) && p.StateId == StateId);
                            int customerCount = result.Where(p => (p.CustomerId == customerAddress.CustomerId && p.Username == customerAddress.Username) && p.StateId == StateId).Sum(p => p._Count);
                            var customer_CatgoryGroup = result.Where(p => (p.CustomerId == customerAddress.CustomerId && p.Username == customerAddress.Username) && p.StateId == StateId).GroupBy(p => p.CategoryName).Select(y =>
                               new CategoryItem()
                               {
                                   CategoryName = y.Key,
                                   CategoryCount = y.Sum(S => S._Count)
                               }).ToList();
                            node CustomerNode = new node()
                            {
                                text = customer.Username,
                                value = customer.CustomerId.ToString(),
                                tags = new List<string>() { customerCount.ToString() },
                                CategoryItems = customer_CatgoryGroup,
                                FilterLevel = "customer"
                            };
                            StateNode.nodes.Add(CustomerNode);
                        }
                        CountryNode.nodes.Add(StateNode);
                    }
                    ParentNode.Add(CountryNode);
                }
            }
            return ParentNode;
            //if (!_workContext.CurrentCustomer.IsInCustomerRole("Administrators"))
            //{
            //    var UserStates = _repositoryUserStetes.Table.Where(p => p.CustomerId == _workContext.CurrentCustomer.Id)
            //        .Select(p => p.StateId).ToList();
            //    query = query.Where(p => UserStates.Contains(p.BillingAddress.StateProvinceId ?? 0));
            //}
        }


        public List<node> ShipmentTreeViewByStatus(int orderState, OrderShipmentStatusEnum shipmentStatusEnum, int categoryId)
        {
            string query = $@"EXEC dbo.Sp_ShipmentTreeViewByStatus @CustomerId,@CurrentStore,@OrderState,@StatusId,@CategoryId";

            SqlParameter P_CustomerId = new SqlParameter()
            {
                ParameterName = "CustomerId",
                SqlDbType = SqlDbType.Int,
                Value = (object)_workContext.CurrentCustomer.Id
            };
            SqlParameter P_orderState = new SqlParameter()
            {
                ParameterName = "OrderState",
                SqlDbType = SqlDbType.Int,
                Value = (object)orderState
            };
            SqlParameter P_CurrentStore = new SqlParameter()
            {
                ParameterName = "CurrentStore",
                SqlDbType = SqlDbType.Int,
                Value = (object)_storeContext.CurrentStore.Id
            };
            SqlParameter P_OrderStatusId = new SqlParameter()
            {
                ParameterName = "StatusId",
                SqlDbType = SqlDbType.Int,
                Value = (object)(int)shipmentStatusEnum
            };
            SqlParameter P_CategoryId = new SqlParameter()
            {
                ParameterName = "CategoryId",
                SqlDbType = SqlDbType.Int,
                Value = (object)(int)categoryId
            };
            SqlParameter[] prms = new SqlParameter[]{
                P_CustomerId,
                P_orderState,
                P_CurrentStore,
                P_OrderStatusId
            };
            var result = _dbContext.SqlQuery<OrderTreeViewModel>(query, prms).ToList();
            List<node> ParentNode = new List<node>();
            if (result.Any())
            {
                foreach (var countryId in result.Select(p => p.CountryId).Distinct())
                {
                    var country = result.First(p => p.CountryId == countryId);
                    int CountryCount = result.Where(p => p.CountryId == countryId).Sum(p => p._Count);
                    var country_CatgoryGroup = result.Where(p => p.CountryId == countryId).GroupBy(p => p.CategoryName).Select(y =>
                     new CategoryItem()
                     {
                         CategoryName = y.Key,
                         CategoryCount = y.Sum(S => S._Count)
                     }).ToList();
                    node CountryNode = new node()
                    {
                        text = country.CountryName,
                        value = country.CountryId.ToString(),
                        tags = new List<string>() { CountryCount.ToString() },
                        nodes = new List<node>(),
                        CategoryItems = country_CatgoryGroup,
                        FilterLevel = "country"
                    };
                    foreach (var StateId in result.Where(p => p.CountryId == countryId).Select(p => p.StateId).Distinct())
                    {
                        var state = result.First(p => p.StateId == StateId);
                        int StateCount = result.Where(p => p.StateId == StateId).Sum(p => p._Count);
                        var state_CatgoryGroup = result.Where(p => p.StateId == StateId).GroupBy(p => p.CategoryName).Select(y =>
                            new CategoryItem()
                            {
                                CategoryName = y.Key,
                                CategoryCount = y.Sum(S => S._Count)
                            }).ToList();
                        node StateNode = new node()
                        {
                            text = state.StateName,
                            value = state.StateId.ToString(),
                            tags = new List<string>() { StateCount.ToString() },
                            nodes = new List<node>(),
                            CategoryItems = state_CatgoryGroup,
                            FilterLevel = "state"
                        };
                        foreach (var customerAddress in result.Where(p => p.StateId == StateId).Select(p => new { p.CustomerId, p.Username }).Distinct())
                        {
                            var customer = result.First(p => (p.CustomerId == customerAddress.CustomerId && p.Username == customerAddress.Username) && p.StateId == StateId);
                            int customerCount = result.Where(p => (p.CustomerId == customerAddress.CustomerId && p.Username == customerAddress.Username) && p.StateId == StateId).Sum(p => p._Count);
                            var customer_CatgoryGroup = result.Where(p => (p.CustomerId == customerAddress.CustomerId && p.Username == customerAddress.Username) && p.StateId == StateId).GroupBy(p => p.CategoryName).Select(y =>
                               new CategoryItem()
                               {
                                   CategoryName = y.Key,
                                   CategoryCount = y.Sum(S => S._Count)
                               }).ToList();
                            node CustomerNode = new node()
                            {
                                text = customer.Username,
                                value = customer.CustomerId.ToString(),
                                tags = new List<string>() { customerCount.ToString() },
                                CategoryItems = customer_CatgoryGroup,
                                FilterLevel = "customer"
                            };
                            StateNode.nodes.Add(CustomerNode);
                        }
                        CountryNode.nodes.Add(StateNode);
                    }
                    ParentNode.Add(CountryNode);
                }
            }
            return ParentNode;
            //if (!_workContext.CurrentCustomer.IsInCustomerRole("Administrators"))
            //{
            //    var UserStates = _repositoryUserStetes.Table.Where(p => p.CustomerId == _workContext.CurrentCustomer.Id)
            //        .Select(p => p.StateId).ToList();
            //    query = query.Where(p => UserStates.Contains(p.BillingAddress.StateProvinceId ?? 0));
            //}
        }
        public class node
        {
            public string text { get; set; }
            public string value { get; set; }
            public string href { get; set; }
            public List<string> tags { get; set; }
            public List<node> nodes { get; set; }
            public List<CategoryItem> CategoryItems { get; set; }
            public string FilterLevel { get; set; }
        }
        public class OrderTreeViewModel
        {
            public string CountryName { get; set; }
            public int CountryId { get; set; }
            public string StateName { get; set; }
            public int StateId { get; set; }
            public string Username { get; set; }
            public int CustomerId { get; set; }
            public int _Count { get; set; }
            public bool NeedPaging { get; set; }
            public string CategoryName { get; set; }
        }
        public class CategoryItem
        {
            public int CategoryCount { get; set; }
            public string CategoryName { get; set; }
        }

        public CoardinationStatisticModel GetOrderStateStatistic(string orderIds, int customerId)
        {
            //List<int> orderIds = getOrderByState(state);
            SqlParameter P_CustomerId = new SqlParameter()
            {
                ParameterName = "CustomerId",
                SqlDbType = SqlDbType.Int,
                Value = (object)customerId
            };
            SqlParameter P_OrderIds = new SqlParameter()
            {
                ParameterName = "OrderIds",
                SqlDbType = SqlDbType.VarChar,
                Value = (object)(orderIds)
            };
            SqlParameter[] prms = new SqlParameter[]{
                P_CustomerId,
                P_OrderIds
            };
            return _dbContext.SqlQuery<CoardinationStatisticModel>(@"EXECUTE [dbo].[Sp_CoardinationStatistic] @CustomerId,@OrderIds", prms).FirstOrDefault();
        }
        #region FirstOrder
        private List<int> CheckFistOrder(List<int> orderIds)
        {
            if (orderIds.Count == 0)
                return new List<int>();
            string Query = @"SELECT
	                        FO.OrderId
                        FROM
	                        dbo.FirstOrder AS FO
                        WHERE
	                        FO.OrderId IN (" + string.Join(",", orderIds) + ")";
            return _dbContext.SqlQuery<int>(Query, new object[0]).ToList();
        }



        public int GetOrdersByCustomerId(int customerId)
        {
            //if (customerId == 273739)
            //    return 0;
            if (customerId == 0)
                return -1;
            string query = $@"SELECT
	                            COUNT(DISTINCT O.Id) _count
                            FROM
	                            dbo.[Order] AS O
	                            INNER JOIN dbo.Shipment AS S ON S.OrderId = O.Id
                            WHERE
	                            O.OrderStatusId <> 40
                                AND O.Deleted = 0
	                            AND O.CustomerId = " + customerId;
            return _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
        }
        public float getFistOrderDiscount()
        {

            string query = $@"SELECT
	                            top(1) TDPAC._Percent
                            FROM
	                            dbo.Tbl_DiscountPlan_AgentCustomer AS TDPAC
                            WHERE
	                            TDPAC.isForFistOrder = 1
                            ORDER BY id DESC";
            return _dbContext.SqlQuery<float?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
        }
        #endregion
        private List<int> CheckNeedPrint(List<int> orderIds)
        {
            if (orderIds.Count == 0)
                return new List<int>();
            string Query = @"SELECT
	                            DISTINCT OI.OrderId
                            FROM
	                            dbo.Tb_OrderItemAttributeValue AS TOIAV
	                            INNER JOIN dbo.ProductAttribute AS PA ON PA.Id = TOIAV.PropertyAttrId
	                            INNER JOIN dbo.ProductAttributeValue AS PAV ON PAV.Id = TOIAV.PropertyAttrValueId
	                            INNER JOIN dbo.OrderItem AS OI ON OI.Id = TOIAV.OrderItemId
                            WHERE
	                            Oi.OrderId IN (" + string.Join(",", orderIds) + @")
                                AND pa.Name LIKE N'%دسترسی به پرینتر%'
	                            AND PAV.Name LIKE N'%خیر%'";
            return _dbContext.SqlQuery<int>(Query, new object[0]).ToList();
        }
        private List<int> CheckIsUbaar(List<int> orderIds)
        {
            if (orderIds.Count == 0)
                return new List<int>();
            string Query = @"SELECT
	                            DISTINCT OI.OrderId
                            FROM
	                             dbo.OrderItem AS OI 
                            WHERE
	                            Oi.OrderId IN (" + string.Join(",", orderIds) + @")
	                            AND  oi.ProductId=10412";
            return _dbContext.SqlQuery<int>(Query, new object[0]).ToList();
        }
        private List<NeedCarton> CheckNeedCarton(List<int> orderIds)
        {
            if (orderIds.Count == 0)
                return new List<NeedCarton>();
            string Query = @"SELECT
	                            DISTINCT OI.OrderId,Pav.Name As CartonSizeName
                            FROM
	                            dbo.Tb_OrderItemAttributeValue AS TOIAV
	                            INNER JOIN dbo.ProductAttribute AS PA ON PA.Id = TOIAV.PropertyAttrId
	                            INNER JOIN dbo.ProductAttributeValue AS PAV ON PAV.Id = TOIAV.PropertyAttrValueId
	                            INNER JOIN dbo.OrderItem AS OI ON OI.Id =TOIAV.OrderItemId
                            WHERE
	                            Oi.OrderId IN (" + string.Join(",", orderIds) + @")
                                AND pa.Name LIKE N'%کارتن و لفاف بندی%'
	                            AND PAV.Name <> N'کارتن نیاز ندارم.'";
            return _dbContext.SqlQuery<NeedCarton>(Query, new object[0]).ToList();
        }
        public class NeedCarton
        {
            public int OrderId { get; set; }
            public string CartonSizeName { get; set; }
        }
        /// <summary>
        /// Get order average report
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to ignore this parameter</param>
        /// <param name="vendorId">Vendor identifier; pass 0 to ignore this parameter</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all orders</param>
        /// <param name="orderId">Order identifier; pass 0 to ignore this parameter</param>
        /// <param name="paymentMethodSystemName">Payment method system name; null to load all records</param>
        /// <param name="osIds">Order status identifiers</param>
        /// <param name="psIds">Payment status identifiers</param>
        /// <param name="ssIds">Shipping status identifiers</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="billingLastName">Billing last name. Leave empty to load all records.</param>
        /// <param name="orderNotes">Search in order notes. Leave empty to load all records.</param>
        /// <returns>Result</returns>
        public OrderAverageReportLine GetOrderAverageReportLine(int storeId = 0,
            int vendorId = 0, int billingCountryId = 0,
            int orderId = 0, string paymentMethodSystemName = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
            DateTime? startTimeUtc = null, DateTime? endTimeUtc = null,
            string billingEmail = null, string billingLastName = "", string orderNotes = null,
            int SenderStateProvinceId = 0, int ReciverCountryId = 0, int ReciverStateProvinceId = 0,
            string ReciverName = null, string SenderName = null)
        {
            var query = _orderRepository.Table;
            query = query.Where(o => !o.Deleted);
            if (storeId > 0)
                query = query.Where(o => o.StoreId == storeId);
            if (orderId > 0)
                query = query.Where(o => o.Id == orderId);
            if (vendorId > 0)
            {
                query = query
                    .Where(o => o.OrderItems
                    .Any(orderItem => orderItem.Product.VendorId == vendorId));
            }
            if (billingCountryId > 0)
                query = query.Where(o => o.BillingAddress != null && o.BillingAddress.CountryId == billingCountryId);
            if (!string.IsNullOrEmpty(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);
            if (osIds != null && osIds.Any())
                query = query.Where(o => osIds.Contains(o.OrderStatusId));
            if (psIds != null && psIds.Any())
                query = query.Where(o => psIds.Contains(o.PaymentStatusId));
            if (ssIds != null && ssIds.Any())
                query = query.Where(o => ssIds.Contains(o.ShippingStatusId));
            if (startTimeUtc.HasValue)
                query = query.Where(o => startTimeUtc.Value <= o.CreatedOnUtc);
            if (endTimeUtc.HasValue)
                query = query.Where(o => endTimeUtc.Value >= o.CreatedOnUtc);
            if (!string.IsNullOrEmpty(billingEmail))
                query = query.Where(o => o.BillingAddress != null && !string.IsNullOrEmpty(o.BillingAddress.Email) && o.BillingAddress.Email.Contains(billingEmail));

            if (SenderStateProvinceId > 0)
            {
                query = query.Where(p => p.BillingAddress != null && p.BillingAddress.StateProvinceId == SenderStateProvinceId);
            }
            if (ReciverCountryId > 0)
            {
                query = query.Where(p => p.ShippingAddress != null && p.ShippingAddress.CountryId == ReciverCountryId);
            }
            if (ReciverStateProvinceId > 0)
            {
                query = query.Where(p => p.ShippingAddress != null && p.ShippingAddress.StateProvinceId == ReciverStateProvinceId);
            }
            if (!string.IsNullOrEmpty(ReciverName))
            {
                query = query.Where(p => p.ShippingAddress != null
                && (!string.IsNullOrEmpty(p.ShippingAddress.FirstName) || !string.IsNullOrEmpty(p.ShippingAddress.LastName))
                && ((p.ShippingAddress.FirstName ?? string.Empty) + " " + (p.ShippingAddress.LastName ?? string.Empty)).Contains(ReciverName));
            }
            if (!string.IsNullOrEmpty(SenderName))
            {
                query = query.Where(p => p.BillingAddress != null
                && (!string.IsNullOrEmpty(p.BillingAddress.FirstName) || !string.IsNullOrEmpty(p.BillingAddress.LastName))
                && ((p.BillingAddress.FirstName ?? string.Empty) + " " + (p.BillingAddress.LastName ?? string.Empty)).Contains(SenderName));
            }

            if (!string.IsNullOrEmpty(billingLastName))
                query = query.Where(o => o.BillingAddress != null && !string.IsNullOrEmpty(o.BillingAddress.LastName) && o.BillingAddress.LastName.Contains(billingLastName));
            if (!string.IsNullOrEmpty(orderNotes))
                query = query.Where(o => o.OrderNotes.Any(on => on.Note.Contains(orderNotes)));

            var item = (from oq in query
                        group oq by 1
                into result
                        select new
                        {
                            OrderCount = result.Count(),
                            OrderShippingExclTaxSum = result.Sum(o => o.OrderShippingExclTax),
                            OrderTaxSum = result.Sum(o => o.OrderTax),
                            OrderTotalSum = result.Sum(o => o.OrderTotal)
                        }
            ).Select(r => new OrderAverageReportLine
            {
                CountOrders = r.OrderCount,
                SumShippingExclTax = r.OrderShippingExclTaxSum,
                SumTax = r.OrderTaxSum,
                SumOrders = r.OrderTotalSum
            }).FirstOrDefault();

            item = item ?? new OrderAverageReportLine
            {
                CountOrders = 0,
                SumShippingExclTax = decimal.Zero,
                SumTax = decimal.Zero,
                SumOrders = decimal.Zero,
            };
            return item;
        }
        public decimal ProfitReport(int storeId = 0, int vendorId = 0,
            int billingCountryId = 0, int orderId = 0, string paymentMethodSystemName = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
            DateTime? startTimeUtc = null, DateTime? endTimeUtc = null,
            string billingEmail = null, string billingLastName = "", string orderNotes = null,
           int SenderStateProvinceId = 0, int ReciverCountryId = 0, int ReciverStateProvinceId = 0,
           string ReciverName = null, string SenderName = null)
        {
            //We cannot use string.IsNullOrEmpty() in SQL Compact
            var dontSearchEmail = string.IsNullOrEmpty(billingEmail);
            //We cannot use string.IsNullOrEmpty() in SQL Compact
            var dontSearchLastName = string.IsNullOrEmpty(billingLastName);
            //We cannot use string.IsNullOrEmpty() in SQL Compact
            var dontSearchOrderNotes = string.IsNullOrEmpty(orderNotes);
            //We cannot use string.IsNullOrEmpty() in SQL Compact
            var dontSearchPaymentMethods = string.IsNullOrEmpty(paymentMethodSystemName);

            var orders = _orderRepository.Table;
            if (osIds != null && osIds.Any())
                orders = orders.Where(o => osIds.Contains(o.OrderStatusId));
            if (psIds != null && psIds.Any())
                orders = orders.Where(o => psIds.Contains(o.PaymentStatusId));
            if (ssIds != null && ssIds.Any())
                orders = orders.Where(o => ssIds.Contains(o.ShippingStatusId));

            if (SenderStateProvinceId > 0)
            {
                orders = orders.Where(p => p.BillingAddress != null && p.BillingAddress.StateProvinceId == SenderStateProvinceId);
            }
            if (ReciverCountryId > 0)
            {
                orders = orders.Where(p => p.ShippingAddress != null && p.ShippingAddress.CountryId == ReciverCountryId);
            }
            if (ReciverStateProvinceId > 0)
            {
                orders = orders.Where(p => p.ShippingAddress != null && p.ShippingAddress.StateProvinceId == ReciverStateProvinceId);
            }
            if (!string.IsNullOrEmpty(ReciverName))
            {
                orders = orders.Where(p => p.ShippingAddress != null
                && (!string.IsNullOrEmpty(p.ShippingAddress.FirstName) || !string.IsNullOrEmpty(p.ShippingAddress.LastName))
                && ((p.ShippingAddress.FirstName ?? string.Empty) + " " + (p.ShippingAddress.LastName ?? string.Empty)).Contains(ReciverName));
            }
            if (!string.IsNullOrEmpty(SenderName))
            {
                orders = orders.Where(p => p.BillingAddress != null
                && (!string.IsNullOrEmpty(p.BillingAddress.FirstName) || !string.IsNullOrEmpty(p.BillingAddress.LastName))
                && ((p.BillingAddress.FirstName ?? string.Empty) + " " + (p.BillingAddress.LastName ?? string.Empty)).Contains(SenderName));
            }

            var query = from orderItem in _orderItemRepository.Table
                        join o in orders on orderItem.OrderId equals o.Id
                        where (storeId == 0 || storeId == o.StoreId) &&
                              (orderId == 0 || orderId == o.Id) &&
                              (billingCountryId == 0 || (o.BillingAddress != null && o.BillingAddress.CountryId == billingCountryId)) &&
                              (dontSearchPaymentMethods || paymentMethodSystemName == o.PaymentMethodSystemName) &&
                              (!startTimeUtc.HasValue || startTimeUtc.Value <= o.CreatedOnUtc) &&
                              (!endTimeUtc.HasValue || endTimeUtc.Value >= o.CreatedOnUtc) &&
                              (!o.Deleted) &&
                              (vendorId == 0 || orderItem.Product.VendorId == vendorId) &&
                              //we do not ignore deleted products when calculating order reports
                              //(!p.Deleted)
                              (dontSearchEmail || (o.BillingAddress != null && !string.IsNullOrEmpty(o.BillingAddress.Email) && o.BillingAddress.Email.Contains(billingEmail))) &&
                              (dontSearchLastName || (o.BillingAddress != null && !string.IsNullOrEmpty(o.BillingAddress.LastName) && o.BillingAddress.LastName.Contains(billingLastName))) &&
                              (dontSearchOrderNotes || o.OrderNotes.Any(oNote => oNote.Note.Contains(orderNotes)))
                        select orderItem;

            var productCost = Convert.ToDecimal(query.Sum(orderItem => (decimal?)orderItem.OriginalProductCost * orderItem.Quantity));

            var reportSummary = GetOrderAverageReportLine(
                storeId: storeId,
                vendorId: vendorId,
                billingCountryId: billingCountryId,
                orderId: orderId,
                paymentMethodSystemName: paymentMethodSystemName,
                osIds: osIds,
                psIds: psIds,
                ssIds: ssIds,
                startTimeUtc: startTimeUtc,
                endTimeUtc: endTimeUtc,
                billingEmail: billingEmail,
                billingLastName: billingLastName,
                orderNotes: orderNotes);
            var profit = reportSummary.SumOrders - reportSummary.SumShippingExclTax - reportSummary.SumTax - productCost;
            return profit;
        }
        #endregion

        public void Print_58mToPdf(IList<Order> orders, Stream stream)
        {

            //var t = new StiReport();
            //t.Load(@"D:\Free\karimi\nopCommerce_4.00_Source\Plugins\Nop.plugin.Orders.ExtendedShipment\Report\COD.mrt");
            //t.Dictionary.BusinessObjects.Clear();
            //t.RegBusinessObject("CODList", "CODList", new CODModel());
            //t.Dictionary.SynchronizeBusinessObjects();
            //t.Save(@"D:\Free\karimi\nopCommerce_4.00_Source\Plugins\Nop.plugin.Orders.ExtendedShipment\Report\COD.mrt");

            List<COD58mModel> Lst_model = new List<COD58mModel>();

            foreach (var order in orders)
            {
                foreach (var orderItem in order.OrderItems)
                {
                    var store = _storeService.GetStoreById(orderItem.Order.StoreId) ?? _storeContext.CurrentStore;

                    var bill = CODorderItemTotal(orderItem);
                    var TotalCost = GetItemCostFromAttr(orderItem) + bill.AttributeCost.Where(p =>
                        p.priceType != PriceType.hagheSabte
                        && p.priceType != PriceType.kala
                        && p.priceType != PriceType.karton).Sum(p => p.value);

                    var TotalValue = GetItemPriceFromAttr(orderItem)
                                     + bill.AttributePrice.Where(p =>
                        p.priceType != PriceType.hagheSabte
                        && p.priceType != PriceType.kala
                        && p.priceType != PriceType.karton).Sum(p => p.value);

                    decimal engPrice = (TotalValue - TotalCost);

                    var kalaPrice = (bill.AttributePrice.FirstOrDefault(p => p.priceType == PriceType.kala)?.value).GetValueOrDefault(0);
                    var HagheSabte = (bill.AttributePrice.FirstOrDefault(p => p.priceType == PriceType.hagheSabte)?.value).GetValueOrDefault(0);

                    var tmp = engPrice + kalaPrice;
                    var HazineKalaTax = Convert.ToInt32((engPrice * bill.Tax) / 100);
                    var HazineKala = Convert.ToInt32(tmp + HagheSabte);


                    var COD3 = Convert.ToInt64(((tmp + HazineKalaTax + HagheSabte) * 3) / 100);

                    TotalCost += (COD3);

                    var HazineErsal = Convert.ToInt32(TotalCost);
                    var HazineErsalTax = Convert.ToInt32((HazineErsal * bill.Tax) / 100);

                    var orderTotal = HazineErsal + HazineErsalTax
                        + HazineKala + HazineKalaTax;
                    for (int i = 0; i < orderItem.Quantity; i++)
                    {
                        COD58mModel codModel = new COD58mModel();

                        codModel.orderId = orderItem.Order.Id;
                        Shipment shipment = null;
                        if (orderItem.Order.Shipments.Any())
                        {
                            shipment = orderItem.Order.Shipments.Where(p => p.ShipmentItems.Any(n => n.OrderItemId == orderItem.Id))
                                .OrderBy(p => p.Id).Skip(i).Take(1).FirstOrDefault();
                            if (shipment != null)
                            {
                                codModel.Barcode = getBarocdeImage(shipment);
                                codModel.BarcodeNo = shipment.TrackingNumber;
                            }
                        }
                        int HazineErsalWithAddtionalFee = HazineErsal;
                        if (shipment != null)
                        {
                            HazineErsalWithAddtionalFee += getAddtionalFee(order.Customer, shipment.Id);
                        }




                        codModel.Weight = Convert.ToInt32(orderItem.ItemWeight * 1000);
                        codModel.ProductName = orderItem.Product.Name;
                        codModel.SenderCountry = orderItem.Order.BillingAddress.Country.Name;
                        codModel.SenderState = orderItem.Order.BillingAddress.StateProvince?.Name;
                        codModel.SenderFullName = orderItem.Order.BillingAddress.FirstName + " " + orderItem.Order.BillingAddress.LastName;
                        codModel.SenderPhoneNo = orderItem.Order.BillingAddress.PhoneNumber;
                        codModel.SenderPostCode = orderItem.Order.BillingAddress.ZipPostalCode;
                        codModel.SenderAddress = orderItem.Order.BillingAddress.Address1;

                        codModel.ReciverCountry = orderItem.Order.ShippingAddress.Country.Name;
                        codModel.ReciverState = orderItem.Order.ShippingAddress.StateProvince?.Name;
                        codModel.ReciverFullName = orderItem.Order.ShippingAddress.FirstName + " " + orderItem.Order.ShippingAddress.LastName;
                        codModel.ReciverPhoneNo = orderItem.Order.ShippingAddress.PhoneNumber;
                        codModel.ReciverPostCode = orderItem.Order.ShippingAddress.ZipPostalCode;
                        codModel.ReciverAddress = orderItem.Order.ShippingAddress.Address1;

                        codModel.OrderDate = MiladyToShamsi2(orderItem.Order.CreatedOnUtc);
                        codModel.EngPrice = HazineKala;
                        codModel.PostPrice = HazineErsalWithAddtionalFee;
                        codModel.PostTaxValue = Convert.ToInt64((codModel.PostPrice * bill.Tax) / 100);
                        codModel.PostTotalValue = (codModel.PostTaxValue + codModel.PostPrice);
                        codModel.EngTaxValue = Convert.ToInt64((codModel.EngPrice * bill.Tax) / 100);
                        codModel.EngTotalValue = (codModel.EngPrice + codModel.EngTaxValue);
                        codModel.TotalValue = Convert.ToInt64(orderTotal);
                        codModel.orderItemId = orderItem.Id;
                        var SendDate = shipment != null ? shipment.CreatedOnUtc : (DateTime?)null;
                        codModel.sendToPostDate = ("تاریخ ارجا به پست" + ":" + (SendDate == null ? "" : MiladyToShamsi2(SendDate.Value, true)));
                        codModel.ProductAttrbiutes = bill.AttributeCost.Select(p => new Models.ProductAttribute()
                        {
                            name = p.name,
                            value = Convert.ToInt64(p.value),
                            orderItemId = orderItem.Id
                        }).ToList();
                        Lst_model.Add(codModel);
                    }
                }
            }
            Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHm4lqN+V3/fvJECXRrqdoEPZqYyFXh3g3K9oCDFvgzMl4c9KudQQwMJ6nO6w7rHz59BhwYgDE0QzKtjY2WxEejTNewbNDXY492M2mDsK1Hb4t6MoFYGbSoID0gow3VC5cFhvcOKVoagiHq6/4iqFc3RS7nZQBp95+WB6N1fR//H7OBlvfuBKldvC1pJh6pTW/7HRdOclErt/EGwivx9SpuDNabWWBfIZJBdJsvKjpoIsDQGibWk8Y8F9V1mCLf88FurWwEZ9WzXePbJn+wfabHM+a7pqDAXhsaW33UEW6vQ3kgLrVjbcHWdhtYbp5j6ZeIMLBCW2LXiCZpzy3N3XqjlroV/s7KRZGeZMvRrLWhcM8YJ6sBTMUyQrF/Fk3d5f0WbAf7+opIA5rhxY+qgwWcE42S+HOOcMgb2IWtj5ycC/GXZ4o9qtk2WWJzCC+ygckXK7iI5pzhzScGmLCBrFnjyE5EP65FViVDSCcNl6hUFw7wwRtcQq5pnOu6wHRXsSpwNPufWaRqE4gd7hbrQoPqki4BLOYgGOQjZ3kJdc7MG47nVT62z1ScLsjH73q4yhABt8h1wmJl0LanKnE6EGkvZPCrDBBf0sZp12sJrxK8FGsPUKrBNZ5f4VuYkdzWS1ed2m5MZNHkDDd3LncYkQnLPh/MYAsnc2ud715njwiM62xIWYk9lIrmCWJU3JVcexRDS3/l7Po9UU5gx2xJKUW1tWOXKQ9GqQZYtn/rl804VMC1/tWr4X2XU1otulny5P8YkZ7BTG6zmkcW6raROxv7xkSUwE0LK2FmIpeBr29Zxn0QUYBtC09J+wVD5y00f/1jMU+k1jQmi1n2tpWgy6CxAHPPrQFhk0FsGXEDg82IELCKncbqkfqgT0QyV262YL2uxrcbcJ1Wrzf4llEX90hMa3oZK5wdQLrCBkXIlXverhGL3I09fR6lcXDMj8A3vrQrhr4bwYcWTH0hyJCPVLYIVxAFLwgV8wk700QOo3z32sSWKnyMHVx3brFB8I7rOPaW2Sc8rBxc1E9EvNcqiTwRd9QA3lIdR2Hpc4jyn5dx8PT6GTNGXNKhoTZDLrGMtYqXw4rVOXfkE8+opF4+/H602ockCI/IY7C65+sxooNgSg+9LpzImUt6GdtrvF8HrvJ0rUW6ZiAmGaQmqdrHfwC3K+QKU4UEWuLU1GPG+PKMrYEbtT/Hei7NPA5sOqHqbzaMsW7zj+enlhuEUeMdlxqfQs1QtTERrzX2HXv/JZ43rASH4ycuGftLR2v3AOHhtBCoYOCszhOiHYGmH0I9lwi8HDRTXjkuoRU1Zxmz9rSIQy35/aypV339OS7p4VydO5LibEiwugzaNHXCZalEPShhcWzTAq6+uCb+jA9ob+Za4yICRdIjgot/E5ZQNZl2VEhFmWR7pYSB7MB24z0ysVf3igAc50/3Lr4d7mQ7SgmtKFoejGOjvb75YOq/pEJGxCo1xIS+jhZklyLuiwmtyDe7Xmpn33fip22qx2iF7FYW71AyuT3tTPn2a7Jc2wXlPCAxB6dqx0eeX4fWA3+RIosUTxTyA7a/SM95pwN9FKYFPZ8TcTqDIq0Ntc57IdX5u1To=";
            Stimulsoft.Base.StiFontCollection.AddFontFile(CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Content") + "/B Mitra.TTF");
            var report = new StiReport();
            report.RegBusinessObject("CODList", "CODList", Lst_model);
            report.Load(CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Report/") + "COD85M.mrt");
            report.Render();
            var settingsJpg = new StiJpegExportSettings();
            settingsJpg.ImageResolution = 300;
            var serviceJpg = new StiJpegExportService();
            serviceJpg.ExportTo(report, stream, settingsJpg);
        }
        public void PrintLable50MM(Order order, Stream stream)
        {

            //var t = new StiReport();
            //t.Load(@"D:\Projects\Web_APP\COD.mrt");
            //t.Dictionary.BusinessObjects.Clear();
            //t.RegBusinessObject("CODList", "CODList", new CODModel());
            //t.Dictionary.SynchronizeBusinessObjects();
            //t.Save(@"D:\Projects\Web_APP\Nope Clear file\PostbarNop\Presentation\Nop.Web\Plugins\Orders.ExtendedShipment\Report\COD.mrt");

            int HagheMaghar = 0;
            int _posthagheMaghar = 0;
            var category = order.OrderItems.First().Product.ProductCategories.First().Category;
            int serviceId = category.Id;
            foreach (var _orderItem in order.OrderItems)
            {
                HagheMaghar = getHagheMaghar(_orderItem.Id, order.BillingAddress.CountryId.Value, serviceId, out _posthagheMaghar);
                if (HagheMaghar > 0)
                    break;
            }
            if (HagheMaghar > 0)
            {
                HagheMaghar -= _posthagheMaghar;
                HagheMaghar = HagheMaghar / order.OrderItems.Sum(p => p.Quantity);
            }

            List<LablePrint> Lst_LablePrintmodel = new List<LablePrint>();
            List<LablePrintEngList> Lst_LablePrintEngListmodel = new List<LablePrintEngList>();

            int itemDiscount = 0;
            if (order.OrderDiscount > decimal.Zero)
            {
                itemDiscount = Convert.ToInt32(order.OrderDiscount / (order.OrderItems.Sum(p => p.Quantity)));
            }

            bool IsmultiShipment = IsMultiShippment(order);
            var mark = GetOrderRegistrationMethod(order);

            foreach (var orderItem in order.OrderItems)
            {

                var store = _storeService.GetStoreById(orderItem.Order.StoreId) ?? _storeContext.CurrentStore;
                int weight = GetItemWeightFromAttr(orderItem.Id);

                for (int i = 0; i < orderItem.Quantity; i++)
                {
                    Shipment shipment = null;
                    Address shippingAddress = null;
                    if (IsmultiShipment)
                    {
                        var multiShipmentData = getShipmentFromMultiShipment(orderItem, i);
                        if (multiShipmentData == null)
                        {
                            Log("اطلاعات مربوط به حمل و نقل ایتم سفارش " + orderItem.Id + " یافت نشد ", "");
                            continue;
                        }
                        shipment = multiShipmentData.shipment;
                        shippingAddress = _addressService.GetAddressById(multiShipmentData.ShipmentAddressId);
                    }
                    else
                    {
                        shippingAddress = order.ShippingAddress;
                        if (orderItem.Order.Shipments != null)
                        {
                            if (orderItem.Order.Shipments.Any())
                            {
                                shipment = orderItem.Order.Shipments.Where(p => p.ShipmentItems.Any(n => n.OrderItemId == orderItem.Id))
                                    .OrderBy(p => p.Id).Skip(i).Take(1).FirstOrDefault();
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(shipment.TrackingNumber))
                    {
                        continue;
                    }

                    #region EngPrice
                    var bill = orderItemTotal(orderItem);
                    var TotalCost = GetItemCostFromAttr(orderItem);
                    var TotalValue = GetItemPriceFromAttr(orderItem);
                    int engPrice = 0;
                    var isPostService = IsPostService(orderItem.Product.ProductCategories.First().CategoryId);
                    if (!isPostService || mark == OrderRegistrationMethod.Ap || mark == OrderRegistrationMethod.bidok)
                    {
                        var prices = getEngAndPostPrice(orderItem);
                        if (prices == null)
                        {
                            if (!CheckHasValidPrice(orderItem.Order))
                            {
                                prices = new ApiOrderItemPrice();
                            }
                            else
                            {
                                prices = getEngAndPostPrice(orderItem);
                            }

                        }
                        engPrice = prices.EngPrice;

                    }
                    else
                    {
                        engPrice = (TotalValue - TotalCost);
                    }
                    engPrice = engPrice + HagheMaghar;
                    int print = 0;
                    if (bill.AttributePrice.Any(p => p.name.Contains("پرینتر") && p.value > 0))
                    {
                        print = Convert.ToInt32(bill.AttributePrice.FirstOrDefault(p => p.name.Contains("پرینتر")).value);
                    }
                    int _cartonValue = 0;
                    int LafafValue = 0;
                    if (bill.AttributePrice.Any(p => p.name.Contains("لفاف") && p.value > 0))
                    {
                        int cartonLafafValue = Convert.ToInt32(bill.AttributePrice.FirstOrDefault(p => p.name.Contains("لفاف")).value);

                        LafafValue = Convert.ToInt32(bill.AttributeCost.FirstOrDefault(p => p.name.Contains("لفاف")).value);
                        _cartonValue = cartonLafafValue - LafafValue;
                    }
                    int bime = 0;
                    if (bill.AttributePrice.Any(p => p.name.Contains("بیمه") && p.value > 0))
                    {
                        bime = Convert.ToInt32(bill.AttributePrice.FirstOrDefault(p => p.name.Contains("بیمه")).value);
                    }
                    int sms = 0;
                    if (bill.AttributePrice.Any(p => p.name.Contains("اطلاع رسانی پیامکی") && p.value > 0))
                    {
                        sms = Convert.ToInt32(bill.AttributePrice.FirstOrDefault(p => p.name.Contains("اطلاع رسانی پیامکی")).value);
                    }
                    int HagheSabt = 0;
                    HagheSabt = getHagheSabt(orderItem.Id);
                    int requestPrintLogo = 0;
                    requestPrintLogo = RequestPrintLogoPrice(orderItem);
                    int ValueAddedByAgnt = 0;
                    if (orderItem.Order.Customer.IsInCustomerRole("mini-Administrators"))
                    {
                        ValueAddedByAgnt = GetValueAddedbyAgent(orderItem);
                    }
                    int TotalEngPirce = (int)((engPrice + print + _cartonValue + bime + sms + HagheSabt + requestPrintLogo + ValueAddedByAgnt));
                    TotalEngPirce -= itemDiscount;
                    TotalEngPirce += (TotalEngPirce * 9) / 100;
                    #endregion

                    #region PostPrice
                    int postPrice = 0;
                    //Post price
                    bool exceptPostTaxPrice = false;
                    if (new int[] { 699, 700, 701, 702, 703, 704, 705, 706, 707, 708, 709, 710, 715, 714, 713, 712, 711 }.Contains(orderItem.Product.ProductCategories.First().CategoryId))
                    {
                        var prices = getEngAndPostPrice(orderItem);
                        if (prices == null)
                        {
                            throw new Exception($"قیمت مربوط به آیتم شماره {orderItem.Id} از سرویس مربوطه دریافت نشده");
                        }
                        postPrice = prices.IncomePrice;
                        exceptPostTaxPrice = true;
                    }
                    else
                    {
                        postPrice = GetItemCostFromAttr(orderItem);
                    }

                    decimal AttCost = 0;
                    if (bill.AttributeCost != null)
                        if (bill.AttributeCost.Any())

                        {
                            AttCost = bill.AttributeCost.Sum(p => p.value);
                        }

                    if (exceptPostTaxPrice)
                    {
                        postPrice += (int)AttCost;
                        int PostTax = (int)((AttCost + _posthagheMaghar) * 9) / 100;
                        postPrice += postPrice + _posthagheMaghar + PostTax;
                    }
                    else
                    {
                        postPrice += (int)AttCost;
                        int PostTax = (int)((postPrice + _posthagheMaghar) * 9) / 100;
                        postPrice += _posthagheMaghar + PostTax;
                    }
                    #endregion


                    //row number
                    LablePrint _LablePrintModel = new LablePrint();
                    _LablePrintModel.CreateDate = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc).ToShortDateString();
                    _LablePrintModel.CreateTime = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc).ToShortTimeString();
                    _LablePrintModel.OfficeName = "بازارمتریال";
                    _LablePrintModel.PostTotalPrice = postPrice.ToString("N0") + " ريال ";
                    _LablePrintModel.ReceiverState = shippingAddress.StateProvince.Name;
                    _LablePrintModel.SenderState = order.BillingAddress.StateProvince.Name;
                    _LablePrintModel.ServiceName = category.Name;
                    _LablePrintModel.Weight = weight;
                    _LablePrintModel.ShipmentId = shipment.Id;
                    _LablePrintModel.orderId = shipment.OrderId;
                    _LablePrintModel.BarcodeImage = getBarocdeImage_LablePrint(shipment);
                    _LablePrintModel.barcodeNo = shipment.TrackingNumber;
                    LablePrintEngList _LablePrintEngList = new LablePrintEngList();
                    _LablePrintEngList.Sender = $@"{order.BillingAddress.Country.Name}-{order.BillingAddress.StateProvince.Name}-{order.BillingAddress.Address1}-{(order.BillingAddress.FirstName + " " ?? "")}{order.BillingAddress.LastName + ""}";
                    _LablePrintEngList.Receiver = $@"{shippingAddress.Country.Name}-{shippingAddress.StateProvince.Name}-{shippingAddress.Address1}-{(shippingAddress.FirstName + " " ?? "")}{shippingAddress.LastName + ""}-{shippingAddress.PhoneNumber}";
                    _LablePrintEngList.ShipmentId = shipment.Id;
                    _LablePrintEngList.orderId = shipment.OrderId;
                    _LablePrintEngList.TotalEngPrice = TotalEngPirce.ToString("N0") + " ريال ";
                    _LablePrintEngList.ApiOrderRefrence = _apiOrderRefrenceCodeService.getRefrenceCode(shipment.OrderId);

                    Lst_LablePrintEngListmodel.Add(_LablePrintEngList);
                    Lst_LablePrintmodel.Add(_LablePrintModel);
                }
            }

            Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHm4lqN+V3/fvJECXRrqdoEPZqYyFXh3g3K9oCDFvgzMl4c9KudQQwMJ6nO6w7rHz59BhwYgDE0QzKtjY2WxEejTNewbNDXY492M2mDsK1Hb4t6MoFYGbSoID0gow3VC5cFhvcOKVoagiHq6/4iqFc3RS7nZQBp95+WB6N1fR//H7OBlvfuBKldvC1pJh6pTW/7HRdOclErt/EGwivx9SpuDNabWWBfIZJBdJsvKjpoIsDQGibWk8Y8F9V1mCLf88FurWwEZ9WzXePbJn+wfabHM+a7pqDAXhsaW33UEW6vQ3kgLrVjbcHWdhtYbp5j6ZeIMLBCW2LXiCZpzy3N3XqjlroV/s7KRZGeZMvRrLWhcM8YJ6sBTMUyQrF/Fk3d5f0WbAf7+opIA5rhxY+qgwWcE42S+HOOcMgb2IWtj5ycC/GXZ4o9qtk2WWJzCC+ygckXK7iI5pzhzScGmLCBrFnjyE5EP65FViVDSCcNl6hUFw7wwRtcQq5pnOu6wHRXsSpwNPufWaRqE4gd7hbrQoPqki4BLOYgGOQjZ3kJdc7MG47nVT62z1ScLsjH73q4yhABt8h1wmJl0LanKnE6EGkvZPCrDBBf0sZp12sJrxK8FGsPUKrBNZ5f4VuYkdzWS1ed2m5MZNHkDDd3LncYkQnLPh/MYAsnc2ud715njwiM62xIWYk9lIrmCWJU3JVcexRDS3/l7Po9UU5gx2xJKUW1tWOXKQ9GqQZYtn/rl804VMC1/tWr4X2XU1otulny5P8YkZ7BTG6zmkcW6raROxv7xkSUwE0LK2FmIpeBr29Zxn0QUYBtC09J+wVD5y00f/1jMU+k1jQmi1n2tpWgy6CxAHPPrQFhk0FsGXEDg82IELCKncbqkfqgT0QyV262YL2uxrcbcJ1Wrzf4llEX90hMa3oZK5wdQLrCBkXIlXverhGL3I09fR6lcXDMj8A3vrQrhr4bwYcWTH0hyJCPVLYIVxAFLwgV8wk700QOo3z32sSWKnyMHVx3brFB8I7rOPaW2Sc8rBxc1E9EvNcqiTwRd9QA3lIdR2Hpc4jyn5dx8PT6GTNGXNKhoTZDLrGMtYqXw4rVOXfkE8+opF4+/H602ockCI/IY7C65+sxooNgSg+9LpzImUt6GdtrvF8HrvJ0rUW6ZiAmGaQmqdrHfwC3K+QKU4UEWuLU1GPG+PKMrYEbtT/Hei7NPA5sOqHqbzaMsW7zj+enlhuEUeMdlxqfQs1QtTERrzX2HXv/JZ43rASH4ycuGftLR2v3AOHhtBCoYOCszhOiHYGmH0I9lwi8HDRTXjkuoRU1Zxmz9rSIQy35/aypV339OS7p4VydO5LibEiwugzaNHXCZalEPShhcWzTAq6+uCb+jA9ob+Za4yICRdIjgot/E5ZQNZl2VEhFmWR7pYSB7MB24z0ysVf3igAc50/3Lr4d7mQ7SgmtKFoejGOjvb75YOq/pEJGxCo1xIS+jhZklyLuiwmtyDe7Xmpn33fip22qx2iF7FYW71AyuT3tTPn2a7Jc2wXlPCAxB6dqx0eeX4fWA3+RIosUTxTyA7a/SM95pwN9FKYFPZ8TcTqDIq0Ntc57IdX5u1To=";
            Stimulsoft.Base.StiFontCollection.AddFontFile(CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Content") + "/B Mitra.TTF");
            var report = new StiReport();
            report.RegBusinessObject("LablePrint", "LablePrint", Lst_LablePrintmodel);
            report.RegBusinessObject("LablePrintEngList", "LablePrintEngList", Lst_LablePrintEngListmodel);


            report.Load(CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Report/") + "LablePrint.mrt");

            report.Compile();
            report.Render();

            var settingsPdf = new StiPdfExportSettings();
            var servicePdf = new StiPdfExportService();
            settingsPdf.ImageQuality = 75;
            settingsPdf.ImageResolution = 600;
            settingsPdf.EmbeddedFonts = true;
            settingsPdf.UseUnicode = true;
            MemoryStream tempStram = new MemoryStream();
            servicePdf.ExportTo(report, stream, settingsPdf);

        }

        public List<ExtendedShipmentModel> GetAllShipments(out int count, int vendorId = 0,
            int warehouseId = 0,
            int billingCountryId = 0,
            int billingStateId = 0,
            string billingCity = null,
            int shippingCountryId = 0,
            int shippingStateId = 0,
            string shippingCity = null,
            string trackingNumber = null,
            bool loadNotShipped = false,
            DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            int PostmanId = 0,
            string ReciverName = null,
            string SenderName = null,
            int orderId = 0,
            int ignorCod = 1,
            int ShipmentState = 0,
            int ShipmentState2 = 0,
            int CODShipmentEventId = 0,
            int CodTrackingDayCoun = 0,
            DateTime? CodEndTime = null,
            bool HasGoodsPrice = false,
            int OrderCustomerId = 0)
        {

            SqlParameter P_shippingCity = new SqlParameter() { ParameterName = "shippingCity", SqlDbType = SqlDbType.NVarChar };

            if (string.IsNullOrEmpty(shippingCity))
                P_shippingCity.Value = DBNull.Value;
            else
                P_shippingCity.Value = shippingCity;

            SqlParameter P_billingCity = new SqlParameter() { ParameterName = "billingCity", SqlDbType = SqlDbType.NVarChar };

            if (string.IsNullOrEmpty(billingCity))
                P_billingCity.Value = DBNull.Value;
            else
                P_billingCity.Value = billingCity;

            SqlParameter P_trackingNumber = new SqlParameter() { ParameterName = "trackingNumber", SqlDbType = SqlDbType.NVarChar };

            if (string.IsNullOrEmpty(trackingNumber))
                P_trackingNumber.Value = DBNull.Value;
            else
                P_trackingNumber.Value = trackingNumber;

            SqlParameter P_createdFromUtc = new SqlParameter() { ParameterName = "createdFromUtc", SqlDbType = SqlDbType.VarChar };

            if (!createdFromUtc.HasValue)
                P_createdFromUtc.Value = DBNull.Value;
            else
                P_createdFromUtc.Value = ConvertToMiladyString(createdFromUtc.Value);
            SqlParameter P_createdToUtc = new SqlParameter() { ParameterName = "createdToUtc", SqlDbType = SqlDbType.VarChar };

            if (!createdToUtc.HasValue)
                P_createdToUtc.Value = DBNull.Value;
            else
                P_createdToUtc.Value = ConvertToMiladyString(createdToUtc.Value);
            SqlParameter P_count = new SqlParameter() { ParameterName = "Count", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

            SqlParameter P_ReciverName = new SqlParameter()
            {
                ParameterName = "ReciverName",
                SqlDbType = SqlDbType.NVarChar,
                Value = string.IsNullOrEmpty(ReciverName) ? (object)DBNull.Value : (object)ReciverName
            };

            SqlParameter P_SenderName = new SqlParameter()
            {
                ParameterName = "SenderName",
                SqlDbType = SqlDbType.NVarChar,
                Value = string.IsNullOrEmpty(SenderName) ? (object)DBNull.Value : (object)SenderName
            };

            SqlParameter P_IgnoreCOD = new SqlParameter()
            {
                ParameterName = "IgnoreCOD",
                SqlDbType = SqlDbType.Int,
                Value = (object)ignorCod
            };
            SqlParameter P_ShipmentState = new SqlParameter()
            {
                ParameterName = "ShipmentState",
                SqlDbType = SqlDbType.Int,
                Value = (object)ShipmentState
            };

            //SqlParameter P_Status = new SqlParameter()
            //{
            //    ParameterName = "StatusId",
            //    SqlDbType = SqlDbType.Int,
            //    Value = (object)statusId
            //};

            SqlParameter P_OrderId = new SqlParameter() { ParameterName = "OrderId ", SqlDbType = SqlDbType.Int, Value = orderId };

            SqlParameter P_CodEndTime = new SqlParameter() { ParameterName = "CodEndTime", SqlDbType = SqlDbType.DateTime };

            if (!CodEndTime.HasValue)
                P_CodEndTime.Value = DBNull.Value;
            else
                P_CodEndTime.Value = CodEndTime.Value;

            SqlParameter[] prms = new SqlParameter[]{
                new SqlParameter() { ParameterName = "vendorId", SqlDbType = SqlDbType.Int, Value = 0 },
                new SqlParameter() { ParameterName = "warehouseId", SqlDbType = SqlDbType.Int, Value = warehouseId },
                new SqlParameter() { ParameterName = "shippingCountryId", SqlDbType = SqlDbType.Int, Value = shippingCountryId },
                new SqlParameter() { ParameterName = "shippingStateId", SqlDbType = SqlDbType.Int, Value = shippingStateId },
                P_shippingCity,
                new SqlParameter() { ParameterName = "billingCountryId", SqlDbType = SqlDbType.Int, Value = billingCountryId },
                new SqlParameter() { ParameterName = "billingStateId", SqlDbType = SqlDbType.Int, Value = billingStateId },
                P_billingCity,
                P_trackingNumber,
                new SqlParameter() { ParameterName = "loadNotShipped", SqlDbType = SqlDbType.Bit, Value = loadNotShipped },
                P_createdFromUtc,
                P_createdToUtc,
                new SqlParameter() { ParameterName = "pageIndex", SqlDbType = SqlDbType.Int, Value = pageIndex },
                new SqlParameter() { ParameterName = "pageSize", SqlDbType = SqlDbType.Int, Value = pageSize },
                new SqlParameter() { ParameterName = "PostmanId", SqlDbType = SqlDbType.Int, Value = PostmanId },
                P_ReciverName,
                P_SenderName,
                P_OrderId,
                P_IgnoreCOD,
                P_ShipmentState,
                P_count,
                new SqlParameter() { ParameterName = "ShipmentState2", SqlDbType = SqlDbType.Int, Value = ShipmentState2 },
                new SqlParameter() { ParameterName = "CustomerId", SqlDbType = SqlDbType.Int, Value = _workContext.CurrentCustomer.Id },
                new SqlParameter() { ParameterName = "CODShipmentEventId", SqlDbType = SqlDbType.Int, Value = CODShipmentEventId },
                new SqlParameter() { ParameterName = "CodTrackingDayCount", SqlDbType = SqlDbType.Int, Value = CodTrackingDayCoun},
                P_CodEndTime,
                new SqlParameter() { ParameterName = "HasGoodsPrice", SqlDbType = SqlDbType.Bit, Value = HasGoodsPrice},
                new SqlParameter() { ParameterName = "CurrentStore", SqlDbType = SqlDbType.Int, Value = _storeContext.CurrentStore.Id},
                new SqlParameter() { ParameterName = "OrderCustomer", SqlDbType = SqlDbType.Int, Value = OrderCustomerId}
                //P_Status
            };

            var allShipment = _dbContext.SqlQuery<ExtendedShipmentModel>(@"EXECUTE [dbo].[Sh_Sp_GetAllShipment] @vendorId,@warehouseId,@shippingCountryId,@shippingStateId,@shippingCity,@billingCountryId
            , @billingStateId, @billingCity, @trackingNumber, @loadNotShipped, @createdFromUtc, @createdToUtc
            , @pageIndex, @pageSize, @PostmanId,@ReciverName,@SenderName,@OrderId,@IgnoreCOD,@ShipmentState,@Count output,@ShipmentState2,@CustomerId,@CODShipmentEventId,
              @CodTrackingDayCount,@CodEndTime,@HasGoodsPrice,@CurrentStore,@OrderCustomer", prms).ToList();

            //var shipments = new PagedList<ExtendedShipmentModel>(allShipment, pageIndex, pageSize);
            count = (int)P_count.Value;
            return allShipment.OrderBy(p => p.BillingCountryId).ThenBy(p => p.BillingStateProvinceId).ThenBy(p => p.Address1).ToList();
        }


        public List<ExtendedShipmentModel> GetAllShipmentsByStatus(out int count, int vendorId = 0,
           int warehouseId = 0,
           int billingCountryId = 0,
           int billingStateId = 0,
           string billingCity = null,
           int shippingCountryId = 0,
           int shippingStateId = 0,
           string shippingCity = null,
           string trackingNumber = null,
           bool loadNotShipped = false,
           DateTime? createdFromUtc = null,
           DateTime? createdToUtc = null,
           int pageIndex = 0,
           int pageSize = int.MaxValue,
           int PostmanId = 0,
           string ReciverName = null,
           string SenderName = null,
           int orderId = 0,
           int ignorCod = 1,
           int ShipmentState = 0,
           int ShipmentState2 = 0,
           int CODShipmentEventId = 0,
           int CodTrackingDayCoun = 0,
           DateTime? CodEndTime = null,
           bool HasGoodsPrice = false,
           int OrderCustomerId = 0,
           int statusId = 0)
        {

            SqlParameter P_shippingCity = new SqlParameter() { ParameterName = "shippingCity", SqlDbType = SqlDbType.NVarChar };

            if (string.IsNullOrEmpty(shippingCity))
                P_shippingCity.Value = DBNull.Value;
            else
                P_shippingCity.Value = shippingCity;

            SqlParameter P_billingCity = new SqlParameter() { ParameterName = "billingCity", SqlDbType = SqlDbType.NVarChar };

            if (string.IsNullOrEmpty(billingCity))
                P_billingCity.Value = DBNull.Value;
            else
                P_billingCity.Value = billingCity;

            SqlParameter P_trackingNumber = new SqlParameter() { ParameterName = "trackingNumber", SqlDbType = SqlDbType.NVarChar };

            if (string.IsNullOrEmpty(trackingNumber))
                P_trackingNumber.Value = DBNull.Value;
            else
                P_trackingNumber.Value = trackingNumber;

            SqlParameter P_createdFromUtc = new SqlParameter() { ParameterName = "createdFromUtc", SqlDbType = SqlDbType.VarChar };

            if (!createdFromUtc.HasValue)
                P_createdFromUtc.Value = DBNull.Value;
            else
                P_createdFromUtc.Value = ConvertToMiladyString(createdFromUtc.Value);
            SqlParameter P_createdToUtc = new SqlParameter() { ParameterName = "createdToUtc", SqlDbType = SqlDbType.VarChar };

            if (!createdToUtc.HasValue)
                P_createdToUtc.Value = DBNull.Value;
            else
                P_createdToUtc.Value = ConvertToMiladyString(createdToUtc.Value);
            SqlParameter P_count = new SqlParameter() { ParameterName = "Count", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

            SqlParameter P_ReciverName = new SqlParameter()
            {
                ParameterName = "ReciverName",
                SqlDbType = SqlDbType.NVarChar,
                Value = string.IsNullOrEmpty(ReciverName) ? (object)DBNull.Value : (object)ReciverName
            };

            SqlParameter P_SenderName = new SqlParameter()
            {
                ParameterName = "SenderName",
                SqlDbType = SqlDbType.NVarChar,
                Value = string.IsNullOrEmpty(SenderName) ? (object)DBNull.Value : (object)SenderName
            };

            SqlParameter P_IgnoreCOD = new SqlParameter()
            {
                ParameterName = "IgnoreCOD",
                SqlDbType = SqlDbType.Int,
                Value = (object)ignorCod
            };
            SqlParameter P_ShipmentState = new SqlParameter()
            {
                ParameterName = "ShipmentState",
                SqlDbType = SqlDbType.Int,
                Value = (object)ShipmentState
            };

            SqlParameter P_Status = new SqlParameter()
            {
                ParameterName = "StatusId",
                SqlDbType = SqlDbType.Int,
                Value = (object)statusId
            };

            SqlParameter P_OrderId = new SqlParameter() { ParameterName = "OrderId ", SqlDbType = SqlDbType.Int, Value = orderId };

            SqlParameter P_CodEndTime = new SqlParameter() { ParameterName = "CodEndTime", SqlDbType = SqlDbType.DateTime };

            if (!CodEndTime.HasValue)
                P_CodEndTime.Value = DBNull.Value;
            else
                P_CodEndTime.Value = CodEndTime.Value;

            SqlParameter[] prms = new SqlParameter[]{
                new SqlParameter() { ParameterName = "vendorId", SqlDbType = SqlDbType.Int, Value = 0 },
                new SqlParameter() { ParameterName = "warehouseId", SqlDbType = SqlDbType.Int, Value = warehouseId },
                new SqlParameter() { ParameterName = "shippingCountryId", SqlDbType = SqlDbType.Int, Value = shippingCountryId },
                new SqlParameter() { ParameterName = "shippingStateId", SqlDbType = SqlDbType.Int, Value = shippingStateId },
                P_shippingCity,
                new SqlParameter() { ParameterName = "billingCountryId", SqlDbType = SqlDbType.Int, Value = billingCountryId },
                new SqlParameter() { ParameterName = "billingStateId", SqlDbType = SqlDbType.Int, Value = billingStateId },
                P_billingCity,
                P_trackingNumber,
                new SqlParameter() { ParameterName = "loadNotShipped", SqlDbType = SqlDbType.Bit, Value = loadNotShipped },
                P_createdFromUtc,
                P_createdToUtc,
                new SqlParameter() { ParameterName = "pageIndex", SqlDbType = SqlDbType.Int, Value = pageIndex },
                new SqlParameter() { ParameterName = "pageSize", SqlDbType = SqlDbType.Int, Value = pageSize },
                new SqlParameter() { ParameterName = "PostmanId", SqlDbType = SqlDbType.Int, Value = PostmanId },
                P_ReciverName,
                P_SenderName,
                P_OrderId,
                P_IgnoreCOD,
                P_ShipmentState,
                P_count,
                new SqlParameter() { ParameterName = "ShipmentState2", SqlDbType = SqlDbType.Int, Value = ShipmentState2 },
                new SqlParameter() { ParameterName = "CustomerId", SqlDbType = SqlDbType.Int, Value = _workContext.CurrentCustomer.Id },
                new SqlParameter() { ParameterName = "CODShipmentEventId", SqlDbType = SqlDbType.Int, Value = CODShipmentEventId },
                new SqlParameter() { ParameterName = "CodTrackingDayCount", SqlDbType = SqlDbType.Int, Value = CodTrackingDayCoun},
                P_CodEndTime,
                new SqlParameter() { ParameterName = "HasGoodsPrice", SqlDbType = SqlDbType.Bit, Value = HasGoodsPrice},
                new SqlParameter() { ParameterName = "CurrentStore", SqlDbType = SqlDbType.Int, Value = _storeContext.CurrentStore.Id},
                new SqlParameter() { ParameterName = "OrderCustomer", SqlDbType = SqlDbType.Int, Value = OrderCustomerId}
                , P_Status
            };
            var allShipment = _dbContext.SqlQuery<ExtendedShipmentModel>(@"EXECUTE [dbo].[Sh_Sp_GetAllShipmentByStatus] @vendorId,@warehouseId,@shippingCountryId,@shippingStateId,@shippingCity,@billingCountryId
            , @billingStateId, @billingCity, @trackingNumber, @loadNotShipped, @createdFromUtc, @createdToUtc
            , @pageIndex, @pageSize, @PostmanId,@ReciverName,@SenderName,@OrderId,@IgnoreCOD,@ShipmentState,@Count output,@ShipmentState2,@CustomerId,@CODShipmentEventId,
              @CodTrackingDayCount,@CodEndTime,@HasGoodsPrice,@CurrentStore,@OrderCustomer,@StatusId", prms).ToList();//,@StatusId

            //var shipments = new PagedList<ExtendedShipmentModel>(allShipment, pageIndex, pageSize);
            count = (int)P_count.Value;
            return allShipment.OrderBy(p => p.BillingCountryId).ThenBy(p => p.BillingStateProvinceId).ThenBy(p => p.Address1).ToList();
        }

        public List<OrderShipmentModel> getOrderShipment(out int count, int orderId, int pageIndex, int pageSize)
        {
            SqlParameter P_count = new SqlParameter() { ParameterName = "dataCount", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
            SqlParameter[] prms = new SqlParameter[] {
                new SqlParameter() { ParameterName = "OrderId", SqlDbType = SqlDbType.Int,Value = orderId },
                new SqlParameter() { ParameterName = "PageSize", SqlDbType = SqlDbType.Int, Value = pageSize },
                new SqlParameter() { ParameterName = "PageIndex", SqlDbType = SqlDbType.Int, Value = pageIndex },
                P_count,

            };
            var allshipment = _dbContext.SqlQuery<OrderShipmentModel>(@"EXECUTE [dbo].[SP_CustomerShipment]  @OrderId,@PageSize,@PageIndex,@dataCount OUTPUT", prms).ToList();
            count = (int)P_count.Value;
            return allshipment;
        }
        public List<SelectListItem> getUserInRole(int RoleId, int? country, int? state, string city)
        {
            try
            {
                return _customerService.GetAllCustomers(
                    customerRoleIds: new[] { RoleId },
                    pageIndex: 0,
                    pageSize: 5000).Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.GetFullName() + ", " + getPhoneNumber(p.Id)
                    }).ToList();
            }
            catch (Exception ex)
            {
                Log("خطا در زمان واکشی اطلاعات پستچی ها ",
                    ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                return new List<SelectListItem>();
            }
        }

        public void ChossePostMan(int postMan, int PostAdmin, int ShipmentId)
        {
            try
            {
                ShipmentAppointmentModel model = null;
                if (_ShipmentAppointmentRepository.Table.Any(p => p.ShipmentId == ShipmentId))
                {
                    model = _ShipmentAppointmentRepository.Table.Where(p => p.ShipmentId == ShipmentId)
                        .OrderByDescending(p => p.Id).FirstOrDefault();
                    model.PostManId = postMan;
                    model.PostAdminId = PostAdmin;
                    _ShipmentAppointmentRepository.Update(model);
                    return;
                }
                model = new ShipmentAppointmentModel
                {
                    AppointmentDate = DateTime.Now,
                    ShipmentId = ShipmentId,
                    PostAdminId = PostAdmin,
                    PostManId = postMan
                };
                _ShipmentAppointmentRepository.Insert(model);
                var Shipment = _shipmentService.GetShipmentById(ShipmentId);

                var extendedShipmentSetting = _setting;
                //_settingService.LoadSetting<ExtendedShipmentSetting>(_storeContext.CurrentStore.Id);
                var postManPhoneNumeber = getPhoneNumber(postMan);
                if (string.IsNullOrWhiteSpace(postManPhoneNumeber))
                    Log("خطا در زمان ارسال پیام برای پستچی", "شماره همراه پستچی مورد نظر نامعتبر است");


                var sended = _notificationService._sendSms(postManPhoneNumeber, extendedShipmentSetting.PostmanMessageTemplate);

                if (sended)
                    Log("ارسال پیام اختصاص سفارش به پستچی " + Shipment.Order.Id, "");
                else
                    Log("عدم ارسال پیام اختصاص سفارش به پستچی " + Shipment.Order.Id,
                        postManPhoneNumeber + "" + extendedShipmentSetting.PostAdminMessageTemplate);
            }
            catch (Exception ex)
            {
                Log("خطا در زمان انتصاب پستچی ", ex.Message +
                                                 (ex.InnerException != null ? ex.InnerException.Message : ""));
            }
        }

        public void SetPersuitCodeMode(int ShipmentId, bool IsPersuitCodeAuto)
        {
            try
            {
                var model = new ShipmentAppointmentModel
                {
                    ShipmentId = ShipmentId,
                    IsAutoPersuitCode = IsPersuitCodeAuto,
                    AppointmentDate = DateTime.Now
                };
                if (_ShipmentAppointmentRepository.Table.Any(p => p.ShipmentId == ShipmentId))
                {
                    _ShipmentAppointmentRepository.Update(model);
                    return;
                }
                _ShipmentAppointmentRepository.Insert(model);
            }
            catch (Exception ex)
            {
                Log("خطا در زمان ثبت نجووه تولید بارکد", ex.Message +
                                                         (ex.InnerException != null ? ex.InnerException.Message : ""));
            }
        }

        public string GenerateBarcodeFromPost(int shipmentId, int SenderStateId, out int OldShipmentId, int? shippingAddressId = null)
        {
            OldShipmentId = 0;
            var shipment = _shipmentService.GetShipmentById(shipmentId);

            Address ShippingAddress = null;
            if (!shippingAddressId.HasValue)
            {
                InsertOrderNote("آدرس گیرنده دارای نقص می باشد و امکان تولید بارکد وجود ندارد", shipment.OrderId);
                return "";
            }
            ShippingAddress = _addressService.GetAddressById(shippingAddressId.Value);
            //else
            //    ShippingAddress = shipment.Order.ShippingAddress;
            OrderItem item = shipment.Order.OrderItems.Single(p => p.Id == shipment.ShipmentItems.First().OrderItemId);

            if (item.Order.Customer.IsInCustomerRole("UseNotUsedBarcode"))
            {
                int serviceId = item.Product.ProductCategories.First().CategoryId;
                int weight = GetItemWeightFromAttr(item);
                var data = getFromNotUsedBarcode(ShippingAddress.CountryId.Value, ShippingAddress.StateProvinceId.Value,
                    item.Order.CustomerId, serviceId, weight);
                if (data != null)
                {
                    if (!string.IsNullOrEmpty(data.TrackingNumber))
                    {
                        OldShipmentId = data.OldShipmentid;
                        return data.TrackingNumber;
                    }
                }
            }

            var extendedShipmentSetting = _setting;
            //_settingService.LoadSetting<ExtendedShipmentSetting>(_storeContext.CurrentStore.Id);
            var postType = GetPostType(item.Product.ProductCategories.Select(p => p.CategoryId).ToList());
            if (postType == 0)
            {
                InsertOrderNote("کد قرارداد نوع پست وارد نشده است", shipment.OrderId);
                return "";
            }
            StateCodemodel SenderCityCode = null;
            //if (DateTime.Now.CompareTo(Convert.ToDateTime("1399/03/18 12:00:00")) > 0)
            //{
            if (new int[] { 4, 581, 582, 583, 584, 585, 580 }.Contains(SenderStateId))
            {
                SenderCityCode =
               _repositoryStateCode.Table.FirstOrDefault(p =>
                   p.stateId == 582);
                saveRealState(shipment.Order);
                shipment.Order.BillingAddress.StateProvinceId = 582;
                _addressService.UpdateAddress(shipment.Order.BillingAddress);

            }
            else
            {
                SenderCityCode =
               _repositoryStateCode.Table.FirstOrDefault(p =>
                   p.stateId == SenderStateId);
            }
            //}
            //else
            //{
            //    SenderCityCode =
            //   _repositoryStateCode.Table.FirstOrDefault(p =>
            //       p.stateId == SenderStateId);
            //}

            if (SenderCityCode == null)
            {
                InsertOrderNote("کد شهر فرستنده مربوط به سفارش " + $"{shipment.OrderId}" + " یافت نشد", shipment.OrderId);
                return "RegeneratByCountryCentral2";
            }
            if (string.IsNullOrEmpty(SenderCityCode.SenderStateCode))
            {
                InsertOrderNote("کد شهر فرستنده مربوط به سفارش " + $"{shipment.OrderId}" + " یافت نشد", shipment.OrderId);
                return "RegeneratByCountryCentral2";
            }
            var RecivercityCode =
                _repositoryStateCode.Table.FirstOrDefault(p =>
                    p.stateId == ShippingAddress.StateProvinceId).StateCode;
            if (RecivercityCode == null)
            {
                InsertOrderNote("کد شهر گیرنده مربوط به سفارش " + $"{shipment.OrderId}" + " یافت نشد", shipment.OrderId);
                return "RegeneratByCountryCentral";

            }
            if (string.IsNullOrEmpty(RecivercityCode))
            {
                InsertOrderNote("کد شهر گیرنده مربوط به سفارش " + $"{shipment.OrderId}" + " یافت نشد", shipment.OrderId);
                return "RegeneratByCountryCentral";
            }
            if (ShippingAddress.CountryId == 1 && new int[] { 4, 581, 582, 583, 584, 585, 580, 579 }.Contains(ShippingAddress.StateProvinceId.Value))//ست کردن کد تهران
            {
                RecivercityCode = "00001";
            }
            var basicHttpbinding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            basicHttpbinding.Name = "getbarcode";
            basicHttpbinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            basicHttpbinding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            //basicHttpbinding.MaxReceivedMessageSize = 2147483647;
            //basicHttpbinding.MaxBufferSize = 2147483647;

            var insertedShippingServiceLog = new Tb_ShippingServiceLog
            {
                xPostUserName = extendedShipmentSetting.PostUserName,
                xPostPassword = extendedShipmentSetting.PostPassword,
                xSenderStateCode = SenderCityCode.SenderStateCode,
                xStateCode = RecivercityCode,
                xPostType = postType,
                xRequestDate = DateTime.Now
            };
            _repository_Tb_ShippingServiceLog.Insert(insertedShippingServiceLog);

            var endpointAddress = new EndpointAddress("http://poffice.post.ir/webbarcode/getbarcode.asmx");
            //var endpointAddress = new EndpointAddress("https://services.post.ir/webbarcode/getbarcode.asmx");

            var proxyClient = new Service1SoapClient(basicHttpbinding, endpointAddress);

            var tr = proxyClient;

            var Str_Error = "";
            var Str_Barcode = "";
            try
            {

                Str_Error = tr.getMassBarcode24(extendedShipmentSetting.PostUserName
                    , extendedShipmentSetting.PostPassword
                    , SenderCityCode.SenderStateCode
                    , RecivercityCode
                    , postType
                    , out Str_Barcode);
            }
            catch (Exception ex)
            { Str_Error = ex.Message + Environment.NewLine + "InnerException : " + (ex.InnerException?.Message); }

            if (!string.IsNullOrEmpty(Str_Error))
            {
                insertedShippingServiceLog.xErrorMessage = Str_Error;
                _repository_Tb_ShippingServiceLog.Update(insertedShippingServiceLog);
                InsertOrderNote("خطا در زمان تولید بارکد از سمت پست" + Str_Error, shipment.OrderId);
                if (Str_Error.Contains("کد شهر یا نوع مرسوله اشتباه است"))
                    return "RegeneratByCountryCentral";
                return "";
            }
            insertedShippingServiceLog.xErrorMessage = Str_Error;
            insertedShippingServiceLog.xResponseDate = DateTime.Now;
            insertedShippingServiceLog.xBarcode_Out = Str_Barcode;

            _repository_Tb_ShippingServiceLog.Update(insertedShippingServiceLog);


            //var result = SetBarcodeIsUsed(Str_Barcode, shipment.Id);
            //if (result == "BarcodeReserved")
            //{
            //    var msg = "بارکد " + "{0}" + " قبلا در سیستم ثبت شده";
            //    msg = string.Format(msg, Str_Barcode);
            //    Log(msg, "");
            //    return "";
            //}
            shipment.TrackingNumber = Str_Barcode;
            _shipmentService.UpdateShipment(shipment);
            return Str_Barcode;
        }

        private void saveRealState(Order order)
        {
            string query = $@"IF NOT EXISTS(SELECT
          	                            TOP(1)1
                                        FROM
				                            dbo.Tb_TehranSender AS TTS
			                            WHERE
				                            OrderId = {order.Id})
                            BEGIN
	                            INSERT INTO dbo.Tb_TehranSender
	                            (
		                            OrderId
		                            , realSenderState
	                            )
	                            VALUES
	                            (	{order.Id}
		                            , {order.BillingAddress.StateProvinceId.Value}
	                            )
	                            SELECT CAST(SCOPE_IDENTITY() AS INT) ID
                            END 
                            ELSE
	                            SELECT CAST(0 AS INT) Id";
            int p = _dbContext.SqlQuery<int>(query, new object[0]).FirstOrDefault();
        }

        private notUsedBarcode getFromNotUsedBarcode(int countryId, int stateProvinceId, int customerId, int serviceId, int weight)
        {
            string query = $@"EXEC dbo.Sp_GetNotUsedBarcode @CountryId, @StateId, @Weight, @ServiceId, @CustomerId";
            SqlParameter[] prms = new SqlParameter[]{
                new SqlParameter(){ ParameterName = "CountryId",SqlDbType= SqlDbType.Int,Value =countryId},
                new SqlParameter(){ ParameterName = "StateId",SqlDbType= SqlDbType.Int,Value =stateProvinceId},
                new SqlParameter(){ ParameterName = "Weight",SqlDbType= SqlDbType.Int,Value =weight},
                new SqlParameter(){ ParameterName = "ServiceId",SqlDbType= SqlDbType.Int,Value =serviceId},
                new SqlParameter(){ ParameterName = "CustomerId",SqlDbType= SqlDbType.Int,Value =customerId},
            };
            return _dbContext.SqlQuery<notUsedBarcode>(query, prms).FirstOrDefault();
        }
        public class notUsedBarcode
        {
            public string TrackingNumber { get; set; }
            public int OldShipmentid { get; set; }
        }
        public bool getDefulteSenderState(int customerId, out int senderStateId)
        {
            string strQuery = $@"SELECT
	                                TOP(1) DefulteStateId
                                FROM
	                                dbo.Tb_CustomerDefulteSenderState
                                WHERE
	                                CustomerId = ${customerId}
                                ORDER BY Id DESC ";
            senderStateId = _dbContext.SqlQuery<int?>(strQuery, new object[0]).FirstOrDefault().GetValueOrDefault(0);
            return senderStateId == 0 ? false : true;
        }

        public byte[] getBarocdeImage(Shipment shipment)
        {
            if (!string.IsNullOrEmpty(shipment.TrackingNumber))
            {
                var imageConvertor = new ImageConverter();
                var BarCode1 = new Barcode();
                var type = TYPE.CODE128;
                BarCode1.Encode(type, shipment.TrackingNumber, 600, 100);
                return (byte[])imageConvertor.ConvertTo(BarCode1.EncodedImage, typeof(byte[]));
            }
            return null;
        }
        public byte[] getBarocdeImage(string TrackingNumber)
        {
            if (!string.IsNullOrEmpty(TrackingNumber))
            {
                var imageConvertor = new ImageConverter();
                var BarCode1 = new Barcode();
                var type = TYPE.CODE128;
                BarCode1.Encode(type, TrackingNumber, 300, 50);
                return (byte[])imageConvertor.ConvertTo(BarCode1.EncodedImage, typeof(byte[]));
            }
            return null;
        }
        public byte[] getBarocdeImage_LablePrint(Shipment shipment)
        {
            if (!string.IsNullOrEmpty(shipment.TrackingNumber))
            {
                var imageConvertor = new ImageConverter();
                var BarCode1 = new Barcode();
                var type = TYPE.CODE128C;
                BarCode1.Encode(type, shipment.TrackingNumber, 900, 100);
                return (byte[])imageConvertor.ConvertTo(BarCode1.EncodedImage, typeof(byte[]));
            }
            return null;
        }
        public string GetBase64Image(string TrackingNumber)
        {
            if (!string.IsNullOrEmpty(TrackingNumber))
            {
                var imageConvertor = new ImageConverter();
                var BarCode1 = new Barcode();
                var type = TYPE.CODE128C;
                BarCode1.Encode(type, TrackingNumber, 600, 100);

                var imageData = (byte[])imageConvertor.ConvertTo(BarCode1.EncodedImage, typeof(byte[]));
                return Convert.ToBase64String(imageData);
            }
            return "";
        }
        public void UpdateFromPost(Shipment shipment, ExtendedShipmentSetting setting = null)
        {
            try
            {
                string error = "";
                _shipmentTrackingService.RegisterLastShipmentStatus(shipment.Id, false, out error);
                //if (setting == null)
                //    setting = _settingService.LoadSetting<ExtendedShipmentSetting>(_storeContext.CurrentStore.Id);

                //var Table = _shipmentTrackingService.getNoramlShipmentTracking(shipment);
                //var sended = Table.AsEnumerable().Where(r => r.Field<byte>("Type") == Convert.ToByte(1)
                //                                                      || r.Field<byte>("Type") == Convert.ToByte(2))
                //    .OrderBy(p => p.Field<DateTime>("TDate")).FirstOrDefault();

                //var Delivery = Table.AsEnumerable()
                //    .OrderByDescending(p => p.Field<DateTime>("TDate")).FirstOrDefault(r => r.Field<int?>("DType") == 3 ||
                //                                                                            r.Field<int?>("DType") == 51);

                //if (sended != null)
                //    shipment.ShippedDateUtc = Convert.ToDateTime(sended.ItemArray[1]).ToUniversalTime();

                //if (Delivery != null)
                //{
                //    shipment.DeliveryDateUtc = Convert.ToDateTime(Delivery.ItemArray[1]).ToUniversalTime();
                //    var TbOrder = _orderRepository.Table.SingleOrDefault(p => p.Id == shipment.Order.Id);
                //    TbOrder.ShippingStatus = ShippingStatus.Delivered;
                //    _orderRepository.Update(TbOrder);
                //}
                //_shipmentRepository.Update(shipment);
                //Table.Dispose();
                Log("خطا در زمان دریافت اطلاعات رهگیری", error);
            }
            catch (Exception ex)
            {
                Log("خطا در زمان دریافت اطلاعات رهگیری", ex.Message +
                                                         (ex.InnerException != null ? ex.InnerException.Message : "") +
                                                         "\r\n" + "خط:" + getline(ex));
            }
        }
        public void Log(string header, string Message)
        {
            var logger =
                (DefaultLogger)EngineContext.Current
                    .Resolve<ILogger>();
            logger.InsertLog(LogLevel.Information, header, Message, null);
        }
        /// <summary>
        /// ارسال پیام اختصاص کد رهگیری برای مشتری و ادمین سایت
        /// </summary>
        /// <param name="model"></param>
        public void SendSmsForCustomerAndAdmin(Shipment model)
        {
            if (!string.IsNullOrEmpty(model.TrackingNumber))
            {
                var extendedShipmentSetting = _setting;
                //_settingService.LoadSetting<ExtendedShipmentSetting>(_storeContext.CurrentStore.Id);
                if (extendedShipmentSetting == null)
                    return;
                var order = _orderService.GetOrderById(model.OrderId);
                if (order == null)
                    return;

                var msg = string.Format(extendedShipmentSetting.CustomerMessageTemplate, model.TrackingNumber,
                    model.OrderId);

                var sended = _notificationService._sendSms(getPhoneNumber(order.Customer.Id), msg);
                if (sended)
                    Log("ارسال پیام اختصاص شماره رهگیری برای سفارش " + order.Id, "");
                else
                    Log("عدم ارسال پیام اختصاص شماره رهگیری برای سفارش " + order.Id, "");
                var storeAdmin = _customerService.GetAllCustomers(
                    customerRoleIds: new[] { extendedShipmentSetting.StoreAdminRoleId },
                    pageIndex: 0,
                    pageSize: 5000).OrderByDescending(p => p.Id).FirstOrDefault();
                if (storeAdmin == null)
                    return;
                sended = _notificationService._sendSms(getPhoneNumber(storeAdmin.Id), msg);
                if (sended)
                    Log("ارسال پیام اختصاص شماره رهگیری برای سفارش " + order.Id, "");
                else
                    Log("عدم ارسال پیام اختصاص شماره رهگیری برای سفارش " + order.Id, "");
            }
        }
        private DataTable getShipmentDetils(Shipment shipment)
        {
            var SenderCityCode =
                _repositoryStateCode.Table.FirstOrDefault(p =>
                    p.stateId == shipment.Order.BillingAddress.StateProvinceId);
            if (SenderCityCode == null)
            {
                Log("کد شهر فرستنده مربوط به سفارش " + $"{shipment.OrderId}" + " یافت نشد", "");
                return null;
            }
            if (string.IsNullOrEmpty(SenderCityCode.SenderStateCode))
            {
                Log("کد شهر فرستنده مربوط به سفارش " + $"{shipment.OrderId}" + " یافت نشد", "");
                return null;
            }
            var RecivercityCode =
                _repositoryStateCode.Table.FirstOrDefault(p =>
                    p.stateId == shipment.Order.ShippingAddress.StateProvinceId);
            if (RecivercityCode == null)
            {
                Log("کد شهر گیرنده مربوط به سفارش " + $"{shipment.OrderId}" + " یافت نشد", "");
                return null;
            }
            if (string.IsNullOrEmpty(RecivercityCode.StateCode))
            {
                Log("کد شهر گیرنده مربوط به سفارش " + $"{shipment.OrderId}" + " یافت نشد", "");
                return null;
            }
            var bill = orderTotal(shipment.Order);
            var TotalProductCost =
                Convert.ToDecimal(
                    shipment.Order.OrderItems.Sum(p => GetItemCostFromAttr(p) * p.Quantity)); //کل سهم پست 
            var pr = new PersianCalendar();
            var shipmentDetailesTb = new DataTable();
            for (var i = 0; i < 25; i++)
                shipmentDetailesTb.Columns.Add(i.ToString());
            var dr = shipmentDetailesTb.NewRow();
            decimal BimeCost = 0;
            bill = orderTotal(shipment.Order);
            BimeCost = shipment.Order.OrderItems.Sum(p => GetBieme(p));
            //_taxService.LoadActiveTaxProvider().GetTaxRate(new CalculateTaxRequest() { })
            TotalProductCost = TotalProductCost + BimeCost;
            var tax = TotalProductCost * bill.Tax / 100;
            var TotalPrice = Convert.ToInt32(TotalProductCost + tax);
            dr[0] = 1011015; //extendedShipmentSetting.PostType;//1- کد قرار داد
            dr[1] = shipment.TrackingNumber; /*2- */
            dr[2] = 2; //ویژه=3/سفارشی=2/ پیشتاز=1  => نوع سرویس
            dr[3] = true; //نوع مرسوله => 0= پاکت / 1= بسته
            dr[4] = int.Parse(SenderCityCode.SenderStateCode);
            dr[5] = int.Parse(RecivercityCode.StateCode);
            dr[6] = (shipment.Order.BillingAddress.FirstName ?? "") + " " +
                    (shipment.Order.BillingAddress.LastName ?? "");
            dr[7] = (shipment.Order.ShippingAddress.FirstName ?? "") + " " +
                    (shipment.Order.ShippingAddress.LastName ?? "");
            dr[8] = pr.GetYear(DateTime.Now) + "/" + pr.GetMonth(DateTime.Now).ToString("00") + "/" +
                    pr.GetDayOfMonth(DateTime.Now).ToString("00");
            dr[9] = DateTime.Now.ToShortTimeString();
            dr[10] = shipment.Order.ShippingAddress.ZipPostalCode;
            dr[11] = shipment.Order.BillingAddress.ZipPostalCode;
            dr[12] = Convert.ToInt32(TotalProductCost).ToString(); // 0;//حق السهم پست به ریال
            dr[13] = 0; //حق السهم طرف قراردادبه ریال
            dr[14] = TotalPrice.ToString(); // جمع کل هزینه به ریال
            dr[15] = Convert.ToInt32(tax); //مالیات بر ارزش افزوده به ریال
            dr[16] = Convert.ToInt32(shipment.TotalWeight * 1000);
            dr[17] = Convert.ToInt16("2"); //بدون الصاق تمبر
            dr[18] = 'N'; //نسیه
            dr[19] = Convert.ToInt32(BimeCost); //BimePriceAdjustment; //مبلغ بیمه
            dr[20] = ""; //سایر اطلاعات
            dr[21] = ""; //کد ملی فرستنده
            dr[22] = ""; //کد ملی گیرنده
            dr[23] = shipment.Order.BillingAddress.Address1;
            dr[24] = shipment.Order.ShippingAddress.Address1;
            shipmentDetailesTb.Rows.Add(dr);
            return shipmentDetailesTb;
        }

        public string getPhoneNumber(int customerId)
        {
            try
            {
                var customer = _customerService.GetCustomerById(customerId);
                if (customer == null)
                    return "";
                if (customer.BillingAddress != null)
                    if (!string.IsNullOrEmpty(customer.BillingAddress.PhoneNumber))
                        return customer.BillingAddress.PhoneNumber;
                if (!customer.Addresses.Any())
                    return "";
                var phoneNUmber = customer.Addresses.First().PhoneNumber;
                if (!string.IsNullOrEmpty(phoneNUmber))
                    return phoneNUmber;
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        /// <summary>
        /// ارساال اطلاعات به پست در جزئیات محموله نسخه قدیمی
        /// </summary>
        /// <param name="shipmentId"></param>
        /// <returns></returns>
        public string SendDataToPost(int shipmentId)
        {
            try
            {
                var shipment = _shipmentService.GetShipmentById(shipmentId);
                if (shipment.Order.OrderNotes.Any(p => p.Note.Contains("SendDataToPost")))
                    return "ارسال اطلاعات قبلا انجام شده";
                var extendedShipmentSetting = _setting;
                //_settingService.LoadSetting<ExtendedShipmentSetting>(_storeContext.CurrentStore.Id);
                if (string.IsNullOrEmpty(shipment.TrackingNumber))
                    return "ابتدا شماره رهگیری را انتصاب دهید";

                var dataTable = getShipmentDetils(shipment);

                if (dataTable == null)
                    return "خطا در زمان واکشی اطلاعات سفارش";
                var basicHttpbinding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                basicHttpbinding.Name = "MassContractsService";
                basicHttpbinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                basicHttpbinding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                //basicHttpbinding.MaxReceivedMessageSize = 2147483647;
                //basicHttpbinding.MaxBufferSize = 2147483647;
                var endpointAddress =
                    new EndpointAddress("http://poffice.post.ir/contracts/MassContractsService.asmx");
                var MCSSC =
                    new MassContractsServiceSoapClient(basicHttpbinding, endpointAddress);
                var ds = new DataSet();
                ds.Tables.Add(dataTable);

                var sendResult = MCSSC.PushVarContractParcels(ds,
                    "1011015", // extendedShipmentSetting.PostUserName,
                    extendedShipmentSetting.PostPassword);
                if (sendResult == null)
                {
                    InsertOrderNote("SendDataToPost", shipment.OrderId);
                    InsertOrderNote(ds.GetXml(), shipment.OrderId);
                    return "";
                }
                Log("خطا در زمان ارسال اطلاعات به پست", sendResult);
                return "خطا در زمان ارسال اطلاعات به پست";
            }
            catch (Exception ex)
            {
                Log("خطا در زمان ارسال اطلاعات به پست ",
                    ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                return "خطا سیستم در زمان ارسال اطلاعات به پست ";
            }
        }
        public int GetPostType(List<int> categuryIds)
        {
            var extendedShipmentSetting = _setting;
            //_settingService.LoadSetting<ExtendedShipmentSetting>(_storeContext.CurrentStore.Id);
            if (extendedShipmentSetting == null)
                return 0;
            if (categuryIds == null)
                return 0;
            foreach (var Cid in categuryIds)
            {
                var item = _repositoryCateguryPostType.Table.FirstOrDefault(p => p.CateguryId == Cid);
                if (item != null)
                    return item.PostType;
            }
            return int.Parse(extendedShipmentSetting.PostType);
        }

        public int getAddtionalFee(Customer customer, int? shipmentId)
        {
            int Int_AddtionalFee = 0;
            if (shipmentId.HasValue)
            {
                Int_AddtionalFee = customer.GetAttribute<int>("AddtionalFeeForCOD_" + shipmentId.Value, _genericAttributeService
                    , _storeContext.CurrentStore.Id);
            }

            return Int_AddtionalFee;
        }

        public OrderBill orderTotal(Order order)
        {
            var bill = new OrderBill
            {
                ProductPrice = order.OrderItems.Sum(p => GetItemPriceFromAttr(p) * p.Quantity),
                Productcost = order.OrderItems.Sum(p => GetItemCostFromAttr(p) * p.Quantity),
                Tax = order.TaxRatesDictionary.First().Key
            };

            foreach (var orderItem in order.OrderItems)
            {
                var OIV = _OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == orderItem.Id);
                var PriceItems = OIV.Where(p => p.PropertyAttrName.Contains("وزن بسته") == false
                    && p.PropertyAttrValuePrice > 0).Select(p => new BillItem
                    {
                        name = p.PropertyAttrName,
                        value = (p.PropertyAttrValuePrice.HasValue ? p.PropertyAttrValuePrice.Value : 0),
                        qty = orderItem.Quantity
                    }).ToList();

                if (bill.AttributePrice == null)
                    bill.AttributePrice = new List<BillItem>();
                if (PriceItems.Any())
                    bill.AttributePrice.AddRange(PriceItems);

                var CostItems = OIV.Where(p => p.PropertyAttrName.Contains("وزن بسته") == false
                    && p.PropertyAttrValueCost > 0).Select(p => new BillItem
                    {
                        name = p.PropertyAttrName,
                        value = (p.PropertyAttrValueCost.HasValue ? p.PropertyAttrValueCost.Value : 0),
                        qty = orderItem.Quantity
                    }).ToList();
                if (bill.AttributeCost == null)
                    bill.AttributeCost = new List<BillItem>();
                if (CostItems.Any())
                    bill.AttributeCost.AddRange(CostItems);
            }
            return bill;
        }
        public OrderBill orderItemTotal(OrderItem orderitem)
        {

            var bill = new OrderBill
            {
                ProductPrice = GetItemPriceFromAttr(orderitem),
                Productcost = GetItemCostFromAttr(orderitem),
                Tax = orderitem.Order.TaxRatesDictionary.First().Key,
                AttributePrice = new List<BillItem>(),
                AttributeCost = new List<BillItem>()
            };
            var OIV = _OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == orderitem.Id);
            var PriceItems = OIV.Where(p => p.PropertyAttrName.Contains("وزن بسته") == false
                && p.PropertyAttrName.Contains("نوع و وزن کالا") == false
                && p.PropertyAttrValuePrice > 0).Select(p => new BillItem
                {
                    name = p.PropertyAttrName,
                    value = (p.PropertyAttrValuePrice.HasValue ? p.PropertyAttrValuePrice.Value : 0),
                    qty = 1
                }).ToList();
            if (PriceItems.Any())
                bill.AttributePrice.AddRange(PriceItems);

            var CostItems = OIV.Where(p => p.PropertyAttrName.Contains("وزن بسته") == false
                && p.PropertyAttrName.Contains("نوع و وزن کالا") == false
                && p.PropertyAttrValueCost > 0).Select(p => new BillItem
                {
                    name = p.PropertyAttrName,
                    value = (p.PropertyAttrValueCost.HasValue ? p.PropertyAttrValueCost.Value : 0),
                    qty = 1
                }).ToList();
            if (bill.AttributeCost == null)
                bill.AttributeCost = new List<BillItem>();
            if (CostItems.Any())
                bill.AttributeCost.AddRange(CostItems);
            return bill;
        }

        public string getOrderItemWehghtName(OrderItem orderitem, bool ignoreBime = false)
        {
            var ItemAttr = _OrderItemAttributeValueRepository.Table.SingleOrDefault(p => p.OrderItemId == orderitem.Id && p.PropertyAttrName.Contains("وزن بسته")); ;

            if (ignoreBime)
            {
                if ((new List<decimal> { 1, 2, Convert.ToDecimal(0.5) }).Contains(ItemAttr.PropertyAttrValueWeight.Value))
                {
                    string _PropertyAttrName = "";
                    if (ItemAttr.PropertyAttrName.Contains("0 تا 500"))
                    {
                        _PropertyAttrName = ItemAttr.PropertyAttrName.Replace("گرم", "کیلو گرم");
                    }
                    else
                        _PropertyAttrName = ItemAttr.PropertyAttrName;
                    return _PropertyAttrName.Replace("0 تا 1", "0 تا 2").Replace("1 تا 2", "0 تا 2").Replace("0 تا 500", "0 تا 2");
                    // .Replace("2 تا 3", "0 تا 2");
                }
                else if ((new List<decimal> { 5, 3, 4 }).Contains(ItemAttr.PropertyAttrValueWeight.Value))
                {
                    return ItemAttr.PropertyAttrValueName.Replace("3 تا 5", "2 تا 5").Replace("2 تا 3", "2 تا 5");
                }
                else if ((new List<decimal> { 15, 20 }).Contains(ItemAttr.PropertyAttrValueWeight.Value))
                {
                    return ItemAttr.PropertyAttrValueName.Replace("5 تا 10", "10 تا 20")
                        .Replace("15 تا 20", "10 تا 20");
                }
                else if ((new List<decimal> { 25, 30 }).Contains(ItemAttr.PropertyAttrValueWeight.Value))
                {
                    return ItemAttr.PropertyAttrValueName.Replace("20 تا 25", "20 تا 30")
                        .Replace("25 تا 30", "20 تا 30");
                }
            }

            return ItemAttr.PropertyAttrValueName;
        }

        /// <summary>
        /// واکشی خطی که خطا در آن رخ داده
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        /// 
        public int getline(Exception ex)
        {
            // Get stack trace for the exception with source file information
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            return frame.GetFileLineNumber();
        }

        #region COD

        public void SendToCod(Order order, bool IsCOD)
        {
            bool hasError = false;
            //error = "";
            bool isSafeBuy = IsSafeBuy(order.Id);
            if (order.OrderNotes.Any(p => p.Note.Contains("SendDataToPost")))
            {
                //error = "ممکن است اطلاعات مرسوله قبلا ارسال شده باشد";
                return;
            }
            InsertOrderNote("SendDataToPost", order.Id);
            if (order.OrderNotes.Any(p => p.Note.Contains("updateprice")))
            {
                updateCodCost(order, IsCOD);
                return;
            }
            string CountryCode = "";
            string StateCode = "";
            bool isMultiShpment = IsMultiShippment(order);
            if (!isMultiShpment)
            {
                string error = processShipmentAddressForCOD(order.ShippingAddress.Id, order.Id, out StateCode, out CountryCode);
                if (!string.IsNullOrEmpty(error))
                {
                    ChangeOrderState(order);
                    return;
                }
            }
            Shipment shipment = null;
            int masterCounter = 0;
            int EngPriceTotal = 0;
            int CodPiceTotal = 0;
            bool IsFreePost = false;
            int counterForRegnarteBarcode = 0;
            int Haghemaghar = getInsertedHagheMaghar(order);
            Haghemaghar += Convert.ToInt32(Haghemaghar * 9 / 100);
            int _haghemagharForshipment = Convert.ToInt32(Haghemaghar / order.OrderItems.Sum(p => p.Quantity));
            List<string> ForIgnorUserName = new List<string>();
            int _orderItemAproxsimateValue = 0;
            int _serviceId = order.OrderItems.First().Product.ProductCategories.First().CategoryId;
            foreach (var item in order.OrderItems)
            {
                _orderItemAproxsimateValue = getApproximateValue(item.Id);

                var postType = GetPostType(item.Product.ProductCategories.Select(p => p.CategoryId).ToList()) == 11 ? "1" : "0";
            ForRegenerateBarcode:
                counterForRegnarteBarcode++;
                for (int i = 0; i < item.Quantity; i++)
                {
                    counterForRegnarteBarcode = 0;
                    masterCounter++;
                    try
                    {
                        Address ShipmentAddress = null;
                        if (isMultiShpment)
                        {
                            var multiShipment = getShipmentFromMultiShipment(item, i);
                            if (multiShipment == null)
                            {
                                InsertOrderNote("اطلاعات مربوط به حمل و نقل ایتم سفارش " + item.Id + " یافت نشد ", order.Id);
                                ChangeOrderState(order);
                                return;
                            }
                            shipment = multiShipment?.shipment;
                            if (!string.IsNullOrEmpty(shipment.TrackingNumber))
                                continue;
                            int? shipmentAddressId = multiShipment?.ShipmentAddressId;
                            string error = processShipmentAddressForCOD(shipmentAddressId.Value, order.Id, out StateCode, out CountryCode);
                            if (!string.IsNullOrEmpty(error))
                            {
                                ChangeOrderState(order);
                                return;
                            }
                            ShipmentAddress = _addressService.GetAddressById(shipmentAddressId.Value);
                        }
                        else
                        {
                            ShipmentAddress = order.ShippingAddress;
                            if (HasShipmpment(item, i))
                                continue;
                        }

                        int ItemWeight = GetItemWeightFromAttr(item);

                        #region Calc EngPrice
                        int discount_percent = 0;
                        if (IsCOD)
                        {
                            IsFreePost = GetFreePost(item);
                            discount_percent = _agentAmountRuleService.GetPrivatePostDiscountForCustomer(order.CustomerId, _serviceId);
                        }
                        var GetPriceRulst = CalcGatewayPrice(order.Customer, CountryCode, StateCode, IsCOD, ItemWeight, postType, order.BillingAddress.StateProvinceId.Value,
                             item, _haghemagharForshipment, ForIgnorUserName: ForIgnorUserName, ServiceId: _serviceId, isFreePost: IsFreePost);

                        if (GetPriceRulst.CodPostPrice != null && GetPriceRulst.HaizneKala <= GetPriceRulst.CodPostPrice[0] && IsFreePost)
                        {
                            InsertOrderNote("امکان ثبت این سفارش به صورت ارسال رایگان وجود ندارد به دلیل اینکه مبلغ کالا از مبلغ کرایه پستی کمتر می باشد", order.Id);
                            ChangeOrderState(order);
                            return;
                        }

                        if (!GetPriceRulst.Success)
                        {
                            ChangeOrderState(order);
                            InsertOrderNote(GetPriceRulst.ErrorMessage, order.Id);
                            hasError = true;
                            continue;
                        }
                        CodPiceTotal += Convert.ToInt32(GetPriceRulst.CodPostPrice[0] + GetPriceRulst.CodPostPrice[1]);
                        EngPriceTotal += GetPriceRulst.HaizneKala;
                        #endregion

                        #region Send To Gateway And Get TrackingNumber
                        var orderDetailes = (string.IsNullOrEmpty(GetPriceRulst.AgentUserName) ? order.Customer.Username : GetPriceRulst.AgentUserName) + "^";
                        orderDetailes += (string.IsNullOrEmpty(ShipmentAddress.PhoneNumber)
                                             ? ""
                                             : ShipmentAddress.PhoneNumber) + "^";

                        //masterCounter+=20;
                        orderDetailes += order.Id.ToString() + "_" + item.Id.ToString() + "_" + masterCounter.ToString() + "1^";
                        orderDetailes += CountryCode + "^";
                        orderDetailes += StateCode + "^";
                        orderDetailes += item.Product.Name + " " + (getOrderItemContent(item) ?? "").Replace(";", "-") + "^";
                        orderDetailes += (ItemWeight < 50 ? 80 : ItemWeight) + "^";
                        orderDetailes += (GetPriceRulst.HaizneKala + (IsCOD ? 0 : GetPriceRulst.approximateValue)) + "^";
                        orderDetailes += postType + "^"; //نحوه ارسال
                        orderDetailes += (IsFreePost ? "88^" : (IsCOD ? "0^" : "1^")); //نوع پرداخت 0= پرداخت در محل 1= پرداخت با کارت شتاب
                        orderDetailes += ((string.IsNullOrEmpty(ShipmentAddress.FirstName) ? "" : ShipmentAddress.FirstName) + " "
                                         + (string.IsNullOrEmpty(ShipmentAddress.LastName) ? "" : ShipmentAddress.LastName) + "^");
                        orderDetailes += (string.IsNullOrEmpty(ShipmentAddress.Address1)
                                            ? ShipmentAddress.Address2
                                            : ShipmentAddress.Address1).Replace(";", ",") + "^";
                        orderDetailes += (string.IsNullOrEmpty(ShipmentAddress.ZipPostalCode)
                                             ? ""
                                             : ShipmentAddress.ZipPostalCode) + "^";
                        orderDetailes += (string.IsNullOrEmpty(ShipmentAddress.Email) ? "" : ShipmentAddress.Email) +
                                         "^";
                        orderDetailes += "0^";
                        // string hazine = HazineErsal + "^" + HazineErsalTax;
                        string hazine = GetPriceRulst.CodPostPrice[0] + "^" + GetPriceRulst.CodPostPrice[1];// ((int)(Convert.ToInt32(CodPostPrice[0])*9/100)).ToString();
                        string NeworderDetailes = orderDetailes + hazine;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        var basicHttpbinding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                        basicHttpbinding.Name = "CODContractsService";
                        basicHttpbinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                        basicHttpbinding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                        InsertOrderNote(NeworderDetailes, order.Id);
                        //if (_workContext.CurrentCustomer.Id != 273739)
                        //{

                        string EncodedOrderDetailes = System.Web.HttpUtility.UrlEncode(NeworderDetailes);

                        var str_brcode = _codService.NewOrder2(EncodedOrderDetailes, "1");

                        str_brcode = str_brcode.Replace(";", "");
                        if (str_brcode.StartsWith("402^") && counterForRegnarteBarcode < 3)
                        {
                            counterForRegnarteBarcode++;
                            string InvalidUserName = (string.IsNullOrEmpty(GetPriceRulst.AgentUserName) ? order.Customer.Username : GetPriceRulst.AgentUserName);
                            ForIgnorUserName.Add(InvalidUserName);
                            InsertOrderNote($"نام کاربری {InvalidUserName} امکان ثبت سفارش برای مرسوله شماره {shipment.Id} ندارد", order.Id);
                            goto ForRegenerateBarcode;
                        }
                        if (str_brcode.StartsWith("502^") && counterForRegnarteBarcode < 3)
                        {
                            counterForRegnarteBarcode++;
                            InsertOrderNote($"کد استان و شهر برای  مقصد مرسوله شماره  {shipment.Id} یافت نشد", order.Id);
                            ChengeReceiverToCentralState(ShipmentAddress.Id);
                            goto ForRegenerateBarcode;
                        }
                        str_brcode = str_brcode.Replace("ErrorNo:5000, You Can not Send Kala Price <50000 Rial505^", "0^");
                        if (string.IsNullOrEmpty(str_brcode))
                        {
                            InsertOrderNote("برای سفارش شماره " + order.Id + " بارکدی از سامانه گیت وی پست اختصاص داده نشد", order.Id);
                            ChangeOrderState(order);
                            return;
                        }
                        string ProcessCodResultmsg = "";
                        bool HasBarcode = false;
                        if (str_brcode.Contains("^"))
                        {
                            hazine = "";
                            HasBarcode = ProcessCodResult(str_brcode, ref ProcessCodResultmsg, ref hazine);
                            Log("آنالیز اطلاعات بازگشتی از گیت وی", ProcessCodResultmsg + "----" + hazine);
                            if (!HasBarcode && !string.IsNullOrEmpty(hazine))
                            {
                                InsertOrderNote($"ثبت سفارش در گیت وی برای سفارش شماره {item.Id} انجام نشد", order.Id);
                                hasError = true;
                                continue;
                                //CodPostPrice = new[] { int.Parse(hazine.Split('^')[0]), int.Parse(hazine.Split('^')[1]) };
                                //int_AddtionalFee = int.Parse(hazine.Split('^')[0]) - HazineErsal;
                                //EncodedOrderDetailes = System.Web.HttpUtility.UrlEncode(orderDetailes + hazine);
                                //Log("لاگ گیت وی", orderDetailes + hazine);
                                //str_brcode = COD.NewOrder2("postbar", "postbar12345", EncodedOrderDetailes, "1");
                                //str_brcode = str_brcode.Replace(";", "");
                                //ProcessCodResultmsg = "";
                                //hazine = "";
                                //HasBarcode = ProcessCodResult(str_brcode, ref ProcessCodResultmsg, ref hazine);
                            }
                        }
                        else
                        {
                            InsertOrderNote("فرمت اطلاعات بازگشتی از سرویس گیت وی نا معتبر می باشد" + str_brcode, order.Id);
                            ChangeOrderState(order);
                            hasError = true;
                            continue;
                        }
                        if (!HasBarcode)
                        {
                            InsertOrderNote(ProcessCodResultmsg, order.Id);
                            ChangeOrderState(order);
                            hasError = true;
                            continue;
                        }

                        if (!isMultiShpment)
                            shipment = AddShipment(item, ProcessCodResultmsg);
                        else if (string.IsNullOrEmpty(shipment.TrackingNumber))
                        {
                            shipment.TrackingNumber = ProcessCodResultmsg;
                            _shipmentService.UpdateShipment(shipment);
                        }
                        //}
                        #endregion

                        #region ثبت مبالغ گیت وی در بانک
                        if (isSafeBuy && !IsCOD)
                        {
                            GetPriceRulst.HaizneKala += _orderItemAproxsimateValue;
                        }
                        setCODCost(shipment.Id, GetPriceRulst.CodPostPrice[0], Convert.ToInt32(GetPriceRulst.HaizneKala));
                        var OrderSource = getSourceByOrder(order.Id);
                        if (OrderSource == 16 || (order.Customer.IsInCustomerRole("mini-Administrators")))
                            _codService.ChangeStatus(shipment.TrackingNumber, 1);
                        // حتما ذخیر کن 
                        InsertGatewayDetailes(shipment.Id, GetPriceRulst.CodPostPrice[0], GetPriceRulst.myBenefitFromPostPrice, GetPriceRulst.AttrPrice,
                            GetPriceRulst.approximateValue, (_haghemagharForshipment * 100 / 109), GetPriceRulst.CodTranPrice, GetPriceRulst.BaseCodPrice[0]);
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        InsertOrderNote("خطا در زمان ارسال اطلاعات برای سامانه گیت وی " +
                            ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""), order.Id);
                        hasError = true;
                        continue;
                    }
                }
            }
            if (hasError)
            {
                ChangeOrderState(order);
                return;
            }
            if (order.Shipments.Any())
            {
                if (IsCOD || IsFreePost)
                {
                    if (CodPiceTotal + EngPriceTotal > 0)
                    {
                        var _order = _orderRepository.Table.Single(p => p.Id == order.Id);
                        int discount = 0;
                        if (order.OrderDiscount > 0)
                        {
                            discount = (int)Math.Floor(order.OrderDiscount / order.Shipments.Count());
                        }
                        int orderTotal = getOrderTotalFromShipment(order.Id, IsFreePost, discount);
                        _order.OrderTotal = orderTotal;
                        _orderRepository.Update(_order);
                    }
                }
                _notificationService.SendSmsToSender_Reciver(order.Id, this);
                InsertOrderNote("A shipment has been added", order.Id);
                order.OrderStatus = OrderStatus.Complete;
                _orderService.UpdateOrder(order);
                SavePostCoordination(order.Id, "ارجاع به پست گیت وی");
            }
        }
        public int InsertGatewayDetailes(int shipmentId, int PostPrice, int EngPrice, int AttrPrice,
            int CodGoodsValue, int HagheMagharPrice, int CodTranPrice, int postBasePrice)
        {
            string query = $@"
                        DECLARE @Id INT = 0
                        SELECT
	                        @Id = TCCD.Id
                        FROM
	                        dbo.Tb_CodCalculationDetailes AS TCCD
                        WHERE
	                        TCCD.ShipmentId = {shipmentId}
                        IF ISNULL(@Id,0) > 0
                        BEGIN
                            SELECT @Id InsertedId
	                        RETURN
                        END
                        INSERT INTO dbo.Tb_CodCalculationDetailes
                        (
	                        ShipmentId
	                        , PostPrice
	                        , EngPrice
	                        , AttrPrice
	                        , CodGoodsValue
	                        , AddedToWallet
	                        , CreateDate
	                        , CodEventId
	                        , HagheMagharPrice
	                        , CodTranPrice
                            , PostBasePrice
                        )
                        VALUES
                        (	{shipmentId} -- ShipmentId - int
	                        , {PostPrice} -- PostPrice - int
	                        , {EngPrice} -- EngPrice - int
	                        , {AttrPrice} -- AttrPrice - int
	                        , {CodGoodsValue} -- CodGoodsValue - int
	                        , null -- AddedToWallet - int
	                        , GETDATE() -- CreateDate - datetime
	                        , null -- CodEventId - int
	                        , {HagheMagharPrice} -- HagheMagharPrice - int
	                        , {CodTranPrice} -- CodTranPrice - int
                            , {postBasePrice}
	                    ) SELECT CAST(SCOPE_IDENTITY() as int) InsertedId";
            int InsertedId = _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
            return InsertedId;
        }
        private int getOrderTotalFromShipment(int orderId, bool isFreePost, int discount)
        {
            string query = $@"DECLARE @IsFreePost bit= {(isFreePost ? "1" : "0")},
                                    @Discount INT = {discount}
                                    SELECT
	                                    sa.CodCost,
	                                    ((sa.CodBmValue - (ISNULL(TOIR.GoodsCodPrice,0) + ISNULL(TCCD.CodTranPrice,0))) * 100 / 109) CodBmValue,
	                                    sa.CodBmValue CodBmValueOrginal, 
	                                    @IsFreePost IsFreePost,
	                                    ISNULL(TOIR.GoodsCodPrice,0) GoodsCodPrice,
	                                    ISNULL(TCCD.CodTranPrice,0) CodTranPrice
                                    INTO #tb1
                                    FROM
	                                    dbo.Shipment AS S
	                                    INNER JOIN dbo.ShipmentAppointment AS SA ON SA.ShipmentId = S.Id
	                                    INNER JOIN dbo.ShipmentItem AS SI ON SI.ShipmentId = S.Id
	                                    INNER JOIN dbo.OrderItem AS OI ON SI.OrderItemId = OI.Id
	                                    INNER JOIN dbo.Tb_OrderItemsRecord AS TOIR ON OI.Id = TOIR.OrderItemId
	                                    INNER JOIN dbo.Tb_CodCalculationDetailes AS TCCD ON TCCD.ShipmentId = S.Id
                                    WHERE 
	                                    s.OrderId={orderId}
                                    SELECT
	                                    SUM(CASE WHEN T.IsFreePost = 1 THEN T.CodBmValueOrginal ELSE (T.CodCost+(T.CodCost*9/100))+(T.CodBmValue - ISNULL(@Discount,0)) + ((T.CodBmValue - ISNULL(@Discount,0)) *9/100) + T.GoodsCodPrice + T.CodTranPrice END) OrderToral
                                    FROM
	                                    #tb1 AS T";
            return _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
        }
        public bool GetFreePost(OrderItem item)
        {
            string query = $@"DECLARE @FreePost BIT = null
                            SELECT
	                            @FreePost = CAST(CASE WHEN TOIAV.PropertyAttrValueName = N'بلی' THEN 1 ELSE 0 END  AS BIT)
                            FROM
	                            dbo.Tb_OrderItemAttributeValue AS TOIAV
                            WHERE
	                            TOIAV.OrderItemId = {item.Id}
	                            AND TOIAV.PropertyAttrName LIKE N'%ارسال رایگان%'
                            SELECT CAST(ISNULL(@FreePost,0) AS BIT) IsFreePost";
            return _dbContext.SqlQuery<bool?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(false);
        }

        public void updateCodCost(Order order, bool IsCOD)
        {
            string CountryCode = "";
            string StateCode = "";
            bool isMultiShpment = IsMultiShippment(order);
            if (!isMultiShpment)
            {
                string error = processShipmentAddressForCOD(order.ShippingAddress.Id, order.Id, out StateCode, out CountryCode);
                if (!string.IsNullOrEmpty(error))
                {
                    ChangeOrderState(order);
                    return;
                }
            }
            Shipment shipment = null;
            int masterCounter = 0;
            int EngPriceTotal = 0;
            int CodPiceTotal = 0;
            int Haghemaghar = getInsertedHagheMaghar(order);
            Haghemaghar += Convert.ToInt32(Haghemaghar * 9 / 100);
            int _haghemagharForshipment = Convert.ToInt32(Haghemaghar / order.OrderItems.Sum(p => p.Quantity));
            bool IsFreePost = false;
            foreach (var item in order.OrderItems)
            {
                var postType = GetPostType(item.Product.ProductCategories.Select(p => p.CategoryId).ToList()) == 11 ? "1" : "0";
                if (IsCOD && !IsFreePost)
                    IsFreePost = GetFreePost(item);
                for (int i = 0; i < item.Quantity; i++)
                {
                    masterCounter++;
                    try
                    {
                        Address ShipmentAddress = null;
                        if (isMultiShpment)
                        {
                            var multiShipment = getShipmentFromMultiShipment(item, i);
                            if (multiShipment == null)
                            {
                                InsertOrderNote("اطلاعات مربوط به حمل و نقل ایتم سفارش " + item.Id + " یافت نشد ", order.Id);
                                ChangeOrderState(order);
                                return;
                            }
                            shipment = multiShipment?.shipment;
                            int? shipmentAddressId = multiShipment?.ShipmentAddressId;
                            string error = processShipmentAddressForCOD(shipmentAddressId.Value, order.Id, out StateCode, out CountryCode);
                            if (!string.IsNullOrEmpty(error))
                            {
                                ChangeOrderState(order);
                                return;
                            }
                        }
                        else
                        {
                            shipment = order.Shipments.First();
                        }

                        int ItemWeight = GetItemWeightFromAttr(item);

                        #region Calc EngPrice
                        var GetPriceRulst = CalcGatewayPrice(order.Customer, CountryCode, StateCode, IsCOD, ItemWeight, postType, order.BillingAddress.StateProvinceId.Value, item
                            , _haghemagharForshipment, ServiceId: 670);
                        //if (IsCOD)
                        //{
                        //    _agentAmountRuleService.GetPrivatePostDiscountForCustomer(order.CustomerId,)
                        //}
                        if (!GetPriceRulst.Success)
                        {
                            ChangeOrderState(order);
                            InsertOrderNote(GetPriceRulst.ErrorMessage, order.Id);
                            continue;
                        }
                        CodPiceTotal += Convert.ToInt32(GetPriceRulst.CodPostPrice[0] + GetPriceRulst.CodPostPrice[1]);
                        EngPriceTotal += GetPriceRulst.HaizneKala;
                        #endregion

                        #region ثبت مبالغ گیت وی در بانک

                        setCODCost(shipment.Id, GetPriceRulst.CodPostPrice[0], Convert.ToInt32(GetPriceRulst.HaizneKala));

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        InsertOrderNote("به روز رسانی قیمت ها انجام نشد" +
                            ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""), order.Id);
                        return;
                    }
                }
            }
            if (order.Shipments.Any())
            {
                if (IsCOD || IsFreePost)
                {
                    if (CodPiceTotal + EngPriceTotal > 0)
                    {
                        var _order = _orderRepository.Table.Single(p => p.Id == order.Id);
                        int discount = 0;
                        if (order.OrderDiscount > 0)
                        {
                            discount = (int)Math.Floor(order.OrderDiscount / order.Shipments.Count());
                        }
                        int orderTotal = getOrderTotalFromShipment(order.Id, IsFreePost, discount);
                        _order.OrderTotal = orderTotal;
                        _orderRepository.Update(_order);
                    }
                }
                _notificationService.SendSmsToSender_Reciver(order.Id, this);
                DeleteOrderNote("updateprice", order.Id);
                InsertOrderNote("به روز رسانی قیمت ها انجام شد", order.Id);
            }
        }
        public CodGetPriceRersult CalcGatewayPrice(Customer customer, string ReciverCountryCode, string ReciverCityCode, bool IsCOD, int ItemWeight, string postType
            , int SenderStateId, OrderItem orderItem, int _haghemagharForshipment, int _approximateValue = 0, List<string> ForIgnorUserName = null, int ServiceId = 0,
            bool isFreePost = false)
        {
            if (ItemWeight < 50)
                ItemWeight = 80;
            int AttrPrice = 0;
            int kalaPrice = 0;
            if (orderItem != null)
            {
                AttrPrice = getOrderItemsPrice(orderItem.Id);
                kalaPrice = getGoodsPrice(orderItem);
            }
            string _AgentUserName = "";
            string Str_error = "";
            int[] _CodBasePrice;
            _CodBasePrice = getCodPostPriceService(customer.Id, ItemWeight, 50000, customer.Username, int.Parse(ReciverCountryCode)
                 , int.Parse(ReciverCityCode), IsCOD, int.Parse(postType), out Str_error, out _AgentUserName, SenderStateId, orderItem, ForIgnorUserName);
            if (!string.IsNullOrEmpty(Str_error))
            {
                return new CodGetPriceRersult()
                {
                    ErrorMessage = Str_error,
                    Success = false
                };
            }
            int addedPercent = 30;//33;// IsCOD ? 18 : 33;

            int basePrice = _CodBasePrice[0] - 8000;

            int _bajePrice = ((basePrice * 100) / (100 - addedPercent)) + 8000;
            int myBenefitFromPostPrice = (((basePrice * 100) / (100 - addedPercent))) - basePrice;

            var HazineKala = AttrPrice + myBenefitFromPostPrice;

            int HazineKalaTax = (int)((HazineKala * 9) / 100);
            if (!isFreePost)
            {
                HazineKala = Convert.ToInt32(HazineKala + HazineKalaTax) + kalaPrice + _haghemagharForshipment;
            }
            else
            {
                HazineKala = kalaPrice;
                HazineKalaTax = 0;
            }
            if (HazineKala < 50000 && IsCOD)//حداقل مبلغ گیت وی برای هزینه ای که از مشتری دریافت 5000 تومان است
            {
                HazineKala = 50000 + ((50000 * 9) / 100);
            }
            int[] CodPostPrice;

            int approximateValue = 0;
            if (!IsCOD)
            {
                if (orderItem == null)
                    approximateValue = _approximateValue;
                else
                    approximateValue = getApproximateValue(orderItem.Id);
                if (Convert.ToInt32(HazineKala) + (IsCOD ? 0 : approximateValue) < 50000)
                {
                    approximateValue += (50000 - (Convert.ToInt32(HazineKala) + (IsCOD ? 0 : approximateValue)));
                }
            }
            int discount_percent = 0;
            if (!isFreePost)
            {
                discount_percent = _agentAmountRuleService.GetPrivatePostDiscountForCustomer(customer.Id, ServiceId);
            }
            int discount = 0;

            int CodTranPrice = 0;
            if (IsCOD)
            {
                int TranPercent = 0;
                if (orderItem == null)
                {
                    TranPercent = GetContractItemPersent(_workContext.CurrentCustomer.Id, 12);
                }
                else
                    TranPercent = GetContractItemPersent(orderItem.Order.CustomerId, 12);
                CodTranPrice = (kalaPrice * TranPercent / 100);
                if (!isFreePost && kalaPrice != 0)
                    HazineKala += CodTranPrice;// اگر قرارداد داشت باید از قرارداد استفاده بشه
            }
            if (discount_percent > 0)
            {
                discount = ((_CodBasePrice[0] + _CodBasePrice[1]) * discount_percent / 100);
                HazineKala = HazineKala - discount;
            }
            CodPostPrice = getCodPostPriceService(customer.Id, ItemWeight
                , Convert.ToInt32(HazineKala) + (IsCOD ? 0 : approximateValue)
                , customer.Username
                , int.Parse(ReciverCountryCode)
                , int.Parse(ReciverCityCode)
                , IsCOD, int.Parse(postType)
                , out Str_error
                , out _AgentUserName
                , SenderStateId
                , orderItem
                , ForIgnorUserName);

            if (!string.IsNullOrEmpty(Str_error))
            {
                //ChangeOrderState(order);
                InsertOrderNote(Str_error, orderItem.Order.Id);
                return new CodGetPriceRersult()
                {
                    ErrorMessage = Str_error,
                    Success = false
                };
            }

            return new CodGetPriceRersult()
            {
                Success = true,
                AgentUserName = _AgentUserName,
                HaizneKala = HazineKala,
                CodPostPrice = CodPostPrice,
                BaseCodPrice = _CodBasePrice,
                approximateValue = approximateValue,
                myBenefitFromPostPrice = myBenefitFromPostPrice,
                CodTranPrice = CodTranPrice,
                AttrPrice = AttrPrice
            };

        }
        public int GetContractItemPersent(int CustomerId, int ContractItemTypeId)
        {
            string query = $@"DECLARE @CodTranPercent INT = 3
					SELECT
						@CodTranPercent = ContractItemPercent
					FROM
						dbo.GetContractItemPercent_Final({CustomerId},12)AS GCIPF

					SET @CodTranPercent=ISNULL(@CodTranPercent,3)

					IF @CodTranPercent = 0 
						SET @CodTranPercent = 3
                    SELECT @CodTranPercent ContractItemPercent";
            return _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(3);

        }
        public class CodGetPriceRersult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public string AgentUserName { get; set; }
            public int HaizneKala { get; set; }
            public int approximateValue { get; set; }
            public int[] CodPostPrice { get; set; }
            public int[] BaseCodPrice { get; set; }
            public int CodTranPrice { get; set; }
            public int myBenefitFromPostPrice { get; set; }
            public int AttrPrice { get; set; }
            public int discount { get; set; }
        }

        private bool ProcessCodResult(string str_brcode, ref string processCodResultmsg, ref string PostSendPrice)
        {
            string[] ResultParts = str_brcode.Split('^');

            if (ResultParts.Length < 2)
            {
                processCodResultmsg += " " + "اطلاعات دریافت شده از سرویس گیت وی نام معتبر می باشد" + "-->" + str_brcode;
                return false;
            }
            if ((ResultParts[0] == "0" || ResultParts[0] == "505") && ResultParts[1].Length > 10)
            {
                processCodResultmsg = ResultParts[1];
                return true;
            }
            if (ResultParts[0] == "807")
            {
                PostSendPrice = ResultParts[4] + "^" + ResultParts[5];
                return false;
            }
            string errorMg = GateWayError.GetErrorMsg(ResultParts[0]);
            if (errorMg == "")
            {
                processCodResultmsg += " " + "اطلاعات دریافت شده از سرویس گیت وی نام معتبر می باشد" + "-->" + str_brcode; ;
                return false;
            }
            else
            {
                processCodResultmsg = errorMg;
                return false;
            }

        }
        public Shipment AddShipment(OrderItem orderItem, string barcode)
        {
            if (orderItem == null)
                return null;
            Shipment shipment = null;
            decimal? totalWeight = null;

            //is shippable
            if (!orderItem.Product.IsShipEnabled)
                return null;

            //ensure that this product can be shipped (have at least one item to ship)

            var qtyToAdd = 1;
            //multiple warehouses are not supported
            var warehouseId = orderItem.Product.WarehouseId;

            //ok. we have at least one item. let's create a shipment (if it does not exist)

            var orderItemTotalWeight = orderItem.ItemWeight.HasValue ? orderItem.ItemWeight * qtyToAdd : null;
            if (orderItemTotalWeight.HasValue)
            {
                totalWeight = orderItemTotalWeight.Value;
            }
            if (shipment == null)
            {
                var trackingNumber = barcode;
                var adminComment = "";
                shipment = new Shipment
                {
                    OrderId = orderItem.OrderId,
                    TrackingNumber = trackingNumber,
                    TotalWeight = null,
                    ShippedDateUtc = null,
                    DeliveryDateUtc = null,
                    AdminComment = adminComment,
                    CreatedOnUtc = DateTime.UtcNow,
                };
            }
            //create a shipment item
            var shipmentItem = new ShipmentItem
            {
                OrderItemId = orderItem.Id,
                Quantity = qtyToAdd,
                WarehouseId = warehouseId
            };
            shipment.ShipmentItems.Add(shipmentItem);

            //if we have at least one item in the shipment, then save it
            if (shipment != null && shipment.ShipmentItems.Any())
            {
                shipment.TotalWeight = totalWeight;
                _shipmentService.InsertShipment(shipment);
                return shipment;
            }
            return null;
        }
        public string processShipmentAddressForCODPrice(int senderCountrId, int SenderStateId, out string StateCode, out string CountryCode)
        {

            string error = "";
            StateCode = CountryCode = "";
            var ReciverCountryCOde =
               _repositoryCountryCode.Table.FirstOrDefault(p => p.CountryId == senderCountrId);
            if (ReciverCountryCOde == null)
            {
                error = "کد استان گیرنده یافت نشد";
                Log(error, "");
                return error;
            }
            if (string.IsNullOrEmpty(ReciverCountryCOde.CountryCode))
            {
                error = "کد استان گیرنده یافت نشد";
                Log(error, "");
                return error; ;
            }
            var RecivercityCode =
                _repositoryStateCode.Table.FirstOrDefault(p =>
                    p.stateId == SenderStateId);
            if (RecivercityCode == null)
            {
                error = "کد شهر گیرنده یافت نشد";
                Log(error, "");
                return error;
            }
            if (string.IsNullOrEmpty(RecivercityCode.StateCode))
            {
                error = "کد شهر گیرنده یافت نشد";
                Log(error, "");
                return error;
            }
            //طبق گفته آقای وزیری گیت وی پست برای مناطق پستی تهران جای کد استان و شهرستان عوض میشه
            //if (RecivercityCode.StateCode.StartsWith("10") && ReciverCountryCOde.CountryCode == "1")
            //{
            //    StateCode = ReciverCountryCOde.CountryCode;
            //    CountryCode = RecivercityCode.StateCode;
            //}
            //else
            //{
            StateCode = RecivercityCode.StateCode;
            CountryCode = ReciverCountryCOde.CountryCode;
            //}
            return error;
        }
        public string processShipmentAddressForCOD(int addressId, int orderId, out string StateCode, out string CountryCode)
        {
            var shipmentAddress = _addressService.GetAddressById(addressId);
            string error = "";
            StateCode = CountryCode = "";
            if (string.IsNullOrEmpty(shipmentAddress.ZipPostalCode))
            {
                error = "کد پستی گیرنده نا معتبر است";
                InsertOrderNote(error, orderId);
                return error;
            }
            if (shipmentAddress.ZipPostalCode.Length != 10)
            {
                error = "کد پستی گیرنده نا معتبر است";
                InsertOrderNote(error, orderId);
                return error;
            }
            var ReciverCountryCOde =
               _repositoryCountryCode.Table.FirstOrDefault(p => p.CountryId == shipmentAddress.CountryId);
            if (ReciverCountryCOde == null)
            {
                error = "کد استان گیرنده مربوط به سفارش " + $"{orderId}" + " یافت نشد";
                Log(error, "");
                return error;
            }
            if (string.IsNullOrEmpty(ReciverCountryCOde.CountryCode))
            {
                error = "کد استان گیرنده مربوط به سفارش " + $"{orderId}" + " یافت نشد";
                Log(error, "");
                return error; ;
            }
            var RecivercityCode =
                _repositoryStateCode.Table.FirstOrDefault(p =>
                    p.stateId == shipmentAddress.StateProvinceId);
            if (RecivercityCode == null)
            {
                error = "کد شهر گیرنده مربوط به سفارش " + $"{orderId}" + " یافت نشد";
                Log(error, "");
                return error;
            }
            if (string.IsNullOrEmpty(RecivercityCode.StateCode))
            {
                error = "کد شهر گیرنده مربوط به سفارش " + $"{orderId}" + " یافت نشد";
                Log(error, "");
                return error;
            }
            //طبق گفته آقای وزیری گیت وی پست برای مناطق پستی تهران جای کد استان و شهرستان عوض میشه
            if (RecivercityCode.StateCode.StartsWith("10") && ReciverCountryCOde.CountryCode == "1")
            {
                //StateCode = ReciverCountryCOde.CountryCode;
                //CountryCode = RecivercityCode.StateCode;


                StateCode = ReciverCountryCOde.CountryCode;
                CountryCode = RecivercityCode.StateCode;
            }
            else
            {
                StateCode = RecivercityCode.StateCode;
                CountryCode = ReciverCountryCOde.CountryCode;
            }
            return error;
        }
        public int GetGatwayPriceCountryCode(int countryId)
        {
            return _repositoryCountryCode.Table.Where(p => p.CountryId == countryId).FirstOrDefault().printCountryCode.GetValueOrDefault(0);
        }
        private bool getPostCountryAndStateCode(int CountryId, int stateId, out string StateCode, out string CountryCode)
        {
            StateCode = "";
            CountryCode = "";
            var ReciverCountryCOde =
               _repositoryCountryCode.Table.FirstOrDefault(p => p.CountryId == CountryId);
            string error = "";
            if (ReciverCountryCOde == null)
            {
                error = "کد پست استان مربوط شناسه استان " + $"{CountryId}" + " یافت نشد";
                Log(error, "");
                return false;
            }
            if (string.IsNullOrEmpty(ReciverCountryCOde.CountryCode))
            {
                error = "کد پست استان مربوط شناسه استان " + $"{CountryId}" + " یافت نشد";
                Log(error, "");
                return false;
            }
            var RecivercityCode =
                _repositoryStateCode.Table.FirstOrDefault(p =>
                    p.stateId == stateId);
            if (RecivercityCode == null)
            {
                error = "کد پست شهرستان مربوط شناسه شهرستان " + $"{stateId}" + " یافت نشد";
                Log(error, "");
                return false;
            }
            if (string.IsNullOrEmpty(RecivercityCode.StateCode))
            {
                error = "کد پست شهرستان مربوط شناسه شهرستان " + $"{stateId}" + " یافت نشد";
                Log(error, "");
                return false;
            }

            StateCode = RecivercityCode.StateCode;
            CountryCode = ReciverCountryCOde.CountryCode;
            return true;
        }
        private string getAddressForCOD(int countryId, int stateId, out string StateCode, out string CountryCode)
        {
            string error = "";
            StateCode = CountryCode = "";
            var ReciverCountryCOde =
               _repositoryCountryCode.Table.FirstOrDefault(p => p.CountryId == countryId);
            if (ReciverCountryCOde == null)
            {
                return error;
            }
            if (string.IsNullOrEmpty(ReciverCountryCOde.CountryCode))
            {
                return error; ;
            }
            var RecivercityCode =
                _repositoryStateCode.Table.FirstOrDefault(p =>
                    p.stateId == stateId);
            if (RecivercityCode == null)
            {
                return error;
            }
            if (string.IsNullOrEmpty(RecivercityCode.StateCode))
            {
                return error;
            }
            //طبق گفته آقای وزیری گیت وی پست برای مناطق پستی تهران جای کد استان و شهرستان عوض میشه
            if (RecivercityCode.StateCode.StartsWith("10") && ReciverCountryCOde.CountryCode == "1")
            {
                StateCode = ReciverCountryCOde.CountryCode;
                CountryCode = RecivercityCode.StateCode;
            }
            else
            {
                StateCode = RecivercityCode.StateCode;
                CountryCode = ReciverCountryCOde.CountryCode;
            }
            return error;
        }
        /// <summary>
        /// واکشی مقادیر وارد شده اظهارت مشتری در مورد مرسوله
        /// </summary>
        /// <param name="orderitem"></param>
        /// <returns></returns>
        public string getOrderItemContent(OrderItem orderitem)
        {
            return _OrderItemAttributeValueRepository.Table.Single(p => p.OrderItemId == orderitem.Id && p.PropertyAttrName.Contains("نوع و وزن")).PropertyAttrValueText;
        }
        /// <summary>
        /// دریافت ارزش کالا
        /// </summary>
        /// <param name="orderitem"></param>
        /// <returns></returns>
        public int getGoodsPrice(OrderItem orderitem)
        {
            var item = _OrderItemAttributeValueRepository.Table.SingleOrDefault(p => p.OrderItemId == orderitem.Id && p.PropertyAttrName.Contains("وجهی که بابت کالایتان"));
            if (item != null)
                return (string.IsNullOrEmpty(item.PropertyAttrValueText) ? "0" : item.PropertyAttrValueText).ToEnDigit();
            return 0;
        }
        public OrderBill CODorderItemTotal(OrderItem orderitem)
        {
            var bill = new OrderBill
            {
                ProductPrice = GetItemPriceFromAttr(orderitem),
                Productcost = GetItemCostFromAttr(orderitem),
                Tax = orderitem.Order.TaxRatesDictionary.First().Key,
                AttributePrice = new List<BillItem>(),
                AttributeCost = new List<BillItem>()
            };

            var OIV = _OrderItemAttributeValueRepository.Table.Where(p => p.OrderItemId == orderitem.Id);
            if (bill.AttributePrice == null)
                bill.AttributePrice = new List<BillItem>();
            var _kalaPriceItem = OIV.SingleOrDefault(p => p.PropertyAttrName.Contains("وجهی که بابت کالایتان"));
            if (_kalaPriceItem != null)
            {
                var kalaPriceItem = new BillItem
                {
                    name = _kalaPriceItem.PropertyAttrName,
                    value = _kalaPriceItem.PropertyAttrValueText.ToEnDigit(),
                    qty = 1,
                    priceType = PriceType.kala
                };
                bill.AttributePrice.Add(kalaPriceItem);
            }

            var PriceItems = OIV.Where(p => p.PropertyAttrName.Contains("وزن بسته") == false
                && p.PropertyAttrValuePrice > 0).Select(p => new BillItem
                {
                    name = p.PropertyAttrName,
                    value = (p.PropertyAttrValuePrice.HasValue ? p.PropertyAttrValuePrice.Value : 0),
                    qty = 1,
                    priceType = p.PropertyAttrName.Contains("بیمه") ? PriceType.bime :
                                    (p.PropertyAttrName.Contains("مقر") ? PriceType.hagheMaghar :
                                        (p.PropertyAttrName.Contains("ثبت") ? PriceType.hagheSabte :
                                            (p.PropertyAttrName.Contains("کارتون") ? PriceType.karton :
                                                (p.PropertyAttrName.Contains("پیامک") ? PriceType.Sms :
                                                    (p.PropertyAttrName.Contains("پرینتر") ? PriceType.AccessPrinter : PriceType.none)))
                                        )
                                    )
                }).ToList();
            if (PriceItems.Any())
                bill.AttributePrice.AddRange(PriceItems);

            var CostItems = OIV.Where(p => p.PropertyAttrName.Contains("وزن بسته") == false
                && p.PropertyAttrValueCost > 0).Select(p => new BillItem
                {
                    name = p.PropertyAttrName,
                    value = (p.PropertyAttrValueCost.HasValue ? p.PropertyAttrValueCost.Value : 0),
                    qty = 1,
                    priceType = p.PropertyAttrName.Contains("بیمه") ? PriceType.bime :
                                    (p.PropertyAttrName.Contains("مقر") ? PriceType.hagheMaghar :
                                        (p.PropertyAttrName.Contains("ثبت") ? PriceType.hagheSabte :
                                            (p.PropertyAttrName.Contains("کارتون") ? PriceType.karton :
                                                (p.PropertyAttrName.Contains("پیامک") ? PriceType.Sms :
                                                    (p.PropertyAttrName.Contains("پرینتر") ? PriceType.AccessPrinter : PriceType.none)))
                                        )
                                    )
                }).ToList();
            if (bill.AttributeCost == null)
                bill.AttributeCost = new List<BillItem>();
            if (CostItems.Any())
                bill.AttributeCost.AddRange(CostItems);
            return bill;
        }
        public int CalcCODPriceApi(Core.Domain.Catalog.Product product
            , int weight
            , string attributesXml
            , string userName
            , int countryId
            , int stateId
            , int postType
            , out string error)
        {
            error = "";
            //var List = new OrderList() { };
            //string AttributesXml = getCheckoutAttributeXml(List, true, product.Id);
            var Lst_PAM = _productAttributeParser.ParseProductAttributeMappings(attributesXml);
            var AttributePrice = new List<BillItem>();
            var AttributeCost = new List<BillItem>();
            #region attr
            foreach (var PAM in Lst_PAM)
            {
                if (PAM.AttributeControlType == AttributeControlType.TextBox)
                {
                    var txtPrice = _productAttributeParser.ParseValues(attributesXml, PAM.Id).FirstOrDefault();
                    decimal fltTxtPrice = 0;
                    if (PAM.ProductAttribute.Name.Contains("وجهی که بابت کالایتان"))
                    {

                        AttributePrice.Add(new BillItem
                        {
                            name = PAM.ProductAttribute.Name,
                            value = txtPrice.ToEnDigit(),
                            qty = 1,
                            priceType = PriceType.kala
                        });
                    }
                }
                else
                {
                    var productAttributeValue =
                        _productAttributeParser.ParseProductAttributeValues(attributesXml, PAM.Id).FirstOrDefault();

                    if (productAttributeValue != null && productAttributeValue.PriceAdjustment > 0)
                    {
                        AttributePrice.Add(new BillItem
                        {
                            name = PAM.ProductAttribute.Name,
                            value = productAttributeValue.PriceAdjustment,
                            qty = 1,
                            priceType = PAM.ProductAttribute.Name.Contains("بیمه")
                                ? PriceType.bime
                                : (PAM.ProductAttribute.Name.Contains("مقر")
                                    ? PriceType.hagheMaghar
                                    : (PAM.ProductAttribute.Name.Contains("ثبت")
                                        ? PriceType.hagheSabte
                                        : (PAM.ProductAttribute.Name.Contains("کارتون")
                                            ? PriceType.karton
                                            : (PAM.ProductAttribute.Name.Contains("وزن بسته") ? PriceType.weight
                                                : (PAM.ProductAttribute.Name.Contains("ثبت سفارش") ? PriceType.sabteSefarsh : PriceType.none)
                                            ))
                                    )
                                )
                        });
                    }

                    if (productAttributeValue != null && productAttributeValue.Cost > 0)
                    {
                        AttributeCost.Add(new BillItem
                        {
                            name = PAM.ProductAttribute.Name,
                            value = productAttributeValue.Cost,
                            qty = 1,
                            priceType = PAM.ProductAttribute.Name.Contains("بیمه")
                                ? PriceType.bime
                                : (PAM.ProductAttribute.Name.Contains("مقر")
                                    ? PriceType.hagheMaghar
                                    : (PAM.ProductAttribute.Name.Contains("ثبت")
                                        ? PriceType.hagheSabte
                                        : (PAM.ProductAttribute.Name.Contains("کارتون")
                                            ? PriceType.karton
                                            : (PAM.ProductAttribute.Name.Contains("وزن بسته") ? PriceType.weight
                                                : (PAM.ProductAttribute.Name.Contains("ثبت سفارش") ? PriceType.sabteSefarsh : PriceType.none)
                                                ))
                                    )
                                )
                        });
                    }
                }
            }
            #endregion
            var totalCost = AttributeCost.Where(p =>
                               p.priceType != PriceType.hagheSabte
                               && p.priceType != PriceType.kala
                               && p.priceType != PriceType.bime//حذف بیمه از هزینه پستی
                               && p.priceType != PriceType.karton).Sum(p => p.value);

            var totalValue = AttributePrice.Where(p =>
                                 p.priceType != PriceType.hagheSabte
                                 && p.priceType != PriceType.kala
                                 && p.priceType != PriceType.karton).Sum(p => p.value);

            decimal engPrice = (totalValue - totalCost);

            var kalaPrice = (AttributePrice.FirstOrDefault(p => p.priceType == PriceType.kala)?.value)
                .GetValueOrDefault(0);

            var hagheSabte = (AttributePrice.FirstOrDefault(p => p.priceType == PriceType.hagheSabte)?.value).GetValueOrDefault(0);

            var tmp = engPrice + kalaPrice;
            var HazineKalaTax = Convert.ToInt32((engPrice * 9) / 100);
            var HazineKala = Convert.ToInt64(engPrice + HazineKalaTax + hagheSabte);

            var COD3 = Convert.ToInt64(((tmp + HazineKalaTax + hagheSabte) * 3) / 100);

            HazineKala += COD3;


            string stateCode, countryCode;
            var result = getAddressForCOD(countryId, stateId, out stateCode, out countryCode);
            if (!string.IsNullOrEmpty(result))
            {
                error = result;
                return 0;
            }
            string _AgentUserName = "";
            int[] CodPrice = getCodPostPriceService(_workContext.CurrentCustomer.Id, weight, Convert.ToInt32(HazineKala), userName, Convert.ToInt32(countryCode)
                , Convert.ToInt32(stateCode), true, postType,
                out error, out _AgentUserName, 0);
            if (CodPrice[0] == 0 && !string.IsNullOrEmpty(error))
            {
                return 0;
            }
            return CodPrice[0] + CodPrice[1] + Convert.ToInt32(HazineKala);
        }
        private bool getCodApiKey(int customerId, out string ApiKey)
        {
            ApiKey = "";
            var genericAttrbuiteXml = _genericAttributeService.GetAttributesForEntity(customerId, "Customer").Where(p => p.Key == "CustomCustomerAttributes").FirstOrDefault()?.Value;
            if (string.IsNullOrEmpty(genericAttrbuiteXml))
                return false;
            ApiKey = _customerAttributeParser.ParseValues(genericAttrbuiteXml, 7).FirstOrDefault();
            if (string.IsNullOrEmpty(ApiKey))
                return false;
            return true;
        }

        /// <summary>
        /// دریافت نام کاربری نماینده تهران که با اکانت آن باید بارکد تولید شودس
        /// </summary>
        /// <param name="OrderItemId"></param>
        /// <returns></returns>
        public int getGatwayUser(int OrderItemId)
        {
            string query = $@"EXEC dbo.Sp_GetGatewayBarcodeUser @OrderItemId";

            SqlParameter P_OrderItemId = new SqlParameter()
            {
                ParameterName = "OrderItemId",
                SqlDbType = SqlDbType.Int,
                Value = (object)OrderItemId
            };

            SqlParameter[] prms = new SqlParameter[]{
                P_OrderItemId
            };
            return _dbContext.SqlQuery<int?>(query, prms).FirstOrDefault().GetValueOrDefault(0);
        }
        private int[] getCodPostPriceService(int customerId, int weight, int HazineKala, string userName, int countryCode, int stateCode
            , bool IsCOD, int postType, out string error, out string AgentUserName, int SenderStateId, OrderItem orderItem = null, List<string> ForIgnorUserName = null
            , bool IsFreePost = false)
        {
            AgentUserName = "";
            var _customer = _customerService.GetCustomerById(customerId);

            string[] ResultParts = new string[] { };
            int[] ResultgetPrice = new int[] { 0, 0 };
            Customer OrderCustomer = _customer;
            if (!string.IsNullOrEmpty(userName))
                OrderCustomer = _customerService.GetCustomerByUsername(userName);
            error = "";
            {
                int[] SenderStateList;

                SenderStateList = new int[] { SenderStateId };
                List<int> AgentLilst = new List<int>();
                int Section = 0;

                if ((orderItem == null))
                {
                    Section = 1;

                    string _IgnoreUserName = "";
                    if (ForIgnorUserName != null && ForIgnorUserName.Any())
                    {
                        _IgnoreUserName = string.Join("','", ForIgnorUserName);
                        if (!string.IsNullOrEmpty(_IgnoreUserName))
                        {
                            _IgnoreUserName = "'" + _IgnoreUserName + "'";
                        }
                    }
                    string query = $@"SELECT distinct
	                                    TOP(5) c.Id
                                    INTO #tb1
                                    FROM
	                                    dbo.Customer AS C
	                                    INNER JOIN dbo.Customer_CustomerRole_Mapping AS CCRM ON CCRM.Customer_Id = C.Id
	                                    INNER JOIN dbo.Tb_UserStates AS TUS ON TUS.CustomerId = C.Id
	                                    INNER JOIN dbo.StateProvince AS SP ON SP.Id = TUS.StateId
                                    WHERE
	                                    C.Active =1
                                        AND C.Deleted = 0 
	                                    AND CCRM.CustomerRole_Id = 7
	                                    AND sp.Id in({string.Join(",", SenderStateList)})";
                    if (!string.IsNullOrEmpty(_IgnoreUserName))
                    {
                        query += $@" AND C.UserName Not in ({_IgnoreUserName})";
                    }
                    query += $@" IF @@ROWCOUNT > 0
                                    BEGIN
                                        SELECT
		                                    Id
                                        FROM
		                                    #tb1 AS T
	                                    RETURN
                                    END
                                    SELECT
	                                    TANN.RepresentativeCustomerId AS Id
                                    FROM
	                                    dbo.Tbl_AgentNearpostNode AS TANN
                                    WHERE
	                                    TANN.NearSateId in({string.Join(",", SenderStateList)})";
                    var items = _dbContext.SqlQuery<int>(query, new object[0]).ToList();
                    if (OrderCustomer.IsInCustomerRole("Collector"))
                    {
                        AgentLilst.Add(OrderCustomer.Id);
                    }
                    if (_workContext.CurrentCustomer.IsInCustomerRole("PostCollector"))
                    {
                        AgentLilst.Add(OrderCustomer.Id);
                    }
                    if (items != null && items.Any())
                    {
                        AgentLilst.AddRange(items);
                    }
                }
                else
                {
                    Section = 2;
                    if (OrderCustomer.IsInCustomerRole("Collector"))
                    {
                        AgentLilst.Add(OrderCustomer.Id);
                    }
                    if (orderItem.Order.Customer.IsInCustomerRole("PostCollector"))
                    {
                        AgentLilst.Add(OrderCustomer.Id);
                    }
                    else
                    {
                        int user = getGatwayUser(orderItem.Id);
                        if (user > 0)
                        {
                            AgentLilst.Add(user);
                            InsertOrderNote($"کاربر {string.Join(",", AgentLilst)} به عنوان نماینده برای این مرسوله تشخیص داده شد", orderItem.Order.Id);
                        }
                    }

                }
                if (!AgentLilst.Any() && Section == 2)
                {
                    string _IgnoreUserName = "";
                    if (ForIgnorUserName != null && ForIgnorUserName.Any())
                    {
                        _IgnoreUserName = string.Join("','", ForIgnorUserName);
                        if (!string.IsNullOrEmpty(_IgnoreUserName))
                        {
                            _IgnoreUserName = "'" + _IgnoreUserName + "'";
                        }
                    }
                    string query = $@"SELECT distinct
	                                    TOP(5) c.Id
                                    FROM
	                                    dbo.Customer AS C
	                                    INNER JOIN dbo.Customer_CustomerRole_Mapping AS CCRM ON CCRM.Customer_Id = C.Id
	                                    INNER JOIN dbo.Tb_UserStates AS TUS ON TUS.CustomerId = C.Id
	                                    INNER JOIN dbo.StateProvince AS SP ON SP.Id = TUS.StateId
                                    WHERE
	                                    C.Active =1
                                        AND C.Deleted = 0 
	                                    AND CCRM.CustomerRole_Id = 7
	                                    AND sp.Id in({string.Join(",", SenderStateList)})";
                    if (!string.IsNullOrEmpty(_IgnoreUserName))
                    {
                        query += $@" AND C.UserName Not in ({_IgnoreUserName})";
                    }
                    AgentLilst = _dbContext.SqlQuery<int>(query, new object[0]).ToList();
                    if (AgentLilst.Any())
                        InsertOrderNote($"امکان تشخیص نماینده با توجه به جمع آور سفارش نبود و کاربر {string.Join(",", AgentLilst)} به عنوان نماینده تشخبص داده شده", orderItem.Order.Id);
                }
                if (AgentLilst.Any())
                {
                    foreach (var _AgentcustomerId in AgentLilst)
                    {
                        var _agentCustomer = _customerService.GetCustomerById(_AgentcustomerId);
                        if (ForIgnorUserName != null && ForIgnorUserName.Contains(_agentCustomer.Username))
                            continue;
                        string _priceResult = _codService.GetPrice(weight, Convert.ToInt32(HazineKala), _agentCustomer.Username
                           , Convert.ToInt32(countryCode), Convert.ToInt32(stateCode), postType, (IsCOD ? 0 : 1)
                           , 1);
                        var _ResultParts = _priceResult.Split(';');
                        if (_ResultParts.Length != 3)
                        {
                            string msg = GateWayError.GetErrorMsg(_ResultParts[0]);
                            Log("خطا در زمان استعلام قیمت گیت وی با اکانت نماینده برای مشتری", msg);
                            error = msg;

                        }
                        else if (_ResultParts[0] == "810")
                        {
                            string msg = GateWayError.GetErrorMsg(_ResultParts[0]);
                            Log("خطا در زمان استعلام قیمت گیت وی با اکانت نماینده برای مشتری", msg);
                            error = msg;
                        }
                        else if (_ResultParts[1] == "0" && _ResultParts[2] == "0")
                        {
                            string msg = GateWayError.GetErrorMsg(_ResultParts[0]);
                            Log("خطا در زمان استعلام قیمت گیت وی با اکانت نماینده برای مشتری", msg);
                            error = msg;

                        }
                        else
                        {
                            error = "";
                            AgentUserName = _agentCustomer.Username;
                            return new int[] { Convert.ToInt32(_ResultParts[0]), Convert.ToInt32(_ResultParts[2]) };
                        }
                    }
                }
                else
                {
                    string msg = GateWayError.GetErrorMsg(ResultParts[0]);
                    Log("نماینده ای برای استعلام قیمت گیت وی یافت نشد", msg);
                }
            }
            return ResultgetPrice;
        }

        public int getCodbasePrice(int customerId, int serviceId, int weight, int countryId, int stateId, int SenderStateId, out string error, bool IsFromAp = false
            , OrderItem orderItem = null, bool IsCOD = true, int approximateValue = 0)
        {
            var customer = _customerService.GetCustomerById(customerId);
            var postType = GetPostType(new List<int>() { serviceId }) == 11 ? "1" : "0";
            string StateCode, CountryCode;
            StateCode = CountryCode = "";
            error = processShipmentAddressForCODPrice(countryId, stateId, out StateCode, out CountryCode);
            if (!string.IsNullOrEmpty(error))
            {
                error = "در حال حاضر امکان دریافت قیمت وجود ندارد";
                return 0;
            }
            int _haghemagharForshipment = 0;
            if (orderItem != null)
            {
                int Haghemaghar = getInsertedHagheMaghar(orderItem.Order);
                Haghemaghar += Convert.ToInt32(Haghemaghar * 9 / 100);
                _haghemagharForshipment = Convert.ToInt32(Haghemaghar / orderItem.Order.OrderItems.Sum(p => p.Quantity));
            }
            var GetPriceRulst = CalcGatewayPrice(customer, CountryCode, StateCode, IsCOD, weight, postType, SenderStateId, orderItem, _haghemagharForshipment, approximateValue, ServiceId: serviceId);
            if (!GetPriceRulst.Success)
            {
                Log(error, "");
                if (orderItem != null)
                    InsertOrderNote(GetPriceRulst.ErrorMessage, orderItem.Order.Id);
                error = "در حال حاضر امکان دریافت قیمت وجود ندارد";
                return 0;
            }
            if (orderItem != null)
            {
                var shipments = orderItem.Order.Shipments.Where(p => p.ShipmentItems.Any(n => n.OrderItemId == orderItem.Id)).ToList();
                foreach (var item in shipments)
                {
                    setCODCost(item.Id, GetPriceRulst.CodPostPrice[0], GetPriceRulst.HaizneKala);
                }
            }
            return GetPriceRulst.CodPostPrice[0] + GetPriceRulst.CodPostPrice[1] + GetPriceRulst.HaizneKala;
        }
        private bool setCODCost(int shipmentId, int CODCost, int CodBMValue)
        {

            var SAR = _ShipmentAppointmentRepository.Table.OrderByDescending(p => p.Id)
                .FirstOrDefault(p => p.ShipmentId == shipmentId);
            if (SAR == null)
            {
                SAR = new ShipmentAppointmentModel()
                {
                    ShipmentId = shipmentId,
                    CodCost = CODCost,
                    CodBmValue = CodBMValue
                };
                _ShipmentAppointmentRepository.Insert(SAR);
                return true;
            }
            SAR.CodCost = CODCost;
            SAR.CodBmValue = CodBMValue;
            _ShipmentAppointmentRepository.Update(SAR);
            //  _notificationService.NotifyCollectShipment(shipmentId);
            return true;

        }

        public int[] getCODCost(int shipmentId)
        {
            var SAR = _ShipmentAppointmentRepository.Table.OrderByDescending(p => p.Id)
                .FirstOrDefault(p => p.ShipmentId == shipmentId);
            if (SAR == null)
                return new int[] { 0, 0 };
            if (SAR.CodCost == 0)
                return new int[] { 0, 0 };
            return new int[] { SAR.CodCost.Value, Convert.ToInt32((SAR.CodCost * 9) / 100) };

        }
        #endregion

        #region سفارشی-پیشتاز
        public void ProcessOrderForPost(Order order)
        {
            bool isMultiShpment = IsMultiShippment(order);
            // InsertOrderItem("SendDataToPost|"+DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"), order.Id);
            string SenderStateCode = "";
            string StateCode = "";
            int DefulteSnderState = 0;
            if (!getDefulteSenderState(order.CustomerId, out DefulteSnderState))
            {
                DefulteSnderState = order.BillingAddress.StateProvinceId.Value;
            }
            var SenderCityCode =
                   _repositoryStateCode.Table.FirstOrDefault(p =>
                       p.stateId == DefulteSnderState);

            if (SenderCityCode == null || string.IsNullOrEmpty(SenderCityCode.SenderStateCode))
            {
                //ChengeSenderToCentralState(null, order);
                InsertOrderNote("کد شهر فرستنده مربوط به سفارش " + $"{order.Id}" + " یافت نشد", order.Id);
                ChangeOrderState(order);
                return;
            }

            SenderStateCode = SenderCityCode.SenderStateCode;
            if (!isMultiShpment)
            {
                string error = "";
                StateCode = processShipmentAddress(order.ShippingAddress.Id, order.Id, out error, true);
                if (!string.IsNullOrEmpty(error))
                {
                    ChangeOrderState(order);
                    return;
                }
            }
            InsertOrderNote("SendDataToPost", order.Id);
            DataTable shipmentDetailesTb = new DataTable();
            for (var i = 0; i < 25; i++)
                shipmentDetailesTb.Columns.Add(i.ToString());
            bool IsDataSendToPost = IsDataSendedToPost(order);
            List<int> LstOldShipmwntId = new List<int>();
            foreach (var orderitem in order.OrderItems)
            {
                for (int i = 0; i < orderitem.Quantity; i++)
                {
                    Shipment shipment = null;
                    int? shipmentAddressId = null;

                    //if (!isMultiShpment)
                    //{
                    //    bool hasShipment = HasShipmpment(orderitem, i);
                    //    if (!hasShipment)
                    //        shipment = AddShipmentNormalPost(orderitem);
                    //    else if (hasShipment && !IsDataSendToPost)
                    //        shipment = GetShipment(orderitem, i);
                    //    else
                    //        continue;
                    //    shipmentAddressId = order.ShippingAddress.Id;
                    //    if (order.BillingAddress.FirstName.ToLower() == "test-api" || order.BillingAddress.LastName == "be-canceled")
                    //    {
                    //        InsertOrderNote("سفارش به صورت تستی ثبت شده وامکان ارسال اطلاعات به پست وجود ندارد", order.Id);
                    //        ChangeOrderState(order);
                    //        return;
                    //    }
                    //}
                    //else
                    {
                        var multiShipment = getShipmentFromMultiShipment(orderitem, i);
                        if (multiShipment == null)
                        {
                            InsertOrderNote("اطلاعات گیرنده ایتم سفارش " + orderitem.Id + " یافت نشد ", order.Id);
                            ChangeOrderState(order);
                            return;
                        }
                        shipment = multiShipment?.shipment;
                        //if (!string.IsNullOrEmpty(shipment.TrackingNumber))
                        //    continue;
                        shipmentAddressId = multiShipment?.ShipmentAddressId;
                        if (shipmentAddressId.HasValue)
                        {
                            var shippingAddress = _addressService.GetAddressById(shipmentAddressId.Value);
                            if (shippingAddress.FirstName?.ToLower() == "test-api" || shippingAddress.LastName?.ToLower() == "be-canceled")
                            {
                                InsertOrderNote("سفارش به صورت تستی ثبت شده وامکان ارسال اطلاعات به پست وجود ندارد", order.Id);
                                ChangeOrderState(order);
                                return;
                            }
                        }
                        string error = "";
                        StateCode = processShipmentAddress(shipmentAddressId.Value, order.Id, out error, true);
                        if (!string.IsNullOrEmpty(error))
                        {
                            ChangeOrderState(order);
                            return;
                        }
                    }
                    if (shipment != null)
                    {
                        int oldShipment = 0;
                        string barcode = "";
                        if (string.IsNullOrEmpty(shipment.TrackingNumber))
                        {
                            int counterForRegnarteBarcode = 1;
                        regenrate:
                            barcode = GenerateBarcodeFromPost(shipment.Id, DefulteSnderState, out oldShipment, shipmentAddressId);
                            if (!string.IsNullOrEmpty(barcode) && (barcode == "RegeneratByCountryCentral" || barcode == "RegeneratByCountryCentral2") && counterForRegnarteBarcode > 1)
                            {
                                barcode = "";
                                counterForRegnarteBarcode = 1;
                            }
                            if (!string.IsNullOrEmpty(barcode))
                            {
                                if (barcode == "RegeneratByCountryCentral")
                                {

                                    counterForRegnarteBarcode++;
                                    ChengeReceiverToCentralState(shipmentAddressId.GetValueOrDefault(0));
                                    barcode = "";
                                    goto regenrate;
                                }
                                else if (barcode == "RegeneratByCountryCentral2")
                                {

                                    counterForRegnarteBarcode++;
                                    ChengeSenderToCentralState(shipment);
                                    barcode = "";
                                    goto regenrate;
                                }

                                if (oldShipment > 0)
                                {
                                    if (UpdateNotUsed(oldShipment, shipment.Id))
                                    {
                                        shipment.TrackingNumber = barcode;
                                        _shipmentService.UpdateShipment(shipment);
                                        InsertOrderNote("برداشت بارکد از مخزن بازیافت بارکد، شماره بارکد :" + barcode, order.Id);
                                        LstOldShipmwntId.Add(oldShipment);
                                    }
                                }
                            }
                        }
                        else
                            barcode = shipment.TrackingNumber;
                        if (!string.IsNullOrEmpty(barcode) && oldShipment == 0)
                        {
                            shipment.TrackingNumber = barcode;
                            _shipmentService.UpdateShipment(shipment);
                            var dr = shipmentDetailesTb.NewRow();
                            dr = getOrderItem(orderitem, barcode, SenderStateCode, StateCode,
                                _addressService.GetAddressById(shipmentAddressId.Value), dr);
                            shipmentDetailesTb.Rows.Add(dr);
                        }
                        //else
                        //{
                        //    ChangeOrderState(order);
                        //    return;
                        //}
                    }
                }
            }
            if (order.Shipments.Any(p => string.IsNullOrEmpty(p.TrackingNumber)))
            {
                ChangeOrderState(order);
                InsertOrderNote("به دلیل اینکه تمامی مرسولات دارای بارکد نمی باشند امکان ارسال اطلاعات به پست نمی باشد", order.Id);
                return;
            }
            if (shipmentDetailesTb.Rows.Count > 0)
            {
                InsertOrderNote("A shipment has been added", order.Id);
                SendDataToPost(order, shipmentDetailesTb);
                _notificationService.SendSmsToSender_Reciver(order.Id, this);
            }
            else if (!LstOldShipmwntId.Any())
                ChangeOrderState(order);
        }
        /// <summary>
        /// تغییر آدرس شهرستان فرستنده به مرکز استان در صورت ناشناس بود کد شهرستان برای پست
        /// </summary>
        /// <param name="shipment"></param>
        private void ChengeSenderToCentralState(Shipment shipment, Order order = null)
        {
            try
            {
                Order _order = null;
                if (order != null)
                    _order = order;
                else
                    _order = shipment.Order;
                string query = $@"SELECT
	                            TCC.CenterOfCountry
                            FROM
	                            dbo.Tb_CountryCenter AS TCC
                            WHERE
	                            TCC.CountryId = {_order.BillingAddress.CountryId}";
                int Centralstate = _dbContext.SqlQuery<int>(query, new object[0]).First();
                if (Centralstate == 0)
                    return;
                _order.BillingAddress.Address1 = _order.BillingAddress.StateProvince.Name + "-" + _order.BillingAddress.Address1;
                _order.BillingAddress.StateProvinceId = Centralstate;
                _addressService.UpdateAddress(shipment.Order.BillingAddress);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }
        /// <summary>
        /// تغییر آدرس شهرستان  گیرنده به مرکز استان در صورت ناشناس بود کد شهرستان برای پست
        /// </summary>
        /// <param name="shipmentAddressId"></param>
        private void ChengeReceiverToCentralState(int shipmentAddressId)
        {
            try
            {
                if (shipmentAddressId == 0)
                {
                    Log("تغییر آدرس شهرستان  گیرنده به مرکز استان امکان پذیر نیست", "شناسه آدرس گیرنده صفر است");
                    return;
                }
                var shipmentAddress = _addressService.GetAddressById(shipmentAddressId);
                string query = $@"SELECT
	                            TCC.CenterOfCountry
                            FROM
	                            dbo.Tb_CountryCenter AS TCC
                            WHERE
	                            TCC.CountryId = {shipmentAddress.CountryId}";
                int Centralstate = _dbContext.SqlQuery<int>(query, new object[0]).First();
                if (Centralstate == 0)
                    return;
                shipmentAddress.Address1 = shipmentAddress.StateProvince.Name + "-" + shipmentAddress.Address1;
                shipmentAddress.StateProvinceId = Centralstate;
                shipmentAddress.ZipPostalCode = GetZipCode(Centralstate);
                _addressService.UpdateAddress(shipmentAddress);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }
        public string GetZipCode(int StateId)
        {
            string query = $@"SELECT dbo.fn_GetZipCode({StateId})";
            return _dbContext.SqlQuery<string>(query, new object[0]).FirstOrDefault();
        }
        private bool UpdateNotUsed(int oldShipment, int newShipmentId)
        {
            string query = $@"UPDATE dbo.Tb_NotUsedTracking SET NewShipmentId = {newShipmentId}
                                WHERE
                                shipmentId = {oldShipment} 
                                UPDATE dbo.Shipment SET TrackingNumber = NULL WHERE Id = {oldShipment}
                                select 1";
            var data = _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault();
            if (data != null)
                return true;
            return false;
        }

        public PostBarcodeGeneratorOutputModel NewGenerateBarcodeFromPost(int shipmentId, int SenderStateId, int? shippingAddressId)
        {
            try
            {

                StateCodemodel SenderCityCode = null;
                var shipment = _shipmentService.GetShipmentById(shipmentId);
                var orderItemId = shipment.ShipmentItems.First().OrderItemId;
                Address ShippingAddress = null;
                if (!shippingAddressId.HasValue)
                {
                    InsertOrderNote("آدرس گیرنده دارای نقص می باشد و امکان تولید بارکد وجود ندارد", shipment.OrderId);
                    return null;
                }
                ShippingAddress = _addressService.GetAddressById(shippingAddressId.Value);
                if (new int[] { 4, 581, 582, 583, 584, 585, 580 }.Contains(SenderStateId))
                {
                    SenderCityCode =
                   _repositoryStateCode.Table.FirstOrDefault(p =>
                       p.stateId == 582);
                    saveRealState(shipment.Order);
                    shipment.Order.BillingAddress.StateProvinceId = 582;//; 579;
                    _addressService.UpdateAddress(shipment.Order.BillingAddress);
                }
                else
                {
                    SenderCityCode =
                   _repositoryStateCode.Table.FirstOrDefault(p => p.stateId == SenderStateId);
                }

                var senderContryCode = _repositoryCountryCode.Table.Where(p => p.CountryId == shipment.Order.BillingAddress.CountryId).FirstOrDefault();
                if (senderContryCode == null)
                {
                    InsertOrderNote("کد استان فرستنده مربوط به سفارش " + $"{shipment.OrderId}" + " یافت نشد", shipment.OrderId);
                    return null;
                }
                if (SenderCityCode == null)
                {
                    InsertOrderNote("کد شهر فرستنده مربوط به سفارش " + $"{shipment.OrderId}" + " یافت نشد", shipment.OrderId);
                    return null;
                }
                if (string.IsNullOrEmpty(SenderCityCode.SenderStateCode))
                {
                    InsertOrderNote("کد شهر فرستنده مربوط به سفارش " + $"{shipment.OrderId}" + " یافت نشد", shipment.OrderId);
                    return null;
                }
                var RecivercityCode =
                    _repositoryStateCode.Table.FirstOrDefault(p =>
                        p.stateId == ShippingAddress.StateProvinceId);
                if (RecivercityCode == null)
                {
                    InsertOrderNote("کد شهر گیرنده مربوط به سفارش " + $"{shipment.OrderId}" + " یافت نشد", shipment.OrderId);
                    return null;
                }
                if (string.IsNullOrEmpty(RecivercityCode.StateCode))
                {
                    InsertOrderNote("کد شهر گیرنده مربوط به سفارش " + $"{shipment.OrderId}" + " یافت نشد", shipment.OrderId);
                    return null;
                }

                int BimeCost = 0;
                var bill = orderTotal(shipment.Order);
                var TotalProductCost =
                    Convert.ToDecimal(
                        shipment.Order.OrderItems.Sum(p => GetItemCostFromAttr(p) * p.Quantity)); //کل سهم پست 
                bill = orderTotal(shipment.Order);
                BimeCost = shipment.Order.OrderItems.Sum(p => GetBiemeForOne(p));
                //_taxService.LoadActiveTaxProvider().GetTaxRate(new CalculateTaxRequest() { })
                OrderItem item = shipment.Order.OrderItems.Single(p => p.Id == orderItemId);
                var postType = GetPostType(item.Product.ProductCategories.Select(p => p.CategoryId).ToList());
                if (postType == 0)
                {
                    InsertOrderNote("کد قرارداد نوع پست وارد نشده است", shipment.OrderId);
                    return null;
                }
                var serviceType = 1;
                if (postType == 19)
                    serviceType = 2;
                else if (new int[] { 76, 77, 78 }.Contains(postType))
                    serviceType = 3;

                var weight = GetItemWeightFromAttr(orderItemId);
                var marsoole = _OrderItemAttributeValueRepository.Table.SingleOrDefault(p => p.OrderItemId == orderItemId && p.PropertyAttrName.Contains("نوع مرسوله"))?.PropertyAttrValueName;
                if (string.IsNullOrEmpty(marsoole))
                {
                    marsoole = "بسته";
                    //InsertOrderNote("نوع مرسوله وارد نشده است", shipment.OrderId);
                    //return null;
                }
                int parcel = 1;
                if (serviceType == 1)
                {
                    if (marsoole == "بسته")
                    {
                        parcel = 2;
                    }
                }
                else if (serviceType == 2)
                {
                    if (marsoole == "بسته")
                    {
                        parcel = 4;
                    }
                    else
                    {
                        parcel = 3;
                    }
                }

                var sourceProvince = ShippingAddress.CountryId;
                var desProvince = shipment.Order.BillingAddress.CountryId;
                int desType = 1;
                if (sourceProvince != desProvince)
                {
                    var count = _dbContext.SqlQuery<int>($"select count(*) from Tb_NeighboringProvinces where CountryId = {sourceProvince} AND AdjacentCountry = {desProvince}").FirstOrDefault();
                    if (count >= 1)
                    {
                        desType = 2;
                    }
                    else
                    {
                        desType = 3;
                    }
                }

                return _postBarCodeService.GenerateAndGetBarcode(new PostBarcodeGeneratorInputModel()
                {
                    UserName = _setting.PostUserName,
                    Password = "Baz@rM%2020",//_setting.PostPassword,
                    ContractCode = 1011015,
                    PostNodeCode = SenderCityCode.SenderStateCode, //bazar material node code
                    DestCode = Convert.ToInt32(RecivercityCode.StateCode),
                    SourceCode = Convert.ToInt32(senderContryCode.CountryCode),
                    SenderAddress = shipment.Order.BillingAddress.Address1,
                    SenderName = (shipment.Order.BillingAddress.FirstName ?? "") + " " +
                        (shipment.Order.BillingAddress.LastName ?? ""),
                    SenderPostalCode = shipment.Order.BillingAddress.ZipPostalCode,
                    SenderMobile = shipment.Order.BillingAddress.PhoneNumber,
                    ReceiverMobile = ShippingAddress.PhoneNumber,
                    ReceiverAddress = ShippingAddress.Address1,
                    ReceiverName = (ShippingAddress.FirstName ?? "") + " " +
                        (ShippingAddress.LastName ?? ""),
                    ReceiverPostalCode = ShippingAddress.ZipPostalCode,
                    InsuranceAmount = BimeCost,
                    Relationalkey = "",
                    ElectroReceiptant = false,
                    IsCot = false,
                    ServiceType = serviceType,
                    TypeCode = postType,
                    IsNonStandard = false,
                    SmsService = false,
                    TwoReceiptant = false,
                    InsuranceType = 1,
                    PostalCostCategoryId = 2,
                    PostalCostTypeFlag = "I",
                    SendPlaceType = 0,
                    TlsServiceType = 0,
                    Weight = weight,
                    ParcelType = parcel,
                    SpsDestinationType = serviceType == 3 ? desType : 0,//صفر در صورت ویژه نبودن
                    SpsParcelType = serviceType == 3 ? (marsoole == "بسته" ? 2 : 1) : 0,//
                    SpsReceiverTimeType = serviceType == 3 ? (postType == 76 ? 1 : postType == 77 ? 2 : postType == 78 ? 3 : 0) : 0,
                });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// تولید بارکد برای تمامی مرسولات یک سفارش
        /// </summary>
        /// <param name="order"></param>
        public void _GenerateBarcodes(Order order, out List<string> strError)
        {
            strError = new List<string>();
            try
            {
                if (order.Shipments.Any())
                {
                    if (order.Shipments.All(p => !string.IsNullOrEmpty(p.TrackingNumber)))

                        return;
                }
                if (order.OrderStatus == OrderStatus.Cancelled)
                {
                    strError.Add("امکان تولید بارکد برای این سفارش وچود ندارد");
                    return;
                }
                if (IsLockThisToSendToPost(OrderStatusToPost.BarcodeIsGenerating, order.Id))
                {
                    strError.Add("بارکد برای این سفارش در حال تولید شدن می باشد");
                    return;
                }
                LockThisToSendToPost(OrderStatusToPost.BarcodeIsGenerating, order.Id);
                bool isMultiShpment = IsMultiShippment(order);

                int DefulteSnderState = 0;
                if (!getDefulteSenderState(order.CustomerId, out DefulteSnderState))
                {
                    DefulteSnderState = order.BillingAddress.StateProvinceId.Value;
                }
                bool addShipment = false;
                foreach (var orderitem in order.OrderItems)
                {
                    for (int i = 0; i < orderitem.Quantity; i++)
                    {
                        Shipment shipment = null;
                        int? shipmentAddressId = null;

                        //if (!isMultiShpment)
                        //{
                        //    bool hasShipment = HasShipmpment(orderitem, i);
                        //    if (!hasShipment)
                        //        shipment = AddShipmentNormalPost(orderitem);
                        //    else if (hasShipment)
                        //        shipment = GetShipment(orderitem, i);
                        //    else
                        //        continue;
                        //    shipmentAddressId = order.ShippingAddress.Id;
                        //}
                        //else
                        {
                            var multiShipment = getShipmentFromMultiShipment(orderitem, i);
                            if (multiShipment == null)
                            {
                                InsertOrderNote("اطلاعات مربوط به حمل و نقل ایتم سفارش " + orderitem.Id + " یافت نشد ", order.Id);
                                strError.Add("اطلاعات مربوط به حمل و نقل ایتم سفارش " + orderitem.Id + " یافت نشد ");
                                //ChangeOrderState(order);
                                continue;
                            }
                            shipment = multiShipment?.shipment;
                            //if (!string.IsNullOrEmpty(shipment.TrackingNumber))
                            //    continue;
                            shipmentAddressId = multiShipment?.ShipmentAddressId;
                            string error = "";
                            var StateCode = processShipmentAddress(shipmentAddressId.Value, order.Id, out error, true);
                            if (!string.IsNullOrEmpty(error))
                            {
                                InsertOrderNote(error + "==>" + orderitem.Id, order.Id);
                                strError.Add(error + "==>" + orderitem.Id);
                                // ChangeOrderState(order);
                                continue;
                            }
                        }
                        if (shipment != null)
                        {
                            string barcode = "";
                            int oldShipment = 0;
                            if (string.IsNullOrEmpty(shipment.TrackingNumber))
                            {
                                addShipment = true;
                                int counterForRegnarteBarcode = 1;
                            regenrate:
                                barcode = GenerateBarcodeFromPost(shipment.Id, DefulteSnderState, out oldShipment, shipmentAddressId);
                                if (!string.IsNullOrEmpty(barcode) && barcode == "RegeneratByCountryCentral" && counterForRegnarteBarcode > 1)
                                {
                                    barcode = "";
                                    counterForRegnarteBarcode = 1;
                                }
                                if (barcode == "RegeneratByCountryCentral")
                                {
                                    counterForRegnarteBarcode++;
                                    ChengeReceiverToCentralState(shipmentAddressId.GetValueOrDefault(0));
                                    barcode = "";
                                    goto regenrate;
                                }
                                if (string.IsNullOrEmpty(barcode))
                                {
                                    strError.Add("خطا در زمان تولید بارکد، لطفا با پشتیبانی تماس بگیرید");
                                    LockThisToSendToPost(OrderStatusToPost.BarcodeGenerateError, order.Id);
                                    UnLockThisOrder(OrderStatusToPost.BarcodeIsGenerating, order.Id);
                                    return;
                                }
                                shipment.TrackingNumber = barcode;
                                _shipmentService.UpdateShipment(shipment);
                                if (oldShipment > 0)
                                {
                                    if (!UpdateNotUsed(oldShipment, shipment.Id))
                                    {
                                        shipment.TrackingNumber = null;
                                        _shipmentService.UpdateShipment(shipment);
                                        InsertOrderNote("برداشت بارکد از مخزن بازیافت بارکد با مشکل مواجه شد :" + barcode, order.Id);
                                    }
                                }
                            }
                        }
                    }
                }
                if (addShipment)
                    InsertOrderNote("A shipments has been added", order.Id);
                if (order.Shipments.All(p => !string.IsNullOrEmpty(p.TrackingNumber)))
                    LockThisToSendToPost(OrderStatusToPost.BarcodeGenerateDown, order.Id);
                else
                    LockThisToSendToPost(OrderStatusToPost.BarcodeGenerateError, order.Id);
                UnLockThisOrder(OrderStatusToPost.BarcodeIsGenerating, order.Id);
            }
            catch (Exception ex)
            {
                strError.Add("خطا در زمان تولید بارکد، لطفا با پشتیبانی تماس بگیرید");
                LockThisToSendToPost(OrderStatusToPost.BarcodeGenerateError, order.Id);
                UnLockThisOrder(OrderStatusToPost.BarcodeIsGenerating, order.Id);
                string error = "خطا در زمان تولید بارکد" + ex.Message + (ex.InnerException != null ? "-->" + ex.InnerException.Message : "");
                InsertOrderNote(error, order.Id);
                Log("خطا در زمان تولید بارکد", error);
            }
        }
        /// <summary>
        /// ارسال اطلاعات محموله های یک سفارش به پست
        /// </summary>
        /// <param name="order"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public bool _SendDataToPost(Order order, out string strError)
        {
            try
            {
                if (order.OrderStatus == OrderStatus.Cancelled)
                {
                    strError = "امکان ارسال اطلاعات به پست برای این سفارش وجود ندارد،سفارش مورد نظر شما کنسل شده است";
                    return false;
                }
                if (IsLockThisToSendToPost(OrderStatusToPost.IsSendingToPost, order.Id))
                {
                    strError = "این سفارش در حال ارسال به پست می باشد";
                    return false;
                }
                LockThisToSendToPost(OrderStatusToPost.IsSendingToPost, order.Id);
                if (order.OrderNotes.Any(p => p.Note.Contains("SendDataToPost")))
                {
                    strError = "اطلاعات این سفارش قبلا به پست ارسال شده است";
                    UnLockThisOrder(OrderStatusToPost.IsSendingToPost, order.Id);
                    return false;
                }
                bool isMultiShpment = IsMultiShippment(order);
                if (order.Shipments.Any(p => string.IsNullOrEmpty(p.TrackingNumber)))
                {
                    string Ids = string.Join(",", order.Shipments.Where(p => string.IsNullOrEmpty(p.TrackingNumber)).Select(p => p.Id).ToList());
                    strError = "محموله های مذکور فاقد بارکد می باشند" + ":" + Ids;
                    UnLockThisOrder(OrderStatusToPost.IsSendingToPost, order.Id);
                    return false;
                }
                string SenderStateCode = "";
                string StateCode = "";
                int DefulteSnderState = 0;
                if (!getDefulteSenderState(order.CustomerId, out DefulteSnderState))
                {
                    DefulteSnderState = order.BillingAddress.StateProvinceId.Value;
                }
                var SenderCityCode =
                       _repositoryStateCode.Table.FirstOrDefault(p =>
                           p.stateId == DefulteSnderState);
                if (SenderCityCode == null)
                {
                    strError = "کد شهر فرستنده مربوط به سفارش " + $"{order.Id}" + " یافت نشد";
                    InsertOrderNote(strError, order.Id);
                    ChangeOrderState(order);
                    UnLockThisOrder(OrderStatusToPost.IsSendingToPost, order.Id);
                    return false;
                }
                if (string.IsNullOrEmpty(SenderCityCode.SenderStateCode))
                {
                    strError = "کد شهر فرستنده مربوط به سفارش " + $"{order.Id}" + " یافت نشد";
                    InsertOrderNote(strError, order.Id);
                    ChangeOrderState(order);
                    UnLockThisOrder(OrderStatusToPost.IsSendingToPost, order.Id);
                    return false;
                }
                SenderStateCode = SenderCityCode.SenderStateCode;
                if (!isMultiShpment)
                {
                    string error = "";
                    StateCode = processShipmentAddress(order.ShippingAddress.Id, order.Id, out error, true);
                    if (!string.IsNullOrEmpty(error))
                    {
                        strError = error;
                        ChangeOrderState(order);
                        UnLockThisOrder(OrderStatusToPost.IsSendingToPost, order.Id);
                        return false;
                    }
                }
                var extendedShipmentSetting = _setting;
                //_settingService.LoadSetting<ExtendedShipmentSetting>(_storeContext.CurrentStore.Id);
                if (extendedShipmentSetting == null)
                {
                    strError = "مشکل در تنظیمات سرور با پشتیبانی تماس حاصل نمایید   ";
                    ChangeOrderState(order);
                    UnLockThisOrder(OrderStatusToPost.IsSendingToPost, order.Id);
                    return false;
                }
                DataTable shipmentDetailesTb = new DataTable();
                for (var i = 0; i < 25; i++)
                    shipmentDetailesTb.Columns.Add(i.ToString());

                foreach (var orderItem in order.OrderItems)
                {
                    for (int i = 0; i < orderItem.Quantity; i++)
                    {
                        Shipment shipment = null;
                        int? shipmentAddressId = 0;
                        if (!isMultiShpment)
                        {
                            bool hasShipment = HasShipmpment(orderItem, i);
                            if (!hasShipment)
                            {
                                strError = "آیتم شماره " + orderItem.Id + "  سفارش شما فاقد محموله می باشد لطفا با پشتیبانی هماهنگ فرمایید";
                                UnLockThisOrder(OrderStatusToPost.IsSendingToPost, order.Id);
                                return false;
                            }
                            shipment = GetShipment(orderItem, i);

                            shipmentAddressId = order.ShippingAddress.Id;
                            if (order.BillingAddress.FirstName.ToLower() == "test-api" || order.BillingAddress.LastName == "be-canceled")
                            {
                                strError = "سفارش به صورت تستی ثبت شده وامکان ارسال اطلاعات به پست وجود ندارد";
                                UnLockThisOrder(OrderStatusToPost.IsSendingToPost, order.Id);
                                return false;
                            }
                        }
                        else
                        {
                            var multiShipment = getShipmentFromMultiShipment(orderItem, i);
                            if (multiShipment == null)
                            {
                                strError = "اطلاعات مربوط به حمل و نقل ایتم سفارش " + orderItem.Id + " یافت نشد ";
                                InsertOrderNote(strError, order.Id);
                                ChangeOrderState(order);
                                UnLockThisOrder(OrderStatusToPost.IsSendingToPost, order.Id);
                                return false;
                            }
                            shipment = multiShipment?.shipment;

                            shipmentAddressId = multiShipment?.ShipmentAddressId;
                            if (shipmentAddressId.HasValue)
                            {
                                var shippingAddress = _addressService.GetAddressById(shipmentAddressId.Value);
                                if (shippingAddress.FirstName.ToLower() == "test-api" || shippingAddress.LastName == "be-canceled")
                                {
                                    strError = "سفارش به صورت تستی ثبت شده وامکان ارسال اطلاعات به پست وجود ندارد";
                                    UnLockThisOrder(OrderStatusToPost.IsSendingToPost, order.Id);
                                    return false;
                                }
                            }
                            string error = "";
                            StateCode = processShipmentAddress(shipmentAddressId.Value, order.Id, out error, true);
                            if (!string.IsNullOrEmpty(error))
                            {
                                strError = error;
                                ChangeOrderState(order);
                                UnLockThisOrder(OrderStatusToPost.IsSendingToPost, order.Id);
                                return false;
                            }
                        }

                        var dr = shipmentDetailesTb.NewRow();
                        dr = getOrderItem(orderItem, shipment.TrackingNumber, SenderStateCode, StateCode,
                            _addressService.GetAddressById(shipmentAddressId.Value), dr);
                        shipmentDetailesTb.Rows.Add(dr);
                    }
                }
                if (shipmentDetailesTb.Rows.Count == 0)
                {
                    strError = "اطلاعاتی جهت ارسال به پست موجود نمی باشد به دلیل خطا یا ارسال اطلاعات در گذشته";
                    ChangeOrderState(order);
                    UnLockThisOrder(OrderStatusToPost.IsSendingToPost, order.Id);
                    return false;
                }



                var basicHttpbinding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                basicHttpbinding.Name = "MassContractsService";
                basicHttpbinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                basicHttpbinding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                var endpointAddress =
                    new EndpointAddress("http://poffice.post.ir/contracts/MassContractsService.asmx");
                var MCSSC =
                    new MassContractsServiceSoapClient(basicHttpbinding, endpointAddress);
                var ds = new DataSet();
                ds.Tables.Add(shipmentDetailesTb);

                var sendResult = MCSSC.PushVarContractParcels(ds,
                    "1011015",
                    extendedShipmentSetting.PostPassword);
                if (sendResult == null)
                {
                    _notificationService.SendSmsToSender_Reciver(order.Id, this);
                    InsertOrderNote(ds.GetXml(), order.Id);
                    strError = "اطلاعات با موفقیت در سامانه پست بار ثبت گردید ";
                    InsertOrderNote("SendDataToPost", order.Id);
                    UnLockThisOrder(OrderStatusToPost.IsSendingToPost, order.Id);
                    SavePostCoordination(order.Id, "هماهنگی با پست توسط سرویس");
                    order.OrderStatus = OrderStatus.Complete;
                    _orderService.UpdateOrder(order);
                    InsertOrderNote("وضعیت سفارش ویرایش شد. وضعیت جدید: تکمیل", order.Id);
                    return true;
                }
                InsertOrderNote("خطا در زمان ارسال اطلاعات به پست" + sendResult, order.Id);
                ChangeOrderState(order);
                strError = "خطا در زمان ارسال اطلاعات به پست با پشتیبانی تماس بگیرید";
                UnLockThisOrder(OrderStatusToPost.IsSendingToPost, order.Id);
                return false;
            }
            catch (Exception ex)
            {
                InsertOrderNote("خطا در زمان ارسال اطلاعات به پست " +
                    ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""), order.Id);
                ChangeOrderState(order);
                strError = "خطا در زمان ارسال اطلاعات به پست با پشتیبانی تماس بگیرید";
                UnLockThisOrder(OrderStatusToPost.IsSendingToPost, order.Id);
                return false;
            }
        }
        public bool SendDataToPost(Order order, DataTable dataTable)
        {
            try
            {
                var extendedShipmentSetting = _setting;
                //_settingService.LoadSetting<ExtendedShipmentSetting>(_storeContext.CurrentStore.Id);
                if (dataTable == null)
                {
                    ChangeOrderState(order);
                    return false;
                }
                var basicHttpbinding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                basicHttpbinding.Name = "MassContractsService";
                basicHttpbinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                basicHttpbinding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                var endpointAddress =
                    new EndpointAddress("http://poffice.post.ir/contracts/MassContractsService.asmx");
                var MCSSC =
                    new MassContractsServiceSoapClient(basicHttpbinding, endpointAddress);
                var ds = new DataSet();
                ds.Tables.Add(dataTable);

                var sendResult = MCSSC.PushVarContractParcels(ds,
                    "1011015",
                    extendedShipmentSetting.PostPassword);
                if (sendResult == null)
                {
                    InsertOrderNote(ds.GetXml(), order.Id);
                    return true;
                }
                InsertOrderNote("خطا در زمان ارسال اطلاعات به پست" + sendResult, order.Id);
                ChangeOrderState(order);
                return false;
            }
            catch (Exception ex)
            {
                InsertOrderNote("خطا در زمان ارسال اطلاعات به پست " +
                    ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""), order.Id);
                ChangeOrderState(order);
                return false;
            }

        }
        #region Tool
        public void LockThisToSendToPost(OrderStatusToPost state, int orderId)
        {
            InsertOrderNote((state).ToString(), orderId);
        }
        public void UnLockThisOrder(OrderStatusToPost state, int orderId)
        {
            var data = _orderNoteRepository.Table.FirstOrDefault(p => p.OrderId == orderId && p.Note == (state).ToString());
            if (data != null)
                _orderNoteRepository.Delete(data);
        }
        public bool IsLockThisToSendToPost(OrderStatusToPost state, int orderId)
        {
            if (_orderNoteRepository.Table.Any(p => p.OrderId == orderId && p.Note == (state).ToString()))
                return true;
            return false;
        }
        #endregion

        private Shipment GetShipment(OrderItem orderItem, int orderNum)
        {
            return orderItem.Order.Shipments.Where(p => //p.TrackingNumber != "" &&
                         p.ShipmentItems.Any(n => n.OrderItemId == orderItem.Id))
                                   .OrderBy(p => p.Id).Skip(orderNum).Take(1).FirstOrDefault();
        }
        private string processShipmentAddress(int addressId, int orderId, out string error, bool isReceiver)
        {
            var shipmentAddress = _addressService.GetAddressById(addressId);
            error = "";
            var RecivercityCode =
                    _repositoryStateCode.Table.FirstOrDefault(p =>
                        p.stateId == shipmentAddress.StateProvinceId)?.StateCode;
            if (RecivercityCode == null || string.IsNullOrEmpty(RecivercityCode))
            {
                ChengeReceiverToCentralState(addressId);
                shipmentAddress = _addressService.GetAddressById(addressId);
                RecivercityCode =
                       _repositoryStateCode.Table.FirstOrDefault(p =>
                           p.stateId == shipmentAddress.StateProvinceId).StateCode;
            }
            if (RecivercityCode == null || string.IsNullOrEmpty(RecivercityCode))
            {
                error = "کد شهر گیرنده مربوط به سفارش " + $"{orderId}" + " یافت نشد";
                InsertOrderNote(error, orderId);
                return "";
            }
            if (isReceiver)
                if (shipmentAddress.CountryId == 1 && new int[] { 4, 581, 582, 583, 584, 585, 580, 579 }.Contains(shipmentAddress.StateProvinceId.Value))//ست کردن کد تهران
                {
                    RecivercityCode = "00001";
                }
            return RecivercityCode;
        }
        public Shipment AddShipmentNormalPost(OrderItem orderItem)
        {
            if (orderItem == null)
                return null;
            Shipment shipment = null;
            decimal? totalWeight = null;

            //is shippable
            if (!orderItem.Product.IsShipEnabled)
                return null;

            //ensure that this product can be shipped (have at least one item to ship)
            //var maxQtyToAdd = orderItem.GetTotalNumberOfItemsCanBeAddedToShipment();
            //if (maxQtyToAdd <= 0)
            //    return null;
            var qtyToAdd = 1;
            //multiple warehouses are not supported
            var warehouseId = orderItem.Product.WarehouseId;
            //validate quantity
            //if (qtyToAdd <= 0)
            //    return null;
            //if (qtyToAdd > maxQtyToAdd)
            //    qtyToAdd = maxQtyToAdd;

            //ok. we have at least one item. let's create a shipment (if it does not exist)

            var orderItemTotalWeight = orderItem.ItemWeight.HasValue ? orderItem.ItemWeight * qtyToAdd : null;
            if (orderItemTotalWeight.HasValue)
            {
                if (!totalWeight.HasValue)
                    totalWeight = 0;
                totalWeight += orderItemTotalWeight.Value;
            }
            if (shipment == null)
            {
                var trackingNumber = "";
                var adminComment = "";
                shipment = new Shipment
                {
                    OrderId = orderItem.Order.Id,
                    TrackingNumber = trackingNumber,
                    TotalWeight = null,
                    ShippedDateUtc = null,
                    DeliveryDateUtc = null,
                    AdminComment = adminComment,
                    CreatedOnUtc = DateTime.UtcNow,
                };
            }
            //create a shipment item
            var shipmentItem = new ShipmentItem
            {
                OrderItemId = orderItem.Id,
                Quantity = qtyToAdd,
                WarehouseId = warehouseId
            };
            shipment.ShipmentItems.Add(shipmentItem);

            //if we have at least one item in the shipment, then save it
            if (shipment != null && shipment.ShipmentItems.Any())
            {
                shipment.TotalWeight = totalWeight;
                _shipmentService.InsertShipment(shipment);

                //add a note

                return shipment;
            }
            return null;
        }
        /// <summary>
        /// برای پست سفارشی و پیشتاز DataRow آماده سازی ارسال اطلاعات به پست به صورت   
        /// </summary>
        /// <param name="orderItem"></param>
        /// <param name="TrackingNumber"></param>
        /// <param name="SenderCityCode"></param>
        /// <param name="RecivercityCode"></param>
        /// <param name="shippingAddress"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        private DataRow getOrderItem(OrderItem orderItem
            , string TrackingNumber
            , string SenderCityCode
            , string RecivercityCode
            , Address shippingAddress
            , DataRow dr)
        {

            var bill = orderTotal(orderItem.Order);
            var TotalProductCost = GetItemCostFromAttr(orderItem); //کل سهم پست 
            var categoryPostType = GetPostType(orderItem.Product.ProductCategories.Select(p => p.CategoryId).ToList());
            int postType = 0;
            if (categoryPostType == 11)
                postType = 1;
            else if (categoryPostType == 19)
                postType = 2;
            else if (new int[] { 76, 77, 78 }.Contains(categoryPostType))
                postType = 3;
            //if (postType == 1)
            //{
            //    TotalProductCost += GetItemCostFromAttr(orderItem);
            //}
            var pr = new PersianCalendar();

            decimal BimeCost = 0;
            BimeCost = GetBiemeForOne(orderItem);
            TotalProductCost = Convert.ToInt32((TotalProductCost) + BimeCost);
            var tax = TotalProductCost * bill.Tax / 100;
            var TotalPrice = Convert.ToInt32(TotalProductCost + tax);

            int ItemWeight = 0;
            //if (postType == 1)
            //{
            ItemWeight = GetItemWeightFromAttr(orderItem);
            string ShipmentContent = (getOrderItemContent(orderItem) ?? "");
            ShipmentContent = ShipmentContent.Length > 50 ? ShipmentContent.Substring(0, 45) + "..." : ShipmentContent;
            //}
            //else
            //{
            //    ItemWeight = Convert.ToInt32(orderItem.Product.Weight * 1000);
            //}
            dr[0] = 1011015; //extendedShipmentSetting.PostType;//1- کد قرار داد
            dr[1] = TrackingNumber; /*2- */
            dr[2] = postType; //ویژه=3/سفارشی=2/ پیشتاز=1  => نوع سرویس
            dr[3] = true; //نوع مرسوله => 0= پاکت / 1= بسته
            dr[4] = int.Parse(SenderCityCode);
            dr[5] = int.Parse(RecivercityCode);
            dr[6] = (orderItem.Order.BillingAddress.FirstName ?? "") + " " +
                    (orderItem.Order.BillingAddress.LastName ?? "");
            dr[7] = (shippingAddress.FirstName ?? "") + " " +
                    (shippingAddress.LastName ?? "");
            dr[8] = pr.GetYear(DateTime.Now) + "/" + pr.GetMonth(DateTime.Now).ToString("00") + "/" +
                    pr.GetDayOfMonth(DateTime.Now).ToString("00");
            //if (_workContext.CurrentCustomer.Id == 6288433)
            //    dr[9] = (DateTime.Now.AddHours(-5)).ToShortTimeString();
            //else
            dr[9] = DateTime.Now.ToShortTimeString();
            dr[10] = shippingAddress.ZipPostalCode;
            dr[11] = orderItem.Order.BillingAddress.ZipPostalCode;
            dr[12] = Convert.ToInt32(TotalProductCost).ToString(); // 0;//حق السهم پست به ریال
            dr[13] = 0; //حق السهم طرف قراردادبه ریال
            dr[14] = TotalPrice.ToString(); // جمع کل هزینه به ریال
            dr[15] = Convert.ToInt32(tax); //مالیات بر ارزش افزوده به ریال
            dr[16] = ItemWeight;
            dr[17] = Convert.ToInt16("2"); //بدون الصاق تمبر
            dr[18] = 'N'; //نسیه
            dr[19] = Convert.ToInt32(BimeCost); //BimePriceAdjustment; //مبلغ بیمه
            dr[20] = ShipmentContent; //سایر اطلاعات
            dr[21] = ""; //کد ملی فرستنده
            dr[22] = ""; //کد ملی گیرنده
            dr[23] = orderItem.Order.BillingAddress.Address1;
            dr[24] = shippingAddress.Address1;
            return dr;
        }

        #endregion

        #region utility
        private string ConvertToMiladyString(DateTime? createdFromUtc)
        {
            if (!createdFromUtc.HasValue)
                return null;
            string p = createdFromUtc.Value.Year + "-" + createdFromUtc.Value.Month + "-" + createdFromUtc.Value.Day + " " + createdFromUtc.Value.ToShortTimeString();
            return p;
        }

        public MultiShipmentModel getShipmentFromMultiShipment(OrderItem orderitem, int i)
        {
            List<MultiShipmentModel> MultiShipment = this._dbContext.SqlQuery<MultiShipmentModel>(
                string.Format(@"SELECT Oi.ProductId,xs.ShipmentId,A.Id AS ShipmentAddressId FROM 
                dbo.Shipment S 
                INNER JOIN dbo.ShipmentItem SI ON SI.ShipmentId = S.Id INNER JOIN dbo.OrderItem OI ON OI.Id = SI.OrderItemId
                INNER JOIN dbo.XtnShippment XS ON XS.ShipmentId = S.Id INNER JOIN dbo.Address A ON A.Id = xs.ShippmentAddressId
                WHERE OI.Id = {0} ", orderitem.Id), new object[0]).ToList();
            if (MultiShipment == null)
                return null;
            if (!MultiShipment.Any())
                return null;
            var multiShipmentModel = MultiShipment.OrderBy(p => p.ShipmentId).Skip(i).Take(1).FirstOrDefault();
            multiShipmentModel.shipment = _shipmentService.GetShipmentById(multiShipmentModel.ShipmentId);
            return multiShipmentModel;
        }
        public Address getAddressFromShipment(int shipmentId)
        {
            string query = $@"SELECT
	                        A.Id
                        FROM
	                        dbo.Shipment AS S
	                        INNER JOIN dbo.XtnShippment AS XS ON XS.ShipmentId = S.Id
	                        INNER JOIN dbo.Address AS A ON A.Id = XS.ShippmentAddressId
                        WHERE
	                        S.Id = {shipmentId} ";
            int AddressId = _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
            if (AddressId == 0)
                return null;
            return _addressService.GetAddressById(AddressId);
        }
        private bool IsDataSendedToPost(Order order)
        {
            if (order.OrderNotes.Any(p => p.Note.Contains("<NewDataSet>")))
                return true;
            return false;
        }

        private bool HasShipmpment(OrderItem orderItem, int orderNum)
        {
            try
            {
                if (!orderItem.Order.Shipments.Any())
                {
                    return false;
                }
                var shipment = orderItem.Order.Shipments.Where(p =>// p.TrackingNumber != "" &&
                                                                    p.ShipmentItems.Any(n => n.OrderItemId == orderItem.Id))
                    .OrderBy(p => p.Id).Skip(orderNum).Take(1).FirstOrDefault();
                if (shipment != null)
                    return true;
            }
            catch (Exception ex)
            {
                Log("خطا در زمان چک کردن محموله مربوط به سفارش" + " " + orderItem.Id,
                    ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
            }
            return false;
        }

        public void CleanToSendDataToPostAgain(List<int> LstOrderId)
        {
            foreach (var item in LstOrderId)
            {
                var order = _orderService.GetOrderById(item);
                if (IsDataSendedToPost(order))
                    continue;
                var orderNote = order.OrderNotes.FirstOrDefault(p => p.Note.Contains("SendDataToPost"));
                if (orderNote != null)
                    _orderNoteRepository.Delete(orderNote);
            }
        }

        public void InsertOrderNote(string note, int orderId)
        {
            OrderNote Onote = new OrderNote()
            {
                Note = note + "-" + _workContext.CurrentCustomer.Id.ToString(),
                CreatedOnUtc = DateTime.Now.ToUniversalTime(),
                DisplayToCustomer = false,
                OrderId = orderId
            };
            _orderNoteRepository.Insert(Onote);
        }
        public void DeleteOrderNote(string note, int orderId)
        {
            var _note = _orderNoteRepository.Table.Where(p => p.Note.Contains(note)).FirstOrDefault();
            if (_note != null)
                _orderNoteRepository.Delete(_note);
        }
        private string MiladyToShamsi2(DateTime dt, bool AddTime = false)
        {
            PersianCalendar pa = new PersianCalendar();
            string PaData = pa.GetYear(dt) + "/" + pa.GetMonth(dt).ToString("00") + "/" + pa.GetDayOfMonth(dt).ToString("00");
            if (AddTime)
                PaData += " " + dt.ToShortTimeString();
            return PaData;
        }
        private void ChangeOrderState(Order order)
        {
            var FetchOrder = _orderRepository.Table.FirstOrDefault(p => p.Id == order.Id);
            if (FetchOrder == null)
                return;
            FetchOrder.OrderStatus = OrderStatus.Processing;
            _orderRepository.Update(FetchOrder);
            InsertOrderNote("وضعیت سفارش به درحال پردازش تعییر کرد", order.Id);
        }
        public void ChangeOrderState(Order order, OrderStatus orderStatus, string msg)
        {
            var FetchOrder = _orderRepository.Table.FirstOrDefault(p => p.Id == order.Id);
            if (FetchOrder == null)
                return;
            FetchOrder.OrderStatus = orderStatus;
            _orderRepository.Update(FetchOrder);
            InsertOrderNote(msg, order.Id);
        }
        private string MiladyToShamsi(DateTime dt, bool AddTime = false)
        {
            PersianCalendar pa = new PersianCalendar();
            string PaData = pa.GetDayOfMonth(dt).ToString("00") + "/" + pa.GetMonth(dt).ToString("00") + "/" + pa.GetYear(dt);
            if (AddTime)
                PaData += " " + dt.ToShortTimeString();
            return PaData;
        }
        #endregion

        #region BarcodeRepositoryX

        public bool ReadExcelFile(MemoryStream stream)
        {
            try
            {
                var excelReader = ExcelReaderFactory.CreateReader(stream);
                var result = excelReader.AsDataSet();
                result.DataSetName = "ArrayOfBarcodeList";
                result.Tables[0].TableName = "BarcodeList";
                result.Tables[0].Columns[0].ColumnName = "Barcode";

                var xmlString = string.Empty;
                using (TextWriter writer = new StringWriter())
                {
                    result.Tables[0].WriteXml(writer);
                    xmlString = writer.ToString();
                }
                stream.Close();
                var serializer = new XmlSerializer(typeof(BarcodeList));
                var barcodeList = new BarcodeList();
                using (TextReader reader = new StringReader(xmlString))
                {
                    barcodeList = (BarcodeList)serializer.Deserialize(reader);
                }

                foreach (var item in barcodeList.list)
                {
                    if (_repositoryBarcodeRepository.Table.Any(p => p.Barcode == item.Barcode))
                        continue;
                    _repositoryBarcodeRepository.Insert(item);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log("خطا در زمان خواندن اطلاعات از فایل اکسل مخزن بارکد ",
                    ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                return false;
            }
        }

        public List<BarcodeRepositoryModel> readBarcodeRepository(string barcode, int shipmentId)
        {
            var Data = _repositoryBarcodeRepository.Table.Select(p => p);
            if (!string.IsNullOrEmpty(barcode))
                Data = Data.Where(p => p.Barcode.Contains(barcode));
            if (shipmentId != 0)
                Data = Data.Where(p => p.ShipmentId == shipmentId);
            return Data.OrderByDescending(p => p.Id).ToList();
        }

        public string SetBarcodeIsUsed(string barcode, int shipmentId)
        {
            if (_repositoryBarcodeRepository.Table.Any(p => p.Barcode == barcode && p.ShipmentId != null))
                return "BarcodeReserved";
            var Barcode = _repositoryBarcodeRepository.Table.FirstOrDefault(p => p.Barcode == barcode);
            if (Barcode == null)
                return "NoBrcode";
            Barcode.ShipmentId = shipmentId;
            _repositoryBarcodeRepository.Update(Barcode);
            return "BarcodeReserved";
        }

        #endregion

        #region categurypostType

        public void SaveCateguryPostType(string categuryName, int categuryId, int postType)
        {
            var cpt = _repositoryCateguryPostType.Table.FirstOrDefault(p => p.CateguryId == categuryId);
            if (cpt == null)
            {
                cpt = new CateguryPostTypeModel
                {
                    CateguryId = categuryId,
                    CateguryName = categuryName,
                    PostType = postType
                };
                _repositoryCateguryPostType.Insert(cpt);
            }
            else
            {
                cpt.CateguryName = categuryName;
                cpt.PostType = postType;
                _repositoryCateguryPostType.Update(cpt);
            }
        }

        public List<CateguryPostTypeModel> ReadCategoryPostType()
        {
            return _repositoryCateguryPostType.Table.ToList();
        }

        public bool DeleteCateguryPostType(int id)
        {
            try
            {
                var item = _repositoryCateguryPostType.Table.FirstOrDefault(p => p.Id == id);
                _repositoryCateguryPostType.Delete(item);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
        public void SavePostCoordination(List<int> orderIds, string Desc)
        {
            foreach (var item in orderIds)
            {
                try
                {
                    var order = _orderService.GetOrderById(item);
                    var mark = GetOrderRegistrationMethod(order);
                    if (order.OrderStatus != OrderStatus.Complete)
                        continue;
                    if (_PostCoordinationRepository.Table.Any(p => p.orderId == item && p.CoordinationDate.HasValue))
                        continue;
                    if (order.Customer.IsInCustomerRole("Collector") && mark != OrderRegistrationMethod.bidok)// سفارشاتی که خود نماینده جمع آوری می کند
                        continue;
                    PostCoordinationModel model = new PostCoordinationModel()
                    {
                        CoordinationDate = DateTime.Now,
                        Desc = Desc,
                        orderId = item
                    };
                    _PostCoordinationRepository.Insert(model);
                    _notificationService.SendSmsByTemplate(order, null, 5, false);
                    int serviceId = order.OrderItems.First().Product.ProductCategories.First().CategoryId;
                    if ((string.IsNullOrEmpty(order.PaymentMethodSystemName)
                       || order.PaymentMethodSystemName != "Payments.CashOnDelivery")) //&& IsPostService(serviceId))
                    {
                        foreach (var _item in order.Shipments)
                        {
                            int collectorid = _collectorService.assignToCollector(_item.Id);

                            _notificationService.SendSmsByTemplate(null, _item, 6, true);
                            if (collectorid == 0)
                                InsertOrderNote($"درحال حاضر امکان جمع آوری این مموله شماره {_item.Id} از سفارش شماره {order.Id} در این زمان وجود ندارد", order.Id);
                        }
                    }
                    InsertOrderNote(string.Format("سفارش {0} با پست در تاریخ {1} هماهنگ شد" + "\r\n" + Desc ?? "", item, DateTime.Now), item);
                }
                catch (Exception ex)
                {
                    LogException(ex);
                    InsertOrderNote(string.Format("خطا در زمان هماهنگی با پست" + " ==> " + ex.Message + (ex.InnerException != null ? " ==> " + ex.InnerException.Message : "")
                        , item, DateTime.Now), item);
                }
            }
        }
        private void GetTozicoFeed(int shipmentId)
        {
            string query = $@"SELECT
	                            ISNULL(A.FirstName,'')+ISNULL(' '+A.LastName,'') [Name],
	                            CAST(NULL AS BIGINT) Branch,
	                            0 StateEnum,
	                            TALL.Lat Latitude,
	                            TALL.Long Longitude,
	                            A.Address1 Address,
	                            a.PhoneNumber Phone,
	                            1 AS [Count],
	                            dbo.GetOnlyNumbers(TOIR.ExactWeight) Weigth,
	                            cast((TOIR.Width*TOIR.Length*TOIR.Height) / 1000 AS BIGINT) Volume,
	                            1 AS VolumeType,
	                            N'' [Description]
                            FROM
	                            dbo.[Order] AS O
	                            INNER JOIN dbo.Shipment AS S ON S.OrderId = O.Id
	                            INNER JOIN dbo.ShipmentItem AS SI ON SI.ShipmentId = S.Id 
	                            INNER JOIN dbo.OrderItem AS OI ON Si.OrderItemId = oi.Id
	                            INNER JOIN dbo.Tb_OrderItemsRecord AS TOIR ON TOIR.OrderItemId = OI.Id
	                            INNER JOIN dbo.Address AS A ON A.Id = O.BillingAddressId
	                            INNER JOIN dbo.Tbl_Address_LatLong AS TALL ON TALL.AddressId = A.Id
                            WHERE
	                            s.Id = {shipmentId}";
        }
        public bool hasPostCordination(int orderId)
        {
            return _PostCoordinationRepository.Table.Any(p => p.orderId == orderId && p.CoordinationDate.HasValue);
        }
        public void SavePostCoordination(int orderId, string Desc)
        {

            if (!_PostCoordinationRepository.Table.Any(p => p.orderId == orderId && p.CoordinationDate.HasValue))
            {
                PostCoordinationModel model = new PostCoordinationModel()
                {
                    CoordinationDate = DateTime.Now,
                    Desc = Desc,
                    orderId = orderId
                };
                _PostCoordinationRepository.Insert(model);
                InsertOrderNote(string.Format("سفارش {0} با پست در تاریخ {1} هماهنگ شد" + "\r\n" + Desc ?? "", orderId, DateTime.Now), orderId);
            }
            var order = _orderService.GetOrderById(orderId);
            int serviceId = order.OrderItems.First().Product.ProductCategories.First().CategoryId;
            var catInfo = GetCategoryInfo(serviceId);
            if (catInfo.NeedToDistribution)
            {
                _collectorService.assignToDistributer(order);
            }
            //if ((string.IsNullOrEmpty(order.PaymentMethodSystemName)
            //   || order.PaymentMethodSystemName != "Payments.CashOnDelivery"))// && IsPostService(serviceId))
            {
                foreach (var _item in order.Shipments)
                {
                    int collectorid = _collectorService.assignToCollector(_item.Id);
                    if (collectorid == 0)
                        InsertOrderNote($"درحال حاضر امکان جمع آوری این مموله شماره {_item.Id} از سفارش شماره {order.Id} در این زمان وجود ندارد", order.Id);
                }
            }
        }
        public void EditPostCoordination(int orderId, DateTime? CoordinationDate)
        {
            var data = _PostCoordinationRepository.Table.Where(p => p.orderId == orderId).ToList();
            foreach (var item in data)
            {
                item.CoordinationDate = CoordinationDate;
                _PostCoordinationRepository.Update(item);
            }
        }
        public void EditCollectDate(int shipmentId, DateTime? CoordinationDate)
        {
            var data = _ShipmentAppointmentRepository.Table.Where(p => p.ShipmentId == shipmentId).ToList();
            foreach (var item in data)
            {
                item.DataCollect = CoordinationDate;
                _ShipmentAppointmentRepository.Update(item);
            }
        }
        public void InsertAddressLocation(int AddressId, float Lat, float Lon)
        {
            _locationService.InsertAddressLocation(AddressId, Lat, Lon);
        }
        public Tbl_Address_LatLong GetAddressLocation(int AddressId)
        {
            return _locationService.getAddressLocation(AddressId);
        }

        public bool IsWallet(Order order)
        {
            return order.OrderItems.Any(p => p.Product.Id == 10277);
        }
        public bool IsIsland(int StateId)
        {
            return ((new int[] { 308, 314, 2259, 2272, 2274, 2275, 2319 }).Contains(StateId));
        }

        public void InsertOrderSource(int OrderId, int SourceId)
        {
            string Query = $@"INSERT INTO dbo.Tb_OrderSource
                                (
	                                SourceId
	                                , OrderId
                                )
                                VALUES
                                (	{SourceId}
	                                , {OrderId}
                                ) SELECT cast(SCOPE_IDENTITY() as int)  Id";
            int i = _dbContext.SqlQuery<int>(Query, new object[0]).FirstOrDefault();
        }
        public bool IsOrderForeign(Order order)
        {

            var categories = order.OrderItems.SelectMany(p => p.Product.ProductCategories).Select(p => p.CategoryId).Distinct();
            foreach (var item in categories)
            {
                var data = GetCategoryInfo(item);
                if (data != null && data.IsForeign)
                {
                    return true;
                }
            }
            return false;
        }

        public void ProcessPhoneOrder(Order order)
        {
            SavePostCoordination(order.Id, "سفارش تلفنی");
            InsertOrderNote("SendDataToPost", order.Id);
        }
        public AddressDetailes getAddressData(int CustomerId)
        {
            string query = $@"SELECT TOP(1)
	                            a.FirstName,
	                            a.LastName,
	                            A.PhoneNumber,
	                            a.ZipPostalCode,
	                            a.Address1,
	                            TALL.Lat,
	                            TALL.Long,
	                            C.Username as token,
	                            A.CountryId,
	                            A.StateProvinceId
                            FROM
	                            dbo.[Order] AS O
	                            INNER JOIN dbo.Shipment AS S ON S.OrderId = O.Id
	                            INNER JOIN dbo.Customer AS C ON C.Id = O.CustomerId
	                            INNER JOIN dbo.Address AS A ON A.Id = O.BillingAddressId
	                            INNER JOIN dbo.Tbl_Address_LatLong AS TALL ON TALL.AddressId = A.Id
                            WHERE
	                            O.CustomerId = {CustomerId}
	                            AND O.OrderStatusId = 30
	                            AND S.TrackingNumber IS NOT NULL
                            ORDER BY O.Id";
            return _dbContext.SqlQuery<AddressDetailes>(query, new object[0]).FirstOrDefault();
        }
    }
    public class AddressDetailes
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string ZipPostalCode { get; set; }
        public double? Lat { get; set; }
        public double? Long { get; set; }
        public string token { get; set; }
        public int CountryId { get; set; }
        public int StateProvinceId { get; set; }

    }
    public class OrderBill
    {
        public decimal ProductPrice { get; set; }
        public List<BillItem> AttributePrice { get; set; }
        public List<BillItem> AttributeCost { get; set; }

        public decimal TotalValue
        {
            get
            {
                decimal AttPrice = 0;
                if (AttributePrice != null)
                    if (AttributePrice.Any())
                        AttPrice = AttributePrice.Sum(p => p.value * p.qty);
                return ProductPrice + AttPrice;
            }
        }

        public decimal Productcost { get; set; }

        public decimal TotalCost
        {
            get
            {
                decimal AttCost = 0;
                if (AttributeCost != null)
                    if (AttributeCost.Any())
                        AttCost = AttributeCost.Sum(p => p.value * p.qty);
                return Productcost + AttCost;
            }
        }

        public decimal Tax { get; set; }
    }

    public class BillItem
    {
        public string name { get; set; }
        public decimal value { get; set; }
        public int qty { get; set; }
        public PriceType priceType { get; set; }
    }
    public enum PriceType
    {
        bime,
        hagheMaghar,
        hagheSabte,
        kala,
        karton,
        sabteSefarsh,
        weight,
        AccessPrinter,
        Sms,
        none
    }
    public static class GateWayError
    {
        private static readonly Dictionary<int, string> _errorDict = new Dictionary<int, string>
        {
            {1, "اطلاعات درخواستی ناقص میباشد"},
            {3, "امکان تغییر وضعیت این سفارش وجود ندارد"},
            {101, " کد پستی وارد شده معتبر نمیباشد"},
            {401, " خطا در شناسایی"},
            {402, " خطا در شناسایی نام کاربري فروشنده"},
            {403, " فروشنده مورد نظر منقضی و یا مسدود شده است."},
            {404, " درخواست / شناسه مورد نظر یافت نشد."},
            {405, " امکان استفاده از این سرویس ارسال براي این فروشگاه امکان پذیر نمیباشد."},
            {502, " خطا در شناسایی کد استان / شهرستان ارسال شده"},
            {503, " امکان ارسال مرسوله براي این مقصد میسر نمیباشد"},
            {505, " شناسه سفارش ارسال شده توسط شما تکراري میباشد"},
            {600, " این درخواست قبلا ثبت شده است."},
            {601, " تغییر وضعیتی براي نمایش وجود ندارد"},
            {800, " String تولید شده براي ثبت سفارش معتبر نمیباشد."},
            {801, " سفارش ارسالی بیشتر از حد مجاز میباشد. ( متد "},
            {802, " روش ارسال / سرویس درخواستی نامعتبر میباشد"},
            {803, " روش پرداخت درخواستی نامعتبر میباشد."},
            {804, " پارامترهاي الزامی به سامانه ارسال نگردیده است."},
            {805, " وزن ارسال شده نامعتبر میباشد."},
            {806, " قیمت ارسال شده براي سفارش نامعتبر میباشد."},
            {807, " هزینه ارسال ارسال شده نامعتبر میباشد."},
            {808, " مالیات بر ارزش افزوده ارسال شده نامعتبر میباشد."},
            {810, "وزن ارسالی بیش از حد مجاز می باشد"},
            {900, " تعداد سفارش ارسالی بیشتر از حد مجاز میباشد "}
        };

        public static string GetErrorMsg(string key)
        {
            if (string.IsNullOrEmpty(key))
                return "";
            if (key.Length > 9)
                return "";
            string value;
            _errorDict.TryGetValue(int.Parse(key), out value);
            return string.IsNullOrEmpty(value) ? "" : value;
        }

    }
    public static class DtsAddressExtensions
    {
        public static int getDtsStateId(this Address model)
        {
            if (new int[] { 4, 579, 580, 581, 582, 583, 584, 585, 579 }.Contains(model.StateProvinceId.Value))//برای مناطق تهران باید کد شهر تهران ارسال شود در دی تی اس
            {
                return 10866;
            }
            var _dbContext = EngineContext.Current.Resolve<IDbContext>();
            string query = $@"SELECT top(1)
	                            TDSP.City_Id
                            FROM
	                            dbo.Tb_Dts_StateProvince AS TDSP
                            WHERE
	                            TDSP.StateId = {model.StateProvinceId} ORDER BY City_Id";
            return _dbContext.SqlQuery<int?>(query).SingleOrDefault().GetValueOrDefault(0);
        }
    }
    public class YarboxStateModel
    {
        public int YarboxCountryId { get; set; }
        public string YarBoxCountryName { get; set; }
        public int YarboxCityId { get; set; }
        public string YarboxCityName { get; set; }
    }
    public class TPGStateModel
    {
        public int TPGStateId { get; set; }
        public string TPGStateName { get; set; }
    }
    public class UbbarRegionModel
    {
        public int RegionId { get; set; }
        public string RegionName { get; set; }
    }
    public class PDEStateModel
    {
        public int PDEStateId { get; set; }
        public bool IsForcedTehran { get; set; }
        public string PDEStateName { get; set; }
    }
    public class VecileItems
    {
        public string vehicle_type { get; set; }
        public string vehicle_options { get; set; }
        public string package_options { get; set; }
        public int SenderRegionId { get; set; }
        public int ReciverRegionId { get; set; }
    }
    public class GetPriceResult
    {
        public int price { get; set; }
        public int CodTranPrice { get; set; }
        public string errorMessage { get; set; }
        public string SLA { get; set; }
        public string UbbarTracking_code { get; set; }
        public bool canSelect { get; set; }
        public int ServiceId { get; set; }
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
    public static class Extensions
    {
        public static string SumAddressValue(this Address model)
        {
            if (model == null)
                return "";

            return model.CountryId.ToString() + "|" + model.StateProvinceId.ToString() + "|" + ((model.FirstName + "|" ?? "") + (model.LastName + "|" ?? "") + (model.PhoneNumber + "|" ?? "") +
                      (model.Address1 + "|" ?? "") + (model.ZipPostalCode + "|" ?? "") + (model.Company + "|" ?? ""));
        }
        public static string SumAddress(this Address model)
        {
            if (model == null)
                return "";

            return (model.Country?.Name ?? "") + "-" + (model.StateProvince?.Name ?? "") + "-" + ((model.FirstName + "-" ?? "") + (model.LastName + "-" ?? "") + (model.PhoneNumber + "-" ?? "") +
                      (model.Address1 + "-" ?? "") + (model.ZipPostalCode + "-" ?? "") + (model.Company ?? ""));
        }
        public static string SumAddress1(this Address model)
        {
            if (model == null)
                return "";

            return (model.Address1 + "-" ?? "") + (model.FirstName + "-" ?? "") + (model.LastName + "-" ?? "") + (model.PhoneNumber ?? "");
        }
    }
    public class _ServiceInfo
    {
        public string ServiceName { get; set; }
        public int ServiceId { get; set; }
        public string SLA { get; set; }
        public int Price { get; set; }
        public string _Price { get; set; }
        public bool IsCod { get; set; }
        public bool IsVIje { get; set; }
        public bool IsPishtaz { get; set; }
        public bool IsSefareshi { get; set; }
        public bool IsNromal { get; set; }
        public bool HasHagheMaghar { get; set; }
        public bool serviceActive { get; set; }
        public bool CanSelect { get; set; }
        public int hagheSabt { get; set; }
        public int CleanPrice { get; set; }
        public string message { get; set; }
        public string messageFroShow { get; set; }
        [NotMapped]
        public getPriceInputModel priceInputModel { get; set; }
    }
    public class CustomAddressModel : Address
    {
        public double? Lat { get; set; }
        public double? Lon { get; set; }
        public string text { get; set; }
        public string StateProvinceName { get; set; }
        public bool? trafficArea { get; set; }
        public bool? bazarArea { get; set; }
    }
}