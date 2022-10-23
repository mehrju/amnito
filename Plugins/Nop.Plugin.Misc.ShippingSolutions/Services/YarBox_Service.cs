using Newtonsoft.Json;
using Nop.Services.Logging;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.ShippingSolutions.Models.Params.YarBox;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.YarBox;
using Nop.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Common;
//using Nop.plugin.Orders.ExtendedShipment.Services;

namespace Nop.Plugin.Misc.ShippingSolutions.Services
{
    /// <summary>
    /// سرویس یارباکس ورژن 1 در تاریخ 17/6/98<para />
    /// شما در این امکان ارسال چندین بسته را به یک مقصد دارید<para />
    /// با توجه به داکیومنت، در این ورژن در هر سفارش امکان ثبت یک رکورد وجود دارد<para />
    /// https://yarbox.co/facktoronline<para />
    /// تنظمیات سرویس در سازنده از جدول تنظیمات خوانده میشود<para />
    /// توکن سرویس در 2 مرحله به دست می ایند<para />
    ///1- ارسال شماره موبایل خودتون را ای پی ای مروبطه<para />
    ///2- ارسال کد پیامک شده بهشماره موبالتون در ای پی ای دوم و دریافت توکن نهایی در خروجی ای پی ای<para />
    ///3- به همین دلیل این دو ای پی ای در این سرویس پیاده سازی نشده و شما میتوانید با کاکشن مربوطه در نرم افزار پست من این کار را انچام دهید<para />
    /// <para>API GetPackingType ای پی ای انواع بسته بندی</para>
    /// <para>API GetAPType ای پی ای نوع بسته</para>
    /// <para>API AddPostPacks ای پی ای ثبت بسته</para>
    /// <para>API Factor ای پی ای ثبت فاکتور</para>
    /// <para>API accept ای پی ای نهایی کردن سفارش</para>
    /// <para>Configuration تابع خواندن اطلاعات از تنظیمات</para>
    /// <para>ChekParamsAddPostPacks تابع چک کردن پرامترهای وردی تابع ثبت بسته</para>
    /// مراحل استفاده از سرویس<para />
    /// 1- خواندن اطلاعات از دو ای پی ای انواع بسته بندی و نوع بسته<para />
    /// 2- ثبت بسته با ای پی ای add Post Packs<para />
    /// 3- ثبت و مشاهده فاکتور با ای پی ای Factor<para />
    /// 4- نهایی کردن سفارش با ای پی ای accept<para />
    ///<para>به دلیل توکن ارائه شده در سرویس تینکست تصمیم بر این شده که از روش singelton pattern استفاده شود</para>
    ///<para>singelton pattern</para>
    ///<para>با توجه به اینکه مدت زمان انقضای توکن ارائه شده یک روز میباشد در کلاس سازنده چک میشود </para>
    ///<para>چک کردن زمان انقضای توکن</para>
    ///<para>تابعی در سرویس ارائه شده برای تست زمان انقضا وجود ندارد</para>
    ///<para>پس در بار اولی که این کلاس ساخته میشود یک رکوئست به سرویس داریم</para>
    ///<para>توکن ساخته شده به دیگر توابع پاس داده خواهد شد</para>
    /// </summary>
    public class YarBox_Service : IYarBox_Service
    {
        private static YarBox_Service instance = null;
        private static readonly object Instancelock = new object();
        private static string Bearer = "";
        private DateTime DateStart = new DateTime();
        private bool isfirst = true;

        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly ShippingSettings _ShippingSettings;
        private readonly ISettingService _settingService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStoreContext _storeContext;
        //private readonly INotificationService _notificationService;

        public YarBox_Service(ILogger logger, IWorkContext workContext, ShippingSettings ShippingSettings, ISettingService settingService,
            IGenericAttributeService genericAttributeService, IStoreContext storeContext/*,INotificationService notificationService*/)
        {
            this._logger = logger;
            this._workContext = workContext;
            this._settingService = settingService;
            this._ShippingSettings = Configuration();
            this._genericAttributeService = genericAttributeService;
            _storeContext = storeContext;
            //_notificationService = notificationService;
            #region new -> check date in setting
            //try
            //{
            //    DateTime DateStart = 
            //    double Day = (DateTime.Now - DateStart).TotalDays;
            //    if (Day >= _ShippingSettings.YarBox_ExpiresDayToken)
            //    {
            //        //send sms
            //     bool status=   _notificationService._sendSms("09139064053", "تاریخ انقضای توکن یارباکس به پایان رسیده است ، لطفا پیگیری بفرمایید");
            //    }
            //}
            //catch (Exception ex)
            //{

            //    Common.LogException(ex);
            //}

            #endregion
            #region دریافت و صحت سنجی توکن از api
            try
            {
                var _DateStart = _genericAttributeService.GetAttributesForEntity(_storeContext.CurrentStore.Id, "Store").OrderByDescending(p => p.Id).FirstOrDefault(p => p.Key == "yarboxTokenStartdate" && p.StoreId == _storeContext.CurrentStore.Id)?.Value;

                if (string.IsNullOrEmpty(_DateStart))
                {
                    DateStart = DateTime.Now;
                    isfirst = false;
                    //Bearer
                    Result_YarBox_Verify r = Verify();
                    if (r.Status == true)
                    {
                        Bearer = "bearer " + r.detail_yarBox_verify.access_token.ToString();
                        _genericAttributeService.SaveAttribute<string>(_storeContext.CurrentStore, "yarboxTokenStartdate", DateStart.ToString(), _storeContext.CurrentStore.Id);
                        _genericAttributeService.SaveAttribute<string>(_storeContext.CurrentStore, "yarboxToken", Bearer, _storeContext.CurrentStore.Id);
                    }
                    else
                    {
                        //bool status = _notificationService._sendSms("09139064053", "عدم دریافت توکن اولیه از یارباکس");
                    }

                }
                else
                {
                    //Common.Log("تاریخ", _DateStart);
                    DateTime DateStart = Convert.ToDateTime(_DateStart);
                    double Day = (DateTime.Now - DateStart).TotalDays;
                    if (Day >= _ShippingSettings.YarBox_ExpiresDayToken)
                    {
                        //new Bearer
                        Result_YarBox_Verify r = Verify();
                        if (r.Status == true)
                        {
                            Bearer = "bearer " + r.detail_yarBox_verify.access_token.ToString();
                            _genericAttributeService.SaveAttribute<string>(_storeContext.CurrentStore, "yarboxTokenStartdate", DateTime.Now.ToString(), _storeContext.CurrentStore.Id);
                            _genericAttributeService.SaveAttribute<string>(_storeContext.CurrentStore, "yarboxToken", Bearer, _storeContext.CurrentStore.Id);
                        }
                        else
                        {
                            //bool status = _notificationService._sendSms("09139064053", "عدم دریافت توکن جدید از یارباکس");
                        }

                    }
                    else
                    {
                        Bearer = _genericAttributeService.GetAttributesForEntity(_storeContext.CurrentStore.Id, "Store").FirstOrDefault(p => p.Key == "yarboxToken" && p.StoreId == _storeContext.CurrentStore.Id)?.Value;
                    }

                }
            }
            catch (Exception ex)
            {
                Common.LogException(ex);
                //bool status = _notificationService._sendSms("09139064053", "عدم دریافت توکن از یارباکس");
            }
            #endregion
        }


        //public static YarBox_Service GetInstance(ILogger logger
        //    , IWorkContext workContext
        //    , ShippingSettings ShippingSettings
        //    , ISettingService settingService)
        //{
        //    lock (Instancelock)
        //    {
        //        if (instance == null)
        //        {
        //            instance = new YarBox_Service(logger, workContext, ShippingSettings, settingService);
        //        }
        //        return instance;
        //    }
        //}

        /// <summary>
        /// خواندن تنظیمات فروشگاه دیفالت
        /// </summary>
        /// <returns></returns>
        private ShippingSettings Configuration()
        {

            var _storeContext = EngineContext.Current.Resolve<IStoreContext>();
            var Settings = _settingService.LoadSetting<ShippingSettings>(_storeContext.CurrentStore.Id);
            return Settings;
        }
        /// <summary>
        /// ای پی ای خواندن لیست انواع بسته بندی<para />
        /// نوع ای پی ای Get<para />
        /// مدل ورودی ندارد<para />
        /// <para>Result_YarBox_PackingType خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناچ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_YarBox_PackingType GetPackingType()
        {
            Result_YarBox_PackingType _result = new Result_YarBox_PackingType();

            try
            {
                //check params and setting

                if (string.IsNullOrEmpty((Bearer ?? "").Trim())//
                    || string.IsNullOrEmpty((_ShippingSettings.YarBox_URLApPackingType ?? "").Trim())
                    )
                {
                    _result.Status = false;
                    _result.Message = "Setting(Bearer orYarBox_URLApPackingType ) is Null!";
                    return _result;

                }
                else
                {
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.YarBox_URLApPackingType);
                    httpWebRequest.ContentType = "application/json; charset=utf-8";
                    httpWebRequest.Headers.Add("Authorization", Bearer);
                    httpWebRequest.Method = "GET";
                    try
                    {
                        using (var response = httpWebRequest.GetResponse() as HttpWebResponse)
                        {

                            if (httpWebRequest.HaveResponse && response != null)
                            {
                                string result_temp;
                                using (var reader = new StreamReader(response.GetResponseStream()))
                                {
                                    result_temp = reader.ReadToEnd();

                                }
                                if (response.StatusCode == HttpStatusCode.OK)
                                {

                                    _result.Status = true;
                                    _result.Message = "Ok";
                                    var t = JsonConvert.DeserializeObject<List<ItemsPackingType>>(result_temp);
                                    _result.Items = t;
                                    return _result;

                                }
                                else
                                {
                                    _result.Status = false;
                                    _result.Message = result_temp;
                                    return _result;
                                }

                            }
                            else
                            {
                                _result.Status = false;
                                _result.Message = "server not response";
                                return _result;

                            }
                        }
                    }
                    catch (WebException e)
                    {


                        if (e.Response != null)
                        {
                            using (var errorResponse = (HttpWebResponse)e.Response)
                            {
                                using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                                {
                                    _result.Status = false;
                                    _result.Message = e.Message;
                                    return _result;

                                }
                            }

                        }
                        else
                        {
                            _result.Status = false;
                            _result.Message = "server not response";
                            return _result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _result.Status = false;
                _result.Message = "An error has occurred in the Plugin::" + ex.ToString();
                return _result;

            }

        }
        /// <summary>
        /// ای پی ای خواندن لیست انواع بسته <para />
        /// نوع ای پی ای Get<para />
        /// مدل ورودی ندارد<para />
        /// <para>Result_YarBox_ApType خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناچ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_YarBox_ApType GetAPType()
        {
            Result_YarBox_ApType _result = new Result_YarBox_ApType();

            try
            {
                //check params and setting
                if (string.IsNullOrEmpty((Bearer ?? "").Trim())//Bearer
                    || string.IsNullOrEmpty((_ShippingSettings.YarBox_URLAp_Type ?? "").Trim())
                    )
                {
                    _result.Status = false;
                    _result.Message = "Setting(Bearer or  YarBox_URLAp_Type ) is Null!";
                    return _result;

                }
                else
                {
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.YarBox_URLAp_Type);
                    httpWebRequest.ContentType = "application/json; charset=utf-8";
                    httpWebRequest.Headers.Add("Authorization", Bearer);
                    httpWebRequest.Method = "GET";
                    try
                    {
                        using (var response = httpWebRequest.GetResponse() as HttpWebResponse)
                        {

                            if (httpWebRequest.HaveResponse && response != null)
                            {
                                string result_temp;
                                using (var reader = new StreamReader(response.GetResponseStream()))
                                {
                                    result_temp = reader.ReadToEnd();

                                }
                                if (response.StatusCode == HttpStatusCode.OK)
                                {

                                    _result.Status = true;
                                    _result.Message = "Ok";
                                    var t = JsonConvert.DeserializeObject<List<ItemsApType>>(result_temp);
                                    _result.itemsApTypes = t;
                                    return _result;

                                }
                                else
                                {
                                    _result.Status = false;
                                    _result.Message = result_temp;
                                    return _result;
                                }

                            }
                            else
                            {
                                _result.Status = false;
                                _result.Message = "server not response";
                                return _result;

                            }
                        }
                    }
                    catch (WebException e)
                    {


                        if (e.Response != null)
                        {
                            using (var errorResponse = (HttpWebResponse)e.Response)
                            {
                                using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                                {
                                    _result.Status = false;
                                    _result.Message = e.Message;
                                    return _result;

                                }
                            }

                        }
                        else
                        {
                            _result.Status = false;
                            _result.Message = "server not response";
                            return _result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _result.Status = false;
                _result.Message = "An error has occurred in the Plugin::" + ex.ToString();
                return _result;

            }

        }
        /// <summary>
        /// ای پی ای خواندن ثبت بسته/بسته ها<para />
        /// نوع ای پی ای POst<para />
        /// حتما تنظیمات سرویس در ناچ کامرس چک شود که توکن و ادرس ای پی ای درست باشد<para />
        /// <para>مدل ورودیParams_YarBox_AddPostPacks</para>
        /// <para>مدل خروچیResult_YarBox_AddPostPacks</para>
        /// 
        /// </summary>
        /// 
        public async Task<Result_YarBox_AddPostPacks> AddPostPacks(Params_YarBox_AddPostPacks param)
        {
            Result_YarBox_AddPostPacks _result = new Result_YarBox_AddPostPacks();
            //check params and setting
            try
            {
                var validparam = param.IsValidParamsAddPostPacks();
                if (validparam.Item1 == false)
                {
                    _result.Status = false;
                    _result.Message = validparam.Item2;
                    return _result;
                }
                else if (string.IsNullOrEmpty((Bearer ?? "").Trim())//Bearer
                    || string.IsNullOrEmpty((_ShippingSettings.YarBox_URLApPostPacks ?? "").Trim())
                    )
                {
                    _result.Status = false;
                    _result.Message = "Setting(Bearer or  YarBox_URLApPostPacks ) is Null!";
                    return _result;

                }

                else
                {
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.YarBox_URLApPostPacks);
                    httpWebRequest.ContentType = "application/json; charset=utf-8";
                    httpWebRequest.Headers.Add("Authorization", Bearer);
                    httpWebRequest.Method = "POST";
                    using (var streamWriter = new StreamWriter(await httpWebRequest.GetRequestStreamAsync()))
                    {
                        _Params_YarBox_AddPostPacks _param = new _Params_YarBox_AddPostPacks();
                        _param.count = param.count;
                        _param.origin = param.origin_City + "(" + param.origin_State + ")";
                        _param.destination = param.destination_City + "(" + param.destination_State + ")";
                        _param.apPackingTypeId = param.apPackingTypeId;
                        _param.apTypeId = param.apTypeId;
                        _param.insurance = param.insurance;
                        _param.receiveType = param.receiveType;
                        _param.senderPhone = param.senderPhone;
                        _param.receiverPhone = param.receiverPhone;
                        _param.latitude = param.latitude;
                        _param.longitude = param.longitude;
                        _param.destinationAddress = param.destinationAddress;
                        _param.originAddress = param.originAddress;

                        var json = JsonConvert.SerializeObject(_param);
                        Debug.Write(json);
                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                    try
                    {
                        using (var response = httpWebRequest.GetResponse() as HttpWebResponse)
                        {

                            if (httpWebRequest.HaveResponse && response != null)
                            {
                                string result_temp;
                                using (var reader = new StreamReader(response.GetResponseStream()))
                                {
                                    result_temp = reader.ReadToEnd();

                                }
                                if (response.StatusCode == HttpStatusCode.OK)
                                {

                                    _result.Status = true;
                                    _result.Message = "Ok";
                                    var t = JsonConvert.DeserializeObject<string>(result_temp);
                                    _result.KEY = new KEYAddPostPacks();
                                    _result.KEY.Key = t;
                                    return _result;

                                }
                                else
                                {
                                    _result.Status = false;
                                    _result.Message = result_temp;
                                    return _result;
                                }

                            }
                            else
                            {
                                _result.Status = false;
                                _result.Message = "server not response";
                                return _result;

                            }
                        }
                    }
                    catch (WebException e)
                    {


                        if (e.Response != null)
                        {
                            using (var errorResponse = (HttpWebResponse)e.Response)
                            {
                                using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                                {
                                    _result.Status = false;
                                    _result.Message = e.Message;
                                    return _result;

                                }
                            }

                        }
                        else
                        {
                            _result.Status = false;
                            _result.Message = "server not response";
                            return _result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _result.Status = false;
                _result.Message = "An error has occurred in the Plugin::  " + ex.ToString();
                return _result;

            }
        }
        /// <summary>
        /// ای پی ای ثبت فاکتور<para />
        ///  نوع ای پی ای GET<para />
        /// حتما تنظیمات سرویس در ناچ کامرس چک شود که توکن و ادرس ای پی ای درست باشد<para />
        /// <para>Params_YarBox_Factorمدل ورودی</para>
        /// <para>Result_YarBox_Factorمدل خروجی</para>
        /// 
        /// </summary>
        public async Task<Result_YarBox_Factor> Factor(Params_YarBox_Factor param)
        {
            Result_YarBox_Factor _result = new Result_YarBox_Factor();
            //check params and setting
            try
            {
                if (string.IsNullOrEmpty((Bearer ?? "").Trim())//Bearer
                    || string.IsNullOrEmpty((_ShippingSettings.YarBox_URLfactor ?? "").Trim())
                    )
                {
                    _result.Status = false;
                    _result.Message = "Setting(Bearer or  YarBox_URLfactor ) is Null!";
                    return _result;

                }
                else if (string.IsNullOrEmpty((param.Key ?? "").Trim()))
                {
                    _result.Status = false;
                    _result.Message = "Key field must be completed";
                    return _result;
                }
                else
                {
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.YarBox_URLfactor + "/" + param.Key);
                    httpWebRequest.ContentType = "application/json; charset=utf-8";
                    httpWebRequest.Headers.Add("Authorization", Bearer);
                    httpWebRequest.Method = "GET";
                    try
                    {
                        using (var response = await httpWebRequest.GetResponseAsync() as HttpWebResponse)
                        {

                            if (httpWebRequest.HaveResponse && response != null)
                            {
                                string result_temp;
                                using (var reader = new StreamReader(response.GetResponseStream()))
                                {
                                    result_temp = reader.ReadToEnd();

                                }
                                if (response.StatusCode == HttpStatusCode.OK)
                                {

                                    _result.Status = true;
                                    _result.Message = "Ok";
                                    var t = JsonConvert.DeserializeObject<DetailFactor>(result_temp);
                                    _result.detailFactor = t;
                                    return _result;

                                }
                                else
                                {
                                    _result.Status = false;
                                    _result.Message = result_temp;
                                    return _result;
                                }

                            }
                            else
                            {
                                _result.Status = false;
                                _result.Message = "server not response";
                                return _result;

                            }
                        }
                    }
                    catch (WebException e)
                    {


                        if (e.Response != null)
                        {
                            using (var errorResponse = (HttpWebResponse)e.Response)
                            {
                                using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                                {
                                    _result.Status = false;
                                    _result.Message = e.Message;
                                    return _result;

                                }
                            }

                        }
                        else
                        {
                            _result.Status = false;
                            _result.Message = "server not response";
                            return _result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _result.Status = false;
                _result.Message = "An error has occurred in the Plugin::" + ex.ToString();
                return _result;

            }

        }
        /// <summary>
        /// ای پی ای نهایی کردن سفارش<para />
        /// نوع ای پی ای POST<para />
        /// حتما تنظیمات سرویس در ناچ کامرس چک شود که توکن و ادرس ای پی ای درست باشد<para />
        /// <para> Params_YarBox_accept مدل ورودی</para>
        /// <para>Result_YarBox_accept مدل خروچی</para>
        /// </summary>
        public async Task<Result_YarBox_accept> accept(Params_YarBox_accept param)
        {
            Result_YarBox_accept _result = new Result_YarBox_accept();
            //check params and setting
            try
            {
                if (string.IsNullOrEmpty((Bearer ?? "").Trim())//Bearer
                    || string.IsNullOrEmpty((_ShippingSettings.YarBox_URLAp_PostPacks_accept ?? "").Trim())
                    )
                {
                    _result.Status = false;
                    _result.Message = "Setting(Bearer or  YarBox_URLAp_PostPacks_accept) is Null!";
                    return _result;

                }
                else if (param.Id <= 0)
                {
                    _result.Status = false;
                    _result.Message = "Field Id must be greater than zero";
                    return _result;
                }
                else
                {
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.YarBox_URLAp_PostPacks_accept + "/" + param.Id.ToString());
                    httpWebRequest.ContentType = "application/json; charset=utf-8";
                    httpWebRequest.Headers.Add("Authorization", Bearer);
                    httpWebRequest.Method = "POST";

                    try
                    {
                        using (var response = await httpWebRequest.GetResponseAsync() as HttpWebResponse)
                        {

                            if (httpWebRequest.HaveResponse && response != null)
                            {
                                string result_temp;
                                using (var reader = new StreamReader(response.GetResponseStream()))
                                {
                                    result_temp = reader.ReadToEnd();

                                }
                                if (response.StatusCode == HttpStatusCode.OK)
                                {

                                    _result.Status = true;
                                    _result.Message = "Ok";
                                    return _result;

                                }
                                else
                                {
                                    _result.Status = false;
                                    _result.Message = result_temp;
                                    return _result;
                                }

                            }
                            else
                            {
                                _result.Status = false;
                                _result.Message = "server not response";
                                return _result;

                            }
                        }
                    }
                    catch (WebException e)
                    {


                        if (e.Response != null)
                        {
                            using (var errorResponse = (HttpWebResponse)e.Response)
                            {
                                using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                                {
                                    _result.Status = false;
                                    _result.Message = e.Message;
                                    return _result;

                                }
                            }

                        }
                        else
                        {
                            _result.Status = false;
                            _result.Message = "server not response";
                            return _result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _result.Status = false;
                _result.Message = "An error has occurred in the Plugin::  " + ex.ToString();
                return _result;

            }
        }
        /// <summary>
        /// ای پی ای لاگین و دریافت توکن جدید
        /// <para>مدل وردی از تنظمیات گرفته میشود</para>
        /// <para>خروجی توکن جدید</para>
        /// </summary>
        /// <returns></returns>
        private Result_YarBox_Verify Verify()
        {
            Result_YarBox_Verify _result = new Result_YarBox_Verify();
            //check params and setting
            try
            {
                if (string.IsNullOrEmpty((_ShippingSettings.YarBox_playerId ?? "").Trim())
                    || string.IsNullOrEmpty((_ShippingSettings.YarBox_verifyCode ?? "").Trim())
                    || string.IsNullOrEmpty((_ShippingSettings.YarBox_phoneNumber ?? "").Trim())
                                        )
                {
                    _result.Status = false;
                    _result.Message = "Setting is Null!";
                    return _result;

                }

                else
                {
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.YarBox_URLAp_Verify);
                    httpWebRequest.ContentType = "application/json; charset=utf-8";
                    httpWebRequest.Method = "POST";
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        Params_YarBox_Verify param = new Params_YarBox_Verify();
                        param.phoneNumber = _ShippingSettings.YarBox_phoneNumber;
                        param.playerId = _ShippingSettings.YarBox_playerId;
                        param.verifyCode = _ShippingSettings.YarBox_verifyCode;

                        var json = JsonConvert.SerializeObject(param);
                        Debug.Write(json);
                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                    try
                    {
                        using (var response = httpWebRequest.GetResponse() as HttpWebResponse)
                        {

                            if (httpWebRequest.HaveResponse && response != null)
                            {
                                string result_temp;
                                using (var reader = new StreamReader(response.GetResponseStream()))
                                {
                                    result_temp = reader.ReadToEnd();

                                }
                                if (response.StatusCode == HttpStatusCode.OK)
                                {

                                    _result.Status = true;
                                    _result.Message = "Ok";
                                    var t = JsonConvert.DeserializeObject<Detail_YarBox_Verify>(result_temp);
                                    _result.detail_yarBox_verify = t;
                                    return _result;

                                }
                                else
                                {
                                    _result.Status = false;
                                    _result.Message = result_temp;
                                    return _result;
                                }

                            }
                            else
                            {
                                _result.Status = false;
                                _result.Message = "server not response";
                                return _result;

                            }
                        }
                    }
                    catch (WebException e)
                    {


                        if (e.Response != null)
                        {
                            using (var errorResponse = (HttpWebResponse)e.Response)
                            {
                                using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                                {
                                    _result.Status = false;
                                    _result.Message = e.Message;
                                    return _result;

                                }
                            }

                        }
                        else
                        {
                            _result.Status = false;
                            _result.Message = "server not response";
                            return _result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _result.Status = false;
                _result.Message = "An error has occurred in the Plugin::  " + ex.ToString();
                return _result;

            }
        }
        /// <summary>
        /// ای پی ای استعلام قیمت
        /// <para>مدل ورودیParams_YarBox_Quote</para>
        /// <para>مدل خروجیResult_Yarbox_Qute</para>
        /// <para>تنظیمات را در ناپ کامرس تنظیم نمایید</para>
        /// 
        /// </summary>
        public async Task<Result_Yarbox_Qute> Qute(Params_YarBox_Quote param)
        {
            Result_Yarbox_Qute _result = new Result_Yarbox_Qute();
            //check params and setting
            try
            {
                if (string.IsNullOrEmpty((Bearer ?? "").Trim())//Bearer
                    || string.IsNullOrEmpty((_ShippingSettings.YarBox_URLAp_Qute ?? "").Trim())
                    )
                {
                    _result.Status = false;
                    _result.Message = "Setting( Bearer or YarBox_URLAp_Qute) is Null!";
                    return _result;
                }

                else
                {
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.YarBox_URLAp_Qute);
                    httpWebRequest.ContentType = "application/json; charset=utf-8";
                    httpWebRequest.Headers.Add("Authorization", Bearer);
                    httpWebRequest.Method = "POST";
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        _Params_YarBox_Quote _param = new _Params_YarBox_Quote();
                        _param.count = param.count;
                        _param.destination = "";// param.destination_City + "(" + param.destination_State + ")";
                        _param.apPackingTypeId = param.apPackingTypeId;
                        _param.apTypeId = param.apTypeId;
                        _param.cityId = param.cityId;
                        var json = JsonConvert.SerializeObject(_param);
                        Common.Log("yarboxInputlog", json);
                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                    try
                    {
                        using (var response = await httpWebRequest.GetResponseAsync() as HttpWebResponse)
                        {

                            if (httpWebRequest.HaveResponse && response != null)
                            {
                                string result_temp;
                                using (var reader = new StreamReader(response.GetResponseStream()))
                                {
                                    result_temp = reader.ReadToEnd();
                                    Common.Log("yarboxlog", result_temp);
                                }
                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    _result.Status = true;
                                    _result.Message = "Ok";
                                    var t = JsonConvert.DeserializeObject<Detail_Result_Yarbox_Qute>(result_temp);
                                    t.price = t.price * 10;
                                    _result.Detail_Result_Yarbox_Qute = t;
                                    return _result;

                                }
                                else
                                {
                                    _result.Status = false;
                                    _result.Message = result_temp;
                                    return _result;
                                }

                            }
                            else
                            {
                                _result.Status = false;
                                _result.Message = "server not response";
                                return _result;

                            }
                        }
                    }
                    catch (WebException e)
                    {


                        if (e.Response != null)
                        {
                            using (var errorResponse = (HttpWebResponse)e.Response)
                            {
                                using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                                {
                                    string p = reader.ReadToEnd();
                                    _result.Status = false;
                                    _result.Message = e.Message;
                                    return _result;

                                }
                            }

                        }
                        else
                        {
                            _result.Status = false;
                            _result.Message = "server not response";
                            return _result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _result.Status = false;
                _result.Message = "An error has occurred in the Plugin::  " + ex.ToString();
                return _result;

            }
        }



        /// <summary>
        /// ای پی ای پیگیری سفارش<para />
        ///  نوع ای پی ای GET<para />
        /// حتما تنظیمات سرویس در ناچ کامرس چک شود که توکن و ادرس ای پی ای درست باشد<para />
        /// <para>Params_YarBox_Tracking ورودی</para>
        /// <para>Result_YarBox_Tracking خروجی</para>
        /// 
        /// </summary>
        public Result_YarBox_Tracking Tracking(Params_YarBox_Tracking param)
        {
            Result_YarBox_Tracking _result = new Result_YarBox_Tracking();
            //check params and setting
            try
            {
                if (string.IsNullOrEmpty((Bearer ?? "").Trim())// Bearer
                    || string.IsNullOrEmpty((_ShippingSettings.YarBox_URL_Tracking ?? "").Trim())
                    )
                {
                    _result.Status = false;
                    _result.Message = "Setting(Bearer or  YarBox_URL Tracking ) is Null!";
                    return _result;

                }
                else if (param.id == 0)
                {
                    _result.Status = false;
                    _result.Message = "id field must be completed";
                    return _result;
                }
                else
                {
                    String URL = string.Format(_ShippingSettings.YarBox_URL_Tracking, param.id);
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                    httpWebRequest.ContentType = "application/json; charset=utf-8";
                    httpWebRequest.Headers.Add("Authorization", Bearer);
                    httpWebRequest.Method = "GET";
                    try
                    {
                        using (var response = httpWebRequest.GetResponse() as HttpWebResponse)
                        {

                            if (httpWebRequest.HaveResponse && response != null)
                            {
                                string result_temp;
                                using (var reader = new StreamReader(response.GetResponseStream()))
                                {
                                    result_temp = reader.ReadToEnd();

                                }
                                if (response.StatusCode == HttpStatusCode.OK)
                                {

                                    _result.Status = true;
                                    _result.Message = "Ok";
                                    var t = JsonConvert.DeserializeObject<Detail_Tracking>(result_temp);
                                    _result.Detail_Tracking = t;
                                    return _result;

                                }
                                else
                                {
                                    _result.Status = false;
                                    _result.Message = result_temp;
                                    return _result;
                                }

                            }
                            else
                            {
                                _result.Status = false;
                                _result.Message = "server not response";
                                return _result;

                            }
                        }
                    }
                    catch (WebException e)
                    {


                        if (e.Response != null)
                        {
                            using (var errorResponse = (HttpWebResponse)e.Response)
                            {
                                using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                                {
                                    _result.Status = false;
                                    _result.Message = e.Message;
                                    return _result;

                                }
                            }

                        }
                        else
                        {
                            _result.Status = false;
                            _result.Message = "server not response";
                            return _result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _result.Status = false;
                _result.Message = "An error has occurred in the Plugin::" + ex.ToString();
                return _result;

            }

        }
    }
}
