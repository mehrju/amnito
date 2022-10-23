using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Taroff;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.Taroff;
using Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces;
using Nop.Services.Configuration;
using Nop.Services.Logging;
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
    public class Taroff_Service: ITaroff_Service
    {
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly ShippingSettings _ShippingSettings;
        private readonly ISettingService _settingService;

        public Taroff_Service(ILogger logger, IWorkContext workContext, ShippingSettings ShippingSettings, ISettingService settingService)
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
        /// ای پی ای خواندن استان ها<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی ندراد</para>
        /// <para>Result_Taroff_GetProvinces خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Taroff_GetProvinces GetProvinces()
        {
            Result_Taroff_GetProvinces Result = new Result_Taroff_GetProvinces();
            try
            {
                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.taroff_URL_GetProvinces ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(URLGetState) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.taroff_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }
                #endregion

                string result = "";
                //set params
                Params_Taroff_GetProvinces param = new Params_Taroff_GetProvinces();
                param.Token = _ShippingSettings.taroff_APP_AUTH;

                //
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.taroff_URL_GetProvinces);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                //httpWebRequest.Headers.Add("APP-AUTH", _ShippingSettings.Chapar_APP_AUTH);
                httpWebRequest.Method = "Post";
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
                                var t = JsonConvert.DeserializeObject<DetailResult_GetProvinces>(result);
                                if (t.state == "ok")
                                {
                                    Result.Status = true;
                                    Result.Message = "Ok";
                                    Result.categories = t.categories;
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.state;
                                }
                                return Result;
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
                                Result.Status = false;
                                Result.Message = e.Message;
                                return Result;
                            }
                        }
                    }
                    else
                    {
                        Result.Status = false;
                        Result.Message = "An error has occurred in the Service Taroff";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Taroff";
                return Result;
            }
        }

        /// <summary>
        /// ای پی ای خواندن شهر ها<para />
        /// نوع ای پی ای Post<para />
        /// <para>مدل ورودی Params_Taroff_GetCity </para>
        /// <para>در مدل ورودی، توکن از تنظیمات پر میشود و شما نیازی به تکمیل ان نداردید</para>
        /// <para>Result_Taroff_GetCity خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Taroff_GetCity GetCity(Params_Taroff_GetCity param)
        {
            Result_Taroff_GetCity Result = new Result_Taroff_GetCity();
            try
            {
                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.taroff_URL_GetCity ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(URLGetCity) is null";
                    return Result;
                }
                if (param.ProvinceId == 0)
                {
                    Result.Status = false;
                    Result.Message = "ProvinceId is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.taroff_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }
                #endregion

                string result = "";


                //set params
                param.Token = _ShippingSettings.taroff_APP_AUTH;

                //
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.taroff_URL_GetCity);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                //httpWebRequest.Headers.Add("APP-AUTH", _ShippingSettings.Chapar_APP_AUTH);
                httpWebRequest.Method = "Post";
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
                                var t = JsonConvert.DeserializeObject<DetailResult_GetCity>(result);
                                if (t.state == "ok")
                                {
                                    Result.Status = true;
                                    Result.Message = "Ok";
                                    Result.categories = t.categories;
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.state;
                                }
                                return Result;
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
                                Result.Status = false;
                                Result.Message = e.Message;
                                return Result;
                            }
                        }
                    }
                    else
                    {
                        Result.Status = false;
                        Result.Message = "An error has occurred in the Service Taroff";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Taroff";
                return Result;
            }
        }


        /// <summary>
        /// ای پی ای خواندن روش های پرداخت <para />
        /// نوع ای پی ای Post<para />
        /// <para> توکن از تنظیمات پر میشود و شما نیازی به تکمیل ان نداردید</para>
        /// <para>Result_Taroff_GetListPaymentMethods خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Taroff_GetListPaymentMethods GetListPaymentMethods()
        {
            Result_Taroff_GetListPaymentMethods Result = new Result_Taroff_GetListPaymentMethods();
            try
            {
                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.taroff_URL_GetListPaymentMethods ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(URL Get List Payment Methods) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.taroff_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }
                #endregion

                string result = "";


                //set params
                Params_Taroff_GetListPaymentMethods param = new Params_Taroff_GetListPaymentMethods();
                param.Token = _ShippingSettings.taroff_APP_AUTH;

                //
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.taroff_URL_GetListPaymentMethods);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                //httpWebRequest.Headers.Add("APP-AUTH", _ShippingSettings.Chapar_APP_AUTH);
                httpWebRequest.Method = "Post";
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
                                var t = JsonConvert.DeserializeObject<DetailResult_GetListPaymentMethods>(result);
                                if (t.state == "ok")
                                {
                                    Result.Status = true;
                                    Result.Message = "Ok";
                                    Result.categories = t.categories;
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.state;
                                }
                                return Result;
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
                                Result.Status = false;
                                Result.Message = e.Message;
                                return Result;
                            }
                        }
                    }
                    else
                    {
                        Result.Status = false;
                        Result.Message = "An error has occurred in the Service Taroff";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Taroff";
                return Result;
            }
        }

        /// <summary>
        /// ای پی ای خواندن لیست حامل ها <para />
        /// نوع ای پی ای Post<para />
        /// <para> توکن از تنظیمات پر میشود و شما نیازی به تکمیل ان نداردید</para>
        /// <para>Result_Taroff_GetListCarriers خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Taroff_GetListCarriers GetListCarriers()
        {
            Result_Taroff_GetListCarriers Result = new Result_Taroff_GetListCarriers();
            try
            {
                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.taroff_URL_GetListCarriers ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(URL Get List Carriers) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.taroff_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }
                #endregion

                string result = "";


                //set params
                Params_Taroff_GetListCarriers param = new Params_Taroff_GetListCarriers();
                param.Token = _ShippingSettings.taroff_APP_AUTH;

                //
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.taroff_URL_GetListCarriers);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                //httpWebRequest.Headers.Add("APP-AUTH", _ShippingSettings.Chapar_APP_AUTH);
                httpWebRequest.Method = "Post";
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
                                var t = JsonConvert.DeserializeObject<DetailResult_GetListCarriers>(result);
                                if (t.state == "ok")
                                {
                                    Result.Status = true;
                                    Result.Message = "Ok";
                                    Result.categories = t.categories;
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.state;
                                }
                                return Result;
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
                                Result.Status = false;
                                Result.Message = e.Message;
                                return Result;
                            }
                        }
                    }
                    else
                    {
                        Result.Status = false;
                        Result.Message = "An error has occurred in the Service Taroff";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Taroff";
                return Result;
            }
        }


        /// <summary>
        /// ای پی ای ثبت سفارش<para />
        /// نوع ای پی ای Post<para />
        /// <para>مدل ورودی Params_Taroff_CreateOrder </para>
        /// <para>در مدل ورودی، توکن از تنظیمات پر میشود و شما نیازی به تکمیل ان نداردید</para>
        /// <para>Result_Taroff_CreateOrder خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public async Task< Result_Taroff_CreateOrder> CreateOrder(Params_Taroff_CreateOrder param)
        {
            Result_Taroff_CreateOrder Result = new Result_Taroff_CreateOrder();
            try
            {
                #region Check Param
                var tt = param.IsValidParams_Taroff_CreateOrder();
                if (tt.Item1 == false)
                {
                    Result.Status = false;
                    Result.Message = tt.Item2;
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.taroff_URL_CreateOrder ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(URLCreateOrder) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.taroff_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }
                #endregion

                string result = "";


                //set params
                param.Token = _ShippingSettings.taroff_APP_AUTH;

                //
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.taroff_URL_CreateOrder);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                //httpWebRequest.Headers.Add("APP-AUTH", _ShippingSettings.Chapar_APP_AUTH);
                httpWebRequest.Method = "Post";
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
                    using (var response =await  httpWebRequest.GetResponseAsync() as HttpWebResponse)
                    {
                        if (httpWebRequest.HaveResponse && response != null)
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                result = reader.ReadToEnd();

                            }
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var t = JsonConvert.DeserializeObject<DetailResult_CreateOrder>(result);
                                if (t.status == "ok")
                                {
                                    Result.Status = true;
                                    Result.Message = "Ok";
                                    Result.DetailResult_CreateOrder = t;
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.status;
                                }
                                return Result;
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
                                Result.Status = false;
                                Result.Message = e.Message;
                                return Result;
                            }
                        }
                    }
                    else
                    {
                        Result.Status = false;
                        Result.Message = "An error has occurred in the Service Taroff";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Taroff";
                return Result;
            }
        }


        /// <summary>
        /// ای پی ای پیگیری سفارش<para />
        /// نوع ای پی ای Post<para />
        /// <para>مدل ورودی Params_Taroff_GetStateOrder </para>
        /// <para>در مدل ورودی، توکن از تنظیمات پر میشود و شما نیازی به تکمیل ان نداردید</para>
        /// <para>Result_Taroff_GetStateOrder خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Taroff_GetStateOrder GetStateOrder(Params_Taroff_GetStateOrder param)
        {
            Result_Taroff_GetStateOrder Result = new Result_Taroff_GetStateOrder();
            try
            {
                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.taroff_URL_GetStateOrder ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(URL Get State Order) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.taroff_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }
                if (param.OrderId <= 0)
                {
                    Result.Status = false;
                    Result.Message = "(Order Id in Get State Order) The value must be greater than zero";
                    return Result;
                }
                #endregion

                string result = "";


                //set params
                param.Token = _ShippingSettings.taroff_APP_AUTH;

                //
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.taroff_URL_GetStateOrder);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                //httpWebRequest.Headers.Add("APP-AUTH", _ShippingSettings.Chapar_APP_AUTH);
                httpWebRequest.Method = "Post";
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
                                var t = JsonConvert.DeserializeObject<DetailResult_Taroff_GetStateOrder>(result);
                                if (t.state == "ok")
                                {
                                    Result.Status = true;
                                    Result.Message = "Ok";
                                    Result.DetailResult_Taroff_GetStateOrder = t;
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.state;
                                }
                                return Result;
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
                                Result.Status = false;
                                Result.Message = e.Message;
                                return Result;
                            }
                        }
                    }
                    else
                    {
                        Result.Status = false;
                        Result.Message = "An error has occurred in the Service Taroff";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Taroff";
                return Result;
            }
        }


        /// <summary>
        /// ای پی ای اعلام اماده بودن بسته/نهایی کردن سفارش<para />
        /// نوع ای پی ای Post<para />
        /// <para>مدل ورودی Params_Taroff_SetStateReady </para>
        /// <para>در مدل ورودی، توکن از تنظیمات پر میشود و شما نیازی به تکمیل ان نداردید</para>
        /// <para>Result_Taroff_SetStateReady خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Taroff_SetStateReady SetStateReady(Params_Taroff_SetStateReady param)
        {
            Result_Taroff_SetStateReady Result = new Result_Taroff_SetStateReady();
            try
            {
                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.taroff_URL_SetStateReady ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(URL Set State Ready) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.taroff_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }
                if (param.OrderId <= 0)
                {
                    Result.Status = false;
                    Result.Message = "(Order Id in Get State Order) The value must be greater than zero";
                    return Result;
                }
                #endregion

                string result = "";


                //set params
                param.Token = _ShippingSettings.taroff_APP_AUTH;

                //
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.taroff_URL_SetStateReady);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                //httpWebRequest.Headers.Add("APP-AUTH", _ShippingSettings.Chapar_APP_AUTH);
                httpWebRequest.Method = "Post";
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
                                var t = JsonConvert.DeserializeObject<DetailResult_Taroff_SetStateReady>(result);
                                if (t.state == "ok")
                                {
                                    Result.Status = true;
                                    Result.Message = "Ok";
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.state;
                                }
                                return Result;
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
                                Result.Status = false;
                                Result.Message = e.Message;
                                return Result;
                            }
                        }
                    }
                    else
                    {
                        Result.Status = false;
                        Result.Message = "An error has occurred in the Service Taroff";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Taroff";
                return Result;
            }
        }

        /// <summary>
        /// ای پی ای کنسل کردن سفارش<para />
        /// نوع ای پی ای Post<para />
        /// <para>مدل ورودی Params_Taroff_SetStateCancel </para>
        /// <para>در مدل ورودی، توکن از تنظیمات پر میشود و شما نیازی به تکمیل ان نداردید</para>
        /// <para>Result_Taroff_SetStateCancel خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Taroff_SetStateCancel SetStateCancel(Params_Taroff_SetStateCancel param)
        {
            Result_Taroff_SetStateCancel Result = new Result_Taroff_SetStateCancel();
            try
            {
                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.taroff_URL_SetStateCancel ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(URL Set State Cancel) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.taroff_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }
                if (param.OrderId <= 0)
                {
                    Result.Status = false;
                    Result.Message = "(Order Id in Get State Order) The value must be greater than zero";
                    return Result;
                }
                #endregion

                string result = "";


                //set params
                param.Token = _ShippingSettings.taroff_APP_AUTH;

                //
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.taroff_URL_SetStateCancel);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                //httpWebRequest.Headers.Add("APP-AUTH", _ShippingSettings.Chapar_APP_AUTH);
                httpWebRequest.Method = "Post";
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
                                var t = JsonConvert.DeserializeObject<DetailResult_Taroff_SetStateCancel>(result);
                                if (t.state == "ok")
                                {
                                    Result.Status = true;
                                    Result.Message = "Ok";
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.state;
                                }
                                return Result;
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
                                Result.Status = false;
                                Result.Message = e.Message;
                                return Result;
                            }
                        }
                    }
                    else
                    {
                        Result.Status = false;
                        Result.Message = "An error has occurred in the Service Taroff";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Taroff";
                return Result;
            }
        }
    }
}
