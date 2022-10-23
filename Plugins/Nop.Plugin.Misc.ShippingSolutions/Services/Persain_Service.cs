using Nop.Services.Logging;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Persain;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.Persain;
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
    public class Persain_Service : IPersian_Service
    {
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly ShippingSettings _ShippingSettings;
        private readonly ISettingService _settingService;

        public Persain_Service(ILogger logger, IWorkContext workContext, ShippingSettings ShippingSettings, ISettingService settingService)
        {
            this._logger = logger;
            this._workContext = workContext;
            this._ShippingSettings = Configuration();
            this._settingService = settingService;
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
        /// ای پی ای ثبت کالا-محموله
        /// <para> Input: Params_Persian_NewCustomer</para>
        /// <para> OutPut: Result_Persian_NewCustomer</para>
        /// <para> Type:POST</para>
        /// </summary>
        public Result_Persian_NewCustomer NewCustomer(Params_Persian_NewCustomer param)
        {

            Result_Persian_NewCustomer Result = new Result_Persian_NewCustomer();
            try
            {
                //set setting
                param.password = _ShippingSettings.Persain_Password;
                param.username = _ShippingSettings.Persain_UserName;
                #region check params
                var z = param.IsValidParamsNewCustomer();
                if (z.Item1 == false)
                {

                    Result.Status = false;
                    Result.Message = z.Item2;
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Persain_URL_NewCustomer ?? "").Trim()))

                {
                    Result.Status = false;
                    Result.Message = "Setting(URL NewCustomer)  is Null";
                    return Result;

                }
                if (string.IsNullOrEmpty((_ShippingSettings.Persain_Password ?? "").Trim()))

                {
                    Result.Status = false;
                    Result.Message = "Setting(Password)  is Null";
                    return Result;

                }
                if (string.IsNullOrEmpty((_ShippingSettings.Persain_UserName ?? "").Trim()))

                {
                    Result.Status = false;
                    Result.Message = "Setting(UserName)  is Null";
                    return Result;

                }
                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Persain_URL_NewCustomer);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
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
                            Detail_Result_Persian_NewCustomer t = JsonConvert.DeserializeObject<Detail_Result_Persian_NewCustomer>(result);
                            if (response.StatusCode == HttpStatusCode.OK)
                            {

                                if (t.status == "done")
                                {
                                    Result.Status = true;
                                    Result.Message = "OK";
                                    Result.Detail_Result_Persian_NewCustomer = t;
                                    return Result;

                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.message;
                                    Result.Detail_Result_Persian_NewCustomer = t;
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
                        Result.Message = "An error has occurred in the Service Tinex";
                        return Result;
                    }
                }

            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Tinex";
                return Result;
            }

        }
        /// <summary>
        /// ای پی ای پیگیری محموله
        /// <para> Input: Params_Persian_ViewCustomer</para>
        /// <para> OutPut: Result_Persian_ViewCustomer</para>
        /// <para> Type:POST</para>
        /// </summary>
        public Result_Persian_ViewCustomer ViewCustomer(Params_Persian_ViewCustomer param)
        {
            Result_Persian_ViewCustomer Result = new Result_Persian_ViewCustomer();
            try
            {
                //set setting
                param.password = _ShippingSettings.Persain_Password;
                param.username = _ShippingSettings.Persain_UserName;

                #region check params
                if (string.IsNullOrEmpty((_ShippingSettings.Persain_URL_ViewCustomer ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(URL ViewCustomer)  is Null";
                    return Result;

                }
                if (string.IsNullOrEmpty((_ShippingSettings.Persain_Password ?? "").Trim()))

                {
                    Result.Status = false;
                    Result.Message = "Setting(Password)  is Null";
                    return Result;

                }
                if (string.IsNullOrEmpty((_ShippingSettings.Persain_UserName ?? "").Trim()))

                {
                    Result.Status = false;
                    Result.Message = "Setting(UserName)  is Null";
                    return Result;

                }
                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Persain_URL_ViewCustomer);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
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
                            Deatil_Result_Persian_ViewCustomer t = JsonConvert.DeserializeObject<Deatil_Result_Persian_ViewCustomer>(result);
                            if (response.StatusCode == HttpStatusCode.OK)
                            {

                                if (t.status == "ok")
                                {
                                    Result.Status = true;
                                    Result.Message = t.data.status;
                                    Result.Deatil_Result_Persian_ViewCustomer = t;
                                    return Result;

                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = result;
                                    Result.Deatil_Result_Persian_ViewCustomer = t;
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
                        Result.Message = "An error has occurred in the Service Tinex";
                        return Result;
                    }
                }

            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Tinex";
                return Result;
            }
        }


    }
}
