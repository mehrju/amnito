using Nop.Services.Logging;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.ShippingSolutions.Models.Params;
using Nop.Plugin.Misc.ShippingSolutions.Models.Params.PDE;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.PDE;
using Nop.Services.Configuration;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces;

namespace Nop.Plugin.Misc.ShippingSolutions.Services
{
    /// <summary>
    /// سرویس پست پی دی ای ورژن 1 در تاریخ 17/6/98<para />
    /// تنظمیات سرویس در سازنده از جدول تنظیمات خوانده میشود<para />
    /// توکن و پسورد و ccode در کالکشن موجود میباشد، این اطلاعات برای پست بار میباشد
    /// <para>API Countries ای پی لیست کشورها</para>
    /// <para>API cities ای پی ای لیست شهرها</para>
    /// <para>API IntenationalCalculator ای پی ای استعلام پست بین الملل</para>
    /// <para>API DomesticCalculator ای پی ای استعلام پست داخلی</para>
    /// <para>API </para>
    /// <para>Configuration تابع خواندن اطلاعات از تنظیمات</para>
    /// مراحل استفاده از سرویس<para />
    /// 1- خواندن اطلاعات از دو ای پی ای لیست کشور و شهرها<para />
    /// 2- استفاده از ای پی استعلام قیمت داخلی ، خارجی<para />
    /// 3- ثبت سفارش با استفاده از ای پی ای order<para />
    /// 4-  پیگیری سفارش با ای پی ایTracking Parcels<para />
    /// </summary>
    public class PDE_Service : IPDE_Service
    {
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly ShippingSettings _ShippingSettings;
        private readonly ISettingService _settingService;

        public PDE_Service(ILogger logger, IWorkContext workContext, ShippingSettings ShippingSettings, ISettingService settingService)
        {
            this._logger = logger;
            this._workContext = workContext;
            this._settingService = settingService;
            this._ShippingSettings = Configuration();
        }

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
        /// ای پی ای خواندن لیست کشورها<para />
        /// نوع ای پی ای Get<para />
        /// مدل ورودی ندارد<para />
        /// <para>ResultCountriesGET خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_PDE_CountriesGET Countries()
        {
            Result_PDE_CountriesGET _result = new Result_PDE_CountriesGET();

            try
            {
                //check params and setting
                if (string.IsNullOrEmpty((_ShippingSettings.PDE_Authorization ?? "").Trim())
                    || string.IsNullOrEmpty((_ShippingSettings.PDE_URLListOfCountries ?? "").Trim())
                    )
                {
                    _result.Status = false;
                    _result.Message = "Setting is Null!";
                    return _result;

                }
                else
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    using (WebClient objWebClient = new WebClient())
                    {
                        try
                        {
                            objWebClient.Headers[HttpRequestHeader.ContentType] = "application/json ; charset=utf-8";
                            string JsonResponse = objWebClient.DownloadString(_ShippingSettings.PDE_URLListOfCountries);
                            List<ItemsCountries> temp = JsonConvert.DeserializeObject<List<ItemsCountries>>(JsonResponse);
                            _result.Status = true;
                            _result.Message = "OK";
                            _result.ItemsCountries = temp;
                            return _result;

                        }
                        catch (Exception ex)
                        {
                            _result.Status = false;
                            _result.Message = ex.Message;
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
        /// ای پی ای خواندن لیست شهرها<para />
        /// نوع ای پی ای Get<para />
        /// مدل ورودی ندارد<para />
        /// <para>ResultCitiesGET خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_PDE_CitiesGET cities()
        {
            Result_PDE_CitiesGET _result = new Result_PDE_CitiesGET();

            try
            {
                //check params and setting
                if (string.IsNullOrEmpty((_ShippingSettings.PDE_Authorization ?? "").Trim())
                    || string.IsNullOrEmpty((_ShippingSettings.PDE_URLListOfCities ?? "").Trim())
                    )
                {
                    _result.Status = false;
                    _result.Message = "Setting is Null!";
                    return _result;

                }
                else
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    using (WebClient objWebClient = new WebClient())
                    {
                        try
                        {
                            objWebClient.Headers[HttpRequestHeader.ContentType] = "application/json ; charset=utf-8";
                            string JsonResponse = objWebClient.DownloadString(_ShippingSettings.PDE_URLListOfCities);
                            List<ItemsCities> temp = JsonConvert.DeserializeObject<List<ItemsCities>>(JsonResponse);
                            _result.Status = true;
                            _result.Message = "OK";
                            _result.ItemsCities = temp;
                            return _result;

                        }
                        catch (Exception ex)
                        {
                            _result.Status = false;
                            _result.Message = ex.Message;
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
        /// ای پی ای خواندن استعلام پست بین الملل<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_PDE_IntenationalCalculator</para>
        /// <para>Result_PDE_Calculator خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public async Task<Result_PDE_Calculator> IntenationalCalculator(Params_PDE_IntenationalCalculator Param)
        {
            Result_PDE_Calculator _result = new Result_PDE_Calculator();

            try
            {
                //check params and setting
                var IsValid = Param.IsValid_PDE_IntenationalCalculator();
                if (IsValid.Item1 == false)
                {
                    _result.Status = false;
                    _result.Message = IsValid.Item2.ToString();
                    return _result;
                }
                else if (string.IsNullOrEmpty((_ShippingSettings.PDE_Authorization ?? "").Trim())
                     || string.IsNullOrEmpty((_ShippingSettings.PDE_URLIntenationalCalculator ?? "").Trim())
                     )
                {
                    _result.Status = false;
                    _result.Message = "Setting is Null!";
                    return _result;

                }
                else
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    using (WebClient objWebClient = new WebClient())
                    {
                        try
                        {
                            //set setting to param
                            Param.Password = _ShippingSettings.PDE_Password;
                            Param.Ccode = _ShippingSettings.PDE_Ccode;
                            //
                            objWebClient.Headers[HttpRequestHeader.ContentType] = "application/json ; charset=utf-8";
                            objWebClient.Encoding = UTF8Encoding.UTF8;
                            string JsonData = JsonConvert.SerializeObject(Param);
                            objWebClient.Headers[HttpRequestHeader.Authorization] = string.Format(_ShippingSettings.PDE_Authorization);
                            //string Response =await objWebClient.UploadStringTaskAsync(_ShippingSettings.PDE_URLIntenationalCalculator, JsonData);
                            string Response =await objWebClient.UploadStringTaskAsync(_ShippingSettings.PDE_URLIntenationalCalculator, JsonData);
                            if (Response != null)
                            {
                                Response = Response.Trim('"');
                                int index = Response.IndexOf(@"***");
                                string Model = Response.Substring(0, index);
                                Model = Model.Replace(@"\", "");
                                string Description = Response.Substring(index);
                                PriceCalculator temp = JsonConvert.DeserializeObject<PriceCalculator>(Model);

                                _result.Status = true;
                                _result.Message = "OK";
                                _result.PriceCalculator = temp;
                                _result.Description = Description;
                                return _result;
                            }
                            else
                            {
                                _result.Status = false;
                                _result.Message = "Server Not Response";
                                return _result;
                            }


                        }
                        catch (Exception ex)
                        {
                            _result.Status = false;
                            _result.Message = ex.Message;
                            return _result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                _result.Status = false;
                _result.Message = "An error has occurred in the Plugin::" + ex.ToString();
                return _result;

            }

        }
        /// <summary>
        /// ای پی ای خواندن استعلام پست داخلی<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_PDE_DomesticCalculator</para>
        /// <para>Result_PDE_Calculator خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public async Task<Result_PDE_Calculator> DomesticCalculator(Params_PDE_DomesticCalculator Param)
        {
            Result_PDE_Calculator _result = new Result_PDE_Calculator();

            try
            {
                //check params and setting
                var IsValid = Param.IsValid_PDE_DomesticCalculator();
                if (IsValid.Item1 == false)
                {
                    _result.Status = false;
                    _result.Message = IsValid.Item2.ToString();
                    return _result;
                }
                else if (string.IsNullOrEmpty((_ShippingSettings.PDE_Authorization ?? "").Trim())
                     || string.IsNullOrEmpty((_ShippingSettings.PDE_URLIntenationalCalculator ?? "").Trim())
                     )
                {
                    _result.Status = false;
                    _result.Message = "Setting is Null!";
                    return _result;

                }
                else
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    using (WebClient objWebClient = new WebClient())
                    {
                        try
                        {
                            //set setting to param
                            Param.Password = _ShippingSettings.PDE_Password;
                            Param.Ccode = _ShippingSettings.PDE_Ccode;
                            //
                            objWebClient.Headers[HttpRequestHeader.ContentType] = "application/json ; charset=utf-8";
                            objWebClient.Encoding = UTF8Encoding.UTF8;
                            string JsonData = JsonConvert.SerializeObject(Param);
                            objWebClient.Headers[HttpRequestHeader.Authorization] = string.Format(_ShippingSettings.PDE_Authorization);
                           // string Response = await objWebClient.UploadStringTaskAsync(_ShippingSettings.PDE_URLDomesticCalculator, JsonData);
                            string Response =await objWebClient.UploadStringTaskAsync(_ShippingSettings.PDE_URLDomesticCalculator, JsonData);
                            if (Response != null)
                            {
                                Response = Response.Trim('"');
                                Response = Response.Replace(@"\", "");
                                PriceCalculator temp = JsonConvert.DeserializeObject<PriceCalculator>(Response);
                                _result.Status = true;
                                _result.Message = "OK";
                                _result.PriceCalculator = temp;
                                return _result;
                            }
                            else
                            {
                                _result.Status = false;
                                _result.Message = "Server Not Response";
                                return _result;
                            }


                        }
                        catch (Exception ex)
                        {
                            _result.Status = false;
                            _result.Message = ex.Message;
                            return _result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                _result.Status = false;
                _result.Message = "An error has occurred in the Plugin::" + ex.ToString();
                return _result;

            }

        }


        /// <summary>
        /// ای پی ای پیگیری مرسوله<para />
        /// نوع ای پی ای Get<para />
        /// <para>Params_PDE_TrackingParcels مدل خروجی</para>
        /// <para>Result_PDE_TrackingParcels خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_PDE_TrackingParcels TrackingParcels(Params_PDE_TrackingParcels param)
        {
            Result_PDE_TrackingParcels _result = new Result_PDE_TrackingParcels();

            try
            {
                //check params and setting
                var IsValid = param.IsValid_PDE_TrackingParcels();
                if (IsValid.Item1 == false)
                {
                    _result.Status = false;
                    _result.Message = IsValid.Item2.ToString();
                    return _result;
                }
                else if (string.IsNullOrEmpty((_ShippingSettings.PDE_Authorization ?? "").Trim())
                    || string.IsNullOrEmpty((_ShippingSettings.PDE_URLTrackingParcels ?? "").Trim())
                    )
                {
                    _result.Status = false;
                    _result.Message = "Setting is Null!";
                    return _result;
                }
                else
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    using (WebClient objWebClient = new WebClient())
                    {
                        try
                        {
                            string URL = _ShippingSettings.PDE_URLTrackingParcels + "/" + param.IdOrder.ToString();

                            objWebClient.Headers[HttpRequestHeader.ContentType] = "application/json ; charset=utf-8";
                            objWebClient.Encoding = UTF8Encoding.UTF8;
                            objWebClient.Headers[HttpRequestHeader.Authorization] = string.Format(_ShippingSettings.PDE_Authorization);
                            string JsonResponse = objWebClient.DownloadString(URL);
                            if (JsonResponse != null)
                            {
                                JsonResponse = JsonResponse.Remove(JsonResponse.Length - 1);
                                JsonResponse = JsonResponse.Substring(1);

                                _result.Status = true;
                                _result.Message = "OK";
                                _result.Detail = JsonConvert.DeserializeObject<DetailOrder>(JsonResponse); ;
                                return _result;
                            }
                            else
                            {
                                _result.Status = false;
                                _result.Message = "Server Not Response";
                                return _result;
                            }
                        }
                        catch (Exception ex)
                        {
                            _result.Status = false;
                            _result.Message = ex.Message;
                            return _result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                _result.Status = false;
                _result.Message = "An error has occurred in the Plugin::" + ex.ToString();
                return _result;

            }

        }



        /// <summary>
        /// ای پی ای خواندن ثبت سفارش بین الملل<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_PDE_RegisterInternationalOrder</para>
        /// <para>Result_PDE_RegisterInternationalOrder خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public async Task<Result_PDE_RegisterOrder> RegisterInternationalOrder(Params_PDE_RegisterInternationalOrder Param)
        {
            Result_PDE_RegisterOrder _result = new Result_PDE_RegisterOrder();

            try
            {
                //check params and setting
                var IsValid = Param.IsValid_PDE_RegisterInternationalOrder();
                if (IsValid.Item1 == false)
                {
                    _result.Status = false;
                    _result.Message = IsValid.Item2.ToString();
                    return _result;
                }
                else if (string.IsNullOrEmpty((_ShippingSettings.PDE_Authorization ?? "").Trim())
                     || string.IsNullOrEmpty((_ShippingSettings.PDE_URLRegisterInternationalOrder ?? "").Trim())
                     )
                {
                    _result.Status = false;
                    _result.Message = "Setting is Null!";
                    return _result;

                }
                else
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    using (WebClient objWebClient = new WebClient())
                    {
                        try
                        {
                            //set config to param
                            Param.Password = _ShippingSettings.PDE_Password;
                            Param.CustCode = _ShippingSettings.PDE_Ccode;
                            Param.ClientId = _ShippingSettings.PDE_ClientId;
                            //
                            objWebClient.Headers[HttpRequestHeader.ContentType] = "application/json ; charset=utf-8";
                            objWebClient.Encoding = UTF8Encoding.UTF8;
                            string JsonData = JsonConvert.SerializeObject(Param);
                            objWebClient.Headers[HttpRequestHeader.Authorization] = string.Format(_ShippingSettings.PDE_Authorization);
                           // string Response =await objWebClient.UploadStringTaskAsync(_ShippingSettings.PDE_URLRegisterInternationalOrder, JsonData);
                            string Response =await objWebClient.UploadStringTaskAsync(_ShippingSettings.PDE_URLRegisterInternationalOrder, JsonData);

                            if (Response != null)
                            {
                                Response = Response.Trim('"');
                                Int64 temp = 0;
                                bool isNumeric = Int64.TryParse(Response, out temp);
                                if (isNumeric == true)
                                {
                                    _result.Status = true;
                                    _result.Message = "OK";
                                    _result.KEY = temp;
                                    return _result;
                                }
                                else
                                {
                                    _result.Status = false;
                                    _result.Message = Response;
                                    return _result;
                                }

                            }
                            else
                            {
                                _result.Status = false;
                                _result.Message = "Server Not Response";
                                return _result;
                            }


                        }
                        catch (Exception ex)
                        {
                            common.LogException(ex);
                            _result.Status = false;
                            _result.Message = ex.Message;
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
        /// ای پی ای خواندن ثبت سفارش داخلی<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_PDE_RegisterDomesticOrder</para>
        /// <para>Result_PDE_RegisterOrder خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public async Task<Result_PDE_RegisterOrder> RegisterDomesticOrder(Params_PDE_RegisterDomesticOrder Param)
        {
            Result_PDE_RegisterOrder _result = new Result_PDE_RegisterOrder();

            try
            {
                //check params and setting
                var IsValid = Param.IsValid_PDE_RegisterDomesticOrder();
                if (IsValid.Item1 == false)
                {
                    _result.Status = false;
                    _result.Message = IsValid.Item2.ToString();
                    return _result;
                }
                else if (string.IsNullOrEmpty((_ShippingSettings.PDE_Authorization ?? "").Trim())
                     || string.IsNullOrEmpty((_ShippingSettings.PDE_URLRegisterDomesticOrder ?? "").Trim())
                     )
                {
                    _result.Status = false;
                    _result.Message = "Setting is Null!";
                    return _result;

                }
                else
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    using (WebClient objWebClient = new WebClient())
                    {
                        try
                        {
                            //set config to param
                            Param.Password = _ShippingSettings.PDE_Password;
                            Param.CustCode = _ShippingSettings.PDE_Ccode;
                            Param.ClientId = _ShippingSettings.PDE_ClientId;
                            //
                            objWebClient.Headers[HttpRequestHeader.ContentType] = "application/json ; charset=utf-8";
                            objWebClient.Encoding = UTF8Encoding.UTF8;
                            string JsonData = JsonConvert.SerializeObject(Param);
                            objWebClient.Headers[HttpRequestHeader.Authorization] = string.Format(_ShippingSettings.PDE_Authorization);
                            //string Response =await objWebClient.UploadStringTaskAsync(_ShippingSettings.PDE_URLRegisterDomesticOrder, JsonData);
                            string Response =await objWebClient.UploadStringTaskAsync(_ShippingSettings.PDE_URLRegisterDomesticOrder, JsonData);

                            if (Response != null)
                            {
                                Response = Response.Trim('"');
                                Int64 temp = 0;
                                bool isNumeric = Int64.TryParse(Response, out temp);
                                if (isNumeric == true)
                                {
                                    _result.Status = true;
                                    _result.Message = "OK";
                                    _result.KEY = temp;
                                    return _result;
                                }
                                else
                                {
                                    _result.Status = false;
                                    _result.Message = Response;
                                    return _result;
                                }

                            }
                            else
                            {
                                _result.Status = false;
                                _result.Message = "Server Not Response";
                                return _result;
                            }


                        }
                        catch (Exception ex)
                        {
                            _result.Status = false;
                            _result.Message = ex.Message;
                            return _result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                _result.Status = false;
                _result.Message = "An error has occurred in the Plugin::" + ex.ToString();
                return _result;

            }

        }




    }
}
