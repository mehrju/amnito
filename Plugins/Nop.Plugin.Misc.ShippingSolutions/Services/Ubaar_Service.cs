using Nop.Services.Logging;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Ubbar;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.Ubbar;
using Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces;
using Nop.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Services
{
    public class Ubaar_Service : IUbaar_Service
    {
        /// <summary>
        /// سرویس شرکت اوبار در تاریخ 19/6/98
        /// در تنظیمات پلاگین در ناپ کامرس حتما تنظیمات را ثبت نمایید</para>
        ///مراحل استفاده از سرویس مذکور</para>
        ///-1 دریافت لیست مناطق</para>
        ///2- استعلام قیمت</para>
        ///3- ثبت بسته و محموله</para>
        ///لیست توابع</para>
        ///1- ای پی ای دریافت لیست مناطق Regionlist</para>
        ///2- ای پی ای استعلام قیمت Priceenquiry</para>
        ///3- ای پی ای ثبت سفارش Modifyorder</para>
        ///Configuration تابع دریافت تنظیمات </para>
        /// </summary>
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly ShippingSettings _ShippingSettings;
        private readonly ISettingService _settingService;

        public Ubaar_Service(ILogger logger, IWorkContext workContext, ShippingSettings ShippingSettings, ISettingService settingService)
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
        /// API Modifyorder
        /// <para> Input: Params_Ubaar_modifyorder</para>
        /// <para> Output: Result_Ubaar_modifyorder</para>
        /// <para> Type Method: POST</para>
        /// </summary>
        public async Task<Result_Ubaar_modifyorder> Modifyorder(Params_Ubaar_modifyorder param)
        {
            Result_Ubaar_modifyorder Result = new Result_Ubaar_modifyorder();
            try
            {
                #region check params
                var ParamsValid = param.IsValidParamsUbaarmodifyorder();
                if (ParamsValid.Item1 == false)
                {
                    Result.Status = false;
                    Result.Message = ParamsValid.Item2;
                    return Result;

                }
                if (_ShippingSettings.Ubaar_Urlmodifyorder == "" || _ShippingSettings.Ubaar_Urlmodifyorder == null)
                {
                    Result.Status = false;
                    Result.Message = "Setting  is Null";
                    return Result;

                }
                if (_ShippingSettings.Ubaar_APIToken == "" || _ShippingSettings.Ubaar_APIToken == null
                   || _ShippingSettings.Ubaar_USERToken == "" || _ShippingSettings.Ubaar_USERToken == null
                   )
                {
                    Result.Status = false;
                    Result.Message = "Setting is null";
                    return Result;
                }

                #endregion
                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Ubaar_Urlmodifyorder);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("APIToken", _ShippingSettings.Ubaar_APIToken);
                httpWebRequest.Headers.Add("USERToken", _ShippingSettings.Ubaar_USERToken);
                httpWebRequest.Method = "PUT";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    var json = JsonConvert.SerializeObject(param);
                    Debug.Write(json);
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                try
                {

                    using (var response =await httpWebRequest.GetResponseAsync() as HttpWebResponse)
                    {

                        if (httpWebRequest.HaveResponse && response != null)
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                result = reader.ReadToEnd();
                            }
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var t = JsonConvert.DeserializeObject<DetailUbaarmodifyorder>(result);
                                if (t.success_flag == 1)
                                {
                                    Result.Status = true;
                                    Result.Message = "OK";
                                    Result.DetailUbaarmodifyorder = t;
                                    return Result;

                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = "در حال حاضر امکان ثبت سفارش در اوبار وجود ندارد";
                                    Result.DetailUbaarmodifyorder = t;
                                    return Result;
                                }
                            }
                            else
                            {
                                Result.Status = false;
                                Result.Message = result;
                                return Result;
                            }

                        }
                        else
                        {
                            Result.Status = false;
                            Result.EnMessage = "server not response";
                            Result.Message = "در حال حاضر امکان ثبت سفارش در اوبار وجود ندارد";
                            return Result;
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
                                string error = reader.ReadToEnd();
                                Result.Status = false;
                                Result.Message = e.Message;
                                return Result;
                            }
                        }

                    }
                    else
                    {
                        Result.Status = false;
                        Result.Message = "An error has occurred in the PLugin";
                        return Result;
                    }
                }



            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the PLugin";
                return Result;
            }

        }
        /// <summary>
        /// API Priceenquiry
        /// <para> Input: Params_Ubaar_priceenquiry</para>
        /// <para> Output: Result_Ubaar_priceenquiry</para>
        /// <para> Type Method: POST</para>
        /// </summary>
        public async Task<Result_Ubaar_priceenquiry> Priceenquiry(Params_Ubaar_priceenquiry param)
        {
            Result_Ubaar_priceenquiry Result = new Result_Ubaar_priceenquiry();
            try
            {
                #region check params
                var z = param.IsValidParamParams_Ubaar_priceenquiry();
                if (string.IsNullOrEmpty((_ShippingSettings.Ubaar_Urlregionlist ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "url  is Null";
                    return Result;

                }
                if (
                     string.IsNullOrEmpty((_ShippingSettings.Ubaar_APIToken ?? "").Trim())
                     || string.IsNullOrEmpty((_ShippingSettings.Ubaar_USERToken ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting Is Null";
                    return Result;
                }
                if (z.Item1 == false)
                {
                    Result.Status = false;
                    Result.Message = z.Item2;
                    return Result;
                }

                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Ubaar_Urlpriceenquiry);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("APIToken", _ShippingSettings.Ubaar_APIToken);
                httpWebRequest.Headers.Add("USERToken", _ShippingSettings.Ubaar_USERToken);
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {

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
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                result = reader.ReadToEnd();
                            }
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var t = JsonConvert.DeserializeObject<DetailUbaarPriceInquiry>(result);
                                if (t.success_flag == 1)
                                {
                                    Result.Status = true;
                                    Result.Message = "OK";
                                    Result.DetailUbaarPriceInquiry = t;
                                    string _result = JsonConvert.SerializeObject(Result);
                                    return Result;

                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = "error";
                                    Result.DetailUbaarPriceInquiry = t;
                                    return Result;
                                }
                            }
                            else
                            {
                                Result.Status = false;
                                Result.Message = result;
                                return Result;
                            }

                        }
                        else
                        {
                            Result.Status = false;
                            Result.EnMessage = "server not response";
                            Result.Message = "در حال حاضر امکان محاسبه قیمت در مسیر درخواستی شما وجود ندارد";
                            return Result;
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
                                string error = reader.ReadToEnd();
                                result = error;
                                Result.Status = false;
                                Result.EnMessage = e.Message;
                                Result.Message = "در حال حاضر امکان محاسبه قیمت در مسیر درخواستی شما وجود ندارد";
                                return Result;
                            }
                        }

                    }
                    else
                    {
                        Result.Status = false;
                        Result.EnMessage = "An error has occurred in the PLugin";
                        Result.Message = "در حال حاضر امکان محاسبه قیمت در مسیر درخواستی شما وجود ندارد";
                        return Result;
                    }
                }

            }
            catch
            {
                Result.Status = false;
                Result.EnMessage = "An error has occurred in the PLugin";
                Result.Message = "در حال حاضر امکان محاسبه قیمت در مسیر درخواستی شما وجود ندارد";
                return Result;
            }

        }
        /// <summary>
        /// API Regionlist
        /// <para> Input: Params_Ubaar_regionlist</para>
        /// <para>در مدل وردی شما میتوانید مقادیر را خالی بگذارید تا تمام مناطق کشور را به شما بدهد</para>
        /// <para> Output: Result_Ubaar_regionlist</para>
        /// <para> Type Method: POST</para>
        /// </summary>
        /// 
        public Result_Ubaar_regionlist Regionlist(Params_Ubaar_regionlist param)
        {
            Result_Ubaar_regionlist Result = new Result_Ubaar_regionlist();
            try
            {
                #region check params
                if (string.IsNullOrEmpty((_ShippingSettings.Ubaar_Urlregionlist ?? "").Trim())
                    )
                {
                    Result.Status = false;
                    Result.Message = "url  is Null";
                    return Result;

                }
                if (string.IsNullOrEmpty((_ShippingSettings.Ubaar_APIToken ?? "").Trim())
                    || string.IsNullOrEmpty((_ShippingSettings.Ubaar_USERToken ?? "").Trim())
                    )
                {
                    Result.Status = false;
                    Result.Message = "Settimg Is Null!";
                    return Result;
                }
                //if (
                //    string.IsNullOrEmpty((param.region_city ?? "").Trim())
                //    || string.IsNullOrEmpty((param.region_name ?? "").Trim()))
                //{
                //    Result.Status = false;
                //    Result.Message = "Params  is Null";
                //    return Result;
                //}
                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Ubaar_Urlregionlist);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("APIToken", _ShippingSettings.Ubaar_APIToken);
                httpWebRequest.Headers.Add("USERToken", _ShippingSettings.Ubaar_USERToken);
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {

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
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                result = reader.ReadToEnd();
                            }
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var t = JsonConvert.DeserializeObject<DetailRegionList>(result);
                                if (t.success_flag == 1)
                                {
                                    Result.Status = true;
                                    Result.Message = "OK";
                                    Result.DetailRegionList = t;
                                    return Result;

                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = "error";
                                    Result.DetailRegionList = t;
                                    return Result;
                                }
                            }
                            else
                            {
                                Result.Status = false;
                                Result.Message = result;
                                return Result;
                            }

                        }
                        else
                        {
                            Result.Status = false;
                            Result.Message = "server not response";
                            return Result;
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
                                string error = reader.ReadToEnd();
                                result = error;
                                Result.Status = false;
                                Result.Message = e.Message;
                                return Result;
                            }
                        }

                    }
                    else
                    {
                        Result.Status = false;
                        Result.Message = "An error has occurred in the PLugin";
                        return Result;
                    }
                }

            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the PLugin";
                return Result;
            }

        }


        /// <summary>
        /// API Tracking
        /// <para> Input: Params_Ubaar_Tracking</para>
        /// <para>در مدل وردی شما میتوانید مقادیر را خالی بگذارید تا تمام مناطق کشور را به شما بدهد</para>
        /// <para> Output: Result_Ubbar_Tracking</para>
        /// <para> Type Method: POST</para>
        /// </summary>
        /// 
        public Result_Ubbar_Tracking Tracking(Params_Ubaar_Tracking param)
        {
            Result_Ubbar_Tracking Result = new Result_Ubbar_Tracking();
            try
            {
                #region check params
                if (string.IsNullOrEmpty((_ShippingSettings.Ubaar_UrlTracking ?? "").Trim())
                    )
                {
                    Result.Status = false;
                    Result.Message = "url Tracking Ubaar is Null";
                    return Result;

                }
                if (string.IsNullOrEmpty((_ShippingSettings.Ubaar_APIToken ?? "").Trim())
                    || string.IsNullOrEmpty((_ShippingSettings.Ubaar_USERToken ?? "").Trim())
                    )
                {
                    Result.Status = false;
                    Result.Message = "Settimg Ubaar Is Null!";
                    return Result;
                }
                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Ubaar_UrlTracking);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("APIToken", _ShippingSettings.Ubaar_APIToken);
                httpWebRequest.Headers.Add("USERToken", _ShippingSettings.Ubaar_USERToken);
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {

                    var json = JsonConvert.SerializeObject(param);
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
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                result = reader.ReadToEnd();
                            }
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var t = JsonConvert.DeserializeObject<DetailResiltTracking>(result);
                                if (t.success_flag == 1)
                                {
                                    Result.Status = true;
                                    Result.Message = "OK";
                                    Result.DetailResiltTracking = t;
                                    return Result;

                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = "error";
                                    Result.DetailResiltTracking = t;
                                    return Result;
                                }
                            }
                            else
                            {
                                Result.Status = false;
                                Result.Message = result;
                                return Result;
                            }

                        }
                        else
                        {
                            Result.Status = false;
                            Result.Message = "server not response";
                            return Result;
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
                                string error = reader.ReadToEnd();
                                result = error;
                                Result.Status = false;
                                Result.Message = e.Message;
                                return Result;
                            }
                        }

                    }
                    else
                    {
                        Result.Status = false;
                        Result.Message = "An error has occurred in the PLugin";
                        return Result;
                    }
                }

            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the PLugin";
                return Result;
            }

        }
    }
}
