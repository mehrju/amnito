using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.ShippingSolutions.Models.Params.TPG;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.TGP;
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
    public class TPG_Service : ITPG_Service
    {
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly ShippingSettings _ShippingSettings;
        private readonly ISettingService _settingService;

        public TPG_Service(ILogger logger, IWorkContext workContext, ShippingSettings ShippingSettings, ISettingService settingService)
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
        /// ای پی ای استعلام قیمت<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_TPG_Compute</para>
        /// <para>Result_TGP_Compute خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public async Task<Result_TGP_Compute> Compute(Params_TPG_Compute param)
        {
            Result_TGP_Compute Result = new Result_TGP_Compute();
            try
            {
                //set param
                param.ContractId = _ShippingSettings.TPG_ContractId;
                param.ContractCode = _ShippingSettings.TPG_ContractCode;
                param.Client = 62195;
                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.TPG_Password ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(TPG Password) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.TPG_UserName ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(TPG UserName) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.TPG_Url_compute ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(TPG_Url_compute) is null";
                    return Result;
                }
                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.TPG_Url_compute);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(_ShippingSettings.TPG_UserName + ":" + _ShippingSettings.TPG_Password));
                httpWebRequest.Headers.Add("Authorization", "Basic " + encoded);
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
                                var t = JsonConvert.DeserializeObject<Deatil_Result_TGP_Compute>(result);
                                Result.Status = true;
                                Result.Message = "Ok";
                                Result.Deatil_Result_TGP_Compute = t;
                                Result.RequestId = response.GetResponseHeader("RequestId");

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
                        Result.Message = "An error has occurred in the Service TPG";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service TPG";
                return Result;
            }
        }
        /// <summary>
        /// ای پی ای ثبت بارنامه<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_TPG_Pickup</para>
        /// <para>Result_TPG_Pickup خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public async Task<Result_TPG_Pickup> Pickup(Params_TPG_Pickup param)
        {
            Result_TPG_Pickup Result = new Result_TPG_Pickup();
            try
            {
                //set param
                param.ContractId = _ShippingSettings.TPG_ContractId;
                param.UserId = _ShippingSettings.TPG_UserId;
                param.Client = 62195;
                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.TPG_Password ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(TPG Password) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.TPG_UserName ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(TPG UserName) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.TPG_Url_Pickup ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(TPG_Url_Pickup) is null";
                    return Result;
                }
                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.TPG_Url_Pickup);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(_ShippingSettings.TPG_UserName + ":" + _ShippingSettings.TPG_Password));
                httpWebRequest.Headers.Add("Authorization", "Basic " + encoded);
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
                    using (var response =await httpWebRequest.GetResponseAsync() as HttpWebResponse)
                    {
                        if (httpWebRequest.HaveResponse && response != null)
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                result = reader.ReadToEnd();

                            }
                            if (response.StatusCode == HttpStatusCode.Created)
                            {
                                var t = JsonConvert.DeserializeObject<Deatil_TPG_Pickup>(result);
                                if (t.Code == 201)
                                {
                                    Result.Status = true;
                                    Result.Message = "Ok";
                                    Result.Deatil_TPG_Pickup = t;
                                    Result.RequestId = response.GetResponseHeader("RequestId");
                                    return Result;
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = result;
                                    Result.CodeStatus = t.Code;
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
                        Result.Message = "An error has occurred in the Service TPG";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service TPG";
                return Result;
            }
        }

        /// <summary>
        /// ای پی ای پیگیری<para />
        /// نوع ای پی ای GET<para />
        /// <para>مدل ورودی Params_TPG_Receipt</para>
        /// <para>Result_TPG_Receipt خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_TPG_Tracking Tracking(Params_TPG_Tracking param)
        {
            Result_TPG_Tracking Result = new Result_TPG_Tracking();
            try
            {

                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.TPG_Password ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(TPG Password) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.TPG_UserName ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(TPG UserName) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.TPG_Url_Tracking ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(TPG_Url_Tracking) is null";
                    return Result;
                }
                #endregion

                string result = "";
                string URL = string.Format(_ShippingSettings.TPG_Url_Tracking, param.CN);
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(_ShippingSettings.TPG_UserName + ":" + _ShippingSettings.TPG_Password));
                httpWebRequest.Headers.Add("Authorization", "Basic " + encoded);
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
                                var t = JsonConvert.DeserializeObject<RootObject>(result);

                                Result.Status = true;
                                Result.Message = "Ok";
                                Result.RootObject = t;
                                Result.RequestId = response.GetResponseHeader("RequestId");
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
                        Result.Message = "An error has occurred in the Service TPG";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service TPG";
                return Result;
            }
        }
    }
}
