using Nop.Services.Logging;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Chapar;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.Chapar;
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
    /// سرویس سفیرچاپار ورژن 20/6/98 
    /// </summary>
    public class Chapar_Service : IChapar_Service
    {
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly ShippingSettings _ShippingSettings;
        private readonly ISettingService _settingService;
        public Chapar_Service(ILogger logger, IWorkContext workContext, ShippingSettings ShippingSettings, ISettingService settingService)
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
        /// نوع ای پی ای Get<para />
        /// <para>مدل ورودی ندراد</para>
        /// <para>Result_Chapar_GetState خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Chapar_GetState GetState()
        {
            Result_Chapar_GetState Result = new Result_Chapar_GetState();
            try
            {
                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.Chapar_URL_GetState ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(URLGetState) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Chapar_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }
                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Chapar_URL_GetState);
                //httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("APP-AUTH", _ShippingSettings.Chapar_APP_AUTH);
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
                                var t = JsonConvert.DeserializeObject<Temp_Result_Chapar_GetState>(result);
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
                        Result.Message = "An error has occurred in the Service Chapar";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Chapar";
                return Result;
            }
        }
        /// <summary>
        /// ای پی ای خواندن شهر ها<para />
        /// نوع ای پی ای Post<para />
        /// <para>مدل ورودی Params_Chapar_GetCity </para>
        /// <para>Result_Chapar_GetCity خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Chapar_GetCity GetCity(Params_Chapar_GetCity param)
        {
            Result_Chapar_GetCity Result = new Result_Chapar_GetCity();
            try
            {
                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.Chapar_URL_City ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(URLGetcity) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Chapar_APP_AUTH ?? "").Trim()))
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
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Chapar_URL_City);
                //httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("APP-AUTH", _ShippingSettings.Chapar_APP_AUTH);
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
                                var t = JsonConvert.DeserializeObject<Temp_Result_Chapar_GetCity>(result);
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
                        Result.Message = "An error has occurred in the Service Chapar";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Chapar";
                return Result;
            }
        }
        /// <summary>
        /// ای پی ای استعلام قیمت<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_Chapar_GetQuote</para>
        /// <para>Result_Chapar_GetQuote خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public async Task<Result_Chapar_GetQuote> GetQuote(Params_Chapar_GetQuote param)
        {
            Result_Chapar_GetQuote Result = new Result_Chapar_GetQuote();
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
                if (string.IsNullOrEmpty((_ShippingSettings.Chapar_URL_GetQuote ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Chapar URL GetQuote) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Chapar_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }

                #endregion
                string JsonData = JsonConvert.SerializeObject(param);
                //This URL not exist, it's only an example.
                string url = _ShippingSettings.Chapar_URL_GetQuote;
                //Instantiate new CustomWebRequest class
                Chapar_CustomWebRequest wr = new Chapar_CustomWebRequest(url);
                // _logger.InsertLog(Core.Domain.Logging.LogLevel.Debug, JsonData);
                //Set values for parameters
                wr.ParamsCollection.Add(new ParamsStruct("input", JsonData));
                // wr.ParamsCollection.Add(new ParamsStruct("password", "bin"));
                //For file type, send the inputstream of selected file

                //PostData
                var Text = await wr.PostData(_ShippingSettings.Chapar_APP_AUTH);
                Common.Log("chaparLog", Text);
                //Set responsestring to textbox1
                //string Text = wr.ResponseString;
                var t = JsonConvert.DeserializeObject<Temp_Result_Chapar_GetQuote>(Text);
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
                Result.Message = "An error has occurred in the Service Chapar";
                return Result;
            }
        }
        /// <summary>
        /// ای پی ای  گزارش<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_Chapar_Report</para>
        /// <para>Result_Chapar_Report خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Chapar_Report Report(Params_Chapar_Report param)
        {
            Result_Chapar_Report Result = new Result_Chapar_Report();
            try
            {
                //ste param
                param.user.username = _ShippingSettings.Chapar_UserName;
                param.user.password = _ShippingSettings.Chapar_Password;
                #region Check Param
                var IsValidParam = param.IsValidParams_Chapar_Report();
                if (IsValidParam.Item1 == false)
                {
                    Result.Status = false;
                    Result.Message = IsValidParam.Item2;
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Chapar_URL_HistoryReport ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Chapar URL HistoryReport) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Chapar_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }


                #endregion
                var json = JsonConvert.SerializeObject(param);
                String URL = string.Format(_ShippingSettings.Chapar_URL_HistoryReport, json);
                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("APP-AUTH", _ShippingSettings.Chapar_APP_AUTH);
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
                                var t = JsonConvert.DeserializeObject<Temp_Result_Chapar_Report>(result);
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
                        Result.Message = "An error has occurred in the Service Chapar";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Chapar";
                return Result;
            }
        }
        /// <summary>
        /// ای پی ای گزارش محموله ها به صورت گروهی<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_Chapar_BulkHistoryReport</para>
        /// <para>Result_Chapar_BulkHistoryReport خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Chapar_BulkHistoryReport BulkHistoryReport(Params_Chapar_BulkHistoryReport param)
        {
            Result_Chapar_BulkHistoryReport Result = new Result_Chapar_BulkHistoryReport();
            try
            {
                //ste param
                param.user.username = _ShippingSettings.Chapar_UserName;
                param.user.password = _ShippingSettings.Chapar_Password;
                #region Check Param
                var IsValidParam = param.IsValidParams_Chapar_BulkHistoryReport();
                if (IsValidParam.Item1 == false)
                {
                    Result.Status = false;
                    Result.Message = IsValidParam.Item2;
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Chapar_URL_HistoryReport ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Chapar URL HistoryReport) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Chapar_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }


                #endregion
                var json = JsonConvert.SerializeObject(param);
                String URL = string.Format(_ShippingSettings.Chapar_URL_BulkHistoryReport, json);
                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("APP-AUTH", _ShippingSettings.Chapar_APP_AUTH);
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
                                var t = JsonConvert.DeserializeObject<Temp_Result_Chapar_BulkHistoryReport>(result);
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
                        Result.Message = "An error has occurred in the Service Chapar";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Chapar";
                return Result;
            }
        }

        /// <summary>
        /// ای پی ای ثبت بارنامه به صورت گروهی<para />
        /// <para> شما در این متد یک فرستنده دارید و چندین گیرنده</para>
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_Chapar_BulkImport</para>
        /// <para>Result_Chapar_Bulkimport خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public async Task<Result_Chapar_Bulkimport> Bulkimport(Params_Chapar_BulkImport param)
        {
            Result_Chapar_Bulkimport Result = new Result_Chapar_Bulkimport();
            try
            {
                // set param
                param.user = new User();


                if (param.bulk.First().cn.payment_term == 1)//COD
                {
                    param.user.username = "postkhone.cod";
                    param.user.password = "popo2021";
                }
                else
                {
                    param.user.username = _ShippingSettings.Chapar_UserName;
                    param.user.password = _ShippingSettings.Chapar_Password;
                }

                #region Check Param
                var CheckIsValidParam = param.IsValidParamBulkImport();
                if (CheckIsValidParam.Item1 == false)
                {
                    Result.Status = false;
                    Result.Message = CheckIsValidParam.Item2;
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Chapar_URL_BulkImport ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(URL BulkImport) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Chapar_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(APP AUTH) is null";
                    return Result;
                }
                #endregion

                string JsonData = JsonConvert.SerializeObject(param);

                string url = _ShippingSettings.Chapar_URL_BulkImport;
                //Instantiate new CustomWebRequest class
                Chapar_CustomWebRequest wr = new Chapar_CustomWebRequest(url);
                //Set values for parameters
                wr.ParamsCollection.Add(new ParamsStruct("input", JsonData));
                //PostData
                string Text = await wr.PostData(_ShippingSettings.Chapar_APP_AUTH);
                //Set responsestring to textbox1
                //string Text = wr.ResponseString;
                var t = JsonConvert.DeserializeObject<Temp_Result_Chapar_Bulkimport>(Text);
                if (t.result == true)
                {
                    Result.Status = true;
                    Result.Message = "Ok";
                    Result.objects = t.objects;
                }
                else
                {
                    Result.Status = false;
                    if (t.objects != null && t.objects.result != null && t.objects.result[0] != null && !string.IsNullOrEmpty(t.objects.result[0].error))
                        Result.Message = t.objects.result[0].error;
                    else
                        Result.Message = t.message.ToString();
                }
                return Result;
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Chapar";
                return Result;
            }
        }
        /// <summary>
        /// ای پی ای خواندن پیگیری<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_Chapar_Tracking</para>
        /// <para>Result_Chapar_Tracking خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Chapar_Tracking Tracking(Params_Chapar_Tracking param)
        {
            Result_Chapar_Tracking Result = new Result_Chapar_Tracking();
            try
            {
                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.Chapar_URL_TRACKING ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Chapar URL TRACKING) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Chapar_APP_AUTH ?? "").Trim()))
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
                //if (string.IsNullOrEmpty((param.order.lang ?? "").Trim()))
                //{
                //    Result.Status = false;
                //    Result.Message = "lang is null lang=fa";
                //    return Result;
                //}
                param.order.lang = "fa";
                #endregion

                string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
                string contentType = "multipart/form-data; boundary=" + formDataBoundary;
                var json = JsonConvert.SerializeObject(param);
                var dict = new Dictionary<string, object>();
                dict.Add("input", json);
                byte[] formData = GetMultipartFormData(dict, formDataBoundary);


                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Chapar_URL_TRACKING);
                httpWebRequest.ContentType = contentType;// "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("APP-AUTH", _ShippingSettings.Chapar_APP_AUTH);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = formData.Length;
                using (var streamWriter = httpWebRequest.GetRequestStream())
                {
                    // common.Log("chapar tracking", json);
                    streamWriter.Write(formData, 0, formData.Length);
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
                                var t = JsonConvert.DeserializeObject<Temp_Result_Chapar_Tracking>(result);
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
                        Result.Message = "An error has occurred in the Service Chapar";
                        return Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LogException(ex);
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Chapar";
                return Result;
            }
        }
        private byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Encoding encoding = Encoding.UTF8;
            using (MemoryStream formDataStream = new System.IO.MemoryStream())
            {
                bool needsCLRF = false;

                foreach (var param in postParameters)
                {
                    // Thanks to feedback from commenters, add a CRLF to allow multiple parameters to be added.
                    // Skip it on the first parameter, add it to subsequent parameters.
                    if (needsCLRF)
                        formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));
                    needsCLRF = true;

                    {
                        string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                            boundary,
                            param.Key,
                            param.Value);
                        formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                    }
                }
                return formDataStream.ToArray();
            }
        }
    }
}
