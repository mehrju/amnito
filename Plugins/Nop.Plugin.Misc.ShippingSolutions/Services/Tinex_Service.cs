using Nop.Services.Logging;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Tinex;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.Tinex;
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
    /// سرویس Tinex در تاریخ 20/6/98
    ///<para>به دلیل توکن ارائه شده در سرویس تینکست تصمیم بر این شده که از روش singelton pattern استفاده شود</para>
    ///<para>singelton pattern</para>
    ///<para>با توجه به اینکه مدت زمان انقضای توکن ارائه شده یک روز میباشد در کلاس سازنده چک میشود </para>
    ///<para>چک کردن زمان انقضای توکن</para>
    ///<para>تابعی در سرویس ارائه شده برای تست زمان انقضا وجود ندارد</para>
    ///<para>پس در بار اولی که این کلاس ساخته میشود یک رکوئست به سرویس داریم</para>
    ///<para>توکن ساخته شده به دیگر توابع پاس داده خواهد شد</para>
    ///<para>API Configuration تابع خواندن تنظیمات فروشگاه</para>
    ///<para>API Token ای پی ای دریافت توکن سرویس</para>
    ///<para> مراحل و لیست API ها</para>
    ///<para>API GetCost ای پی ای استعلام قیمت</para>
    ///<para>API Insert ثبت محموله ای پی ای </para>
    ///<para>API InsertCommit ای پی ای تایید سفارش</para>
    ///<para>API cancelreasons ای پی ای دریافت لیست دلایل کنسل کردن</para>
    ///<para>API Cancel ای پی ای کسل کردن سفارش</para>

    /// </summary>
    public class Tinex_Service : ITinex_Service
    {

        private static Tinex_Service instance = null;
        private static readonly object Instancelock = new object();
        private static string Bearer = "";
        private DateTime DateStart = new DateTime();
        private bool isfirst = true;
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly ShippingSettings _ShippingSettings;
        private readonly ISettingService _settingService;

        private Tinex_Service(ILogger logger, IWorkContext workContext, ShippingSettings ShippingSettings, ISettingService settingService)
        {
            this._logger = logger;
            this._workContext = workContext;
            this._settingService = settingService;


            this._ShippingSettings = Configuration();


            if (isfirst)
            {
                DateStart = DateTime.Now;
                isfirst = false;
                //Bearer
                Result_Tinex_Token r = Token();
                if (r.Status == true)
                {
                    Bearer = r.DetailTinexToken.access_token;
                }


            }
            if (DateStart != null)
            {
                DateTime Now = DateTime.Now;
                double Day = (Now - DateStart).TotalDays;
                if (Day >= _ShippingSettings.Tinex_ExpireDayToken)
                {
                    //new Bearer
                    Result_Tinex_Token r = Token();
                    if (r.Status == true)
                    {
                        Bearer = r.DetailTinexToken.access_token;
                    }
                }

            }


        }

        ///<summary> singelton pattern
        ///<para>line1</para>
        ///<para>line2</para>
        ///</summary>
        ///
        public static Tinex_Service GetInstance(ILogger logger, IWorkContext workContext, ShippingSettings ShippingSettings, ISettingService settingService)
        {

            lock (Instancelock)
            {
                if (instance == null)
                {
                    instance = new Tinex_Service(logger, workContext, ShippingSettings, settingService);
                }
                return instance;
            }

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



        ///<summary> تابع ساخت توکن که به صورت خصوصی است
        ///<para>پارامترهای ورودی از کلاس Adminsetting</para>
        ///<para>ادرس متد از شی URL</para>
        ///<para>خروجی داخل کلاس داده میشودActionResult</para>
        ///</summary>
        ///
        private Result_Tinex_Token Token()
        {
            Result_Tinex_Token Result = new Result_Tinex_Token();
            try
            {
                #region check params and Class constructor
                if (string.IsNullOrEmpty((_ShippingSettings.Tinex_UrlToken ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(url Token) Is Null.";
                    return Result;

                }
                if (string.IsNullOrEmpty((_ShippingSettings.Tinex_grant_type ?? "").Trim())
                   || string.IsNullOrEmpty((_ShippingSettings.Tinex_client_id ?? "").Trim())
                   || string.IsNullOrEmpty((_ShippingSettings.Tinex_client_secret ?? "").Trim())
                   || string.IsNullOrEmpty((_ShippingSettings.Tinex_scope ?? "").Trim())
                     )
                {
                    Result.Status = false;
                    Result.Message = "Setting(Tinex_grant_type or Tinex_client_id or Tinex_client_secret or Tinex_scope) Is Null.";
                    return Result;
                }
                #endregion
                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Tinex_UrlToken);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    Params_Tinex_Token AD = new Params_Tinex_Token();
                    AD.Tinex_client_id = _ShippingSettings.Tinex_client_id;
                    AD.Tinex_client_secret = _ShippingSettings.Tinex_client_secret;
                    AD.Tinex_grant_type = _ShippingSettings.Tinex_grant_type;
                    AD.Tinex_scope = _ShippingSettings.Tinex_scope;

                    var json = JsonConvert.SerializeObject(AD);
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
                                Result.Status = true;
                                Result.Message = "OK";
                                Result.DetailTinexToken = JsonConvert.DeserializeObject<DetailTinexToken>(result);

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
                            Result.Message = "The server is not responding";
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
        /// API GET Cost
        /// <para> Input: Params_Tinex_Get_Cost</para>
        /// <para> OutPut: Result_Tinex_GetCost</para>
        /// <para> Type:POST</para>
        /// </summary>
        public Result_Tinex_GetCost GetCost(Params_Tinex_Get_Cost param)
        {
            Result_Tinex_GetCost Result = new Result_Tinex_GetCost();
            try
            {
                #region check params
                //1 check Beare
                //2 check 20 k 
                if (Bearer == null)
                {
                    Result.Status = false;
                    Result.Message = "Beare Code is Null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Tinex_UrlGetCost ?? "").Trim()))

                {
                    Result.Status = false;
                    Result.Message = "url  is Null";
                    return Result;

                }
                if (param.sub_orders.Count == 0)
                {
                    Result.Status = false;
                    Result.Message = "The list sub_order must have at least one member";
                    return Result;
                }
                float TotalWeight = 0;
                if (param.sub_orders.Count > 0)
                {
                    foreach (var item in param.sub_orders)
                    {
                        TotalWeight += item.weight;
                    }

                    if (TotalWeight > 20)
                    {
                        Result.Status = false;
                        Result.Message = "The total weight should be less than 20 kg";
                        return Result;
                    }
                }

                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Tinex_UrlGetCost);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("Authorization", Bearer);
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
                            Detail_TinexGetCost t = JsonConvert.DeserializeObject<Detail_TinexGetCost>(result);
                            if (response.StatusCode == HttpStatusCode.OK)
                            {

                                if (t.status == 0)
                                {
                                    Result.Status = true;
                                    Result.Message = "OK";
                                    Result.Detail_TinexGetCost = t;
                                    return Result;

                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.errors.ToString();
                                    Result.Detail_TinexGetCost = t;
                                    return Result;
                                }
                            }
                            else
                            {
                                Result.Status = false;
                                Result.Message = result;
                                Result.Detail_TinexGetCost = t;
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
        /// API GET Cost
        /// <para> Input: Params_Tinex_insert</para>
        /// <para> OutPut:Result_Tinex_insert </para>
        /// <para> Type:POST</para>
        /// </summary>
        public Result_Tinex_insert Insert(Params_Tinex_insert param)
        {
            Result_Tinex_insert Result = new Result_Tinex_insert();
            try
            {
                #region check params
                //1 check Beare
                //2 check 20 k 
                if (Bearer == null)
                {
                    Result.Status = false;
                    Result.Message = "Beare Code is Null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Tinex_Urlinsert ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(url Insert)  is Null";
                    return Result;

                }
                var ParamValid = param.IsValidParams_Tinex_insert();
                if (ParamValid.Item1 == false)
                {
                    Result.Status = false;
                    Result.Message = ParamValid.Item2;
                    return Result;
                }
                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Tinex_Urlinsert);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("Authorization", Bearer);
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
                            Detail_TinexInsert t = JsonConvert.DeserializeObject<Detail_TinexInsert>(result);
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                if (t.status == 0)
                                {
                                    Result.Status = true;
                                    Result.Message = "OK";
                                    Result.Detail_TinexInsert = t;
                                    return Result;
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.message;
                                    Result.Detail_TinexInsert = t;
                                    return Result;
                                }

                            }
                            else
                            {
                                Result.Status = false;
                                Result.Message = result;
                                Result.Detail_TinexInsert = t;
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
        /// API GET Cost
        /// <para> Input:Params_Tinex_insertcommit </para>
        /// <para> OutPut: Result_Tinex_insertcommit</para>
        /// <para> Type:POST</para>
        /// </summary>
        public Result_Tinex_insertcommit InsertCommit(Params_Tinex_insertcommit param)
        {
            Result_Tinex_insertcommit Result = new Result_Tinex_insertcommit();
            try
            {
                #region check params
                //1 check Beare
                //2 check order_no
                if (Bearer == null)
                {
                    Result.Status = false;
                    Result.Message = "Beare Code is Null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Tinex_Urlinsertcommit ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(url Insert)  is Null";
                    return Result;

                }
                if (string.IsNullOrEmpty((param.order_no ?? "").Trim()))

                {
                    Result.Status = false;
                    Result.Message = "order_no Code is Null";
                    return Result;
                }
                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Tinex_Urlinsertcommit);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("Authorization", Bearer);
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
                            DetailTinexInsertCommit t = JsonConvert.DeserializeObject<DetailTinexInsertCommit>(result);
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                if (t.status == 0)
                                {
                                    Result.Status = true;
                                    Result.Message = "OK";
                                    Result.DetailTinexInsertCommit = t;
                                    return Result;
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.message;
                                    Result.DetailTinexInsertCommit = t;
                                    return Result;
                                }

                            }
                            else
                            {
                                Result.Status = false;
                                Result.Message = result;
                                Result.DetailTinexInsertCommit = t;
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
        /// API GET Cost
        /// <para> Input: Params_Tinex_cancel</para>
        /// <para> OutPut: Result_Tinex_cancel</para>
        /// <para> Type:PATCH</para>
        /// </summary>
        public Result_Tinex_cancel Cancel(Params_Tinex_cancel param)
        {
            Result_Tinex_cancel Result = new Result_Tinex_cancel();
            try
            {
                #region check params
                //1 check Beare
                //2 check order_no
                if (Bearer == null)
                {
                    Result.Status = false;
                    Result.Message = "Beare Code is Null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Tinex_Urlcancel ?? "").Trim()))

                {
                    Result.Status = false;
                    Result.Message = "Setting(url Cancel)  is Null";
                    return Result;

                }
                if (string.IsNullOrEmpty((param.order_no ?? "").Trim()))

                {
                    Result.Status = false;
                    Result.Message = "order_no Code is Null";
                    return Result;
                }
                if (string.IsNullOrEmpty((param.reason_id ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "reason_id Code is Null";
                    return Result;
                }
                #endregion


                string Url = string.Format(_ShippingSettings.Tinex_Urlcancel, param.order_no);
                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("Authorization", Bearer);
                httpWebRequest.Method = "PATCH";

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
                            DetailTinexCancel t = JsonConvert.DeserializeObject<DetailTinexCancel>(result);
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                if (t.status == 0)
                                {
                                    Result.Status = true;
                                    Result.Message = "OK";
                                    Result.DetailTinexCancel = t;
                                    return Result;
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.message;
                                    Result.DetailTinexCancel = t;
                                    return Result;
                                }

                            }
                            else
                            {
                                Result.Status = false;
                                Result.Message = result;
                                Result.DetailTinexCancel = t;
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
                                Result.CodeStatus = 0;
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
        /// API GET Cost
        /// <para> Input: No Exist </para>
        /// <para> OutPut: Result_Tinex_cancelreasons </para>
        /// <para> Type: GET</para>
        /// </summary>
        public Result_Tinex_cancelreasons cancelreasons()
        {
            Result_Tinex_cancelreasons Result = new Result_Tinex_cancelreasons();
            try
            {
                #region check params
                //1 check Beare
                //2 check order_no
                if (Bearer == null)
                {
                    Result.Status = false;
                    Result.Message = "Beare Code is Null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Tinex_Urlcancelreasons ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "url  is Null";
                    return Result;

                }
                #endregion
                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Tinex_Urlcancelreasons);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("Authorization", Bearer);
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

                                List<ItemTinexCancelReasons> t = JsonConvert.DeserializeObject<List<ItemTinexCancelReasons>>(result);
                                Result.Status = true;
                                Result.Message = "OK";
                                Result.ListItemsTinexCancelReasons = t;
                                return Result;


                            }
                            else
                            {
                                Result.Status = false;
                                Result.Message = result;
                                //Result.cancelreasons = t;
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
