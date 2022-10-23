using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Snappbox;
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
    /// <summary>
    /// سرویس اسنپ باکس
    /// <para>لیست شهرهایی مه در تاریخ10 آذر اسنپ فعاب میباشد </para>
    /// <para>karaj > van</para>
    /// <para>tehran > van & bike & bike-without-box</para>
    /// <para>mashhad > van</para>
    /// <para>shiraz >  van & bike</para>
    /// <para>isfahan >van & bike & bike-without-box</para>
    /// <para>mashhad > bike & bike-without-box</para>
    /// <para>qom> van & bike & bike-without-box</para>
    /// <para></para>
    /// 
    /// </summary>
    public class Snappbox_Service : ISnappbox_Service
    {
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly ShippingSettings _ShippingSettings;
        private readonly ISettingService _settingService;

        public Snappbox_Service(ILogger logger, IWorkContext workContext, ShippingSettings ShippingSettings, ISettingService settingService)
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
        /// <para>مدل ورودی Params_Snappbox_Get_Price</para>
        /// <para>Result_Snappbox_GetPrice خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public async Task<Result_Snappbox_GetPrice> GetPrice(Params_Snappbox_Get_Price param)
        {
            Result_Snappbox_GetPrice Result = new Result_Snappbox_GetPrice();
            try
            {


                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.Snappbox_CustomerId ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Customer Id SnappBox) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Snappbox_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Token SnappBox) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Snappbox_URL_Get_Price ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Snappbox Url Get Price) is null";
                    return Result;
                }

                //set Param
                param.customerId = _ShippingSettings.Snappbox_CustomerId;
                #endregion
                param.customerId = _ShippingSettings.Snappbox_CustomerId;
                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Snappbox_URL_Get_Price);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("Authorization", _ShippingSettings.Snappbox_APP_AUTH);
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
                                var t = JsonConvert.DeserializeObject<Detail_Result_Snappbox_GetPrice>(result);

                                Result.Status = true;
                                Result.Message = "Ok";
                                Result.Detail_Result_Snappbox_GetPrice = t;
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
                        Result.Message = "An error has occurred in the Service Snappbox";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Snappbox";
                return Result;
            }
        }

        /// <summary>
        /// ای پی ای ثبت سفارش<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_Snappbox_create_order</para>
        /// <para>Result_Snappbox_CreateOrder خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public async Task<Result_Snappbox_CreateOrder> CreateOrder(Params_Snappbox_create_order param)
        {
            Result_Snappbox_CreateOrder Result = new Result_Snappbox_CreateOrder();
            try
            {


                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.Snappbox_CustomerId ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Customer Id SnappBox) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Snappbox_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Token SnappBox) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Snappbox_URL_Create_Order ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Snappbox Url Create Order) is null";
                    return Result;
                }

                //set Param
                param.customerId = _ShippingSettings.Snappbox_CustomerId;


                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Snappbox_URL_Create_Order);
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("Authorization", _ShippingSettings.Snappbox_APP_AUTH);
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    var json = JsonConvert.SerializeObject(param);
                    Common.Log("Snap Json Request", json);
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
                            Common.Log("Snap Json Responce", result);
                            if (response.StatusCode == HttpStatusCode.Created)
                            {
                                var t = JsonConvert.DeserializeObject<DetailResult_Snappbox_CreateOrder>(result);
                                if (t.api_status == "success")
                                {
                                    Result.Status = true;
                                    Result.Message = "Ok";
                                    Result.DetailResult_Snappbox_CreateOrder = t;
                                    return Result;
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.message;
                                    Result.DetailResult_Snappbox_CreateOrder = t;
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
                        Result.Message = "An error has occurred in the Service Snappbox";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Snappbox";
                return Result;
            }
        }

        /// <summary>
        /// ای پی ای دریافت جزییات سفارش<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_Snappbox_Get_Order_Details</para>
        /// <para>Result_Snappbox_Get_Order_Detail خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Snappbox_Get_Order_Detail Get_Order_Detail(Params_Snappbox_Get_Order_Details param)
        {
            Result_Snappbox_Get_Order_Detail Result = new Result_Snappbox_Get_Order_Detail();
            try
            {


                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.Snappbox_CustomerId ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Customer Id SnappBox) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Snappbox_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Token SnappBox) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Snappbox_URL_Get_Order_Details ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Snappbox Url Create Order) is null";
                    return Result;
                }

                //set Param
                param.customerId = _ShippingSettings.Snappbox_CustomerId;


                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Snappbox_URL_Get_Order_Details);
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("Authorization", _ShippingSettings.Snappbox_APP_AUTH);
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
                                var t = JsonConvert.DeserializeObject<OrderDetails_Get_order_Detail>(result);

                                Result.Status = true;
                                Result.Message = "Ok";
                                Result.orderDetails = t;
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
                        Result.Message = "An error has occurred in the Service Snappbox";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Snappbox";
                return Result;
            }
        }

        /// <summary>
        /// ای پی ای دریافت لیست سفارشات<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_Snappbox_Get_Order_List</para>
        /// <para>Result_Snappbox_Get_Order_List خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Snappbox_Get_Order_List Get_Order_List(Params_Snappbox_Get_Order_List param)
        {
            Result_Snappbox_Get_Order_List Result = new Result_Snappbox_Get_Order_List();
            try
            {


                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.Snappbox_CustomerId ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Customer Id SnappBox) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Snappbox_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Token SnappBox) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Snappbox_URL_Get_Order_List ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Snappbox Url List Order) is null";
                    return Result;
                }

                //set Param
                param.customerId = _ShippingSettings.Snappbox_CustomerId;


                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Snappbox_URL_Get_Order_List);
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("Authorization", _ShippingSettings.Snappbox_APP_AUTH);
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
                                var t = JsonConvert.DeserializeObject<DetailResult_Snappbox_Get_Order_List>(result);

                                Result.Status = true;
                                Result.Message = "Ok";
                                Result.DetailResult_Snappbox_Get_Order_List = t;
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
                        Result.Message = "An error has occurred in the Service Snappbox";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Snappbox";
                return Result;
            }
        }

        /// <summary>
        /// ای پی ای دریافت مانده حساب<para />
        /// نوع ای پی ای POST<para />
        /// <para>Result_Snappbox_Get_Account_Balance خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Snappbox_Get_Account_Balance GetAccountBalance()
        {
            Result_Snappbox_Get_Account_Balance Result = new Result_Snappbox_Get_Account_Balance();
            try
            {


                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.Snappbox_CustomerId ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Customer Id SnappBox) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Snappbox_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Token SnappBox) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Snappbox_URL_Get_Account_Balance ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Snappbox Url Get_Account_Balance) is null";
                    return Result;
                }

                //set Param
                Params_Snappbox_Get_Account_Balance param = new Params_Snappbox_Get_Account_Balance();
                param.customerId = Convert.ToInt32(_ShippingSettings.Snappbox_CustomerId);


                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Snappbox_URL_Get_Account_Balance);
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("Authorization", _ShippingSettings.Snappbox_APP_AUTH);
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
                                var t = JsonConvert.DeserializeObject<DetailResult_Snappbox_Get_Account_Balance>(result);

                                Result.Status = true;
                                Result.Message = "Ok";
                                Result.DetailResult_Snappbox_Get_Account_Balance = t;
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
                        Result.Message = "An error has occurred in the Service Snappbox";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Snappbox";
                return Result;
            }
        }

        /// <summary>
        /// ای پی ای کنسل کردن سفارش<para />
        /// نوع ای پی ای POST<para />
        /// <para>مدل ورودی Params_Snappbox_Cancel_Order</para>
        /// <para>Result_Snappbox_Cancel_Order خروجی ای پی ای</para>
        /// حتما تنظیمات سرویس در ناپ کامرس چک شود که توکن و ادرس ای پی ای درست باشد
        /// </summary>
        /// <returns></returns>
        public Result_Snappbox_Cancel_Order CancelOrder(Params_Snappbox_Cancel_Order param)
        {
            Result_Snappbox_Cancel_Order Result = new Result_Snappbox_Cancel_Order();
            try
            {


                #region Check Param
                if (string.IsNullOrEmpty((_ShippingSettings.Snappbox_CustomerId ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Customer Id SnappBox) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Snappbox_APP_AUTH ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Token SnappBox) is null";
                    return Result;
                }
                if (string.IsNullOrEmpty((_ShippingSettings.Snappbox_URL_Cancel_Order ?? "").Trim()))
                {
                    Result.Status = false;
                    Result.Message = "Setting(Snappbox URL_Cancel_Order) is null";
                    return Result;
                }

                //set Param
                param.customerId = _ShippingSettings.Snappbox_CustomerId;


                #endregion

                string result = "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_ShippingSettings.Snappbox_URL_Cancel_Order);
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("Authorization", _ShippingSettings.Snappbox_APP_AUTH);
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
                                var t = JsonConvert.DeserializeObject<DetailResult_Snappbox_Cancel_Order>(result);
                                if (t.api_status == "success")
                                {
                                    Result.Status = true;
                                    Result.Message = "Ok";
                                    Result.DetailResult_Snappbox_Cancel_Order = t;
                                    return Result;
                                }
                                else
                                {
                                    Result.Status = false;
                                    Result.Message = t.message;
                                    Result.DetailResult_Snappbox_Cancel_Order = t;
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
                        Result.Message = "An error has occurred in the Service Snappbox";
                        return Result;
                    }
                }
            }
            catch
            {
                Result.Status = false;
                Result.Message = "An error has occurred in the Service Snappbox";
                return Result;
            }
        }
    }
}
