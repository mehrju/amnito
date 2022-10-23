using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.ShippingSolutions.Models.Params.SkyBlue;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.SkyBlue;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.Snappbox;
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
    public class SkyBlue_Service : ISkyBlue_Service
    {
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly ShippingSettings _ShippingSettings;
        private readonly ISettingService _settingService;

        public SkyBlue_Service(ILogger logger, IWorkContext workContext, ShippingSettings ShippingSettings, ISettingService settingService)
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
        /// ای پی ای استعلام خدمت رسانی به کشور مذکور<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_SkyBlue_CheckService</para>
        /// <para>Result_SkyBlue_CheckService خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public async Task<Result_SkyBlue_CheckService> CheckService(Params_SkyBlue_CheckService param)
        {
            Result_SkyBlue_CheckService Result = new Result_SkyBlue_CheckService();
            try
            {


                #region Check Param

                if (string.IsNullOrEmpty((_ShippingSettings.SkyBlue_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(SkyBlue APP AUTH) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((param.CountryCode ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(SkyBlue CountryCode) is null";
                    return Result;
                }
                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.SkyBlue_URL_CheckService);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("token", _ShippingSettings.SkyBlue_APP_AUTH);
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
                    using (var response = await httpWebRequest.GetResponseAsync() as HttpWebResponse)
                    {
                        if (httpWebRequest.HaveResponse && response != null)
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                result = reader.ReadToEnd();

                            }
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var t = JsonConvert.DeserializeObject<Detail_CheckService>(result);
                                if (t.Result == "YES")
                                {
                                    Result.Status = true;
                                    Result.Message = "Ok";
                                    Result.Detail_CheckService = t;
                                    return Result;
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.Message;
                                    Result.Detail_CheckService = t;
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
                                Result.Status = false;
                                Result.Message = e.Message;
                                return Result;
                            }
                        }
                    }
                    else
                    {
                        Result.Status = false;
                        Result.Message = "An error has occurred in the Service SkyBlue";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service SkyBlue";
                return Result;
            }
        }

        /// <summary>
        /// ای پی ای استعلام قیمت<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_SkyBlue_ParcelPrice</para>
        /// <para>Result_Snappbox_GetPrice خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public async Task<Result_SkyBlue_ParcelPrice> GetParcelPrice(Params_SkyBlue_ParcelPrice param)
        {
            Result_SkyBlue_ParcelPrice Result = new Result_SkyBlue_ParcelPrice();
            try
            {


                #region Check Param
                var tt = param.IsValidParams_SkyBlue_ParcelPrice();
                if (tt.Item1 == false)
                {
                    Result.Status = false;
                    Result.Message = tt.Item2;
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.SkyBlue_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(SkyBlue APP AUTH) is null";
                    return Result;
                }

                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.SkyBlue_URL_ParcelPrice);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("token", _ShippingSettings.SkyBlue_APP_AUTH);
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
                    using (var response = await httpWebRequest.GetResponseAsync() as HttpWebResponse)
                    {
                        if (httpWebRequest.HaveResponse && response != null)
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                result = reader.ReadToEnd();

                            }
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var t = JsonConvert.DeserializeObject<DetailParcelPrice>(result);
                                if (string.IsNullOrEmpty(t.errorMessage))
                                {
                                    Result.Status = true;
                                    Result.Message = "Ok";
                                    Result.DetailParcelPrice = t;
                                    return Result;
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.errorMessage;
                                    Result.DetailParcelPrice = t;
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
                    common.LogException(e);
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
                        Result.Message = "An error has occurred in the Service SkyBlue";
                        return Result;
                    }
                }
            }
            catch(Exception ex)
            {
                common.LogException(ex);
                Result.Status = false;
                Result.Message = "An error has occurred in the Service SkyBlue";
                return Result;
            }
        }

        /// <summary>
        /// ای پی ای ثبت سفارش<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_SkyBlue_RegisterOrder</para>
        /// <para>Result_Snappbox_GetPrice خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public async Task<Result_SkyBlue_RegisterOrder> RegisterOrder(Params_SkyBlue_RegisterOrder param)
        {
            Result_SkyBlue_RegisterOrder Result = new Result_SkyBlue_RegisterOrder();
            try
            {


                #region Check Param
                var tt = param.IsValidParams_SkyBlue_RegisterOrder();
                if (tt.Item1 == false)
                {
                    Result.Status = false;
                    Result.Message = tt.Item2;
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.SkyBlue_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(SkyBlue APP AUTH) is null";
                    return Result;
                }

                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.SkyBlue_URL_RegisterOrder);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("token", _ShippingSettings.SkyBlue_APP_AUTH);
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
                    using (var response = await httpWebRequest.GetResponseAsync() as HttpWebResponse)
                    {
                        if (httpWebRequest.HaveResponse && response != null)
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                result = reader.ReadToEnd();

                            }
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var t = JsonConvert.DeserializeObject<DetailRegisterOrder>(result);
                                if (string.IsNullOrEmpty(t.errorMessage))
                                {
                                    Result.Status = true;
                                    Result.Message = "Ok";
                                    Result.DetailRegisterOrder = t;
                                    return Result;
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.errorMessage;
                                    Result.DetailRegisterOrder = t;
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
                                Result.Status = false;
                                Result.Message = e.Message;
                                return Result;
                            }
                        }
                    }
                    else
                    {
                        Result.Status = false;
                        Result.Message = "An error has occurred in the Service SkyBlue";
                        return Result;
                    }
                }
            }
            catch(Exception ex)
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service SkyBlue";
                return Result;
            }
        }

        /// <summary>
        /// ای پی ای کنسل کردن<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_SkyBlue_Cancel_Tracking</para>
        /// <para>Result_SkyBlue_Cancell خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public async Task<Result_SkyBlue_Cancell> Cancell(Params_SkyBlue_Cancel_Tracking param)
        {
            Result_SkyBlue_Cancell Result = new Result_SkyBlue_Cancell();
            try
            {


                #region Check Param

                if (string.IsNullOrEmpty((_ShippingSettings.SkyBlue_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(SkyBlue APP AUTH) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((param.OrderNumber ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(SkyBlue OrderNumber) is null";
                    return Result;
                }
                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.SkyBlue_URL_Cancel);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("token", _ShippingSettings.SkyBlue_APP_AUTH);
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
                    using (var response = await httpWebRequest.GetResponseAsync() as HttpWebResponse)
                    {
                        if (httpWebRequest.HaveResponse && response != null)
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                result = reader.ReadToEnd();

                            }
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var t = JsonConvert.DeserializeObject<Detail_sk_Camcell>(result);
                                if (t.Result == "Ok")
                                {
                                    Result.Status = true;
                                    Result.Message = "Ok";
                                    Result.Detail_sk_Camcell = t;
                                    return Result;
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.Message;
                                    Result.Detail_sk_Camcell = t;
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
                                Result.Status = false;
                                Result.Message = e.Message;
                                return Result;
                            }
                        }
                    }
                    else
                    {
                        Result.Status = false;
                        Result.Message = "An error has occurred in the Service SkyBlue";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service SkyBlue";
                return Result;
            }
        }


        /// <summary>
        /// ای پی ای ترکینگ<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_SkyBlue_Cancel_Tracking</para>
        /// <para>Result_SkyBlue_Tracking خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public  Result_SkyBlue_Tracking Tracking(Params_SkyBlue_Cancel_Tracking param)
        {
            Result_SkyBlue_Tracking Result = new Result_SkyBlue_Tracking();
            try
            {


                #region Check Param

                if (string.IsNullOrEmpty((_ShippingSettings.SkyBlue_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(SkyBlue APP AUTH) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((param.OrderNumber ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(SkyBlue OrderNumber) is null";
                    return Result;
                }
                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.SkyBlue_URL_Tracking);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("token", _ShippingSettings.SkyBlue_APP_AUTH);
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
                    using (var response =  httpWebRequest.GetResponse() as HttpWebResponse)
                    {
                        if (httpWebRequest.HaveResponse && response != null)
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                result = reader.ReadToEnd();

                            }
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var t = JsonConvert.DeserializeObject<List<Detail_sk_Tracking>>(result);
                                Result.Status = true;
                                Result.Message = "Ok";
                                Result.Detail_sk_Tracking = t;
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
                        Result.Message = "An error has occurred in the Service SkyBlue";
                        return Result;
                    }
                }
            }
            catch(Exception ex)
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service SkyBlue";
                return Result;
            }
        }
    }
}
