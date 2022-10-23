using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces;
using Nop.Plugin.Orders.MultiShipping.Model;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Plugin.Orders.MultiShipping.Models.GeoPoint;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Factories;
using Nop.Web.Models.Checkout;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static Nop.Plugin.Orders.MultiShipping.Services.CartonSaleModel;
//using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran;

namespace Nop.Plugin.Orders.MultiShipping.Services
{
    public class NewCheckout : INewCheckout
    {
        #region field
        private readonly IHagheMaghar _hagheMagharl;
        private readonly ICollectorService _collectorService;
        private readonly IDbContext _dbContext;
        private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ICountryService _countryService;
        private readonly IAddressService _addressService;
        private readonly ICustomerService _customerService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IRepository<ShippingMethod> _shipingMethodRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IDiscountService _discountService;
        private readonly IExtnOrderProcessingService _extnOrderProcessingService;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IOrderService _orderService;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly IWorkContext _workContext;
        private readonly IRepository<StateProvince> _repositStateProvince;
        private readonly ISettingService _settingService;
        private readonly IGenericAttributeService _genericAttributeService;

        private readonly IRepository<Tb_StateProvinceGeoPoints> _repositStateProvinceGeoPoints;
        private readonly IRepository<Tb_CountryGeoPoints> _repositCountryGeoPoints;
        private readonly ICheckRegionDesVijePost _CheckRegionDesVijePost;
        private readonly IAgentAmountRuleService _agentAmountRuleService;
        private readonly INotificationService _notificationService;


        #endregion

        #region ctor
        public NewCheckout(
            ICollectorService collectorService,
            IAgentAmountRuleService agentAmountRuleService,
            ICheckRegionDesVijePost CheckRegionDesVijePost,
            IRepository<Tb_StateProvinceGeoPoints> repositStateProvinceGeoPoints,
            IRepository<Tb_CountryGeoPoints> repositCountryGeoPoints,
            IDbContext dbContext,
            IWorkContext workContext,
            IStaticCacheManager staticCacheManager,
            IAddressService addressService,
            ICustomerService customerService,
            ICountryService countryService,
            IShoppingCartService shoppingCartService,
            IProductService productService,
            IRepository<ShippingMethod> shipingMethodRepository,
            ICheckoutModelFactory checkoutModelFactory,
            IStoreContext storeContext,
            IDiscountService discountService,
            IExtnOrderProcessingService extnOrderProcessingService,
            IOrderService orderService,
            IExtendedShipmentService extendedShipmentService,
            IRepository<OrderNote> orderNoteRepository,
             ILocalizationService localizationService,
             IRepository<StateProvince> repositStateProvince,
             ISettingService settingService,
             IHagheMaghar hagheMagharl,
             IGenericAttributeService genericAttributeService,
             INotificationService notificationService)
        {
            _collectorService = collectorService;
            _notificationService = notificationService;
            _agentAmountRuleService = agentAmountRuleService;
            _genericAttributeService = genericAttributeService;
            _CheckRegionDesVijePost = CheckRegionDesVijePost;
            _repositStateProvinceGeoPoints = repositStateProvinceGeoPoints;
            _repositCountryGeoPoints = repositCountryGeoPoints;
            _hagheMagharl = hagheMagharl;
            _settingService = settingService;
            _repositStateProvince = repositStateProvince;
            _workContext = workContext;
            _orderNoteRepository = orderNoteRepository;
            _addressService = addressService;
            _localizationService = localizationService;
            _extendedShipmentService = extendedShipmentService;
            _orderService = orderService;
            _storeContext = storeContext;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _extnOrderProcessingService = extnOrderProcessingService;
            _checkoutModelFactory = checkoutModelFactory;
            _shipingMethodRepository = shipingMethodRepository;
            _customerService = customerService;
            _staticCacheManager = staticCacheManager;
            _dbContext = dbContext;
            _countryService = countryService;
            _discountService = discountService;
        }
        #endregion

        #region Get Lat & Long
        public (decimal Lat, decimal Lon) GetLatLong(int? CountryId, int? StateProvinceId)
        {
            if (StateProvinceId != null && StateProvinceId > 0)
            {
                var tt = _repositStateProvinceGeoPoints.Table.Where(p => p.StateProvinceId == StateProvinceId).FirstOrDefault();
                if (tt != null)
                {
                    return (tt.Lat, tt.Lon);
                }
            }
            if (CountryId != null && CountryId > 0)
            {
                var t = _repositCountryGeoPoints.Table.Where(p => p.CountryId == CountryId).FirstOrDefault();
                if (t != null)
                {
                    return (t.Lat, t.Lon);
                }

            }
            return (0, 0);
        }
        #endregion

        #region Ubbars

        public List<SelectListItem> getUbbarTruckType()
        {
            string query = $@"SELECT
                                TUVM.Vehicle_ModelsName+'|'+cast(TUVM.MaxWeight AS NVARCHAR(2)) Value
	                            , TUVM.Vehicle_ModelsNameFa Text
                            FROM    
                                dbo.Tb_Ubbar_VehicleModels AS TUVM";
            return _dbContext.SqlQuery<SelectListItem>(query, new object[0]).ToList();
        }
        public List<SelectListItem> getUbbarVechileOption(string TruckTypeName)
        {
            string query = "";
            if (string.IsNullOrEmpty(TruckTypeName))
            {
                return new List<SelectListItem>();
            }
            //if (TruckTypeName.ToLower() != "nissan")
            //{
            query = $@"SELECT
	                            TUVO.Vehicle_optionsName Value
	                            , TUVO.Vehicle_optionsNameFa Text
                            FROM
	                            dbo.Tb_Ubbar_VehicleOptions AS TUVO
                            WHERE
	                            TUVO.Vehicle_optionsName LIKE N'%'+N'{TruckTypeName}'+N'%'";
            //}
            //else
            //{
            //    query = $@"SELECT
            //                 TUVO.Vehicle_optionsName Value
            //                 , TUVO.Vehicle_optionsNameFa Text
            //                FROM
            //                 dbo.Tb_Ubbar_VehicleOptions AS TUVO
            //                WHERE
            //                 TUVO.Vehicle_optionsName LIKE N'%'+N'nissan'+N'%' or LIKE N'%'+N'van_'+N'%'";
            //}

            return _dbContext.SqlQuery<SelectListItem>(query, new object[0]).ToList();
        }
        #endregion

        public List<SelectListItem> getWeightItem()
        {
            int storeId = _storeContext.CurrentStore.Id;
            string CashKey = "getWeightItem";
            //if (_staticCacheManager.IsSet(CashKey))
            //{
            //    return _staticCacheManager.Get<List<SelectListItem>>(CashKey);
            //}
            //else
            //{
            string query = $@"SELECT DISTINCT
                                PAV.Name as [Text] ,
	                            CAST(CAST(PAV.WeightAdjustment as  int)*1000 AS VARCHAR(10))  as [Value]
                            INTO #tb1
                            FROM
	                            dbo.Category AS C
	                            INNER JOIN dbo.Product_Category_Mapping AS PCM ON PCM.CategoryId = C.Id
	                            INNER JOIN dbo.StoreMapping AS SM ON sm.EntityId = C.Id AND sm.EntityName ='Category'
	                            INNER JOIN dbo.CateguryPostType AS CPT ON CPT.CateguryId = C.Id
	                            INNER JOIN dbo.Product AS P ON P.Id = PCM.ProductId
	                            INNER JOIN dbo.Product_ProductAttribute_Mapping AS PPAM ON PPAM.ProductId = P.Id
	                            INNER JOIN dbo.ProductAttribute AS PA ON PA.Id = PPAM.ProductAttributeId
	                            INNER JOIN dbo.ProductAttributeValue AS PAV ON PAV.ProductAttributeMappingId = PPAM.Id
                            WHERE	
	                            C.ParentCategoryId <> 0
                                AND C.Deleted = 0
	                            AND C.Published = 1
	                            AND SM.StoreId = {storeId}
	                            AND p.Published = 1
	                            AND p.Deleted = 0
	                            AND pa.Name LIKE N'%وزن بسته%'

                            SELECT 
	                            [tb1].[Text],
	                            CAST(tb1.[Value] AS VARCHAR(10)) [Value]
                            FROM
	                            #tb1 tb1
                            ORDER BY LEN([Value]),[value]";

            var data = _dbContext.SqlQuery<_SelectListItem>(query, new object[0]).ToList<_SelectListItem>().Select(p => new SelectListItem()
            {
                Text = p.Text,
                Value = p.Value.ToString()
            }).ToList();
            //  _staticCacheManager.Set(CashKey, data, 1440);
            return data;
            // }
        }
        public List<SelectListItem> getInsuranceItems()
        {
            int storeId = _storeContext.CurrentStore.Id;
            string CashKey = "getInsuranceItems";
            //if (_staticCacheManager.IsSet(CashKey))
            //{
            //    return _staticCacheManager.Get<List<SelectListItem>>(CashKey);
            //}
            //else
            //{
            string query = $@"SELECT DISTINCT
                                PAV.Name as [Text] ,
                                PAV.Name as [Value],
	                            Cast(PAV.PriceAdjustment as  int) as [PriceAdjustment]
                            FROM
	                            dbo.Category AS C
	                            INNER JOIN dbo.Product_Category_Mapping AS PCM ON PCM.CategoryId = C.Id
	                            INNER JOIN dbo.StoreMapping AS SM ON sm.EntityId = C.Id AND sm.EntityName ='Category'
	                            INNER JOIN dbo.Tb_CategoryInfo AS TCI ON C.Id = TCi.CategoryId
	                            INNER JOIN dbo.Product AS P ON P.Id = PCM.ProductId
	                            INNER JOIN dbo.Product_ProductAttribute_Mapping AS PPAM ON PPAM.ProductId = P.Id
	                            INNER JOIN dbo.ProductAttribute AS PA ON PA.Id = PPAM.ProductAttributeId
	                            INNER JOIN dbo.ProductAttributeValue AS PAV ON PAV.ProductAttributeMappingId = PPAM.Id
                            WHERE	
	                            C.ParentCategoryId <> 0
                                AND C.Deleted = 0
	                            AND C.Published = 1
	                            AND SM.StoreId = {storeId}
	                            AND p.Published = 1
	                            AND p.Deleted = 0
	                            AND pa.Name LIKE N'%بیمه%'
                            ORDER BY  Cast(PAV.PriceAdjustment as  int)";
            var data = _dbContext.SqlQuery<_SelectListItem>(query, new object[0]).ToList<_SelectListItem>().Select(p => new SelectListItem()
            {
                Text = p.Text + (p.PriceAdjustment == 0 ? "" : " [" + p.PriceAdjustment + " ريال]"),
                Value = p.Text//p.Value
            }).ToList();
            //  _staticCacheManager.Set(CashKey, data, 1440);
            return data;
            // }
        }
        public List<SelectListItem> getKartonItems()
        {
            string CashKey = "getKartonItems";
            int storeId = _storeContext.CurrentStore.Id;
            //if (_staticCacheManager.IsSet(CashKey))
            //{
            //    return _staticCacheManager.Get<List<SelectListItem>>(CashKey);
            //}
            //else
            //{
            string query = $@"SELECT DISTINCT
                                PAV.Name as [Text] ,
                                PAV.Name as [Value],
	                            Cast(PAV.PriceAdjustment as  int) as [PriceAdjustment],
								pav.DisplayOrder
                            FROM
	                            dbo.Category AS C
	                            INNER JOIN dbo.Product_Category_Mapping AS PCM ON PCM.CategoryId = C.Id
	                            INNER JOIN dbo.StoreMapping AS SM ON sm.EntityId = C.Id AND sm.EntityName ='Category'
	                            INNER JOIN dbo.Tb_CategoryInfo AS TCI ON C.Id = TCi.CategoryId
	                            INNER JOIN dbo.Product AS P ON P.Id = PCM.ProductId
	                            INNER JOIN dbo.Product_ProductAttribute_Mapping AS PPAM ON PPAM.ProductId = P.Id
	                            INNER JOIN dbo.ProductAttribute AS PA ON PA.Id = PPAM.ProductAttributeId
	                            INNER JOIN dbo.ProductAttributeValue AS PAV ON PAV.ProductAttributeMappingId = PPAM.Id
                            WHERE	
	                            C.ParentCategoryId <> 0
                                AND C.Deleted = 0
	                            AND C.Published = 1
	                            AND SM.StoreId = {storeId}
	                            AND p.Published = 1
	                            AND p.Deleted = 0
	                            AND pa.Name LIKE N'%لفاف%'
                            ORDER BY PAV.DisplayOrder";
            var data = _dbContext.SqlQuery<_SelectListItem>(query, new object[0]).ToList<_SelectListItem>().Select(p => new SelectListItem()
            {
                Text = p.Text,//+ (p.PriceAdjustment == 0 ? "" : " [" + p.PriceAdjustment + " ريال]"),
                Value = p.Text//p.Value
            }).ToList();
            //  _staticCacheManager.Set(CashKey, data, 1440);
            return data;
            // }
        }
        public List<SelectListItem> getForginCountry()
        {
            return _extendedShipmentService.getForinCountry();
        }

        /// <summary>
        /// دریافت لیست استان ها
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> getCountryList()
        {

            if (_staticCacheManager.IsSet("getCountryList"))
            {
                return _staticCacheManager.Get<List<SelectListItem>>("getCountryList").OrderBy(P => P.Text).ToList();
            }
            else
            {
                var data = _countryService.GetAllCountries().Select(p => new SelectListItem()
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).OrderBy(P => P.Text).ToList();
                _staticCacheManager.Set("getCountryList", data, 1440);
                return data;
            }
        }
        public List<SelectListItem> getStateByCountryId(int countryId)
        {
            string QUERY = $@"SELECT
	                            sP.Id,
	                            sP.Name,
	                            CASE WHEN (SP.CountryId = 1 AND SP.ID = 582) OR (SP.CountryId <> 1 AND  TCC.Id IS NOT NULL) THEN 1 ELSE 0 END IsCapital
                            INTO #tb1
                            FROM
	                            dbo.StateProvince AS SP
	                            LEFT JOIN dbo.Tb_CountryCenter AS TCC ON TCC.CenterOfCountry = SP.Id
                            WHERE
	                            sP.Published = 1
	                            AND SP.CountryId = {countryId}

                            SELECT
	                            CAST(T.Id as varchar(10)) Value
	                            , T.Name as Text
                            FROM
	                            #tb1 AS T
                            ORDER BY IsCapital DESC ,T.Name ";
            return _dbContext.SqlQuery<SelectListItem>(QUERY, new object[0]).ToList();

        }
        public List<SelectListItem> getUbbarStateByCountryId(int countryId)
        {
            string query = $@"SELECT
	                            TUCS.CityName+'/'+TUCS.RegionName Text
	                            ,Cast(TUCS.RegionId as varchar(10)) Value
                            FROM
	                            dbo.Tb_ubbar_CityState AS TUCS
                            WHERE
	                            TUCS.PostbarCountryId = {countryId}
                            ORDER BY TUCS.CityName,TUCS.RegionName";
            return _dbContext.SqlQuery<SelectListItem>(query, new object[0]).ToList();
        }
        public async Task<List<_ServiceInfo>> GetServiceInfo(int senderCountry
            , int senderState
            , int receiverCountry
            , int receiverState
            , int weightItem
            , int AproximateValue
            , int customerId
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
            , string receiver_ForeginCountryNameEn = null
            , bool FromAPi = false//حق مقر شمرده نمیشود API در 
            , bool? IsCod = null
            , int serviceId = 0
            , bool IsFromAp = false
            , bool ShowPrivatePost = true
            , bool ShowDistributer = true
            , CustomAddressModel SenderAddress = null
            , CustomAddressModel ReciverAddress = null
            , bool IsFromUi = false
            , bool IsFromSep = false
            )
        {
            if (isInvalidSernder(senderCountry, senderState))
            {
                return new List<_ServiceInfo>();
            }
            bool isFromBidoc = _workContext.CurrentCustomer.Id == 4144899 || _workContext.CurrentCustomer.AffiliateId == 1149;
            int dealerId = IsFromAp ? 1 : (isFromBidoc ? 4 : 0);

            int storeId = _storeContext.CurrentStore.Id;
            List<_ServiceInfo> _data = _extendedShipmentService.GetBasPriceAndSlA(senderCountry, senderState, receiverCountry, receiverState, weightItem, storeId
                , customerId, receiver_ForeginCountry, IsCod, serviceId, ShowPrivatePost, ShowDistributer);
            bool IsFirstOrder = _extendedShipmentService.GetOrdersByCustomerId(_workContext.CurrentCustomer.Id) == 0;
            List<_ServiceInfo> data = new List<_ServiceInfo>();
            //if (IsFromUi || FromAPi)
            //{
            //    data = _data.Where(p => p.ServiceId != 654).ToList();
            //}
            // else
            data = _data;
            if (data == null)
            {
                return new List<_ServiceInfo>();
            }
            if (storeId == 5)
            {
                // متد چک کردن ورودی ها با محدودیت های سرویس دهنده ها
                try
                {
                    string error = "";
                    bool canSelect = true;
                    //طول و عرض و ارتفاع باید در توابع پست داخلی حذف شوند
                    foreach (var item in data)
                    {
                        item.CanSelect = true;
                        item.hagheSabt = 0;// _extendedShipmentService.CalcHagheSabet(_workContext.CurrentCustomer.Id, item.ServiceId, 0);
                        item.priceInputModel = _extendedShipmentService.PriceInpuModelFactory(senderCountry, senderState, receiverCountry, receiverState, weightItem,
                            AproximateValue, out error, out canSelect, height, length, width, consType, content, dispach_date, PackingOption, vechileType,
                            VechileOption, receiver_ForeginCountry, item.ServiceId, SenderAddress, ReciverAddress, receiver_ForeginCountryNameEn, quantity: 1);
                        if (!canSelect)
                        {
                            item.CanSelect = canSelect;
                            item.Price = 0;
                            item._Price = error;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogException(ex);
                    Log("خطا در زمان آماده سازی ورودی سرویس های استعلام قیمت", "");
                }
            }

            var Currentcustomer = _customerService.GetCustomerById(customerId);
            foreach (var item in data)
            {

                // اگر کاربر پست را انتخاب کرده باشد
                if (_extendedShipmentService.isInvalidSender(senderCountry, senderState, item.ServiceId))
                {
                    string serviceName = string.IsNullOrEmpty(item.ServiceName) ? "مورد نظر وجود ندارد" : item.ServiceName;
                    item.CanSelect = false;
                    item._Price = $"در حال حاضر امکان ارسال از مبدا مورد نظر برای {serviceName} وجود ندارد  ";
                    continue;
                }
                try
                {
                    if (new int[] { 654, 655, 667, 670, 660, 661, 662, 654, 722, 723, 725, 726, 727 }.Contains(item.ServiceId) && item.CanSelect)
                    {
                        //if (_workContext.CurrentCustomer.IsInCustomerRole("onlineGateway") && new int[] { 654, 655, 660, 661, 662, 654, 725, 726, 727 }.Contains(item.ServiceId))
                        //{
                        //    item.CanSelect = false;
                        //    item._Price = "امکان استفاده از سرویس پست بار برای شما فعال نیست";
                        //    continue;
                        //}
                        //if (!_workContext.CurrentCustomer.IsInCustomerRole("onlineGateway") && new int[] { 722, 723 }.Contains(item.ServiceId))
                        //{
                        //    item.CanSelect = false;
                        //    item._Price = "امکان استفاده از سرویس گیت وی برای شما فعال نیست";
                        //    continue;
                        //}
                        //DateTime dt = new DateTime(2020, 5, 3, 12, 0, 0);

                        //if ((_workContext.CurrentCustomer.CreatedOnUtc.ToLocalTime().CompareTo(dt) > 0)
                        //    && _storeContext.CurrentStore.Id == 5
                        //    && _workContext.CurrentCustomer.AffiliateId == 0
                        //    && !isFromBidoc
                        //    && !IsFromAp
                        //    && !FromAPi
                        //   // && !_workContext.CurrentCustomer.IsInCustomerRole("postex")
                        //    && !IsFromSep)
                        //{
                        //    item.CanSelect = false;
                        //    item._Price = "سرویسی برای شما یافت نشد";
                        //    continue;
                        //}
                        if (width > 100 || height > 100 || length > 100)
                        {
                            item.CanSelect = false;
                            item._Price = "با توجه به ابعاد مرسوله شما سرویس پست بار ارائه نمی شود(محدودیت 100 سانتیمتر در هر ضلع)";
                            continue;
                        }
                        if (weightItem >= 30000)
                        {
                            item.CanSelect = false;
                            item._Price = "سقف وزن بسته 30 کیلو گرم";
                            continue;
                        }
                        //if (_workContext.CurrentCustomer.Id != 13653617)//اسفندی نماینده تبریز
                        //{
                        //    if (weightItem > 5000 && serviceId == 654)
                        //    {
                        //        item.CanSelect = false;
                        //        item._Price = "در وزن بیش از 5000 گرم(5 کیلو گرم) در سرویس پست بار فقط امکان ثبت سفارش به صورت پیشتاز وجود دارد";
                        //        continue;
                        //    }
                        //}
                        item.CanSelect = true;
                    }
                    if (dealerId == 0)
                    {
                        item.Price += item.hagheSabt;
                        item.Price += (item.Price * 9) / 100;
                        item._Price = item.Price.ToString("N0");
                    }
                }
                catch (Exception ex)
                {
                    item.CanSelect = false;
                    LogException(ex);
                    Log("خطا در زمان مشخص کردن قیمت سرویس های پست بار", "");
                }
                try
                {
                    if (new int[] { 667, 670, 722, 723 }.Contains(item.ServiceId))//gateWay
                    {
                        if (weightItem <= 30)
                            weightItem = 31;
                        if (weightItem <= 30)
                        {
                            item.CanSelect = false;
                            item._Price = "در سرویس پس کرایه حداقل وزن مجاز بیش از 30 گرم می باشد";
                            continue;
                        }
                        if (new int[] { 667, 670 }.Contains(item.ServiceId))
                        {
                            if (_extendedShipmentService.IsIsland(senderState) || _extendedShipmentService.IsIsland(receiverState))
                            {
                                item.CanSelect = false;
                                item._Price = "امکان ارسال و دریافت به جزایر ایران در سرویس پسکرایه وجود ندارد";
                                continue;
                            }
                        }
                        else
                        {
                            if (_extendedShipmentService.IsIsland(senderState))
                            {
                                item.CanSelect = false;
                                item._Price = "امکان ارسال از جزایر ایران در سرویس پیش کرایه پست وجود ندارد";
                                continue;
                            }
                        }
                        //if (Currentcustomer.IsInCustomerRole("Administrators") || Currentcustomer.IsInCustomerRole("COD"))
                        //{
                        #region Post COD
                        int _price = 0;
                        //int postType = item.IsPishtaz ? 1 : 0;
                        string error = "";
                        bool _isCod = new int[] { 667, 670 }.Contains(item.ServiceId) ? true : false;
                        int _customerId = 0;
                        _customerId = _workContext.CurrentCustomer.Id;

                        _price = _extendedShipmentService.getCodbasePrice(_customerId, item.ServiceId, weightItem, receiverCountry
                            , receiverState, senderState, out error, dealerId > 0, IsCOD: _isCod, approximateValue: AproximateValue);
                        if (_price != 0)
                        {
                            item._Price = (_price).ToString("N0");
                            item.Price = _price;
                        }
                        else
                            item.CanSelect = false;
                        #endregion
                        //}
                        //else
                        //{
                        //    item.CanSelect = false;
                        //    item._Price = "این سرویس برای شما فعال نمی باشد";
                        //    continue;
                        //}
                    }
                }
                catch (Exception ex)
                {
                    item.CanSelect = false;
                    LogException(ex);
                    Log("خطا در زمان مشخص کردن قیمت سرویس های گیت وی پست", "");
                }

            }
            //}
            //watch.Start();
            if (storeId == 5)
            {
                List<Task<GetPriceResult>> LstTask = new List<Task<GetPriceResult>>();

                foreach (var item in data)
                {
                    try
                    {
                        if (new int[] { 703, 699, 705, 706 }.Contains(item.ServiceId) && item.CanSelect) // DTS
                        {

                            #region DTS

                            var task = _extendedShipmentService.getDtsPrice(item.priceInputModel, 10825);
                            LstTask.Add(task);
                            #endregion
                        }
                        else if (new int[] { 707, 708 }.Contains(item.ServiceId) && item.CanSelect)// PDE
                        {

                            #region PDE
                            if (item.ServiceId == 708) // داخلی
                            {

                                var task = _extendedShipmentService.getPDE_Price(item.priceInputModel);
                                LstTask.Add(task);

                            }
                            else if (item.ServiceId == 707)// خارجی
                            {
                                var task = _extendedShipmentService.getPDEInternational_Price(item.priceInputModel);
                                LstTask.Add(task);
                            }
                            #endregion

                        }
                        else if (new int[] { 733 }.Contains(item.ServiceId) && item.CanSelect)// kalaResan
                        {

                            #region PDE
                            var task = _extendedShipmentService.getKalaresanPrice(item.priceInputModel);
                            LstTask.Add(task);
                            #endregion

                        }
                        else if (new int[] { 709, 710, 711 }.Contains(item.ServiceId) && item.CanSelect) // TPG
                        {
                            #region TPG
                            var task = _extendedShipmentService.getTPGPrice(item.priceInputModel, true);
                            LstTask.Add(task);
                            #endregion
                        }
                        else if (item.ServiceId == 701 && item.CanSelect) // اوبار
                        {

                            #region ubaar
                            var task = _extendedShipmentService.getUbarPrice(item.priceInputModel);
                            LstTask.Add(task);
                            #endregion
                        }
                        else if (item.ServiceId == 702 && item.CanSelect) // یارباکس
                        {

                            if (weightItem <= 50)
                                weightItem = 50;
                            #region یارباکس

                            var task = _extendedShipmentService.getYarboxPrice(item.priceInputModel);
                            LstTask.Add(task);
                            #endregion
                        }
                        else if (new int[] { 712, 713, 714, 715 }.Contains(item.ServiceId) && item.CanSelect)// چاپار
                        {

                            #region chapar
                            var task = _extendedShipmentService.getChaparPrice(item.priceInputModel, 29863);
                            LstTask.Add(task);
                            #endregion
                        }
                        else if ((item.ServiceId == 730 || item.ServiceId == 731) && item.CanSelect)//ماهکس
                        {
                            #region mahex
                            var task = _extendedShipmentService.getmahexPrice(item.priceInputModel);
                            LstTask.Add(task);
                            #endregion
                        }
                        else if (item.ServiceId == 719 && item.CanSelect)// Blus Sky
                        {
                            #region skublue
                            var task = _extendedShipmentService.getBlueSkyPrice(item.priceInputModel);
                            LstTask.Add(task);
                            #endregion
                        }
                        else if (item.ServiceId == 717 && item.CanSelect)// SnapBox
                        {
                            #region snappbox

                            var task = _extendedShipmentService.getsnappboxPrice(item.priceInputModel);
                            LstTask.Add(task);
                            #endregion
                        }
                        else if (item.ServiceId == 718 && item.CanSelect)
                        {
                            var task = _extendedShipmentService.peykhubGetPrice(item.priceInputModel);
                            LstTask.Add(task);
                        }
                    }
                    catch (Exception ex)
                    {
                        item.CanSelect = false;
                        Log($"خطا در زمان دریافت قیمت سرویس {item.ServiceId} از سرور مربوطه", "");
                        LogException(ex);
                    }
                }
                try
                {
                    if (LstTask.Any())
                        await Task.WhenAll(LstTask.ToArray());
                    foreach (var _item in LstTask)
                    {
                        var result = _item.Result;

                        var item = data.SingleOrDefault(p => p.ServiceId == result.ServiceId);
                        if (result.ServiceId == 730 || result.ServiceId == 731)
                        {
                            if (result.SLA == "-")
                                item.SLA = "از 24 تا 48 ساعت";
                            else
                                item.SLA = result.SLA;
                        }
                        item.CanSelect = result.canSelect;
                        if (!item.CanSelect)
                        {
                            item._Price = result.errorMessage;
                            item.Price = 0;
                        }
                        else
                        {
                            item.Price = result.price;
                            item._Price = result.price.ToString("N0");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogException(ex);
                    Log("خطا در زمان مشخص کردن نتیجه استعلام قیمت  سرویس ها ", "");
                }
                //watch.Stop();
                //common.Log($"دریافت قیمت سرویس ها از سرور ها :" + watch.ElapsedMilliseconds, "");
                //Total += watch.ElapsedMilliseconds;
                //watch.Reset();
                //watch.Start();
                foreach (var item in data)
                {
                    try
                    {
                        if (item.CanSelect)
                        {

                            int Totalprice = 0;
                            if (dealerId == 0)
                            {
                                if (!new int[] { 654, 655, 660, 661, 662, 667, 670, 698, 697, 696, 695, 694, 693, 691, 690, 725, 726, 727, 722, 723 }.Contains(item.ServiceId))// بعدا برای یکسان سازی حق فنی مهندسی پست باید این فیلید برداشته شود
                                    Totalprice = _extendedShipmentService.getOrderTotalbyIncomeApiPrice(item.Price, 0, item.ServiceId, weightItem);
                                else
                                    Totalprice = item.Price;
                            }
                            else
                            {
                                if (new int[] { 654, 655, 660, 661, 662, 667, 670, 698, 697, 696, 695, 694, 693, 691, 690, 725, 726, 727 }.Contains(item.ServiceId))
                                    Totalprice = _extendedShipmentService.getOrderTotalbyIncomeApiPrice(item.CleanPrice, 0, item.ServiceId, weightItem, dealerId);
                                else
                                    Totalprice = _extendedShipmentService.getOrderTotalbyIncomeApiPrice(item.Price, 0, item.ServiceId, weightItem, dealerId);
                            }
                            Totalprice += ((item.hagheSabt) + (((item.hagheSabt) * 9) / 100));
                            item.Price = Totalprice;
                            item._Price = Totalprice.ToString("N0");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogException(ex);
                        Log("خطا در زمان اضافه کردن حق فنی مهندسی به سرویس  ها ", "");
                    }
                }
                //watch.Stop();
                //common.Log("دریافت قیمت ها از بانک :" + watch.ElapsedMilliseconds, "");
                //Total += watch.ElapsedMilliseconds;
                //watch.Reset();
                //common.Log("کل زمان :" + Total, "");
            }

            #region اضافه کردن قیمت پیشتاز به سفارش خارجی
            foreach (var item in data)
            {
                if (!(item.ServiceId == 707 || item.ServiceId == 719))
                    continue;
                int reciverStateId = item.ServiceId == 719 ? 581 : 582;
                int Internal_serviceId = 0;
                if (senderCountry != 1)
                {
                    Internal_serviceId = 714;//730;//ماهکس
                }
                else
                {
                    Internal_serviceId = 723;//پست پیشتاز
                }
                var Pishtazdata = await GetServiceInfo(senderCountry, senderState, 1, reciverStateId, weightItem, AproximateValue, customerId, height, length, width, consType,
                    content, null, null, null, null, 0, null, FromAPi, false, Internal_serviceId, IsFromAp, true, false, SenderAddress, ReciverAddress);

                if (Pishtazdata == null || !Pishtazdata.Any() || Pishtazdata.Any(p => p.CanSelect == false))
                {
                    item.CanSelect = false;
                }
                else
                {
                    item.Price += Pishtazdata.First().Price;
                    item._Price = item.Price.ToString("N0");
                }
            }
            #endregion

            #region اعمال تخفیف سفارش اولی ها
            if (!(_workContext.CurrentCustomer.Id == 8186075 || _workContext.CurrentCustomer.AffiliateId == 1186))
            {
                if (!IsFromSep && !IsFromAp && !isFromBidoc && !(_workContext.CurrentCustomer.IsInCustomerRole("mini-Administrators")))
                {
                    if (IsFirstOrder || _extendedShipmentService.IsInValidDiscountPeriod())
                    {
                        var firstOrderDiscount = _extendedShipmentService.getFistOrderDiscount();
                        if (firstOrderDiscount > 0)
                        {
                            foreach (var item in data)
                            {
                                if (_extendedShipmentService.IsPostService(item.ServiceId))
                                {
                                    item.message = item._Price;
                                    item.Price -= (int)((item.Price * firstOrderDiscount) / 100);
                                    item._Price = item.Price.ToString("N0");
                                }
                            }
                        }
                    }
                }
            }
            if (IsFromSep)//سپ سامان
            {
                foreach (var item in data)
                {
                    if (_extendedShipmentService.IsPostService(item.ServiceId))// پست
                    {
                        item.message = item._Price;
                        item.Price -= (int)((item.Price * 20) / 100);
                        item._Price = item.Price.ToString("N0");
                    }
                    else if (item.ServiceId == 712 || item.ServiceId == 714 || item.ServiceId == 702)// چاپار و یارباکس
                    {
                        item.message = item._Price;
                        item.Price -= (int)((item.Price * 5) / 100);
                        item._Price = item.Price.ToString("N0");
                    }
                    else if (item.ServiceId == 707 || item.ServiceId == 710 || item.ServiceId == 719)//پست خارجی
                    {
                        item.message = item._Price;
                        item.Price -= (int)((item.Price * 2) / 100);
                        item._Price = item.Price.ToString("N0");
                    }
                }
            }
            //else if (new DateTime(2021, 01, 19).CompareTo(DateTime.Now) > 0)// اتاق ایران اتریش
            //{
            //    foreach (var item in data)
            //    {
            //        if (_extendedShipmentService.IsPostService(item.ServiceId))
            //        {
            //            item.message = item._Price;
            //            item.Price -= (int)((item.Price * 10) / 100);
            //            item._Price = item.Price.ToString("N0");
            //        }
            //    }
            //}
            #endregion 
            //if (_workContext.CurrentCustomer.Id != 13653617)//اسفندی نماینده تبریز
            //{

            //    foreach (var item in data)
            //    {
            //        if (weightItem > 5000 && item.ServiceId == 654)
            //        {
            //            item.CanSelect = false;
            //            item._Price = "در وزن بیش از 5000 گرم(5 کیلو گرم) در سرویس پست بار فقط امکان ثبت سفارش به صورت پیشتاز وجود دارد";
            //            break;
            //        }
            //    }
            //}
            var ServiceList = data.Where(p => p.CanSelect == true).OrderBy(p => p.Price).ThenBy(p => p.SLA).ToList();
            if (!_extendedShipmentService.HasAgentInCity(senderState))
            {
                ServiceList.ForEach(delegate (_ServiceInfo serInfo)
                {
                    serInfo.messageFroShow = "کاربر محترم در مبدا مورد نظر شما نماینده فعال وجود ندارد." +
" جهت تحویل مرسوله  می بایست به دفتر پستی که از سوی پشتیبانی اعلام میشود مراجعه کنید";
                });
            }
            return ServiceList;
        }



        public async Task<List<_ServiceInfo>> GetServiceInfoAnonymouse(int senderCountry
            , int senderState
            , int receiverCountry
            , int receiverState
            , int weightItem
            , int AproximateValue
            //, int customerId
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
            , string receiver_ForeginCountryNameEn = null
            , bool FromAPi = false//حق مقر شمرده نمیشود API در 
            , bool? IsCod = null
            , int serviceId = 0
            , bool IsFromAp = false
            , bool ShowPrivatePost = true
            , bool ShowDistributer = true
            , CustomAddressModel SenderAddress = null
            , CustomAddressModel ReciverAddress = null
            , bool IsFromUi = false
            , bool IsFromSep = false
            )
        {
            if (isInvalidSernder(senderCountry, senderState))
            {
                return new List<_ServiceInfo>();
            }
            bool isFromBidoc = _workContext.CurrentCustomer.Id == 4144899 || _workContext.CurrentCustomer.AffiliateId == 1149;
            int dealerId = IsFromAp ? 1 : (isFromBidoc ? 4 : 0);

            int storeId = _storeContext.CurrentStore.Id;
            List<_ServiceInfo> _data = _extendedShipmentService.GetBasPriceAndSlA(senderCountry, senderState, receiverCountry, receiverState, weightItem, storeId
                , 0, receiver_ForeginCountry, IsCod, serviceId, ShowPrivatePost, ShowDistributer);
            bool IsFirstOrder = _extendedShipmentService.GetOrdersByCustomerId(_workContext.CurrentCustomer.Id) == 0;
            List<_ServiceInfo> data = new List<_ServiceInfo>();
            //if (IsFromUi || FromAPi)
            //{
            //    data = _data.Where(p => p.ServiceId != 654).ToList();
            //}
            // else
            data = _data;
            if (data == null)
            {
                return new List<_ServiceInfo>();
            }
            if (storeId == 5)
            {
                // متد چک کردن ورودی ها با محدودیت های سرویس دهنده ها
                try
                {
                    string error = "";
                    bool canSelect = true;
                    //طول و عرض و ارتفاع باید در توابع پست داخلی حذف شوند
                    foreach (var item in data)
                    {
                        item.CanSelect = true;
                        item.hagheSabt = _extendedShipmentService.CalcHagheSabet(_workContext.CurrentCustomer.Id, item.ServiceId, 0);
                        item.priceInputModel = _extendedShipmentService.PriceInpuModelFactory(senderCountry, senderState, receiverCountry, receiverState, weightItem,
                            AproximateValue, out error, out canSelect, height, length, width, consType, content, dispach_date, PackingOption, vechileType,
                            VechileOption, receiver_ForeginCountry, item.ServiceId, SenderAddress, ReciverAddress, receiver_ForeginCountryNameEn);
                        if (!canSelect)
                        {
                            item.CanSelect = canSelect;
                            item.Price = 0;
                            item._Price = error;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogException(ex);
                    Log("خطا در زمان آماده سازی ورودی سرویس های استعلام قیمت", "");
                }
            }

            //var Currentcustomer = _customerService.GetCustomerById(customerId);
            foreach (var item in data)
            {
                // اگر کاربر پست را انتخاب کرده باشد
                if (_extendedShipmentService.isInvalidSender(senderCountry, senderState, item.ServiceId))
                {
                    string serviceName = string.IsNullOrEmpty(item.ServiceName) ? "مورد نظر وجود ندارد" : item.ServiceName;
                    item.CanSelect = false;
                    item._Price = $"در حال حاضر امکان ارسال از مبدا مورد نظر برای {serviceName} وجود ندارد  ";
                    continue;
                }
                try
                {
                    if (new int[] { 654, 655, 667, 670, 660, 661, 662, 654, 722, 723, 725, 726, 727 }.Contains(item.ServiceId) && item.CanSelect)
                    {
                        //if (_workContext.CurrentCustomer.IsInCustomerRole("onlineGateway") && new int[] { 654, 655, 660, 661, 662, 654, 725, 726, 727 }.Contains(item.ServiceId))
                        //{
                        //    item.CanSelect = false;
                        //    item._Price = "امکان استفاده از سرویس پست بار برای شما فعال نیست";
                        //    continue;
                        //}
                        //if (!_workContext.CurrentCustomer.IsInCustomerRole("onlineGateway") && new int[] { 722, 723 }.Contains(item.ServiceId))
                        //{
                        //    item.CanSelect = false;
                        //    item._Price = "امکان استفاده از سرویس گیت وی برای شما فعال نیست";
                        //    continue;
                        //}
                        //DateTime dt = new DateTime(2020, 5, 3, 12, 0, 0);

                        //if ((_workContext.CurrentCustomer.CreatedOnUtc.ToLocalTime().CompareTo(dt) > 0)
                        //    && _storeContext.CurrentStore.Id == 5
                        //    && _workContext.CurrentCustomer.AffiliateId == 0
                        //    && !isFromBidoc
                        //    && !IsFromAp
                        //    && !FromAPi
                        //   // && !_workContext.CurrentCustomer.IsInCustomerRole("postex")
                        //    && !IsFromSep)
                        //{
                        //    item.CanSelect = false;
                        //    item._Price = "سرویسی برای شما یافت نشد";
                        //    continue;
                        //}
                        if (width > 100 || height > 100 || length > 100)
                        {
                            item.CanSelect = false;
                            item._Price = "با توجه به ابعاد مرسوله شما سرویس پست بار ارائه نمی شود(محدودیت 100 سانتیمتر در هر ضلع)";
                            continue;
                        }
                        if (weightItem >= 30000)
                        {
                            item.CanSelect = false;
                            item._Price = "سقف وزن بسته 30 کیلو گرم";
                            continue;
                        }
                        if (_workContext.CurrentCustomer.Id != 13653617)//اسفندی نماینده تبریز
                        {
                            if (weightItem > 5000 && serviceId == 654)
                            {
                                item.CanSelect = false;
                                item._Price = "در وزن بیش از 5000 گرم(5 کیلو گرم) در سرویس پست بار فقط امکان ثبت سفارش به صورت پیشتاز وجود دارد";
                                continue;
                            }
                        }
                        item.CanSelect = true;
                    }
                    if (dealerId == 0)
                    {
                        item.Price += item.hagheSabt;
                        item.Price += (item.Price * 9) / 100;
                        item._Price = item.Price.ToString("N0");
                    }
                }
                catch (Exception ex)
                {
                    item.CanSelect = false;
                    LogException(ex);
                    Log("خطا در زمان مشخص کردن قیمت سرویس های پست بار", "");
                }
                try
                {
                    if (new int[] { 667, 670, 722, 723 }.Contains(item.ServiceId))//gateWay
                    {
                        if (weightItem <= 30)
                            weightItem = 31;
                        if (weightItem <= 30)
                        {
                            item.CanSelect = false;
                            item._Price = "در سرویس پس کرایه حداقل وزن مجاز بیش از 30 گرم می باشد";
                            continue;
                        }
                        if (_extendedShipmentService.IsIsland(senderState))//|| _extendedShipmentService.IsIsland(receiverState))
                        {
                            item.CanSelect = false;
                            item._Price = "امکان ارسال و دریافت به جزایر ایران در سرویس پسکرایه وجود ندارد";
                            continue;
                        }
                        //if (Currentcustomer.IsInCustomerRole("Administrators") || Currentcustomer.IsInCustomerRole("COD"))
                        //{
                        #region Post COD
                        int _price = 0;
                        //int postType = item.IsPishtaz ? 1 : 0;
                        string error = "";
                        bool _isCod = new int[] { 667, 670 }.Contains(item.ServiceId) ? true : false;
                        int _customerId = 0;
                        _customerId = _workContext.CurrentCustomer.Id;

                        _price = _extendedShipmentService.getCodbasePrice(_customerId, item.ServiceId, weightItem, receiverCountry, receiverState
                            , senderState, out error, dealerId > 0, IsCOD: _isCod, approximateValue: AproximateValue);
                        if (_price != 0)
                        {
                            item._Price = (_price).ToString("N0");
                            item.Price = _price;
                        }
                        else
                        {
                            if (_workContext.CurrentCustomer.IsInCustomerRole("mini-Administrators") &&
                                !_isCod)
                            {
                                _customerId = _dbContext.SqlQuery<int>($@"SELECT 
			                                                TUS.CustomerId
		                                                FROM
			                                                dbo.Tb_UserStates AS TUS
			                                                INNER JOIN dbo.Customer_CustomerRole_Mapping AS CCRM ON CCRM.Customer_Id = TUS.CustomerId
		                                                WHERE 
			                                                TUS.StateId = {receiverState}
			                                                AND CCRM.CustomerRole_Id = 7", new object[0]).FirstOrDefault(); ;// 
                                _price = _extendedShipmentService.getCodbasePrice(_customerId, item.ServiceId, weightItem, receiverCountry, receiverState
                                    , senderState, out error, dealerId > 0, IsCOD: _isCod, approximateValue: AproximateValue);
                                if (_price != 0)
                                {
                                    item._Price = (_price).ToString("N0");
                                    item.Price = _price;
                                }
                                else
                                {
                                    item.CanSelect = false;
                                }
                            }
                            else
                                item.CanSelect = false;
                        }
                        #endregion
                        //}
                        //else
                        //{
                        //    item.CanSelect = false;
                        //    item._Price = "این سرویس برای شما فعال نمی باشد";
                        //    continue;
                        //}
                    }
                }
                catch (Exception ex)
                {
                    item.CanSelect = false;
                    LogException(ex);
                    Log("خطا در زمان مشخص کردن قیمت سرویس های گیت وی پست", "");
                }

            }
            //}
            //watch.Start();
            if (storeId == 5)
            {
                List<Task<GetPriceResult>> LstTask = new List<Task<GetPriceResult>>();

                foreach (var item in data)
                {
                    try
                    {
                        if (new int[] { 703, 699, 705, 706 }.Contains(item.ServiceId) && item.CanSelect) // DTS
                        {

                            #region DTS

                            var task = _extendedShipmentService.getDtsPrice(item.priceInputModel, 10825);
                            LstTask.Add(task);
                            #endregion
                        }
                        else if (new int[] { 707, 708 }.Contains(item.ServiceId) && item.CanSelect)// PDE
                        {

                            #region PDE
                            if (item.ServiceId == 708) // داخلی
                            {

                                var task = _extendedShipmentService.getPDE_Price(item.priceInputModel);
                                LstTask.Add(task);

                            }
                            else if (item.ServiceId == 707)// خارجی
                            {
                                var task = _extendedShipmentService.getPDEInternational_Price(item.priceInputModel);
                                LstTask.Add(task);
                            }
                            #endregion

                        }
                        else if (new int[] { 733 }.Contains(item.ServiceId) && item.CanSelect)// kalaResan
                        {

                            #region PDE
                            var task = _extendedShipmentService.getKalaresanPrice(item.priceInputModel);
                            LstTask.Add(task);
                            #endregion

                        }
                        else if (new int[] { 709, 710, 711 }.Contains(item.ServiceId) && item.CanSelect) // TPG
                        {
                            #region TPG
                            var task = _extendedShipmentService.getTPGPrice(item.priceInputModel, true);
                            LstTask.Add(task);
                            #endregion
                        }
                        else if (item.ServiceId == 701 && item.CanSelect) // اوبار
                        {

                            #region ubaar
                            var task = _extendedShipmentService.getUbarPrice(item.priceInputModel);
                            LstTask.Add(task);
                            #endregion
                        }
                        else if (item.ServiceId == 702) // یارباکس
                        {
                            if (weightItem <= 50)
                                weightItem = 51;
                            #region یارباکس

                            var task = _extendedShipmentService.getYarboxPrice(item.priceInputModel);
                            LstTask.Add(task);
                            #endregion
                        }
                        else if (new int[] { 712, 713, 714, 715 }.Contains(item.ServiceId) && item.CanSelect)// چاپار
                        {

                            #region chapar
                            var task = _extendedShipmentService.getChaparPrice(item.priceInputModel, 10825);
                            LstTask.Add(task);
                            #endregion
                        }
                        else if (item.ServiceId == 730 || item.ServiceId == 731)//ماهکس
                        {
                            #region mahex
                            var task = _extendedShipmentService.getmahexPrice(item.priceInputModel);
                            LstTask.Add(task);
                            #endregion
                        }
                        else if (item.ServiceId == 719)
                        {

                            #region skublue

                            var task = _extendedShipmentService.getBlueSkyPrice(item.priceInputModel);
                            LstTask.Add(task);
                            #endregion
                        }
                        else if (item.ServiceId == 717)
                        {
                            #region snappbox

                            var task = _extendedShipmentService.getsnappboxPrice(item.priceInputModel);
                            LstTask.Add(task);
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        item.CanSelect = false;
                        Log($"خطا در زمان دریافت قیمت سرویس {item.ServiceId} از سرور مربوطه", "");
                        LogException(ex);
                    }
                }
                try
                {
                    if (LstTask.Any())
                        await Task.WhenAll(LstTask.ToArray());
                    foreach (var _item in LstTask)
                    {
                        var result = _item.Result;
                        var item = data.SingleOrDefault(p => p.ServiceId == result.ServiceId);
                        item.CanSelect = result.canSelect;
                        if (!item.CanSelect)
                        {
                            item._Price = result.errorMessage;
                            item.Price = 0;
                        }
                        else
                        {
                            item.Price = result.price;
                            item._Price = result.price.ToString("N0");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogException(ex);
                    Log("خطا در زمان مشخص کردن نتیجه استعلام قیمت  سرویس ها ", "");
                }
                //watch.Stop();
                //common.Log($"دریافت قیمت سرویس ها از سرور ها :" + watch.ElapsedMilliseconds, "");
                //Total += watch.ElapsedMilliseconds;
                //watch.Reset();
                //watch.Start();
                foreach (var item in data)
                {
                    try
                    {
                        if (item.CanSelect)
                        {

                            int Totalprice = 0;
                            if (dealerId == 0)
                            {
                                if (!new int[] { 654, 655, 660, 661, 662, 667, 670, 698, 697, 696, 695, 694, 693, 691, 690, 725, 726, 727 }.Contains(item.ServiceId))// بعدا برای یکسان سازی حق فنی مهندسی پست باید این فیلید برداشته شود
                                    Totalprice = _extendedShipmentService.getOrderTotalbyIncomeApiPrice(item.Price, 0, item.ServiceId, weightItem);
                                else
                                    Totalprice = item.Price;
                            }
                            else
                            {
                                if (new int[] { 654, 655, 660, 661, 662, 667, 670, 698, 697, 696, 695, 694, 693, 691, 690, 725, 726, 727 }.Contains(item.ServiceId))
                                    Totalprice = _extendedShipmentService.getOrderTotalbyIncomeApiPrice(item.CleanPrice, 0, item.ServiceId, weightItem, dealerId);
                                else
                                    Totalprice = _extendedShipmentService.getOrderTotalbyIncomeApiPrice(item.Price, 0, item.ServiceId, weightItem, dealerId);
                            }
                            Totalprice += ((item.hagheSabt) + (((item.hagheSabt) * 9) / 100));
                            item.Price = Totalprice;
                            item._Price = Totalprice.ToString("N0");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogException(ex);
                        Log("خطا در زمان اضافه کردن حق فنی مهندسی به سرویس  ها ", "");
                    }
                }
                //watch.Stop();
                //common.Log("دریافت قیمت ها از بانک :" + watch.ElapsedMilliseconds, "");
                //Total += watch.ElapsedMilliseconds;
                //watch.Reset();
                //common.Log("کل زمان :" + Total, "");
            }

            #region اضافه کردن قیمت پیشتاز به سفارش خارجی
            foreach (var item in data)
            {
                if (!(item.ServiceId == 707 || item.ServiceId == 719))
                    continue;
                int reciverStateId = item.ServiceId == 719 ? 581 : 582;
                int Internal_serviceId = 0;
                if (senderCountry != 1)
                {
                    Internal_serviceId = 714;// 730;//ماهکس
                }
                else
                {
                    Internal_serviceId = 723;//پست پیشتاز
                }
                var Pishtazdata = await GetServiceInfoAnonymouse(senderCountry, senderState, 1, reciverStateId, weightItem, AproximateValue, /*customerId,*/ height, length, width, consType,
                    content, null, null, null, null, 0, null, FromAPi, false, Internal_serviceId, IsFromAp, true, false, SenderAddress, ReciverAddress);

                if (Pishtazdata == null || Pishtazdata.Any(p => p.CanSelect == false))
                {
                    item.CanSelect = false;
                }
                else
                {
                    item.Price += Pishtazdata.First().Price;
                    item._Price = item.Price.ToString("N0");
                }
            }
            #endregion

            //#region اعمال تخفیف سفارش اولی ها
            //if (!(_workContext.CurrentCustomer.Id == 8186075 || _workContext.CurrentCustomer.AffiliateId == 1186))
            //{
            //    if (!IsFromSep && !IsFromAp && !isFromBidoc && !(_workContext.CurrentCustomer.IsInCustomerRole("mini-Administrators")))
            //    {
            //        if (IsFirstOrder || _extendedShipmentService.IsInValidDiscountPeriod())
            //        {
            //            var firstOrderDiscount = _extendedShipmentService.getFistOrderDiscount();
            //            if (firstOrderDiscount > 0)
            //            {
            //                foreach (var item in data)
            //                {
            //                    if (_extendedShipmentService.IsPostService(item.ServiceId))
            //                    {
            //                        item.message = item._Price;
            //                        item.Price -= (int)((item.Price * firstOrderDiscount) / 100);
            //                        item._Price = item.Price.ToString("N0");
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //if (IsFromSep)//سپ سامان
            //{
            //    foreach (var item in data)
            //    {
            //        if (_extendedShipmentService.IsPostService(item.ServiceId))// پست
            //        {
            //            item.message = item._Price;
            //            item.Price -= (int)((item.Price * 20) / 100);
            //            item._Price = item.Price.ToString("N0");
            //        }
            //        else if (item.ServiceId == 712 || item.ServiceId == 714 || item.ServiceId == 702)// چاپار و یارباکس
            //        {
            //            item.message = item._Price;
            //            item.Price -= (int)((item.Price * 5) / 100);
            //            item._Price = item.Price.ToString("N0");
            //        }
            //        else if (item.ServiceId == 707 || item.ServiceId == 710 || item.ServiceId == 719)//پست خارجی
            //        {
            //            item.message = item._Price;
            //            item.Price -= (int)((item.Price * 2) / 100);
            //            item._Price = item.Price.ToString("N0");
            //        }
            //    }
            //}
            ////else if (new DateTime(2021, 01, 19).CompareTo(DateTime.Now) > 0)// اتاق ایران اتریش
            ////{
            ////    foreach (var item in data)
            ////    {
            ////        if (_extendedShipmentService.IsPostService(item.ServiceId))
            ////        {
            ////            item.message = item._Price;
            ////            item.Price -= (int)((item.Price * 10) / 100);
            ////            item._Price = item.Price.ToString("N0");
            ////        }
            ////    }
            ////}
            //#endregion 
            //if (_workContext.CurrentCustomer.Id != 13653617)//اسفندی نماینده تبریز
            //{

            //    foreach (var item in data)
            //    {
            //        if (weightItem > 5000 && item.ServiceId == 654)
            //        {
            //            item.CanSelect = false;
            //            item._Price = "در وزن بیش از 5000 گرم(5 کیلو گرم) در سرویس پست بار فقط امکان ثبت سفارش به صورت پیشتاز وجود دارد";
            //            break;
            //        }
            //    }
            //}
            return data.Where(p => p.CanSelect == true).OrderBy(p => p.Price).ThenBy(p => p.SLA).ToList();
        }

        public bool isInvalidSernder(int senderCountry, int senderState)
        {
            return _extendedShipmentService.isInvalidSender(senderCountry, senderState);
        }

        public PlaceOrderResult ProccessOrder(List<NewCheckoutModel> orderList, int CustomerId)
        {
            if (orderList.Any(p => _extendedShipmentService.IsPostService(p.ServiceId)) && orderList.Any(p => !_extendedShipmentService.IsPostService(p.ServiceId)))
            {
                return new PlaceOrderResult()
                {
                    Errors = new List<string>() { "در هر سفارش از چند سرویس به صورت همزمان نمی توانید استفاده کنید" },
                    PlacedOrder = null
                };
            }
            var customer = _customerService.GetCustomerById(CustomerId);
            int stateProvianceId = 0;
            if (orderList.Any(p => !string.IsNullOrEmpty(p.UbbraTruckType)))
            {
                stateProvianceId = orderList[0].billingAddressModel.StateProvinceId.Value;
                orderList[0].billingAddressModel.StateProvinceId = null;
            }
            orderList[0].billingAddressModel = ProcessAddress(orderList[0], customer, "billingAddress");
            if (orderList.Any(p => !string.IsNullOrEmpty(p.UbbraTruckType)))
            {
                orderList[0].billingAddressModel.StateProvinceId = stateProvianceId;
            }
            if (customer.ShoppingCartItems.Any())
                CleanShopingCartItem(customer);

            var lstExShipments = new List<ExnShippmentModel>();
            string Str_ShippmentMethod = _shipingMethodRepository.Table.OrderBy(p => p.DisplayOrder).First().Name;
            int hagheSabt = 0;
            if (_storeContext.CurrentStore.Id == 5)
            {
                hagheSabt = _extendedShipmentService.CalcHagheSabet(_workContext.CurrentCustomer.Id, orderList.First().ServiceId, 0);
            }
            foreach (var orderItem in orderList)
            {
                int receiverStateId = 0;
                if (orderItem.receiver_ForeginCountry > 0)
                {
                    orderItem.shippingAddressModel.Address2 = orderItem.receiver_ForeginCountry + "|" + orderItem.receiver_ForeginCountryName;
                    if (!string.IsNullOrEmpty(orderItem.receiver_ForeginCityName))
                    {
                        orderItem.shippingAddressModel.Address2 += "|" + orderItem.receiver_ForeginCityName + "|" + (orderItem.receiver_ForeginCountryNameEn ?? "");
                    }
                }
                else if (!string.IsNullOrEmpty(orderItem.UbbraTruckType))
                {
                    receiverStateId = orderItem.shippingAddressModel.StateProvinceId.Value;
                    orderItem.shippingAddressModel.StateProvinceId = null;
                }
                else
                    orderItem.shippingAddressModel.Address2 = null;
                orderItem.shippingAddressModel = ProcessAddress(orderItem, customer, "shippingAddress");
                if (!string.IsNullOrEmpty(orderItem.UbbraTruckType))
                {
                    orderItem.shippingAddressModel.StateProvinceId = receiverStateId;
                }
                if (string.IsNullOrEmpty(orderItem.boxType) && _storeContext.CurrentStore.Id == 5 && orderItem.ServiceId != 701)
                {
                    return new PlaceOrderResult()
                    {
                        Errors = new List<string>() { "نوع مرسوله را در مرحله اول سفارش مشخص نمایید" },
                        PlacedOrder = null
                    };
                }
                var dectectedProduct = DetectProduct(orderItem.ServiceId);
                if (dectectedProduct == null)
                {
                    return new PlaceOrderResult()
                    {
                        Errors = new List<string>() { "درحال حاضر سرویس مورد نظر شما ارائه نمی شود" },
                        PlacedOrder = null
                    };
                }
                var product = _productService.GetProductById(dectectedProduct.ProductId);
                orderItem.IsCOD = dectectedProduct.IsCod;
                if (orderItem.AgentSaleAmount > 0 && !(customer.CustomerRoles.Any(p => p.Active && new int[] { 1, 7 }.Contains(p.Id))))
                {
                    orderItem.AgentSaleAmount = 0;
                }
                var checkoutAttributeXml =
                    getCheckoutAttributeXmlApi(orderItem, product.Id, hagheSabt);
                if (!string.IsNullOrEmpty(orderItem.UbbraTruckType))
                {
                    orderItem.shippingAddressModel.StateProvinceId = null;
                }


                for (int i = 0; i < orderItem.Count; i++)
                {
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
                        ShippmentAddressId = orderItem.shippingAddressModel.Id,
                        ShippmentMethod = Str_ShippmentMethod,
                    });
                }
            }//End Of For
            if (orderList.Any(p => !string.IsNullOrEmpty(p.UbbraTruckType)))
            {
                orderList[0].billingAddressModel.StateProvinceId = null;
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
                int TotalWeight = orderList.Sum(p => p.Weight * p.Count);
                float Weight_V = 0;
                foreach (var item in orderList)
                {
                    if (!(item.width.HasValue && item.length.HasValue && item.height.HasValue))
                        continue;
                    Weight_V += ((item.length.Value * item.width.Value * item.height.Value) / 6000) * 1000;
                }
                if (Weight_V > TotalWeight)
                    TotalWeight = Convert.ToInt32(Weight_V);
                //int HagheMagharPrice = CalcHagheMaghar(customer.BillingAddress, orderList[0].ServiceId, TotalWeight, _workContext.CurrentCustomer.Id);
                //if (HagheMagharPrice == 25000)
                //    orderList[0].discountCouponCode = "2500bdk";
                //else if (HagheMagharPrice == 30000)
                //    orderList[0].discountCouponCode = "3000bdk";
            }
            if (!string.IsNullOrEmpty(orderList[0].discountCouponCode))
            {
                List<string> Lst_Error = new List<string>();
                ApplyDiscountCoupon(customer, orderList[0].discountCouponCode, out Lst_Error);
                if (Lst_Error.Any())
                {
                    return new PlaceOrderResult()
                    {
                        Errors = Lst_Error,
                        PlacedOrder = null
                    };
                }
            }
            string selectedPaymentMethodSystemName = IsOrderCod(orderList[0].ServiceId) ? "Payments.CashOnDelivery" : "NopFarsi.Payments.AsanPardakht";
            var ppr = new ProcessPaymentRequest
            {
                CustomerId = CustomerId,
                StoreId = _storeContext.CurrentStore.Id,
                PaymentMethodSystemName = selectedPaymentMethodSystemName
            };
            var orderResult = _extnOrderProcessingService.PlaceOrderNewCheckOut(ppr, lstExShipments, orderList[0].SenderLat, orderList[0].SenderLon, orderList[0].IsFromAp, orderList[0].IsFromSep);
            if (orderResult.Success)
            {
                bool needCheck = false;
                var pishtazResult = RegisterPishtazForForginRequest(orderResult.PlacedOrder, orderList, out needCheck);
                if (needCheck)
                {
                    if (!pishtazResult.Success)
                    {
                        InsertOrderNote("سفارش پیشتاز برای پست خارجی ثبت نشد" + string.Join("\r\n", pishtazResult.Errors), orderResult.PlacedOrder.Id);
                    }
                    else
                    {
                        // int orderValue = getForginAddtionalValue(orderResult.PlacedOrder.Id);
                        orderResult.PlacedOrder.OrderTotal += pishtazResult.PlacedOrder.OrderTotal;
                        _orderService.UpdateOrder(orderResult.PlacedOrder);
                        int relatedorderPrice = Convert.ToInt32(pishtazResult.PlacedOrder.OrderTotal);
                        pishtazResult.PlacedOrder.OrderTotal = 0;
                        _orderService.UpdateOrder(pishtazResult.PlacedOrder);
                        InsertRelatedOrder(orderResult.PlacedOrder.Id, pishtazResult.PlacedOrder.Id, relatedorderPrice);
                        InsertOrderNote($"شماره سفارش پیشتاز متناظر با پست خارجی مورد نظر {pishtazResult.PlacedOrder.Id} می باشد", orderResult.PlacedOrder.Id);
                    }

                }
            }
            return orderResult;
        }
        public int CalcHagheMaghar(Address BillingAddress, int serviceId, int totalWeight, int customerId)
        {
            var categoryInfo = _extendedShipmentService.GetCategoryInfo(serviceId);
            if (!categoryInfo.HasHagheMaghar)
                return 0;
            return _extnOrderProcessingService.CheckAddressNeedHagheMaghar(BillingAddress, customerId, serviceId, totalWeight);
        }
        public PlaceOrderResult RegisterPishtazForForginRequest(Order order, List<NewCheckoutModel> orderList, out bool needCheck)
        {
            needCheck = false;
            if (!order.OrderItems.Any(p => p.Product.ProductCategories.Any(n => new int[] { 707, 719 }.Contains(n.CategoryId))))
            {
                needCheck = false;
                return null;
            }
            //if (!order.Customer.IsInCustomerRole("onlineGateway"))
            //{
            if (!_CheckRegionDesVijePost.CheckValidSourceForInternationalPost(order.BillingAddress.CountryId.Value, order.BillingAddress.StateProvinceId.Value))
            {
                return new PlaceOrderResult()
                {
                    Errors = new List<string>() { $"امکان ارسال پست خارجی از شهر {order.BillingAddress.StateProvince.Name} در حال حاضر وجود ندارد" },
                    PlacedOrder = null
                };
            }
            //}
            needCheck = true;
            var categoryId = order.OrderItems.Select(p => p.Product.ProductCategories.First().CategoryId).First();
            int reciverStateId = categoryId == 719 ? 581 : 582;
            string Addresss = (categoryId == 719 ? "خیابان آزادی، انتهای خیابان اسکندری شمالی-شماره 144- شرکت پستی راه آسمان آبی" : "بزرگراه حقانی غرب بین دیدار شمالی و 4 راه جهان کودک، پلاک -35 -شرکت پستی پی دی ای");
            string phoneNumber = (categoryId == 719 ? "09331473290" : "09050587273");
            string FirstName = (categoryId == 719 ? "شرکت" : "شرکت");
            string LastName = (categoryId == 719 ? "راه آسمان آبی" : "پست پی دی ای");
            List<NewCheckoutModel> newOrderList = new List<NewCheckoutModel>();

            int Internal_serviceId = (new int[] { 4, 579, 580, 581, 582, 583, 584, 585, 779, 122 }.Contains(order.BillingAddress.StateProvinceId.Value)) ? 655 : 662;

            foreach (var item in orderList)
            {
                var newItem = item;
                newItem.ServiceId = Internal_serviceId;
                newItem.receiver_ForeginCityName = null;
                newItem.receiver_ForeginCountry = 0;
                newItem.receiver_ForeginCountryName = null;
                newItem.shippingAddressModel = (Address)item.shippingAddressModel.Clone();
                newItem.shippingAddressModel.Id = 0;
                newItem.shippingAddressModel.Address1 = Addresss;
                newItem.shippingAddressModel.FirstName = FirstName;
                newItem.shippingAddressModel.LastName = LastName;
                newItem.shippingAddressModel.CountryId = 1;
                newItem.shippingAddressModel.StateProvinceId = reciverStateId;
                newItem.shippingAddressModel.PhoneNumber = phoneNumber;
                newOrderList.Add(newItem);
            }
            var pishtazResult = ProccessOrder(newOrderList, order.CustomerId);
            return pishtazResult;
        }
        public void InsertRelatedOrder(int orderId1, int OrderId2, int relatedOrderPrice)
        {
            string Query = $@"INSERT INTO dbo.Tb_RelatedOrders
                    (
	                    ParentOrderId
	                    , ChildOrderId
                        , ChildOrderPrice
                    )
                    VALUES
                    (	{orderId1} -- ParentOrderId - int
	                    , {OrderId2} -- ChildOrderId - int
                        , {relatedOrderPrice}
	                ) SELECT @@ROWCOUNT ";
            _dbContext.SqlQuery<int>(Query, new object[0]).FirstOrDefault();
        }
        public bool canAddValueToForginRequest(int parentOrderId)
        {
            var order = _orderService.GetOrderById(parentOrderId);
            int orderValue = getForginAddtionalValue(parentOrderId);
            int childOrderId = getRelatedOrder(parentOrderId);

            string ToatalPrice = _genericAttributeService.GetAttributesForEntity(parentOrderId, "Order").FirstOrDefault(p => p.Key == "AddValueToForginRequest"
            && p.StoreId == order.StoreId)?.Value;
            if (string.IsNullOrEmpty(ToatalPrice))
            {
                _genericAttributeService.SaveAttribute<string>(order, "AddValueToForginRequest", orderValue.ToString(), order.StoreId);
                return true;
            }
            return false;
        }
        public PlaceOrderResult ProccessWalletOrder(int Amount, string paymentMethodName, int CustomerId)
        {
            var customer = _customerService.GetCustomerById(CustomerId);
            if (customer.BillingAddress == null)
            {
                customer.BillingAddress = new Address()
                {
                    Address1 = "-",
                    CountryId = 1,
                    CreatedOnUtc = DateTime.Now.ToUniversalTime(),
                    Email = customer.Email,
                    FirstName = "-",
                    LastName = "-",
                    PhoneNumber = customer.Username,
                    StateProvinceId = 580,
                    ZipPostalCode = "1311111111",
                };
                _customerService.UpdateCustomer(customer);
                return new PlaceOrderResult()
                {
                    Errors = new List<string>() { " ابتدا یک سفارش  پستی ثبت کنید و سپس اقدام به شارژ کیف پول کنید" },
                    PlacedOrder = null
                };
            }
            if (customer.ShoppingCartItems.Any())
                CleanShopingCartItem(customer);
            var setting =
                _settingService.GetSetting("NopMaster.Wallet_ProductId", _storeContext.CurrentStore.Id, false);
            int productId = setting == null ? 0 : int.Parse(setting.Value);
            var product = _productService.GetProductById(productId);
            if (product == null)
            {
                return new PlaceOrderResult()
                {
                    Errors = new List<string>() { "در حال حاضر امکان شارژ کیف پول وجود ندارد.با پشتیبای سامانه تماس بگیرید" },
                    PlacedOrder = null
                };
            }
            var checkoutAttributeXml = "";
            var warnings = _shoppingCartService.AddToCart(customer, product, ShoppingCartType.ShoppingCart,
                         _storeContext.CurrentStore.Id, checkoutAttributeXml,
                         Amount, automaticallyAddRequiredProductsIfEnabled: false);
            if (warnings.Any())
            {
                return new PlaceOrderResult()
                {
                    Errors = warnings,
                    PlacedOrder = null
                };
            }
            string selectedPaymentMethodSystemName = paymentMethodName;
            var ppr = new ProcessPaymentRequest
            {
                CustomerId = CustomerId,
                StoreId = _storeContext.CurrentStore.Id,
                PaymentMethodSystemName = selectedPaymentMethodSystemName
            };
            return _extnOrderProcessingService.PlaceOrderWallet(ppr);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Amount"></param>
        /// <param name="paymentMethodName"></param>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public PlaceOrderResult ProccessCartonOrder(CartonSaleModel _model)
        {
            if (_workContext.CurrentCustomer.BillingAddress == null)
            {
                _workContext.CurrentCustomer.BillingAddress = new Address()
                {
                    Address1 = "-",
                    Email = _workContext.CurrentCustomer.Email,
                    FirstName = "-",
                    LastName = "-",
                    CreatedOnUtc = DateTime.Now.ToUniversalTime()
                };
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                //return new PlaceOrderResult()
                //{
                //    Errors = new List<string>() { "شماره سفارش پستی که وارد کردید مربوط به حساب کاربری شما نمی باشد" },
                //    PlacedOrder = null
                //};
            }
            if (_workContext.CurrentCustomer.ShoppingCartItems.Any())
                CleanShopingCartItem(_workContext.CurrentCustomer);

            var product = _productService.GetProductById(10430);
            if (product == null)
            {
                return new PlaceOrderResult()
                {
                    Errors = new List<string>() { "در حال حاضر امکان خرید کارتن و لفاف وجود ندارد.با پشتیبای سامانه تماس بگیرید" },
                    PlacedOrder = null
                };
            }
            foreach (var item in _model.List_Sizeitem)
            {
                for (int i = 0; i < item.Count; i++)
                {
                    var checkoutAttributeXml = getKartonCheckoutAttributeXml(item.Name, _model.OrderId, _model.ShipmentId, product.Id);
                    var warnings = _shoppingCartService.AddToCart(_workContext.CurrentCustomer, product, ShoppingCartType.ShoppingCart,
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
                }
            }
            string selectedPaymentMethodSystemName = _model.paymentmethod;
            var ppr = new ProcessPaymentRequest
            {
                CustomerId = _workContext.CurrentCustomer.Id,
                StoreId = _storeContext.CurrentStore.Id,
                PaymentMethodSystemName = "NopFarsi.Payments.AsanPardakht"
            };
            return _extnOrderProcessingService.PlaceOrderCarton(ppr);
        }
        public CategoryInfoModel getCategoryInfo(Order order)
        {
            try
            {
                var serviceId = order.OrderItems.First().Product.ProductCategories.First().CategoryId;
                return _extendedShipmentService.GetCategoryInfo(serviceId);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public bool IsOrderCod(int serviceId)
        {
            var data = _extendedShipmentService.GetCategoryInfo(serviceId);
            if (data != null && data.IsCod)
            {
                return true;
            }
            return false;
        }
        public Address ProcessAddress(NewCheckoutModel model, Customer customer, string type)
        {
            Address address = null;
            float? Lat = null;
            float? Lon = null;
            if (type == "billingAddress")
            {
                address = model.billingAddressModel;
                Lat = Convert.ToSingle(model.SenderLat);
                Lon = Convert.ToSingle(model.SenderLon);
            }
            else
            {
                address = model.shippingAddressModel;
                Lat = Convert.ToSingle(model.ReciverLat);
                Lon = Convert.ToSingle(model.ReciverLon);
            }
            #region Check Customer Address Exist
            SqlParameter P_CustomerId = new SqlParameter()
            {
                ParameterName = "CustomerId",
                SqlDbType = SqlDbType.Int,
                Value = (object)customer.Id
            };
            SqlParameter P_CountryId = new SqlParameter()
            {
                ParameterName = "CountryId",
                SqlDbType = SqlDbType.Int,
                Value = (object)address.CountryId ?? (object)DBNull.Value
            };
            SqlParameter P_StateId = new SqlParameter()
            {
                ParameterName = "StateId",
                SqlDbType = SqlDbType.Int,
                Value = (object)address.StateProvinceId ?? (object)DBNull.Value
            };
            SqlParameter P_FirstName = new SqlParameter()
            {
                ParameterName = "FirstName",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)address.FirstName ?? ""
            };
            SqlParameter P_LastName = new SqlParameter()
            {
                ParameterName = "LastName",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)address.LastName ?? ""
            };
            SqlParameter P_PhoneNumber = new SqlParameter()
            {
                ParameterName = "PhoneNumber",
                SqlDbType = SqlDbType.VarChar,
                Value = (object)address.PhoneNumber ?? ""
            };
            SqlParameter P_Address = new SqlParameter()
            {
                ParameterName = "Address",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)address.Address1 ?? ""
            };
            SqlParameter P_Company = new SqlParameter()
            {
                ParameterName = "Company",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)address.Company ?? (object)DBNull.Value
            };
            SqlParameter P_Address2 = new SqlParameter()
            {
                ParameterName = "Address2",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)address.Address2 ?? (object)DBNull.Value
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
            if (AddressId == 0)
            {
                address.CreatedOnUtc = DateTime.UtcNow;
                address.CustomAttributes = "";
                if (string.IsNullOrEmpty(address.ZipPostalCode) || address.ZipPostalCode.Length != 10)
                {
                    if (address.StateProvinceId.HasValue)
                        address.ZipPostalCode = getZipCode(address.StateProvinceId.Value);
                }
                customer.Addresses.Add(address);
            }
            else
            {
                address = _addressService.GetAddressById(AddressId);
            }
            #endregion           
            if (type == "billingAddress")
                customer.BillingAddress = address;
            else if (type == "shippingAddress")
            {
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                customer.ShippingAddress = address;
            }
            _customerService.UpdateCustomer(customer);
            if (Lat.HasValue && Lon.HasValue)
            {
                _extendedShipmentService.InsertAddressLocation(address.Id, Lat.Value, Lon.Value);
            }
            return address;
        }
        public Address ProcessAddress(Address address, Customer customer, string type)
        {
            #region Check Customer Address Exist
            SqlParameter P_CustomerId = new SqlParameter()
            {
                ParameterName = "CustomerId",
                SqlDbType = SqlDbType.Int,
                Value = (object)customer.Id
            };
            SqlParameter P_CountryId = new SqlParameter()
            {
                ParameterName = "CountryId",
                SqlDbType = SqlDbType.Int,
                Value = (object)address.CountryId
            };
            SqlParameter P_StateId = new SqlParameter()
            {
                ParameterName = "StateId",
                SqlDbType = SqlDbType.Int,
                Value = (object)address.StateProvinceId
            };
            SqlParameter P_FirstName = new SqlParameter()
            {
                ParameterName = "FirstName",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)address.FirstName ?? ""
            };
            SqlParameter P_LastName = new SqlParameter()
            {
                ParameterName = "LastName",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)address.LastName ?? ""
            };
            SqlParameter P_PhoneNumber = new SqlParameter()
            {
                ParameterName = "PhoneNumber",
                SqlDbType = SqlDbType.VarChar,
                Value = (object)address.PhoneNumber ?? ""
            };
            SqlParameter P_Address = new SqlParameter()
            {
                ParameterName = "Address",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)address.Address1 ?? ""
            };
            SqlParameter P_Company = new SqlParameter()
            {
                ParameterName = "Company",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)address.Company ?? (object)DBNull.Value
            };
            SqlParameter P_Address2 = new SqlParameter()
            {
                ParameterName = "Address2",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)address.Address2 ?? (object)DBNull.Value
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
            if (AddressId == 0)
            {
                address.CreatedOnUtc = DateTime.UtcNow;
                address.CustomAttributes = "";
                if (string.IsNullOrEmpty(address.ZipPostalCode) || address.ZipPostalCode.Length != 10)
                {
                    address.ZipPostalCode = getZipCode(address.StateProvinceId.Value);
                }
                customer.Addresses.Add(address);
            }
            else
            {
                address = _addressService.GetAddressById(AddressId);
            }
            #endregion           
            if (type == "billingAddress")
                customer.BillingAddress = address;
            else if (type == "shippingAddress")
                customer.ShippingAddress = address;
            _customerService.UpdateCustomer(customer);
            return address;
        }

        public int InsertCustomerAddress(Address model, int CustomerId)
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
            return StateCode + new string((StateCode == "11" ? '0' : '1'), count);
        }
        private void CleanShopingCartItem(Customer customer)
        {
            while (customer.ShoppingCartItems.Any())
            {
                customer.ShoppingCartItems.ToList().ForEach(sci => _shoppingCartService.DeleteShoppingCartItem(sci, false));
            }
        }
        public DectectProductModel DetectProduct(int serviceType)
        {
            int storeId = _storeContext.CurrentStore.Id;
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
	                            TOP(1) P.Id as ProductId,
	                            TCI.Id
	                            , TCI.CategoryId
	                            , TCI.IsCod
	                            , TCI.IsPishtaz
	                            , TCI.IsSefareshi
	                            , TCI.IsVIje
	                            , TCI.IsNromal
	                            , TCI.IsDroonOstani
	                            , TCI.IsAdjoining
	                            , TCI.IsNotAdjacent
	                            , TCI.IsHeavyTransport
	                            , TCI.IsForeign
	                            , TCI.IsInCity
	                            , TCI.IsAmanat 
                            FROM
	                            dbo.Category AS C
	                            INNER JOIN dbo.Product_Category_Mapping AS PCM ON PCM.CategoryId = C.Id
	                            INNER JOIN dbo.Product AS P ON P.Id = PCM.ProductId
	                            INNER JOIN dbo.StoreMapping AS SM ON sm.EntityId = C.Id AND sm.EntityName ='Category'
	                            INNER JOIN dbo.Tb_CategoryInfo AS TCI ON TCI.CategoryId = PCM.CategoryId
                            WHERE	
	                            C.ParentCategoryId <> 0
                                AND C.Deleted = 0
	                            AND C.Published = 1
	                            AND SM.StoreId = {storeId}
	                            AND p.Deleted= 0
	                            AND p.Published = 1
	                            AND C.Id = @CategoryId";
            return _dbContext.SqlQuery<DectectProductModel>(query, prms).FirstOrDefault();
        }
        private string getCheckoutAttributeXmlApi(NewCheckoutModel item, int productId, int hagheSabt)
        {
            var attributeByValueTofetch = $"N'{item.CartonSizeName}',N'{item.InsuranceName}'";
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

            if (item.RequestPrintAvatar)
                propertyFetcher += " OR (pa.Name LIKE N'%چاپ نشان تجاری من بر روی فاکتور%' AND PAV.Name LIKE N'%بلی%') ";
            else
                propertyFetcher += " OR (pa.Name LIKE N'%چاپ نشان تجاری من بر روی فاکتور%' AND PAV.Name LIKE N'%خیر%') ";



            if (item.hasNotifRequest)
                propertyFetcher += " OR (pa.Name LIKE N'%اطلاع رسانی پیامکی%' AND PAV.Name LIKE N'%بلی%') ";
            else
                propertyFetcher += " OR (pa.Name LIKE N'%اطلاع رسانی پیامکی%' AND PAV.Name LIKE N'%خیر%') ";

            if (_extendedShipmentService.IsPostService(item.ServiceId))
                propertyFetcher += " OR (pa.Name LIKE N'%تضمین غرامت پست%' AND PAV.Name LIKE N'%هزینه%') ";

            if (!string.IsNullOrEmpty(item.boxType))
                propertyFetcher += " OR (pa.Name LIKE N'%نوع مرسوله%' AND PAV.Name LIKE N'%" + item.boxType + "%') ";

            string caseWhen = " WHEN pa.Name LIKE N'%نوع و وزن%' THEN ISNULL(CAST(pav.Id AS VARCHAR(10)),'GoodsType') ";
            if (!string.IsNullOrEmpty(item.tehranCityArea))
            {
                caseWhen += " WHEN pa.Name LIKE N'منطقه شهرداری' THEN ISNULL(CAST(pav.Id AS VARCHAR(50)),'_tca') ";
                propertyFetcher += "OR (pa.Name LIKE N'منطقه شهرداری')";
            }
            if (item.isInCityArea)
                propertyFetcher += " OR (pa.Name LIKE N'%آیا منطقه شهرداری است؟%' AND PAV.Name LIKE N'%بلی%') ";
            else
                propertyFetcher += " OR (pa.Name LIKE N'%آیا منطقه شهرداری است؟%' AND PAV.Name LIKE N'%خیر%') ";

            if (item.trafficArea)
                propertyFetcher += " OR (pa.Name LIKE N'%نیاز به طرح ترافیک؟%' AND PAV.Name LIKE N'%بلی%') ";
            else
                propertyFetcher += " OR (pa.Name LIKE N'%نیاز به طرح ترافیک؟%' AND PAV.Name LIKE N'%خیر%') ";

            if (!string.IsNullOrEmpty(item.collectorArea))
            {
                caseWhen += " WHEN pa.Name LIKE N'%محدوده جمع آوری%' THEN ISNULL(CAST(pav.Id AS VARCHAR(20)),'collectorArea') ";
                propertyFetcher += " OR (pa.Name LIKE N'%محدوده جمع آوری%')";
            }
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
            var query = @"DECLARE @xml XML =(SELECT
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
                        ORDER BY pa.id desc
                        FOR XML PATH('ProductAttribute'),ROOT('Attributes'), ELEMENTS) 
                        SELECT CAST(@xml AS NVARCHAR(MAX)) as _xml";
            //Log("query2", query);
            string xml = _dbContext.SqlQuery<string>(query, new object[0]).FirstOrDefault();
            if (item.CodGoodsPrice != null)
                xml = xml.Replace("CODValue", item.CodGoodsPrice.ToString());
            if (!string.IsNullOrEmpty(item.collectorArea))
                xml = xml.Replace("collectorArea", item.collectorArea);
            if (item.AgentSaleAmount != 0)
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
                xml = xml.Replace("_SenderStateId", item.billingAddressModel.StateProvinceId.Value.ToString());
                xml = xml.Replace("_ReciverStateId", item.shippingAddressModel.StateProvinceId.Value.ToString());
            }
            xml = xml.Replace("ApproximateValue", item.ApproximateValue.ToString());
            xml = xml.Replace("_tca", item.tehranCityArea);
            xml = xml.Replace("GoodsType", string.IsNullOrEmpty(item.GoodsType) ? "-" : item.GoodsType);// + " به ارزش" + item.ApproximateValue.ToString() + " ريال ");
            return xml;
        }
        private string getKartonCheckoutAttributeXml(string CartonSizeName, int RelatedOrderId, int RelatedShipment, int productId)
        {
            string propertyFetcher = $" (pa.Name LIKE N'%سایز%' AND PAV.Name LIKE N'%{CartonSizeName}%') ";
            propertyFetcher += $" OR (pa.Name LIKE N'%شماره سفارش%')";
            propertyFetcher += $" OR (pa.Name LIKE N'%شماره محموله%')";
            string caseWhen = $" WHEN pa.Name LIKE N'%شماره سفارش%' THEN ISNULL(CAST(pav.Id AS VARCHAR(10)),'{RelatedOrderId.ToString()}') ";
            caseWhen += $" WHEN pa.Name LIKE N'%شماره محموله%' THEN ISNULL(CAST(pav.Id AS VARCHAR(10)),'{RelatedShipment.ToString()}') ";

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
	                        AND (" + propertyFetcher + @")
                        FOR XML PATH('ProductAttribute'),ROOT('Attributes'), ELEMENTS";
            Log("kartonXmlQuery", query);
            string xml = _dbContext.SqlQuery<string>(query, new object[0]).FirstOrDefault();
            Log("kartonXml", xml);
            return xml;
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
        public List<OrderItemProperty> getOrdersItem(int orderItemId, bool IsCod)
        {
            SqlParameter P_Orderid = new SqlParameter()
            {
                ParameterName = "orderItemId",
                SqlDbType = SqlDbType.Int,
                Value = (object)orderItemId
            };
            SqlParameter P_IsCod = new SqlParameter()
            {
                ParameterName = "IsCod",
                SqlDbType = SqlDbType.Bit,
                Value = (object)IsCod
            };
            SqlParameter[] prms = new SqlParameter[]
            {
                P_Orderid,
                P_IsCod
            };
            string Query = @"EXEC dbo.Sp_getOrderItemFactorItem @orderItemId,@IsCod";
            return _dbContext.SqlQuery<OrderItemProperty>(Query, prms).ToList();
        }
        public BillAndPaymentModel GetFactorModel(List<int> OrderIds, bool safebuy = false)
        {
            BillAndPaymentModel BAPM = new BillAndPaymentModel();
            BAPM.FactorItems = new List<FactorItem>();

            foreach (var orderId in OrderIds)
            {
                var order = _orderService.GetOrderById(orderId);
                bool isForeign = order.IsOrderForeign();
                int relatedOrderPrice = 0;
                if (new int[] { 719, 707 }.Contains(order.OrderItems.First().Product.ProductCategories.First().CategoryId))
                {
                    int _relatedOrderPrice = getRelatedOrderPrice(order.Id);
                    if (_relatedOrderPrice > 0)
                        relatedOrderPrice = (_relatedOrderPrice / order.OrderItems.Select(p => p.Quantity).Sum());
                }
                if (order.Customer.Id != _workContext.CurrentCustomer.Id)
                    continue;
                var RegistrationMethod = _extendedShipmentService.GetOrderRegistrationMethod(order);
                bool isFromAP = RegistrationMethod == OrderRegistrationMethod.Ap || RegistrationMethod == OrderRegistrationMethod.bidok;
                int itemDiscount = 0;
                if (order.OrderDiscount > decimal.Zero)
                {
                    itemDiscount = Convert.ToInt32(order.OrderDiscount / (order.OrderItems.Sum(p => p.Quantity)));
                }
                int HagheMaghar = 0;
                int _posthagheMaghar = 0;
                foreach (var item in order.OrderItems)
                {
                    int ServiceId = item.Product.ProductCategories.First().CategoryId;
                    HagheMaghar = _extendedShipmentService.getHagheMaghar(item.Id, order.BillingAddress.CountryId.Value, ServiceId, out _posthagheMaghar);
                    if (HagheMaghar > 0)
                    {
                        HagheMaghar -= _posthagheMaghar;
                        HagheMaghar = HagheMaghar / order.OrderItems.Sum(p => p.Quantity);
                        break;
                    }
                }
                bool hasAgentAmountRule = _agentAmountRuleService.IsCustmoerInAgentRole(order.CustomerId);
                foreach (var item in order.OrderItems)
                {
                    int ServiceId = item.Product.ProductCategories.First().CategoryId;
                    Shipment shipment = null;
                    Address ShippingAddress = null;
                    int _discountByRulePrice = 0;
                    if (hasAgentAmountRule)
                    {
                        AgentAmountRuleService.PrivatePostDiscount postDiscount;
                        var discountByRule = _agentAmountRuleService.getInlineAgentSaleAmount(item, out postDiscount);
                        if (discountByRule != null && discountByRule.Price > 0)
                        {
                            _discountByRulePrice = discountByRule.Price / item.Quantity;
                        }
                    }
                    for (int i = 0; i < item.Quantity; i++)
                    {
                        var MultiShipmentData = _extendedShipmentService.getShipmentFromMultiShipment(item, i);
                        shipment = MultiShipmentData.shipment;
                        ShippingAddress = _addressService.GetAddressById(MultiShipmentData.ShipmentAddressId);

                        FactorItem FItem = new FactorItem();
                        FItem.IsForeign = isForeign;
                        FItem.CreateDate = order.CreatedOnUtc.ToLocalTime();
                        FItem.BillingAddress = order.BillingAddress;
                        FItem.ShippingAddress = ShippingAddress;

                        if (!string.IsNullOrEmpty(shipment.TrackingNumber))
                        {
                            FItem.TrackingNumber = shipment.TrackingNumber;
                            FItem.Base64TrackingNumber = _extendedShipmentService.GetBase64Image(FItem.TrackingNumber);
                        }
                        int approximateValue = _extendedShipmentService.getApproximateValue(item.Id);
                        FItem.OrderTotal = ((getShipmentFactorPrice(shipment.Id, isFromAP)) + relatedOrderPrice + (safebuy ? approximateValue : 0)) - itemDiscount;// getOrderTotal(orderId) + AdditionalValueForFforeginRequest;
                        FItem.OrderId = orderId;
                        FItem.discountbyRule = _discountByRulePrice;
                        FItem.ShipmentId = shipment.Id;
                        FItem.IsCod = (order.PaymentMethodSystemName == "Payments.CashOnDelivery");
                        var orderItemsAttr = getOrdersItem(item.Id, FItem.IsCod);
                        FItem.PostItems = new List<BillItems>();
                        FItem.hasPostPriceTax = !isFromAP;// true;// برای تعارف باید این اصلاح شود
                        var isPostService = _extendedShipmentService.IsPostService(ServiceId);
                        if (!isPostService)
                        {
                            FItem.hasPostPriceTax = false;
                        }
                        else
                        {
                            FItem.hasPostPriceTax = true;
                        }
                        var prices = _extendedShipmentService.getEngAndPostPrice(order.OrderItems.First());
                        if (!new int[] { 667, 670, 722, 723 }.Contains(ServiceId))
                        {
                            if (!isPostService || isFromAP)
                            {
                                if (prices != null)
                                {
                                    FItem.PostItems = orderItemsAttr.Where(p => p.ISForPost == true).Select(p => new BillItems()
                                    {
                                        Count = 1,
                                        Price = p.PropertyAttrName.Contains("وزن بسته") ? (prices.IncomePrice + relatedOrderPrice) : p.PropertyAttrValueCost.GetValueOrDefault(0),
                                        description = p.PropertyAttrName.Contains("وزن بسته") ? p.ProductName + " " + p.PropertyAttrValueName :
                                    (p.PropertyAttrValueName.Contains("هزینه") ? p.PropertyAttrName : p.PropertyAttrValueName),
                                    }).ToList();
                                    if (_posthagheMaghar > 0)
                                    {
                                        FItem.PostItems.Add(new BillItems()
                                        {
                                            Count = 1,
                                            Price = _posthagheMaghar,
                                            description = "حق مقر"
                                        });
                                        _posthagheMaghar = 0;
                                    }
                                }
                                else
                                {
                                    FItem.PostItems = new List<BillItems>();
                                }
                            }
                            else
                            {
                                FItem.PostItems = orderItemsAttr.Where(p => p.ISForPost == true).Select(p => new BillItems()
                                {
                                    Count = 1,
                                    Price = p.PropertyAttrValueCost.GetValueOrDefault(0),
                                    description = p.PropertyAttrName.Contains("وزن بسته") ? p.ProductName + " " + p.PropertyAttrValueName :
                                (p.PropertyAttrValueName.Contains("هزینه") ? p.PropertyAttrName : p.PropertyAttrValueName),
                                }).ToList();
                                if (_posthagheMaghar > 0)
                                {
                                    FItem.PostItems.Add(new BillItems()
                                    {
                                        Count = 1,
                                        Price = _posthagheMaghar,
                                        description = "حق مقر"
                                    });
                                    _posthagheMaghar = 0;
                                }
                            }
                        }
                        else
                        {
                            var CodPrices = getCodFactorsItem(shipment.Id);
                            if (CodPrices != null)
                            {
                                FItem.OrderTotal = CodPrices.CodCost + (CodPrices.CodCost * 9 / 100)
                                        + CodPrices.AttrAndEngPrice + (CodPrices.AttrAndEngPrice * 9 / 100)
                                        + CodPrices.CodGodsPrice + (safebuy ? approximateValue : 0);
                                FItem.OrderTotal -= itemDiscount;
                            }
                            FItem.hasPostPriceTax = true;
                            FItem.PostItems.Add(new BillItems()
                            {
                                Count = 1,
                                Price = CodPrices.CodCost,
                                description = "هزینه پستی",
                            });
                            //FItem.PostItems.Add(new BillItems()
                            //{
                            //    Count = 1,
                            //    Price = Convert.ToInt32(CodPrices.CodCost * 9 / 100),
                            //    description = "مالیات بر ارزش افزوده",
                            //});
                            FItem.EngItems = new List<BillItems>();
                            FItem.EngItems.Add(new BillItems()
                            {
                                Count = 1,
                                Price = CodPrices.AttrAndEngPrice + (safebuy ? approximateValue : 0),
                                description = "خدمات فنی، بیمه و جمع آوری کالا از محل و...",
                            });
                            if (CodPrices.CodGodsPrice > 0)
                            {
                                FItem.EngItems.Add(new BillItems()
                                {
                                    Count = 1,
                                    description = "هزینه کالا",
                                    Price = CodPrices.CodGodsPrice
                                });
                            }
                            FItem.EngItems.Add(new BillItems()
                            {
                                Count = 1,
                                Price = Convert.ToInt32(CodPrices.AttrAndEngPrice * 9 / 100),
                                description = "مالیات بر ارزش افزوده",
                            });
                        }
                        int j = 0;
                        foreach (var rowItem in FItem.PostItems)
                        {
                            j++;
                            rowItem.RowNumber = j;
                        }
                        if (!new int[] { 667, 670, 722, 723 }.Contains(ServiceId))
                        {
                            FItem.EngItems = new List<BillItems>();

                            FItem.EngItems.Add(new BillItems()
                            {
                                Count = 1,
                                Price = orderItemsAttr.Select(p => (p.PropertyAttrName.Contains("وزن بسته") && prices != null) ? prices.EngPrice : p.EngPrice).Sum(p => p)
                                + HagheMaghar + _extendedShipmentService.getHagheSabt(order) + (safebuy ? approximateValue : 0),
                                description = "خدمات فنی، بیمه و جمع آوری کالا از محل و..."
                            });
                        }
                        if (itemDiscount > 0)
                        {
                            FItem.EngItems.Add(new BillItems()
                            {
                                Count = 1,
                                Price = itemDiscount,
                                description = "تخفیف"
                            });
                        }
                        FItem.GoodsType = orderItemsAttr.Where(p => p.PropertyAttrName.Contains("لطفا نوع و وزن کالا را در باکس زیر وارد کنید.")
                        && !string.IsNullOrEmpty(p.PropertyAttrValueText)).FirstOrDefault()?.PropertyAttrValueText;
                        FItem.Weight = orderItemsAttr.Select(p => p.PropertyAttrValueWeight).Sum();

                        BAPM.FactorItems.Add(FItem);
                        BAPM.PaymentMethods = getPaymentMethod(order);
                    }
                }
            }
            BAPM.SafeBuy = safebuy;
            return BAPM;
        }
        /// <summary>
        /// دریافت اطلاعات قیمتی محموله پس کرایه
        /// </summary>
        /// <param name="shipmentId"></param>
        /// <returns></returns>
        public CodFactorItem getCodFactorsItem(int shipmentId)
        {
            string query = $@"SELECT
	                    sa.CodCost,
	                    ((ISNULL(dbo.GetOnlyNumbers(TOIR.GoodsCodPrice),0) - ISNULL(sa.CodBmValue,0)) * -1) AttrAndEngPrice,
	                    ISNULL(dbo.GetOnlyNumbers(TOIR.GoodsCodPrice),0) CodGodsPrice
                    INTO #tb1
                    FROM
	                    dbo.Shipment AS S
	                    INNER JOIN dbo.ShipmentAppointment AS SA ON SA.ShipmentId = S.Id
	                    INNER JOIN dbo.ShipmentItem AS SI ON SI.ShipmentId = S.Id
	                    INNER JOIN dbo.OrderItem AS OI ON OI.Id= SI.OrderItemId
	                    INNER JOIN dbo.Tb_OrderItemsRecord AS TOIR ON TOIR.OrderItemId = OI.Id
                    WHERE
	                    S.Id = {shipmentId} 

                    SELECT
	                    T.CodCost
	                    , CAST(T.AttrAndEngPrice*100 / 109 AS INT) AttrAndEngPrice
	                    , T.CodGodsPrice
                    FROM
	                    #tb1 AS T";
            return _dbContext.SqlQuery<CodFactorItem>(query, new object[0]).FirstOrDefault();
        }
        public class CodFactorItem
        {
            public int CodCost { get; set; }
            public int AttrAndEngPrice { get; set; }
            public int CodGodsPrice { get; set; }
        }
        public List<InvoiceDetails> Sh_GetFactorModel(List<int> OrderIds)
        {
            List<InvoiceDetails> lst_InvoiceDetails = new List<InvoiceDetails>();
            foreach (var item in OrderIds)
            {
                var order = _orderService.GetOrderById(item);
                if (order.Customer.Id != _workContext.CurrentCustomer.Id)
                    continue;
                SqlParameter P_Orderid = new SqlParameter()
                {
                    ParameterName = "OrderId",
                    SqlDbType = SqlDbType.Int,
                    Value = (object)item
                };
                SqlParameter[] prms = new SqlParameter[]
                {
                P_Orderid
                };

                string Query = @"EXEC dbo.Sp_OrderBillDetails_Order @orderId";
                var result = _dbContext.SqlQuery<InvoiceDetails>(Query, prms);
                if (result != null && result.Any())
                {
                    lst_InvoiceDetails.AddRange(result);
                }
            }
            return lst_InvoiceDetails;
        }
        public int getShipmentFactorPrice(int shipmentId, bool IsFormAp)
        {
            string query = $@"EXEC dbo.Sp_ShipmentFactorPrice @ShipmentId, @IsFromAp";
            SqlParameter P_shipmentId = new SqlParameter()
            {
                ParameterName = "shipmentId",
                SqlDbType = SqlDbType.Int,
                Value = (object)shipmentId
            };
            SqlParameter P_IsFormAp = new SqlParameter()
            {
                ParameterName = "IsFromAp",
                SqlDbType = SqlDbType.Bit,
                Value = (object)IsFormAp
            };
            SqlParameter[] prms = new SqlParameter[]
            {
                P_shipmentId,
                P_IsFormAp
            };
            return _dbContext.SqlQuery<int?>(query, prms).FirstOrDefault().GetValueOrDefault(0);
        }
        public int getForginAddtionalValue(int orderId)
        {
            string query = $@"SELECT
	                            CAST(O.OrderTotal AS INT) OrderTotal
                            FROM
	                            dbo.Tb_RelatedOrders AS TRO
	                            INNER JOIN dbo.[Order] AS O ON O.Id = TRO.ChildOrderId
                            WHERE
	                            TRO.ParentOrderId = " + orderId;
            return _dbContext.SqlQuery<int?>(query, new object[0]).SingleOrDefault().GetValueOrDefault(0);
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
        public int getRelatedOrderPrice(int ParentOrderId)
        {
            string query = $@"SELECT
	                           ISNULL(ChildOrderPrice,0) ChildOrderPrice
                            FROM
	                            dbo.Tb_RelatedOrders AS TRO
                            WHERE
	                            TRO.ParentOrderId = " + ParentOrderId;
            return _dbContext.SqlQuery<int?>(query, new object[0]).SingleOrDefault().GetValueOrDefault(0);
        }
        public bool LoadOnlyMellatPayment(Customer customer)
        {
            var attr = _genericAttributeService.GetAttributesForEntity(customer.Id, "Customer");
            if (attr != null && attr.Any())
            {
                var _attr = attr.Where(p => p.Key == "InputSource").OrderByDescending(p => p.Id).FirstOrDefault();
                if (_attr != null && _attr.Value == "seke")
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        public CheckoutPaymentMethodModel getPaymentMethod(Order order)
        {
            if (!order.IsOrderCod() && (!order.IsOrderForeign() || order.canPayForgin()))
            {
                bool OnlyMelat = LoadOnlyMellatPayment(order.Customer);

                CleanShopingCartItem(order.Customer);
                foreach (var orderItem in order.OrderItems)
                {
                    var results = _shoppingCartService.AddToCart(order.Customer, orderItem.Product,
                         ShoppingCartType.ShoppingCart, order.StoreId,
                         orderItem.AttributesXml, orderItem.UnitPriceExclTax,
                         orderItem.RentalStartDateUtc, orderItem.RentalEndDateUtc,
                         orderItem.Quantity, false);
                    if (results.Any())
                        return null;
                    break;
                }
                var paymentMethodModel = _checkoutModelFactory.PreparePaymentMethodModel(order.Customer.ShoppingCartItems.ToList(), 0);
                if (OnlyMelat)
                {
                    var temp2 = paymentMethodModel.PaymentMethods.Where(p => p.PaymentMethodSystemName == "Payments.Mellat").ToList();
                    paymentMethodModel.PaymentMethods = temp2;
                }
                CleanShopingCartItem(order.Customer);
                return paymentMethodModel;
            }
            return null;
        }
        public CheckoutPaymentMethodModel getPaymentMethodForSafeBuy(Order order, Customer customer)
        {
            if (!order.IsOrderCod() && !order.IsOrderForeign())
            {
                bool OnlyMelat = LoadOnlyMellatPayment(customer);

                CleanShopingCartItem(customer);
                foreach (var orderItem in order.OrderItems)
                {
                    _shoppingCartService.AddToCart(customer, orderItem.Product,
                        ShoppingCartType.ShoppingCart, order.StoreId,
                        orderItem.AttributesXml, orderItem.UnitPriceExclTax,
                        orderItem.RentalStartDateUtc, orderItem.RentalEndDateUtc,
                        orderItem.Quantity, false);
                    break;
                }
                var paymentMethodModel = _checkoutModelFactory.PreparePaymentMethodModel(customer.ShoppingCartItems.ToList(), 0);
                if (OnlyMelat)
                {
                    var temp2 = paymentMethodModel.PaymentMethods.Where(p => p.PaymentMethodSystemName == "Payments.Mellat").ToList();
                    paymentMethodModel.PaymentMethods = temp2;
                }
                CleanShopingCartItem(customer);
                return paymentMethodModel;
            }
            return null;
        }
        public CheckoutPaymentMethodModel getPaymentMethodForChargeWallet()
        {
            var setting =
               _settingService.GetSetting("NopMaster.Wallet_ProductId", _storeContext.CurrentStore.Id, false);
            int productId = setting == null ? 0 : int.Parse(setting.Value);
            var product = _productService.GetProductById(productId);
            if (product == null)
            {
                return null;
            }
            CleanShopingCartItem(_workContext.CurrentCustomer);
            _shoppingCartService.AddToCart(_workContext.CurrentCustomer, product,
                ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id,
                "", 100000,
                null, null,
               1, false);
            bool OnlyMellat = LoadOnlyMellatPayment(_workContext.CurrentCustomer);

            var paymentMethodModel = _checkoutModelFactory.PreparePaymentMethodModel(_workContext.CurrentCustomer.ShoppingCartItems.ToList(), 0);
            if (OnlyMellat)
            {
                var temp2 = paymentMethodModel.PaymentMethods.Where(p => p.PaymentMethodSystemName == "Payments.Mellat").ToList();
                paymentMethodModel.PaymentMethods = temp2;
            }
            CleanShopingCartItem(_workContext.CurrentCustomer);
            return paymentMethodModel;
        }
        public CheckoutPaymentMethodModel getPaymentMethodForSaleCarton()
        {
            var product = _productService.GetProductById(10430);
            if (product == null)
            {
                return null;
            }
            string xml = "";
            xml = getKartonCheckoutAttributeXml("سایز A5(22*11)", 0, 0, 10430);
            CleanShopingCartItem(_workContext.CurrentCustomer);
            var p = _shoppingCartService.AddToCart(_workContext.CurrentCustomer, product,
                ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id,
                xml, 20000,
                null, null,
               1, false);
            var paymentMethodModel = _checkoutModelFactory.PreparePaymentMethodModel(_workContext.CurrentCustomer.ShoppingCartItems.ToList(), 0);
            bool OnlyMellat = LoadOnlyMellatPayment(_workContext.CurrentCustomer);
            if (OnlyMellat)
            {
                var temp2 = paymentMethodModel.PaymentMethods.Where(n => n.PaymentMethodSystemName == "Payments.Mellat").ToList();
                paymentMethodModel.PaymentMethods = temp2;
            }
            CleanShopingCartItem(_workContext.CurrentCustomer);
            return paymentMethodModel;
        }

        public CheckoutPaymentMethodModel GetPaymentMethodsForCODRequest()
        {
            var product = _productService.GetProductById(10435);
            if (product == null)
            {
                return null;
            }
            CleanShopingCartItem(_workContext.CurrentCustomer);
            _shoppingCartService.AddToCart(_workContext.CurrentCustomer, product,
                ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id,
                "", 100000,
                null, null,
               1, false);
            bool OnlyMellat = LoadOnlyMellatPayment(_workContext.CurrentCustomer);
            var paymentMethodModel = _checkoutModelFactory.PreparePaymentMethodModel(_workContext.CurrentCustomer.ShoppingCartItems.ToList(), 0);
            if (OnlyMellat)
            {
                var temp2 = paymentMethodModel.PaymentMethods.Where(p => p.PaymentMethodSystemName == "Payments.Mellat").ToList();
                paymentMethodModel.PaymentMethods = temp2;
            }
            CleanShopingCartItem(_workContext.CurrentCustomer);
            return paymentMethodModel;
        }

        private bool IsMultiShippment(Order order)
        {
            bool IsMultiShippment = _extendedShipmentService.IsMultiShippment(order);
            if (IsMultiShippment)
            {
                if (order.OrderItems.Count == 1 && order.OrderItems.First().Quantity == 1)
                    return false;
                return true;
            }
            return false;
        }
        public int getOrderTotal(int OrderId)
        {
            SqlParameter P_Orderid = new SqlParameter()
            {
                ParameterName = "orderId",
                SqlDbType = SqlDbType.Int,
                Value = (object)OrderId
            };
            SqlParameter[] prms = new SqlParameter[]
            {
                P_Orderid
            };

            string Query = @"EXEC dbo.Sp_GetOrderTotal @orderId";
            return _dbContext.SqlQuery<int>(Query, prms).FirstOrDefault();
        }
        private int getHagheMaghar(int OrderId)
        {
            string Query = @"SELECT
	                            TOP(1)THM.HagheMagharPrice+ISNULL(ShipmentHagheMaghr,0) HagheMagharPrice
                            FROM
	                            dbo.Tb_HagheMaghar AS THM
	                            INNER JOIN dbo.OrderItem AS OI ON OI.Id = THM.OrderItemId
                            WHERE
	                            OI.OrderId = " + OrderId;
            int? hagheMaghar = _dbContext.SqlQuery<int>(Query, new object[0]).FirstOrDefault();
            return hagheMaghar.GetValueOrDefault(0);
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
        public void Log(string header, string Message)
        {
            var logger =
                (DefaultLogger)EngineContext.Current
                    .Resolve<ILogger>();
            logger.InsertLog(LogLevel.Information, header, Message, null);
        }
        public bool IsValidServiceForCustomer(int serviceId)
        {
            var data = _extendedShipmentService.GetCategoryInfo(serviceId);
            if (data == null)
                return true;
            if (data.IsCod)
            {
                if (_workContext.CurrentCustomer.IsInCustomerRole("COD"))
                    return true;
                return false;
            }
            return true;
        }
        public List<CustomAddressModel> FetchAddress(int CustomerId, int CountryId, int StateId, string AddressValue)
        {
            SqlParameter P_CustomerId = new SqlParameter()
            {
                ParameterName = "customerId",
                SqlDbType = SqlDbType.Int,
                Value = (object)CustomerId
            };
            SqlParameter P_CountryId = new SqlParameter()
            {
                ParameterName = "CountryId",
                SqlDbType = SqlDbType.Int,
                Value = (object)CountryId
            };
            SqlParameter P_StateId = new SqlParameter()
            {
                ParameterName = "StateId",
                SqlDbType = SqlDbType.Int,
                Value = (object)StateId
            };
            SqlParameter P_AddressValue = new SqlParameter()
            {
                ParameterName = "AddressValue",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)AddressValue
            };
            SqlParameter[] prms = new SqlParameter[]
            {
                P_CustomerId,
                P_CountryId,
                P_StateId,
                P_AddressValue
            };
            string Query = @"EXEC dbo.Sp_FetchAddress @customerId,@CountryId,@StateId,@AddressValue";
            return _dbContext.SqlQuery<CustomAddressModel>(Query, prms).Distinct().ToList();
        }
        public long getTimeSpan()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(); ;
        }
        public void LogException(Exception exception)
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var logger = EngineContext.Current.Resolve<ILogger>();

            var customer = workContext.CurrentCustomer;
            logger.Error(exception.Message, exception, customer);
        }
        public SubMarketModel GetStoreAndSubMarket(string host, string Path)
        {
            SqlParameter P_Host = new SqlParameter()
            {
                ParameterName = "Host",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)host ?? (object)DBNull.Value
            };
            SqlParameter P_Path = new SqlParameter()
            {
                ParameterName = "Path",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)Path ?? (object)DBNull.Value
            };
            SqlParameter[] prms = new SqlParameter[]
            {
                P_Host,
                P_Path
            };
            string Query = @"EXEC dbo.Sp_DetectStoreAndSubMarket @Host, @Path";
            var data = _dbContext.SqlQuery<SubMarketModel>(Query, prms).FirstOrDefault();
            return data;
        }
        public bool _SendDataToPost(Order order, out string error)
        {
            error = "";
            return _extendedShipmentService._SendDataToPost(order, out error);
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
            return _extendedShipmentService.CalcCODPriceApi(product, weight, attributesXml, userName, countryId, stateId, postType, out error);
        }

        public int CalcHagheSabet(int customerId, int ServiceId, int OrderId = 0)
        {
            return _extendedShipmentService.CalcHagheSabet(customerId, ServiceId, OrderId);
        }
        public void RestartStopwatch(System.Diagnostics.Stopwatch watch, string logNote, ref long Total)
        {
            _extendedShipmentService.RestartStopwatch(watch, logNote, ref Total);
        }
        public bool CanPayForOrder(int orderId, out string msg)
        {
            msg = "";
            var order = _orderService.GetOrderById(orderId);
            int ServiceId = order.OrderItems.First().Product.ProductCategories.First().CategoryId;
            if (ServiceId == 717 || ServiceId == 701)
            {
                var SnapBoxPaymentAttr = _genericAttributeService.GetAttributesForEntity(order.Id, "Order").FirstOrDefault(p => p.Key == "SnapBoxPayment"
             && p.StoreId == order.StoreId);
                string SnapBoxPayment = SnapBoxPaymentAttr?.Value;
                if (string.IsNullOrEmpty(SnapBoxPayment))
                {
                    _genericAttributeService.SaveAttribute<string>(order, "SnapBoxPayment", "false", order.StoreId);
                    if (ServiceId == 717)
                    {
                        msg = "کاربر محترم منتظر دریافت پیامک جهت پرداخت سفارش خود باشید، عدم دریافت پیامک در 10 دقیقه آینده به این معنا می باشد که سفیری برای سفارش شما یافت نشده و سفارش شما به صورت اتوماتیک کنسل می شود";
                    }
                    else if (ServiceId == 701)
                    {
                        msg = "کاربر محترم منتظر تماس کارشناسان به جهت تکمیل سفارش خود باشید";
                    }
                    return false;
                }
                else if (SnapBoxPayment == "false")
                {
                    SnapBoxPaymentAttr.Value = "true";
                    _genericAttributeService.UpdateAttribute(SnapBoxPaymentAttr);
                    return true;
                }
                return true;
            }
            return true;
        }
        public int getloosCount()
        {
            string query = $@"";
            return _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
        }
        public ShipmentDimention getDimentionByName(string name)
        {
            string query = $@"SELECT TOP(1)
	                        CAST(TCI.Length AS INT) Length
	                        , CAST(TCI.Width AS INT) Width
	                        , CAST(TCI.Height AS INT) Height
                        FROM
	                        dbo.Tbl_CartonInfo AS TCI
                        WHERE
	                        TCI.Name =@Name";
            SqlParameter[] prms = new SqlParameter[] {
                new SqlParameter() { ParameterName = "Name", SqlDbType = SqlDbType.NVarChar,Value = name },
            };
            return _dbContext.SqlQuery<ShipmentDimention>(query, prms).FirstOrDefault();
        }
        public NewCheckout_Sp_Output CheckoutBySp(NewCheckout_Sp_Input model, OrderRegistrationMethod RegistrationMethod, int BulkOrderId,
            bool IsFromAp, bool isFromSep, bool IsForInternational = false)
        {
            string query = $@"EXEC dbo.Checkout_Order_Insert @JsonData, @JsonOrderList,@JsonDataOut OUTPUT,@ErrorCode OUTPUT,@ErrorMessage OUTPUT";
            SqlParameter JsonData = new SqlParameter()
            {
                ParameterName = "JsonData",
                SqlDbType = SqlDbType.NVarChar,
                Value = string.IsNullOrEmpty(model.JsonData) ? (object)DBNull.Value : (object)model.JsonData,
                Size = -1
            };
            SqlParameter JsonOrderList = new SqlParameter()
            {
                ParameterName = "JsonOrderList",
                SqlDbType = SqlDbType.NVarChar,
                Value = string.IsNullOrEmpty(model.JsonOrderList) ? (object)DBNull.Value : (object)model.JsonOrderList,
                Size = -1
            };
            SqlParameter JsonDataOut = new SqlParameter()
            {
                ParameterName = "JsonDataOut",
                SqlDbType = SqlDbType.NVarChar,
                Direction = ParameterDirection.Output,
                Size = -1
            };
            SqlParameter ErrorCode = new SqlParameter()
            {
                ParameterName = "ErrorCode",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };
            SqlParameter ErrorMessage = new SqlParameter()
            {
                ParameterName = "ErrorMessage",
                SqlDbType = SqlDbType.NVarChar,
                Direction = ParameterDirection.Output,
                Size = -1
            };
            SqlParameter[] prms = new SqlParameter[]
            {
                JsonData,
                JsonOrderList,
                JsonDataOut,
                ErrorCode,
                ErrorMessage
            };
            //Log("سفارش", model.JsonOrderList);
            //Log("اطلاعات اصلی", model.JsonData);
            _dbContext.ExecuteSqlCommand(query, true, null, prms);
            var ret = JsonConvert.DeserializeObject<NewCheckout_Sp_Output>(JsonDataOut.Value.ToString());
            ret.ErrorCode = (int)ErrorCode.Value;
            ret.ErrorMessage = (string)ErrorMessage.Value;
            if (ret.orderId > 0)
            {
                var order = _orderService.GetOrderById(ret.orderId);
                if (RegistrationMethod == OrderRegistrationMethod.PhoneOrder)
                {
                    _extendedShipmentService.MarkOrder(OrderRegistrationMethod.PhoneOrder, order);
                }
                var Errors = _extnOrderProcessingService.EndOfOrderPlaced(order, (int)RegistrationMethod, BulkOrderId, IsFromAp, isFromSep);
                if (Errors.Any())
                {
                    if (Errors.Any(p => p.Contains("|")))
                    {
                        var error = Errors.Where(p => p.Contains("|")).First();
                        ret.ErrorCode = int.Parse(error.Split('|')[1]);
                    }
                    else
                        ret.ErrorCode = -1;
                    ret.ErrorMessage += string.Join(",", Errors);
                    ret.orderId = 0;
                    return ret;
                }
                else
                {
                    var _categoryInfo = getCategoryInfo(order);
                    var CollectingInfo = _collectorService.CalculatePrice(order.BillingAddress.CountryId.Value, order.BillingAddress.StateProvinceId.Value
                        , order.BillingAddress.Address1, order.CustomerId, _categoryInfo.CategoryId, order.Id, ServicesType.DistributionAndCollectionPaykhub);

                    _collectorService.SaveCollectingPrices(CollectingInfo);

                    order.OrderTotal += CollectingInfo.WalletCollection + CollectingInfo.WalletDistribution;
                    _orderService.UpdateOrder(order);

                    bool needCheck;
                    var res = CompleteInternationalPost(order, model, RegistrationMethod, BulkOrderId, IsFromAp, isFromSep, out needCheck);
                    if (needCheck)
                    {
                        if (res.ErrorCode != 0)
                        {
                            InsertOrderNote("سفارش پیشتاز برای پست خارجی ثبت نشد" + res.ErrorMessage, res.orderId);
                        }
                        else
                        {
                            // int orderValue = getForginAddtionalValue(orderResult.PlacedOrder.Id);
                            var orderResult = order;// _orderService.GetOrderById(res.orderId);
                            var pishtazResult = _orderService.GetOrderById(res.orderId);

                            orderResult.OrderTotal = Convert.ToInt32(orderResult.OrderTotal) + Convert.ToInt32(pishtazResult.OrderTotal);
                            _orderService.UpdateOrder(orderResult);
                            int relatedOrderPrice = Convert.ToInt32(pishtazResult.OrderTotal);
                            pishtazResult.OrderTotal = 0;
                            _orderService.UpdateOrder(pishtazResult);
                            InsertRelatedOrder(ret.orderId, pishtazResult.Id, relatedOrderPrice);
                            InsertOrderNote($"شماره سفارش پیشتاز متناظر با پست خارجی مورد نظر {pishtazResult.Id} می باشد", ret.orderId);
                        }

                    }
                }
                var _newCheckoutModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NewCheckoutModel>>(model.JsonOrderList);
                RequestToApplyDiscount(ret.orderId, _newCheckoutModel.First().discountCouponCode, IsForInternational);
                if (_extendedShipmentService.IsSafeBuy(order.Id))
                {
                    var ApproximateValue = 0;
                    foreach (var item in order.OrderItems)
                    {
                        ApproximateValue += _extendedShipmentService.getApproximateValue(item.Id) * item.Quantity;
                    }
                    var Address = _extendedShipmentService.getAddressFromShipment(order.Shipments.First().Id);

                    if (Address != null)
                    {
                        _notificationService._sendSms(Address.PhoneNumber, $@"خریدار/ گیرنده محترم
|
سفارش 
{order.Id}
از سوی
{order.BillingAddress.FirstName + " " + order.BillingAddress.LastName} به مبلغ
{Convert.ToInt32(ApproximateValue).ToString("N0") + " ریال "}
و مبلغ پستی 
{Convert.ToInt32(order.OrderTotal - ApproximateValue).ToString("N0") + " ریال "}
برای شما ثبت شد.
|
لطفا با کلیک بروی لینک زیر جهت واریز امانی وجه اقدام فرمایید
|
https://postex.ir/order/SafeBuyPayForOrderIndex?orderId={order.Id}
|
امنیتو".Replace(Environment.NewLine, " ").Replace("|", Environment.NewLine));

                    }
                }
            }
            return ret;
        }
        public NewCheckout_Sp_Output CompleteInternationalPost(Order order,
            NewCheckout_Sp_Input model, OrderRegistrationMethod RegistrationMethod, int BulkOrderId,
            bool IsFromAp, bool isFromSep, out bool needCheck)
        {
            needCheck = false;
            if (!order.OrderItems.Any(p => p.Product.ProductCategories.Any(n => new int[] { 707, 719 }.Contains(n.CategoryId))))
            {
                return new NewCheckout_Sp_Output();//object data never mind due the needCheck is false
            }
            //if (!order.Customer.IsInCustomerRole("onlineGateway"))
            //{
            if (!_CheckRegionDesVijePost.CheckValidSourceForInternationalPost(order.BillingAddress.CountryId.Value, order.BillingAddress.StateProvinceId.Value))
            {
                model.ErrorCode = 30;
                model.ErrorMessage = $"امکان ارسال پست خارجی از شهر {order.BillingAddress.StateProvince.Name} در حال حاضر وجود ندارد";
                return new NewCheckout_Sp_Output();//object data never mind due the needCheck is false
            }
            //}
            needCheck = true;
            var categoryId = order.OrderItems.Select(p => p.Product.ProductCategories.First().CategoryId).First();
            int reciverStateId = categoryId == 719 ? 581 : 582;
            string Addresss = (categoryId == 719 ? "خیابان آزادی، انتهای خیابان اسکندری شمالی-شماره 144- شرکت پستی راه آسمان آبی" : "بزرگراه حقانی غرب بین دیدار شمالی و 4 راه جهان کودک، پلاک -35 -شرکت پستی پی دی ای");
            string phoneNumber = (categoryId == 719 ? "09331473290" : "09050587273");
            string FirstName = (categoryId == 719 ? "شرکت" : "شرکت");
            string LastName = (categoryId == 719 ? "راه آسمان آبی" : "پست پی دی ای");
            var newOrderList = new List<NewCheckoutModel>(JsonConvert.DeserializeObject<List<NewCheckoutModel>>(model.JsonOrderList));

            int Internal_serviceId = 0;
            if (order.BillingAddress.CountryId != 1)
            {
                Internal_serviceId = 714;//730;//ماهکس
            }
            else
            {
                Internal_serviceId = 723;//پست پیشتاز
            }

            for (int i = 0; i < newOrderList.Count; i++)
            {
                newOrderList[i].ServiceId = Internal_serviceId;
                newOrderList[i].receiver_ForeginCityName = null;
                newOrderList[i].receiver_ForeginCountry = 0;
                newOrderList[i].receiver_ForeginCountryName = null;
                newOrderList[i].shippingAddressModel = (Address)newOrderList[i].shippingAddressModel.Clone();
                newOrderList[i].shippingAddressModel.Id = 0;
                newOrderList[i].shippingAddressModel.Address1 = Addresss;
                newOrderList[i].shippingAddressModel.FirstName = FirstName;
                newOrderList[i].shippingAddressModel.LastName = LastName;
                newOrderList[i].shippingAddressModel.CountryId = 1;
                newOrderList[i].shippingAddressModel.StateProvinceId = reciverStateId;
                newOrderList[i].shippingAddressModel.PhoneNumber = phoneNumber;
            }
            model.JsonOrderList = JsonConvert.SerializeObject(newOrderList);
            return CheckoutBySp(model, RegistrationMethod, BulkOrderId, IsFromAp, isFromSep, IsForInternational: true);

        }
        /// <summary>
        /// اعمال تخفیف ها بر روی سفارش
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="CopunCode"></param>
        /// <returns></returns>
        public bool RequestToApplyDiscount(int orderId, string _CopunCode, bool IsForInternational = false)
        {
            if (IsForInternational)
                return true;
            //if (!_workContext.CurrentCustomer.IsInCustomerRole("DiscountTest"))
            //    return true;
            try
            {
                string query = $@"EXEC dbo.Checkout_Sp_ApplyDiscountToOrder @OrderId, @CoupunCode,@ErrorCode OUTPUT, @ErrorMessage OUTPUT";
                SqlParameter OrderId = new SqlParameter()
                {
                    ParameterName = "OrderId",
                    SqlDbType = SqlDbType.Int,
                    Value = orderId,
                };
                SqlParameter CoupunCode = new SqlParameter()
                {
                    ParameterName = "CoupunCode",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = (object)_CopunCode ?? (object)DBNull.Value,
                    Size = 50
                };
                SqlParameter ErrorCode = new SqlParameter()
                {
                    ParameterName = "ErrorCode",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };
                SqlParameter ErrorMessage = new SqlParameter()
                {
                    ParameterName = "ErrorMessage",
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Output,
                    Size = 600
                };
                SqlParameter[] prms = new SqlParameter[]
                {
                    OrderId,
                    CoupunCode,
                    ErrorCode,
                    ErrorMessage
                };
                _dbContext.ExecuteSqlCommand(query, parameters: prms);
                int _ErrorCode = (int)ErrorCode.Value;
                string _ErrorMessage = (string)ErrorMessage.Value;
                if (_ErrorCode != 0)
                {
                    InsertOrderNote(_ErrorMessage, orderId);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }

        }
    }
    public class ShipmentDimention
    {
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class _SelectListItem
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public int PriceAdjustment { get; set; }
    }
    public class StateCodemodel : BaseEntity
    {
        public int stateId { get; set; }
        public string StateCode { get; set; }
        public string SenderStateCode { get; set; }
    }
    public class OrderItemProperty
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string PropertyAttrValueText { get; set; }
        public int? PropertyAttrValuePrice { get; set; }
        public int? PropertyAttrValueCost { get; set; }
        public string PropertyAttrValueName { get; set; }
        public string PropertyAttrName { get; set; }
        public decimal PropertyAttrValueWeight { get; set; }
        public int EngPrice { get; set; }
        public bool ISForPost { get; set; }
    }
    public class CartonSaleModel
    {
        public int Amount { get; set; }
        public int OrderId { get; set; }
        public int ShipmentId { get; set; }
        public bool UseRewardPoints { get; set; }
        public string paymentmethod { get; set; }
        public bool isFromApp { get; set; }
        public List<CartonSizeSelected> List_Sizeitem { get; set; }

    }

    public class CartonSizeSelected
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }

    /// <summary>
    /// Invoice Detaile
    /// </summary>
    public class InvoiceDetails
    {
        public string Username { get; set; }
        public string SenderPhoneNumber { get; set; }
        public string RevicerPhoneNumber { get; set; }
        public int OrderId { get; set; }
        public int ShipmentId { get; set; }
        public string TrackingNumber { get; set; }
        public string BoxContecnt { get; set; }
        public string ServiceName { get; set; }
        public string ServiceProviderName { get; set; }
        public string OrderDate { get; set; }
        public string SenderCountryName { get; set; }
        public string SenderStateName { get; set; }
        public string ReciverCountryName { get; set; }
        public string ReciverStateName { get; set; }
        public int PostBasePrice { get; set; }
        public int EngPrice { get; set; }
        public int OrderDiscount { get; set; }
        public string GoodsCodPrice { get; set; }
        public int SmsPrice { get; set; }
        public int PrintLogoPrice { get; set; }
        public int CartonCost { get; set; }
        public int PackingPrice { get; set; }
        public int AccessPrinterPrice { get; set; }
        public int CompulsoryInsurancePrice { get; set; }
        public int InsurancePrice { get; set; }
        public int DepotCost { get; set; }
        public int CollectPrice { get; set; }
        public int RegisterCost { get; set; }
        public int OrderTotal { get; set; }
        public int paymentMethod { get; set; }

    }
}
