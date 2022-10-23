using Nop.Services.Logging;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.Safiran;
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
    /// <summary>
    /// سرویس mDTS ورژن 20/6/98 
    /// </summary>
    public class Safiran_Service : ISafiran_Service
    {
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly ShippingSettings _ShippingSettings;
        private readonly ISettingService _settingService;
        public Safiran_Service(ILogger logger, IWorkContext workContext, ShippingSettings ShippingSettings, ISettingService settingService)
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
        /// ای پی ای خواندن پیگیری<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_Safiran_Tracking</para>
        /// <para>Result_Safiran_Tracking خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Safiran_Tracking Tracking(Params_Safiran_Tracking param)
        {
            Result_Safiran_Tracking Result = new Result_Safiran_Tracking();
            try
            {
                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.Safiran_URL_TRACKING ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Safiran URL TRACKING) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Safiran_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((param.order.reference ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "reference is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((param.order.lang ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "lang is null lang=fa";
                    return Result;
                }
                param.order.lang = "fa";
                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Safiran_URL_TRACKING);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("APP-AUTH", _ShippingSettings.Safiran_APP_AUTH);
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
                                var t = JsonConvert.DeserializeObject<Temp_Result_Safiran_Tracking>(result);
                                if (t.result == true)
                                {
                                    Result.Status = true;
                                    Result.Message = "Ok";
                                    Result.objects = t.objects;
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.message;
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
                        Result.Message = "An error has occurred in the Service Safiran";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Safiran";
                return Result;
            }
        }
        /// <summary>
        /// ای پی ای ثبت بارنامه<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_Safiran_PickupRequest</para>
        /// <para>Result_Safiran_PickupRequest خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public async Task<Result_Safiran_PickupRequest> Pickup(Params_Safiran_PickupRequest param)
        {
            Result_Safiran_PickupRequest Result = new Result_Safiran_PickupRequest();
            try
            {
                // set param
                param.pickup_man = _ShippingSettings.Safiran_PickupMan;
                param.sender.code = _ShippingSettings.Safiran_SenderCode;
                #region Check Param
                var CheckIsValidParam = param.IsValidParamPickupRequest();
                if (CheckIsValidParam.Item1 == false)
                {
                    Result.Status = false;
                    Result.Message = CheckIsValidParam.Item2;
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Safiran_URL_PickupRequest ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(URLPickupRequest) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Safiran_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }
                #endregion

                string JsonData = JsonConvert.SerializeObject(param);
                //This URL not exist, it's only an example.
                string url = _ShippingSettings.Safiran_URL_PickupRequest;
                //Instantiate new CustomWebRequest class
                Safiran_CustomWebRequest wr = new Safiran_CustomWebRequest(url);
                //Set values for parameters
                wr.ParamsCollection.Add(new ParamsStruct("input", JsonData));
                // wr.ParamsCollection.Add(new ParamsStruct("password", "bin"));
                //For file type, send the inputstream of selected file
                wr.ParamsCollection.Add(new ParamsStruct("signature", ""));
                wr.ParamsCollection.Add(new ParamsStruct("scanned_documents", ""));
                //PostData
                string Text =await wr.PostData(_ShippingSettings.Safiran_APP_AUTH);
                var t = JsonConvert.DeserializeObject<Temp_Result_Safiran_PickupRequest>(Text);
                if (t.result == true)
                {
                    Result.Status = true;
                    Result.Message = "Ok";
                    Result.objects = t.objects;
                }
                else
                {
                    Result.Status = false;
                    Result.Message = t.message;
                }
                return Result;
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Safiran";
                return Result;
            }
        }
        /// <summary>
        /// ای پی ای خواندن استان ها<para />
        /// نوع ای پی ای Get<para />
        /// <para>مدل ورودی ندراد</para>
        /// <para>Result_Safiran_GetState خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Safiran_GetState GetState()
        {
            Result_Safiran_GetState Result = new Result_Safiran_GetState();
            try
            {
                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.Safiran_URL_GetState ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(URLGetState) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Safiran_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }
                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Safiran_URL_GetState);
                //httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("APP-AUTH", _ShippingSettings.Safiran_APP_AUTH);
                httpWebRequest.Method = "GET";

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
                                var t = JsonConvert.DeserializeObject<Temp_Result_Safiran_GetState>(result);
                                if (t.result == true)
                                {
                                    Result.Status = true;
                                    Result.Message = "Ok";
                                    Result.objects = t.objects;
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.message;
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
                        Result.Message = "An error has occurred in the Service Safiran";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Safiran";
                return Result;
            }
        }
        /// <summary>
        /// ای پی ای خواندن شهر ها<para />
        /// نوع ای پی ای Post<para />
        /// <para>مدل ورودی Params_Safiran_GetCity </para>
        /// <para>Result_Safiran_GetCity خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Safiran_GetCity GetCity(Params_Safiran_GetCity param)
        {
            Result_Safiran_GetCity Result = new Result_Safiran_GetCity();
            try
            {
                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.Safiran_URL_City ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(URLGetcity) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Safiran_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }
                if (param.state.no <= 0)
                {
                    Result.Status = false;
                    Result.Message = "No State is null";
                    return Result;
                }
                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Safiran_URL_City);
                //httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("APP-AUTH", _ShippingSettings.Safiran_APP_AUTH);
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
                                var t = JsonConvert.DeserializeObject<Temp_Result_Safiran_GetCity>(result);
                                if (t.result == true)
                                {
                                    Result.Status = true;
                                    Result.Message = "OK";
                                    Result.objects = t.objects;
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.message;

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
                        Result.Message = "An error has occurred in the Service Safiran";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Safiran";
                return Result;
            }
        }
        /// <summary>
        /// ای پی ای استعلام قیمت<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_Safiran_GetQuote</para>
        /// <para>Result_Safiran_GetQuote خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public async Task<Result_Safiran_GetQuote> GetQuote(Params_Safiran_GetQuote param)
        {
            Result_Safiran_GetQuote Result = new Result_Safiran_GetQuote();
            try
            {
                #region Check Param
                var IsValid = param.IsValidParamsGetQuote();
                if (IsValid.Item1 == false)
                {
                    Result.Status = false;
                    Result.Message = IsValid.Item2;
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Safiran_URL_GetQuote ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Safiran URL GetQuote) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Safiran_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }

                #endregion
                string JsonData = JsonConvert.SerializeObject(param);
                //This URL not exist, it's only an example.
                string url = _ShippingSettings.Safiran_URL_GetQuote;
                //Instantiate new CustomWebRequest class
                Safiran_CustomWebRequest wr = new Safiran_CustomWebRequest(url);
                // _logger.InsertLog(Core.Domain.Logging.LogLevel.Debug, JsonData);
                //Set values for parameters
                wr.ParamsCollection.Add(new ParamsStruct("input", JsonData));
                // wr.ParamsCollection.Add(new ParamsStruct("password", "bin"));
                //For file type, send the inputstream of selected file

                //PostData
                var Text =await wr.PostData(_ShippingSettings.Safiran_APP_AUTH);
                //Set responsestring to textbox1
                //string Text = wr.ResponseString;
                var t = JsonConvert.DeserializeObject<Temp_Result_Safiran_GetQuote>(Text);
                if (t == null)
                {
                    Result.Status = false;
                    Result.Message = "قیمت دریافت نشد";
                }
                if (t.result == true)
                {
                    if (t.objects.order.quote == 0)
                    {
                        Result.Status = false;
                        Result.Message = "باتوجه به مبدا و مقصد این سرویس ارائه نمی شود";
                        return Result;
                    }
                    Result.Status = true;
                    Result.Message = "OK";
                    Result.objects = t.objects;
                }
                else
                {
                    Result.Status = false;
                    Result.Message = t.message;
                }

                return Result;
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Safiran";
                return Result;
            }
        }
        /// <summary>
        /// ای پی ای  گزارش<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_Safiran_Report</para>
        /// <para>Result_Safiran_Report خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Safiran_Report Report(Params_Safiran_Report param)
        {
            Result_Safiran_Report Result = new Result_Safiran_Report();
            try
            {
                //ste param
                param.user.username = _ShippingSettings.Safiran_UserName;
                param.user.password = _ShippingSettings.Safiran_Password;
                #region Check Param
                var IsValidParam = param.IsValidParams_Safiran_Report();
                if (IsValidParam.Item1 == false)
                {
                    Result.Status = false;
                    Result.Message = IsValidParam.Item2;
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Safiran_URL_HistoryReport ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Safiran URL HistoryReport) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Safiran_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }


                #endregion
                var json = JsonConvert.SerializeObject(param);
                String URL = string.Format(_ShippingSettings.Safiran_URL_HistoryReport, json);
                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("APP-AUTH", _ShippingSettings.Safiran_APP_AUTH);
                httpWebRequest.Method = "POST";

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
                                var t = JsonConvert.DeserializeObject<Temp_Result_Safiran_Report>(result);
                                if (t.result == true)
                                {
                                    Result.Status = true;
                                    Result.Message = "Ok";
                                    Result.objects = t.objects;
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.message;
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
                        Result.Message = "An error has occurred in the Service Safiran";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Safiran";
                return Result;
            }
        }
        /// <summary>
        /// ای پی ای ثبت بارنامه به صورت گروهی<para />
        /// <para> شما در این متد یک فرستنده دارید و چندین گیرنده</para>
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_Safiran_BulkImport</para>
        /// <para>Result_Safiran_Bulkimport خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Safiran_Bulkimport Bulkimport(Params_Safiran_BulkImport param)
        {
            Result_Safiran_Bulkimport Result = new Result_Safiran_Bulkimport();
            try
            {
                // set param
                param.user.username = _ShippingSettings.Safiran_UserName;
                param.user.password = _ShippingSettings.Safiran_Password;
                #region Check Param
                var CheckIsValidParam = param.IsValidParamBulkImport();
                if (CheckIsValidParam.Item1 == false)
                {
                    Result.Status = false;
                    Result.Message = CheckIsValidParam.Item2;
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Safiran_URL_BulkImport ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(URL BulkImport) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Safiran_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }
                #endregion

                string JsonData = JsonConvert.SerializeObject(param);

                string url = _ShippingSettings.Safiran_URL_BulkImport;
                //Instantiate new CustomWebRequest class
                Safiran_CustomWebRequest wr = new Safiran_CustomWebRequest(url);
                //Set values for parameters
                wr.ParamsCollection.Add(new ParamsStruct("input", JsonData));
                //PostData
                wr.PostData(_ShippingSettings.Safiran_APP_AUTH);
                //Set responsestring to textbox1
                string Text = wr.ResponseString;
                var t = JsonConvert.DeserializeObject<Temp_Result_Safiran_Bulkimport>(Text);
                if (t.result == true)
                {
                    Result.Status = true;
                    Result.Message = "Ok";
                    Result.objects = t.objects;
                }
                else
                {
                    Result.Status = false;
                    Result.Message = t.message.ToString();
                }
                return Result;


            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Safiran";
                return Result;
            }
        }
        /// <summary>
        /// ای پی ای گزارش محموله ها به صورت گروهی<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_Safiran_BulkHistoryReport</para>
        /// <para>Result_Safiran_BulkHistoryReport خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Safiran_BulkHistoryReport BulkHistoryReport(Params_Safiran_BulkHistoryReport param)
        {
            Result_Safiran_BulkHistoryReport Result = new Result_Safiran_BulkHistoryReport();
            try
            {
                //ste param
                param.user.username = _ShippingSettings.Safiran_UserName;
                param.user.password = _ShippingSettings.Safiran_Password;
                #region Check Param
                var IsValidParam = param.IsValidParams_Safiran_BulkHistoryReport();
                if (IsValidParam.Item1 == false)
                {
                    Result.Status = false;
                    Result.Message = IsValidParam.Item2;
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Safiran_URL_HistoryReport ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Safiran URL HistoryReport) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Safiran_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }


                #endregion
                var json = JsonConvert.SerializeObject(param);
                String URL = string.Format(_ShippingSettings.Safiran_URL_BulkHistoryReport, json);
                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("APP-AUTH", _ShippingSettings.Safiran_APP_AUTH);
                httpWebRequest.Method = "POST";
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
                                var t = JsonConvert.DeserializeObject<Temp_Result_Safiran_BulkHistoryReport>(result);
                                if (t.result == true)
                                {
                                    Result.Status = true;
                                    Result.Message = "Ok";
                                    Result.objects = t.objects;
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.message;
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
                        Result.Message = "An error has occurred in the Service Safiran";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Safiran";
                return Result;
            }
        }


        /// <summary>
        /// ای پی ای کنسل کردن بارنامه<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_Safiran_Cancel</para>
        /// <para>Result_Safiran_Cancel خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public async Task<Result_Safiran_Cancel> Cancel(Params_Safiran_Cancel param)
        {
            Result_Safiran_Cancel Result = new Result_Safiran_Cancel();
            try
            {

                #region Check Param
                var IsValidParam = param.IsValidParams_Safiran_Cancel();
                if (IsValidParam.Item1 == false)
                {
                    Result.Status = false;
                    Result.Message = IsValidParam.Item2;
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Safiran_URL_Cancel ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Safiran URL Cancel) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Safiran_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }
                #endregion
                string JsonData = JsonConvert.SerializeObject(param);
                //This URL not exist, it's only an example.
                string url = _ShippingSettings.Safiran_URL_Cancel;
                //Instantiate new CustomWebRequest class
                Safiran_CustomWebRequest wr = new Safiran_CustomWebRequest(url);
                //Set values for parameters
                wr.ParamsCollection.Add(new ParamsStruct("input", JsonData));
                // wr.ParamsCollection.Add(new ParamsStruct("password", "bin"));
                //For file type, send the inputstream of selected file

                //PostData
                string Text =await wr.PostData(_ShippingSettings.Safiran_APP_AUTH);
                //Set responsestring to textbox1
                // string Text = wr.ResponseString;
                var t = JsonConvert.DeserializeObject<Detail_Safiran_Cancel>(Text);
                if (t.result == true)
                {
                    Result.Status = true;
                    Result.Message = "Ok";
                    Result.Detail_Safiran_Cancel = t;
                }
                else
                {
                    Result.Status = false;
                    Result.Message = t.message;
                }
                return Result;





            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Safiran";
                return Result;
            }
        }


    }
}
